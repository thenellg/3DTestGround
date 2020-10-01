using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using Cinemachine;
using UnityEngine;

public class newThirdPersonMovement : MonoBehaviour
{

    [Header("General")]
    [SerializeField] private Rigidbody rb;
    public Transform cam;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    [SerializeField] private Vector3 inputVector;
    public Animator playerAnim;

    //jumping
    [Header("Jumping")] 
    [SerializeField] bool moving;
    public float gravity = -9.81f;
    public float gravityScale = 2f;
    public float jumpHeight = 3;
    float jumpForce;
    float jump2Timer = 2;
    [SerializeField] bool jump2 = false;
    float jump3Timer = 2;
    [SerializeField] bool jump3 = false;
    bool jumpCheck = true;
    public float secondJumpPoint = 3f; //0.4f

    //gravity
    [Header("Grounding")]
    public float vertical = -10f;
    public bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Quaternion tempRotation;
    float tempGrav;

    //Sprinting
    [Header("Dash")]
    public float speed = 10;
    float trueSpeed;
    [SerializeField] bool dashing = false;
    Vector3 dashForce;
    public float dashSpeed = 100f;
    public float dashTimer = 1f;
    bool canDash = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        trueSpeed = speed;
        tempGrav = gravity;
    }

    private void Update()
    {
        //Seting up is grounded and keeping the player grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (!isGrounded)
        {
            gravity = tempGrav;
            playerAnim.SetBool("isGrounded", false);
        }
        else
        {
            gravity = 0f;
            playerAnim.SetBool("isGrounded", true);
        }


        //setting movement variable for triple jump
        if ((new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized).magnitude > 0.1f)
            moving = true;
        else
            moving = false;

        //Triple jump timers
        if (isGrounded)
        {
            jump2Timer -= 0.01f;
            jump3Timer -= 0.01f;
        }

        if (jump2Timer < 0)
        {
            jump2 = false;
        }

        if (jump3Timer < 0)
        {
            jump3 = false;
        }

        //The actual jump

        jumpForce = gravity;
        if (Input.GetButtonDown("Jump") && isGrounded && jumpCheck)
        {
            if (jump2 && moving)
            {
                jump3 = true;
                jump3Timer = 2f;
                jumpForce = jumpHeight * 1.2f;
                playerAnim.SetTrigger("doubleJump");
                jump2 = false;
            }
            else if (jump3 && moving)
            {
                jumpForce = jumpHeight * 1.45f;
                playerAnim.SetTrigger("tripleJump");
                jump3 = false;
            }
            else
            {
                jumpForce = jumpHeight;
                playerAnim.SetTrigger("singleJump");
                jump2Timer = 2f;
                jump2 = true;
            }

            //Apply add force

            //Debug.Log(jumpForce);
            rb.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);
        }

        //implementing gravity
        //vertical = rb.velocity.y;                              //Uncomment this line for ramping
        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);

        //actual gravitational force
        Vector3 gravForce = Physics.gravity.y * gravityScale * Vector3.up;
        rb.AddForce(gravForce, ForceMode.Acceleration);

        //Determining character movement input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //Moving character
        if (moving)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            tempRotation = transform.rotation;

            inputVector = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            inputVector = inputVector.normalized * trueSpeed;
        //rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            rb.AddForce(inputVector.x * trueSpeed, inputVector.y * trueSpeed, inputVector.z * trueSpeed);
        }
        else
        {
            //rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            transform.rotation = tempRotation;
        }

            float fullDash = dashSpeed * speed;

        if (Input.GetButtonDown("Dash") && canDash)
        {
            //Same code as determining movement direction. Copy pasta-ed because there were 
            //some issues with the stand still otherwise. 
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            tempRotation = transform.rotation;
            inputVector = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            inputVector = inputVector.normalized * trueSpeed;
            dashForce = new Vector3(inputVector.x * fullDash, inputVector.y * fullDash, inputVector.z * fullDash);

            playerAnim.SetBool("dashing", true);
            playerAnim.SetTrigger("dashTrigger");
            
            dashing = true;
            canDash = false;
            Invoke("resetDash", dashTimer);
        }
        if (dashing) {
            rb.AddForce(dashForce);
        }
    }

    void resetDash()
    {
        dashing = false;
        playerAnim.SetBool("dashing", false);

        if (isGrounded)
        {
            canDash = true;
        }
        else
        {
            Invoke("resetDash", 0.1f);
        }
    }

}
