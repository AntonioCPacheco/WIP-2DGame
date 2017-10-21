using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player_Movement : MonoBehaviour {
	
	public float maxSpeed = 5f;
	public float maxJumpingSpeed = 7f;

	float stamina = 1.0f;
	bool running = false;
    public bool ableToRun = false;

    public bool onTopOfSomething = true;
    public Vector2 somethingsVelocity;


	bool facingRight = true;

	bool isWaiting = false;
	int framesToWait = 0;

	Animator anim;

	//Ground Check Variables
	bool grounded = false;
	float groundRadius = 0.4f;
	public Transform groundCheck;

    //WallJumping Variables
    public bool CanWallJump = false;
	bool isWallJumping = false;
	bool alreadyWallJumped = false;
	bool huggingBackWall = false;
	bool huggingFrontWall = false;
	public Transform rightWallCheck;
	public Transform leftWallCheck;

	//Jumped Twice boolean
	//bool jumpedTwice = false;

	//Masks
	public LayerMask whatIsGround;
	public LayerMask whatIsWall;

	//Jump Forces
	public float jumpForce = 700f;
	public float jumpSideForce = 1000f;

    public float knockback = 300f;

	bool jumpNext = false;
	public int jumpNextWindow = 10;
	int jumpNextDeltaFrames = 0;

	//public int maxJumpFrames = 90;
	//int jumpFrames = 0;
	Game_RoomManager roomManager;
	Rigidbody2D rBody;
	Camera_Movement mainCamera;
	
	void Start(){
		anim = GetComponent<Animator>();
		roomManager = GameObject.Find ("RoomManager").GetComponent<Game_RoomManager> ();
		rBody = GetComponent<Rigidbody2D> ();
		mainCamera = GameObject.Find ("Main Camera").GetComponent<Camera_Movement> ();
	}

	//Deals with groundchecks and wallchecks
	void FixedUpdate(){
		if (canMove()) {
			//Check if the GameObject is on the ground
			Vector3 auxGCPos1 = new Vector3 (groundCheck.position.x + 4.0f, groundCheck.position.y, groundCheck.position.z);
			Vector3 auxGCPos2 = new Vector3 (groundCheck.position.x - 4.0f, groundCheck.position.y, groundCheck.position.z);
			grounded = Physics2D.OverlapCircle (auxGCPos1, groundRadius, whatIsGround);
			grounded = grounded ? grounded : Physics2D.OverlapCircle (auxGCPos2, groundRadius, whatIsGround);
			anim.SetBool ("Ground", grounded); //telling the Animator wether the GameObject is on the ground

			//Setting jump booleans in the case the GameObject is on the ground
			if (grounded) {
				//jumpedTwice = false;
				alreadyWallJumped = false;
				isWallJumping = false;
				if (jumpNext) {
					Jump ();
				}
				//jumpFrames = 0;
			} else {
				if (jumpNext) {
					jumpNextDeltaFrames++;
				}
				if (jumpNextDeltaFrames > jumpNextWindow) {
					jumpNext = false;
					jumpNextDeltaFrames = 0;
				}
			}

            //Checking if the GameObject is hugging any walls
            if (CanWallJump) {
                huggingFrontWall = Physics2D.OverlapCircle(rightWallCheck.position, groundRadius, whatIsWall);
                if (!huggingFrontWall) {
                    huggingBackWall = Physics2D.OverlapCircle(leftWallCheck.position, groundRadius, whatIsWall);
                }
            }
		
			anim.SetFloat ("vSpeed", rBody.velocity.y);
			anim.SetFloat ("hSpeed", rBody.velocity.x);
			anim.SetFloat ("AbsHSpeed", Mathf.Abs (rBody.velocity.x));
			if (!isWallJumping){
				
				float move = Input.GetAxis ("Horizontal");
				anim.SetFloat ("Speed", Mathf.Abs (move));

				if (!isWaiting) {
                    if (grounded)
                    {
                        rBody.velocity = new Vector2(move * (maxSpeed * (running ? 2 : 1)), rBody.velocity.y);
                        if (onTopOfSomething)
                        {
                            rBody.velocity += somethingsVelocity;
                            somethingsVelocity = Vector2.zero;
                        }
                    }
                    else
                        rBody.velocity = new Vector2(move * (maxJumpingSpeed * (running ? 2 : 1)), rBody.velocity.y);
				}
				if (move > 0 && !facingRight)
					Flip ();
				else if (move < 0 && facingRight)
					Flip ();
			}
		}
	}

	//Main function, deals with most of the player input. I should probably move the input elsewhere...
	void Update(){
		if (!canMove ()) {
			rBody.velocity = Vector2.zero;
			return;
		}

		if (running) {
			stamina -= 0.0025f;
		} else if (stamina < 1.0f) {
			stamina += 0.0025f;
		}

		if (isWaiting) {
			framesToWait--;
			if (framesToWait == 0) {
				isWaiting = false;
			} else {
				return;
			}
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			if (grounded /*|| !jumpedTwice*/) {
				//if (!grounded) {
				//	jumpedTwice = true;
				//	Jump ();
				//} else {
				Jump ();
				//anim.SetBool ("Ground", false);		
				//}
			} else if (CanWallJump && !alreadyWallJumped) {
				if (huggingFrontWall) {
					JumpBackward ();
					alreadyWallJumped = true;
				} else if (huggingBackWall) {
					JumpForward ();
					alreadyWallJumped = true;
				} else {
					jumpNext = true;
				}
			}
		}
		if (((Input.GetKeyDown (KeyCode.LeftShift) && stamina > 0.1f) || ((!running && Input.GetKey (KeyCode.LeftShift)) && stamina > 0.3f)) && ableToRun) {
			anim.SetBool ("Running", true);
			running = true;
		}
		if (Input.GetKeyUp (KeyCode.LeftShift) || stamina < 0.01) {
			anim.SetBool ("Running", false);
			running = false;
		}
		if (Input.GetKeyUp (KeyCode.Space) && rBody.velocity.y > 0f) {
			rBody.velocity = new Vector2 (rBody.velocity.x, rBody.velocity.y*0.6f);
			//jumpFrames = maxJumpFrames + 1;
		}
		//Enter doors
		if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.W)) {
			List<Transform> doors = roomManager.GetActiveDoors ();
			if (roomManager.GetInitialDoor () != null)
				doors.Add (roomManager.GetInitialDoor ());
			Transform ladder = roomManager.GetActiveLadder ();
			if (ladder != null)
				doors.Add (ladder);
			for (int i = 0; i < doors.Count; i++) {
				Transform child = doors [i];
				if (GetComponent<PolygonCollider2D> ().OverlapPoint (new Vector2 (child.Find ("EntrancePoint").position.x, child.Find ("EntrancePoint").position.y)) && child.GetComponent<Player_EnterDoors>().isOpen) {
					child.GetComponent<Player_EnterDoors> ().changeRoom ();
					break;
				}
			}
		}

		//Open chests
		if(Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.S)) {
			List<Transform> cases = roomManager.GetActiveCases ();
			for (int i = 0; i < cases.Count; i++) {
				Transform child = cases [i];
				Debug.Log ("[Player_Movement] Found a case in " + child.parent.parent.gameObject.name);
				if (GetComponent<BoxCollider2D> ().OverlapPoint (new Vector2 (child.Find ("OpeningPoint").position.x, child.Find ("OpeningPoint").position.y))) {
					child.GetComponent<Case_Open> ().Open ();
					Debug.Log ("[Player_Movement] Tried to open a case in " + child.parent.parent.gameObject.name);
					break;
				}
			}

			List<Transform> items = roomManager.GetActiveItems ();
			for (int i = 0; i < items.Count; i++) {
				Transform child = items [i];
				Debug.Log ("[Player_Movement] Found an item in " + child.parent.parent.gameObject.name);
				if (GetComponent<BoxCollider2D> ().OverlapPoint (new Vector2 (child.Find ("PickUpPoint").position.x, child.Find ("PickUpPoint").position.y))) {
					child.GetComponent<Item_SuperClass> ().OnPickUp ();
					Debug.Log ("[Player_Movement] Tried to pick up an item in " + child.parent.parent.gameObject.name);
					break;
				}
			}
		}
	}
		
	//Wall jump if player is facing the wall
	void JumpBackward(){
		isWallJumping = true;
		rBody.velocity.Set (0,rBody.velocity.y);
		rBody.AddForce (new Vector2 ((facingRight ? -1 : 1) *jumpSideForce, jumpForce - (rBody.velocity.y * 35)));
		Flip ();
	}

	//Wall jump if player is with their back against the wall
	void JumpForward(){
		isWallJumping = true;
		rBody.velocity.Set (0,rBody.velocity.y);
		rBody.AddForce (new Vector2 ((facingRight ? 1 : -1) *jumpSideForce, jumpForce - (rBody.velocity.y * 35)));
	}

	//Main jump
	void Jump(){
		rBody.AddForce (new Vector2 (0, (jumpForce * ((Mathf.Abs(rBody.velocity.x)>80 ) ? 1 : 0.7f)) - (rBody.velocity.y * 45)));
	}

	//Flip player sprite to face the opposite direction
	void Flip(){
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;	
	}

	//Wait
	void waitXFrames(int xFrames){
		if (isWaiting){
			return;
		}
		isWaiting = true;
		framesToWait = xFrames;
	}


	//Teleport player to given position
	public void MovePlayerTo(Vector3 vec3){
		//mainCamera.followPlayer = false;
		GameObject.Find ("Main Camera").transform.Translate (vec3-transform.position);
		transform.Translate (vec3-transform.position);
		//mainCamera.followPlayer = true;
		//Wait ();
	}

	//Secondary jump. Used when the player takes damage and enters invicibility frames
	public void TakeDamage(){
		waitXFrames (30);
		isWallJumping = true; //Horrible hack need to change
		rBody.AddForce ((facingRight ? -1 : 1) * transform.right * knockback);
        rBody.AddForce(transform.up * knockback);
        rBody.velocity.Set ((anim.GetFloat("Speed") > 0 ? -1 : 1) * 10000, 0);
		Flip ();
		/* rBody.AddForce (new Vector2 (0, (jumpForce * 0.4f)));
		Wait ();
		rBody.AddForce (new Vector2 ((facingRight ? -1 : 1) * (jumpForce *), 0)); */
	}

	//Auxiliary function
	IEnumerator Wait() {
		yield return new WaitForEndOfFrame();
	}

	//Secondary jump, or rather bounce. Used when the player jumps on an enemy
	public void jumpOnEnemy() {
		rBody.AddForce (new Vector2 (0, (jumpForce * 0.8f) - (rBody.velocity.y * 45)));
	}

	//Function that returns true if the player is facing right (x>0 direction) and false if the player is facing left (x<0 direction)
	public bool isFacingRight(){
		return facingRight;
	}

	//Function that returns true if the player is allowed to move and false otherwise
	//Needs work though
	bool canMove(){
		return !gameObject.GetComponent<Player_Health> ().isDead ();
	}

	public bool isGrounded(){
		return grounded;
	}

	public float getStamina(){
		return stamina;
	}

}