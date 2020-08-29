using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleportDoor : MonoBehaviour
{
    public GameObject teleportSpot;
    public GameObject player;

    private void Start()
    {
        teleportSpot.SetActive(false);
    }

    void hideTeleport()
    {
        teleportSpot.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            teleportSpot.SetActive(true);
            other.GetComponent<CharacterController>().enabled = false;
            Debug.Log("test");
            player.transform.position = teleportSpot.transform.position;
            Invoke("hideTeleport", 1);
            other.GetComponent<CharacterController>().enabled = true;

        }
    }
}
