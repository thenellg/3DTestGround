using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    public Rigidbody rb;
    public float dashSpeed = 5f;
    [SerializeField] bool canDash = true;

    void Update()
    {
        if (Input.GetButtonDown("Dash"))
        {
            //movement
            Vector3 dashVector = Vector3.forward * dashSpeed;
            rb.AddForce(dashVector, ForceMode.Acceleration);
            canDash = false;
            Invoke("refreshDash", 1f);
        }
    }

    void refreshDash()
    {
        canDash = true;
    }
}
