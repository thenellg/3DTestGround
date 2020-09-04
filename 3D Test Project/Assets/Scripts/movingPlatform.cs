using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingPlatform : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player Entered");
        other.transform.parent = transform;
        other.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Player Left");
        other.transform.parent = null;
        //other.transform.localScale = new Vector3(1, 1, 1);
    }
}
