using UnityEngine;
using System.Collections;

public class Crawler_TakeDamage : MonoBehaviour {

	GameObject player;
	Collider2D playerBoxCollider;
    Collider2D playerPolygonCollider;

    // Use this for initialization
    void Start () {
		player = GameObject.Find ("Player Prefab");
		playerBoxCollider = player.GetComponent<BoxCollider2D> ();
        playerPolygonCollider = player.GetComponent<PolygonCollider2D>();
    }

	// Update is called once per frame
	void Update () {
		if (playerBoxCollider.IsTouching(GetComponent<PolygonCollider2D>())/* && !playerPolygonCollider.IsTouching(GetComponent<PolygonCollider2D>())*/ && !player.GetComponent<Player_Info>().isInvincible()) {
			TakeDamageFunction();
		}
	}

	void TakeDamageFunction(){
		GameObject.Find ("Player Prefab").GetComponent<Player_Movement> ().jumpOnEnemy ();
		Destroy (this.gameObject);
		Debug.Log (this.GetInstanceID() + " died.");
	}
}
