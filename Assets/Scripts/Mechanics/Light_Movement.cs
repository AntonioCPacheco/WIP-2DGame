using UnityEngine;
using System.Collections;

public class Light_Movement : MonoBehaviour {
    private static float PERMANENT_Z = -95f;

    public float dampTime = 0.2f;
    public float maxSpeed = 1000f;
    public Vector2 maxDistanceToPlayer;

    Vector2 positionToRecallTo = Vector2.zero; //Temporary position to move in the direction of the player
    bool recallingToPlayer = false; //If the light is travelling to the player
    bool onPlayer = false; //If the light should be on top of the player and follow their movement

    Transform player; //Player Transform

    Vector3 velocity = Vector3.zero;
    Rigidbody2D rbody2D;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player").transform;
        rbody2D = this.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 targetPos = transform.position;
        Vector3 move = new Vector3(MyInput.GetRightHorizontal(), MyInput.GetRightVertical(), 0.0f);
        velocity = rbody2D.velocity;

        if (recallingToPlayer)
        {
            if (Mathf.Abs(move.x) + Mathf.Abs(move.y) > 0.2f) recallingToPlayer = false;
            else checkIfWithingPlayersReach();
        }

        if (recallingToPlayer && onPlayer) targetPos = (Vector2)player.position + Vector2.up * 30f; //On top of the player
        else if (recallingToPlayer) //Recalling movement
        {
            targetPos = positionToRecallTo; 
            targetPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, dampTime, maxSpeed * 1.3f, Time.deltaTime);
        }
        else //Normal Movement
        {
            targetPos = transform.position + move * maxSpeed;
            targetPos = KeepLightInPlayersRange(targetPos);
            targetPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, dampTime, maxSpeed, Time.deltaTime);
        }

        print("targetPos : " + targetPos);
        targetPos.z = PERMANENT_Z;
        transform.position = targetPos;
        rbody2D.velocity = velocity;
    }

    public void CallToPlayer()
    {
        if (checkIfRecallable())
        {
            positionToRecallTo = (Vector2)player.position + (Vector2.up * 30f);
            recallingToPlayer = true;
        }
    }

    public void CancelCall()
    {
        recallingToPlayer = false;
        onPlayer = false;
    }

    void checkIfWithingPlayersReach()
    {
        Vector2 p = player.position;
        Vector2 t = transform.position;
        Vector2 targetPos = (Vector2)p + (Vector2.up * 30f);

        if ((t - targetPos).sqrMagnitude < 20f)
        {
            onPlayer = true;
        }

        if ((positionToRecallTo - t).sqrMagnitude < 1500f)
        {
            positionToRecallTo = targetPos;
        }
    }

    bool checkIfRecallable()
    {
        Vector2 thisPosition = transform.position;
        Vector2 playerPosition = player.position;
        RaycastHit2D hit;
        Vector2 direction;

        int layer =~ LayerMask.GetMask("Light", "Camera");
        direction = (playerPosition - thisPosition).normalized;
        hit = Physics2D.Raycast(thisPosition, direction, 1000f, layer);
        if (hit.collider != null && hit.collider.CompareTag("Player")) return true;

        direction = ((playerPosition + Vector2.up * 10f) - thisPosition).normalized;
        hit = Physics2D.Raycast(thisPosition, direction, 1000f, layer);
        if (hit.collider != null && hit.collider.CompareTag("Player")) return true;

        direction = ((playerPosition + Vector2.up * 20f) - thisPosition).normalized;
        hit = Physics2D.Raycast(thisPosition, direction, 1000f, layer);
        if (hit.collider != null && hit.collider.CompareTag("Player")) return true;

        return false;
    }

    private Vector3 KeepLightInPlayersRange(Vector3 pos)
    {
        Vector3 p = player.position;
        float x = p.x - pos.x;
        float y = p.y - pos.y;

        if(Mathf.Abs(x) > maxDistanceToPlayer.x)
        {
            pos.x = p.x + (Mathf.Sign(x) * maxDistanceToPlayer.x * -1);
        }

        if (Mathf.Abs(y) > maxDistanceToPlayer.y)
        {
            pos.y = p.y + (Mathf.Sign(y) * maxDistanceToPlayer.y * -1);
        }

        return pos;
    }
}
