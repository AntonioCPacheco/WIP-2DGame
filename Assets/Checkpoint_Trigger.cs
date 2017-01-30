using UnityEngine;
using System.Collections;

public class Checkpoint_Trigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.name == "Player Prefab")
        {
            Checkpoint_Handler ch = coll.gameObject.GetComponent<Checkpoint_Handler>();
            ch.checkpoint = transform;
            GetComponent<SpriteRenderer>().sprite = ch.checkpointOn;
        }
    }
}
