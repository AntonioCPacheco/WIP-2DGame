using UnityEngine;
using System.Collections;

public class Camera_Movement : MonoBehaviour {

	public float lookAhead = 3f;
	public float lookUp = 0f;
	public float dampTime = 0.15f;

	public bool followPlayer = true;

	Vector3 velocity = Vector3.zero;

	Transform player;
	Vector3 topLeft = new Vector3(-3000f, 3000f, 0f);
	Vector3 bottomRight = new Vector3(3000f, -3000f, 0f);
	Vector3 target;

	bool isGrounded = true;
	bool facingRight = false;

	Camera cam;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player Prefab").transform;

		facingRight = GameObject.Find ("Player Prefab").GetComponent<Player_Movement>().isFacingRight();
		lookAhead = facingRight ? lookAhead : -lookAhead;

		target = new Vector3 ((player.position.x + lookAhead),(player.position.y + lookUp),(player.position.z));

		cam = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		isGrounded = player.gameObject.GetComponent<Player_Movement> ().isGrounded ();
		if (followPlayer) {
			if (GameObject.Find ("RoomManager").GetComponent<Game_RoomManager> ().GetActiveRoom () != null) {
				topLeft = GameObject.Find ("RoomManager").GetComponent<Game_RoomManager> ().GetActiveRoom ().FindChild ("TopLeft").position;
				bottomRight = GameObject.Find ("RoomManager").GetComponent<Game_RoomManager> ().GetActiveRoom ().FindChild ("BottomRight").position;
			}
			target = new Vector3 ((player.position.x + lookAhead), (player.position.y + lookUp), (player.position.z));

			facingRight = GameObject.Find ("Player Prefab").GetComponent<Player_Movement> ().isFacingRight ();
			lookAhead = facingRight ? Mathf.Abs (lookAhead) : -(Mathf.Abs (lookAhead));
			if (player) {
				Vector3 point = cam.WorldToViewportPoint (target);
				Vector3 delta = target - cam.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
				delta = new Vector3(delta.x, (isGrounded ? delta.y : 0f), delta.z);
				Vector3 destination = transform.position + delta;
				//correcting X
				if (destination.x > (bottomRight.x - 200)) {
					destination = new Vector3 (bottomRight.x - 200, destination.y, destination.z);
				} else if (destination.x < (topLeft.x + 200)) {
					destination = new Vector3 (topLeft.x + 200, destination.y, destination.z);
				}
				//correcting Y
				if (destination.y < (bottomRight.y + 123.5f)) {
					destination = new Vector3 (destination.x, bottomRight.y + 123.5f, destination.z);
				} else if (destination.y > (topLeft.y - 123.5f)) {
					destination = new Vector3 (destination.x, topLeft.y - 123.5f, destination.z);
				}

				transform.position = Vector3.SmoothDamp (transform.position, destination, ref velocity, dampTime);
			}
		}

	}
}
