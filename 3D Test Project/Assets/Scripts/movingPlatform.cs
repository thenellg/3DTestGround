using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingPlatform : MonoBehaviour
{
    public Transform parent;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.parent = parent;
        Debug.Log(other.transform.localScale);
        //other.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.parent = null;
        //other.transform.localScale = new Vector3(1, 1, 1);
    }
}
