using UnityEngine;
using System.Collections;
using System;

public class TakeDamageOnTriggerEnter : MonoBehaviour {

    private bool triggered = false;
    public static event Action OnTakeDamage = delegate { };

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggered && collision.CompareTag("Player"))
        {
            triggered = true;
            OnTakeDamage();
            TakeDamageFunction();
        }
    }
    
    void TakeDamageFunction(){
        if(this.GetComponent<Blob_SpawnerBehaviour>() != null) this.GetComponent<Blob_SpawnerBehaviour>().triggerSpawner();
        //GameObject.Find ("Player").GetComponent<Player_Movement> ().jumpOnEnemy ();
		Destroy (this.transform.parent.gameObject);
		Debug.Log (this.GetInstanceID() + " died.");
	}
}
