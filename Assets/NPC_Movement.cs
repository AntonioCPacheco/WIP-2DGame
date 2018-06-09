using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC_Movement : MonoBehaviour
{
    public float nextStep = 160;

    public float maxSpeed = 4f;
    public float jumpForce = 100f;
    bool facingPlayer;
    public bool facingRight = false;

    bool grounded = false;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    
    Animator anim;
    Rigidbody2D rbody;
    GameObject player;

    float xBeforeTarget;
    float target;
    bool reachedTarget = true;

    bool isJumping = false;

    bool inDialogue = false;
    bool changedNextStep = true;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player Prefab");
        anim = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inDialogue) { return; }
        if (isReadyForNextStep())
        {
            xBeforeTarget = this.transform.position.x;
            target = getNextStep();
            facingRight = xBeforeTarget < target;
            Turn();
        }
        if ((!grounded && !isJumping) || reachedTarget && !changedNextStep)
        {
            rbody.velocity = new Vector2(0, rbody.velocity.y);
        }
        else
        {
            rbody.velocity = new Vector2((facingRight ? 1 : -1) * 0.3f * maxSpeed, rbody.velocity.y);
        }

        facingPlayer = ((player.transform.position.x - transform.position.x) >= 0 && facingRight) || ((player.transform.position.x - transform.position.x) <= 0 && !facingRight);
        //anim.SetFloat("Speed", 0);
        checkReachedTarget();
    }

    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle (groundCheck.position, 0.4f, whatIsGround);
        anim.SetBool("Grounded", grounded);
        
        anim.SetFloat("Speed", Mathf.Abs(rbody.velocity.x));
        anim.SetFloat("vSpeed", rbody.velocity.y);
        //if (!facingPlayer && (Mathf.Abs(player.transform.position.x - transform.position.x) > 7))
        //    Flip();
    }

    float getDistance(float x1, float x2)
    {
        return Mathf.Abs((x1 + Mathf.Abs(x1) + Mathf.Abs(x2)) - (x2 + Mathf.Abs(x1) + Mathf.Abs(x2)));
    }

    void Turn()
    {
        if (facingRight)
        {
            Vector3 theScale = transform.localScale;
            theScale.x = Mathf.Abs(theScale.x);
            transform.localScale = theScale;
        }
        else
        {
            Vector3 theScale = transform.localScale;
            theScale.x = - Mathf.Abs(theScale.x);
            transform.localScale = theScale;
        }
    }

    void checkReachedTarget()
    {
        if(xBeforeTarget > target)
        {
            reachedTarget = (this.transform.position.x <= target);
        }
        else
        {
            reachedTarget = (this.transform.position.x > target);
        }
    }

    bool isReadyForNextStep()
    {
        return (!inDialogue && changedNextStep);
    }

    public void setNextStep(float nextStep)
    {
        changedNextStep = true;
        this.nextStep = nextStep;
    }

    float getNextStep()
    {
        changedNextStep = false;
        return nextStep;
    }

    public void startDialogue()
    {
        inDialogue = true;
    }

    public void stopDialogue()
    {
        inDialogue = false;
    }

    public void moveNPCto(Vector2 newPos)
    {
        this.transform.position = newPos;
        nextStep = newPos.x + 50;
    }

    public bool tryEnterDoor(Player_EnterDoors sc)
    {
        if (reachedTarget)
        {
            enterDoor(sc);
            return true;
        }
        return false;
    }

    private void enterDoor(Player_EnterDoors sc)
    {
        sc.changeRoom(false);
        setVisibility(false);
    }

    public void setVisibility(bool visibility)
    {
        this.GetComponent<SpriteRenderer>().enabled = visibility;
    }

    public void jump()
    {
        isJumping = true;
        rbody.AddForce(new Vector2(0, (jumpForce - (rbody.velocity.y * 45))));
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        print(coll.gameObject.name);
        if (coll.gameObject.layer != LayerMask.NameToLayer("Floor")) isJumping = false;
    }
}