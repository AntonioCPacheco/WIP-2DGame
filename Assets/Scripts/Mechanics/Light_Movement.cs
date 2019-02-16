using UnityEngine;
using System.Collections;

public class Light_Movement : MonoBehaviour {
    private static float DEFAULT_Z = 0f;

    public float onPlayerMultiplier = 0.3f;
    public float dampTime = 0.2f;
    public float maxSpeed = 10f;
    public Vector2 maxDistanceToPlayer;

    Vector2 positionToRecallTo = Vector2.zero; //Temporary position to move in the direction of the player
    bool recallingToPlayer = false; //If the light is travelling to the player
    public static bool onPlayer = false; //If the light should be on top of the player and follow their movement

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

        if (recallingToPlayer || onPlayer)
        {
            if (Mathf.Abs(move.x) + Mathf.Abs(move.y) > 0.2f)
            {
                CancelOnPlayer();
                CancelRecall();
            }
            else checkIfWithinPlayersReach();
        }

        if (onPlayer) targetPos = (Vector2)player.position + Vector2.up * onPlayerMultiplier; //On top of the player
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
        targetPos.z = DEFAULT_Z;
        transform.position = targetPos;
        rbody2D.velocity = velocity;
    }

    public void CallToPlayer()
    {
        if (checkIfRecallable())
        {
            positionToRecallTo = (Vector2)player.position + (Vector2.up * onPlayerMultiplier);
            recallingToPlayer = true;
        }
    }

    public void CancelRecall()
    {
        recallingToPlayer = false;
    }

    void CancelOnPlayer()
    {
        onPlayer = false;
    }

    void checkIfWithinPlayersReach()
    {
        Vector2 p = player.position;
        Vector2 t = transform.position;
        Vector2 targetPos = (Vector2)p + (Vector2.up * onPlayerMultiplier);

        if ((t - targetPos).sqrMagnitude < 0.1f)
        {
            onPlayer = true;
        }

        if ((positionToRecallTo - t).sqrMagnitude < 10f)
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
        hit = Physics2D.Raycast(thisPosition, direction, 30f, layer);
        if (hit.collider != null && hit.collider.CompareTag("Player")) return true;

        direction = ((playerPosition + Vector2.up * 0.75f) - thisPosition).normalized;
        hit = Physics2D.Raycast(thisPosition, direction, 30f, layer);
        if (hit.collider != null && hit.collider.CompareTag("Player")) return true;

        direction = ((playerPosition + Vector2.up * 1.5f) - thisPosition).normalized;
        hit = Physics2D.Raycast(thisPosition, direction, 30f, layer);
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
