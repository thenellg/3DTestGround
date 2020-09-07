using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class newThirdPersonMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    public Transform cam;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    [SerializeField] private Vector3 inputVector;

    //jumping
    [SerializeField] bool moving;
    public float gravity = -9.81f;
    public float jumpHeight = 3;
    float jumpForce;
    float jump2Timer = 2;
    [SerializeField] bool jump2 = false;
    float jump3Timer = 2;
    [SerializeField] bool jump3 = false;
    bool jumpCheck = true;
    public float secondJumpPoint = 0.4f;

    //gravity
    public float vertical = -10f;
    public bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Quaternion tempRotation;
    float tempGrav;

    //Sprinting
    public float speed = 10;
    float trueSpeed;
    [SerializeField] bool sprinting = false;
    //float sprintTimer = 0f;
    public float sprintWaitTimer = 3f;
    //bool canSprint = true;

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
        }
        else
        {
            gravity = 0f;
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
                jump3Timer = secondJumpPoint;
                jumpForce = Mathf.Sqrt((jumpHeight * 1.25f) * -2 * tempGrav);
                //playerAnim.SetTrigger("DoubleJump");
                jump2 = false;
            }
            else if (jump3 && moving)
            {
                jumpForce = Mathf.Sqrt((jumpHeight * 2) * -2 * tempGrav);
                //playerAnim.SetTrigger("TripleJump");
                jump3 = false;
            }
            else
            {
                jumpForce = Mathf.Sqrt(jumpHeight * -2 * tempGrav);
                //playerAnim.SetTrigger("SingleJump");
                jump2Timer = secondJumpPoint;
                jump2 = true;
            }

            //Apply add force

            Debug.Log(jumpForce);
            rb.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);
        }
        /*
        if (Input.GetButtonUp("Jump") && !isGrounded)
        {
            if (velocity.y > Mathf.Sqrt(-2 * gravity))
            {
                velocity.y = Mathf.Sqrt(-gravity);
            }
        }
        */

        //Implementing a sprint function
        if (Input.GetButtonDown("Sprint"))
        {
            if (sprinting)
            {
                sprinting = false;
            }
            else
            {
                sprinting = true;
            }
        }

        if (sprinting)
        {
            trueSpeed = speed * 1.5f;
        }

        //implementing gravity
        //vertical = rb.velocity.y;                              //Uncomment this line for ramping
        //rb.AddForce(0f, Physics.gravity.y, 0f);
        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);

        //Determining character movement
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
            rb.AddForce(inputVector.x, inputVector.y, inputVector.z);
        }
        /*
        else
        {
            rb.velocity = new Vector3(0f,rb.velocity.y, 0f);
            transform.rotation = tempRotation;
            sprinting = false;
        }
        */
    }
}
