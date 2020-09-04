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

    //gravity
    public float gravity;
    public bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    //Sprinting
    public float speed = 10;
    float trueSpeed;
    bool sprinting = false;
    float sprintTimer = 0f;
    public float sprintWaitTimer = 3f;
    bool canSprint = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        trueSpeed = speed;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (!isGrounded)
        {
            gravity = -9.81f;
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




        //implementing gravity
        gravity = rb.velocity.y;
        //rb.AddForce(0f, Physics.gravity.y, 0f);
        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);

        //Determining character movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //Moving character
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        if (moving)
        {
            inputVector = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rb.velocity = inputVector.normalized * trueSpeed;
            rb.velocity = new Vector3(rb.velocity.x, gravity, rb.velocity.z);
        }
        else
        {
            rb.velocity = new Vector3(0f,rb.velocity.y, 0f);
            
        }
    }
}
