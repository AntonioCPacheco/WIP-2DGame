using UnityEngine;
using System.Collections;

public class Spikes_Deal_Damage : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.name == "Player")
        {
            coll.GetComponent<Checkpoint_Handler>().returnToLastCheckpoint();
            GameObject.Find("DeathScreen").GetComponent<Animator>().SetTrigger("Die");
            VibrateController.vibrateControllerForXSeconds(0.5f, VibrateController.MEDIUM, VibrateController.MEDIUM);
        }
    }
}
