using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampolin_Behavior : MonoBehaviour {

    public float force = 400f;
    public float jumpTime = 1f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player_Movement>().addDirectionalForce(this.transform.up, force, jumpTime);
            GetComponent<Animator>().SetTrigger("Triggered");
        }

        else if (other.CompareTag("Box"))
        {
            other.GetComponent<BoxBounce>().addDirectionalForce(this.transform.up, force * 0.8f);
            GetComponent<Animator>().SetTrigger("Triggered");
        }
    }
}
