using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    //character movement and animation
    public CharacterController controller;
    //public Animator playerAnim;
    public Transform cam;

    public float speed = 6;
    float trueSpeed;
    bool sprinting = false;
    float sprintTimer = 0f;
    public float sprintWaitTimer = 3f;
    bool canSprint = true;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    //Jumping and gravity
    public float gravity = -9.81f;
    public float jumpHeight = 3;
    float jump2Timer = 0;
    [SerializeField] bool jump2 = false;
    float jump3Timer = 0;
    [SerializeField] bool jump3 = false;
    bool jumpCheck = true;
    public float secondJumpPoint = 0.4f;
    bool moving;

    Vector3 velocity;
    public bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private void Start()
    {
        trueSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        //setting movement variable for triple jump
        if ((new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized).magnitude > 0.1f)
            moving = true;
        else
            moving = false;
            
        //Seting up is grounded and keeping the player grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //Actual jump implemented for a triple jump with short jump abilities
        if (Input.GetButtonDown("Jump") && isGrounded && jumpCheck)
        {
            if (jump2 && moving)
            {
                jump3 = true;
                jump3Timer = secondJumpPoint;
                velocity.y = Mathf.Sqrt((jumpHeight * 1.25f) * -2 * Physics.gravity.y);
                //playerAnim.SetTrigger("DoubleJump");
                jump2 = false;
            }
            else if (jump3 && moving)
            {
                velocity.y = Mathf.Sqrt((jumpHeight * 2) * -2 * Physics.gravity.y);
                //playerAnim.SetTrigger("TripleJump");
                jump3 = false;
            }
            else
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
                //playerAnim.SetTrigger("SingleJump");
                jump2Timer = secondJumpPoint;
                jump2 = true;
            }
        }
        if (Input.GetButtonUp("Jump") && !isGrounded)
        {
            if (velocity.y > Mathf.Sqrt(-2 * gravity))
            {
                velocity.y = Mathf.Sqrt(-gravity);
            }
        }

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

        //Implementing a sprint function
        if (Input.GetButtonDown("Sprint"))
        {
            Invoke("sprintingCheck", 5);
            if (sprinting)
            {
                sprinting = false;
            }
            else
            {
                if (canSprint)
                {
                    sprinting = true;
                    sprintTimer = Time.time;
                }

            }
        }

        if (sprinting && canSprint)
        {
            trueSpeed = speed * 1.5f;
        }
        else
        {
            trueSpeed = speed;
        }


        //gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        //transform.Translate(velocity * Time.deltaTime, Space.World);

        //Determining character movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //Moving character
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * trueSpeed * Time.deltaTime);
            //transform.Translate(moveDir.normalized * trueSpeed * Time.deltaTime, Space.World);
        }
    }

    void sprintingCheck()
    {

        if (sprinting)
        {
            Debug.Log("You can't sprint for " + 0);
            sprinting = false;
            canSprint = false;
            resetSprint();
        }

    }

    void resetSprint()
    {
        Debug.Log("You can sprint again");
        canSprint = true;
    }
}
