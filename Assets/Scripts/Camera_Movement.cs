using UnityEngine;
using System.Collections;

public class Camera_Movement : MonoBehaviour
{

    public float lookAhead = 3f;
    public float lookUp = 0f;
    public float vDampTime = 0.15f;
    public float hDampTime = 0.15f;
    public float minMouseDistance = 30f;

    public bool followPlayer = true;
    public bool followMouse = true;

    public Vector3 camPositionSnap = Vector3.zero;

    Vector3 velocity = Vector3.zero;

    Transform player;
    Vector3 mouse;
    public Vector3 topLeft = new Vector3(-3000f, 3000f, 0f);
    public Vector3 bottomRight = new Vector3(3000f, -3000f, 0f);
    Vector3 target;

    bool isGrounded = true;

    Camera cam;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player Prefab").transform;
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = player.gameObject.GetComponent<Player_Movement>().isGrounded();
        if (followPlayer)
        {
            mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseRelative = mouse - player.position;

            if (followMouse && mouseRelative.magnitude > minMouseDistance)
            {
                Vector2 compensation2D = new Vector2();
                if (Mathf.Abs(mouseRelative.x / mouseRelative.y) <= 0.5f)
                    compensation2D = new Vector2(0, Mathf.Sign(mouseRelative.y));
                else if (Mathf.Abs(mouseRelative.x / mouseRelative.y) >= 0.5f && Mathf.Abs(mouseRelative.y / mouseRelative.x) >= 0.5f)
                    compensation2D = new Vector2(Mathf.Sign(mouseRelative.x), Mathf.Sign(mouseRelative.y));
                else if (Mathf.Abs(mouseRelative.y / mouseRelative.x) <= 0.5f)
                    compensation2D = new Vector2(Mathf.Sign(mouseRelative.x), 0);

                compensation2D = lookAhead * compensation2D;
                float targetX = (followMouse ? compensation2D.x : 0);
                float targetY = (followMouse ? compensation2D.y : 0);
                target = new Vector3((player.position.x + targetX), (player.position.y + targetY), (player.position.z));
            }
            else
            {
                target = player.position + new Vector3(lookAhead * player.localScale.x, lookUp, 0);
            }
        }
        else{ target = camPositionSnap;}

        Vector3 point = cam.WorldToViewportPoint(target);
        Vector3 delta = target - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
        delta = new Vector3(delta.x, (isGrounded ? delta.y : 0f), delta.z);
        Vector3 destination = transform.position + delta;

        //correcting X
        if (destination.x > (bottomRight.x - 200))
            destination = new Vector3(bottomRight.x - 200, destination.y, destination.z);
        else if (destination.x < (topLeft.x + 200))
            destination = new Vector3(topLeft.x + 200, destination.y, destination.z);

        //correcting Y
        if (destination.y < (bottomRight.y + 123.5f))
            destination = new Vector3(destination.x, bottomRight.y + 123.5f, destination.z);
        else if (destination.y > (topLeft.y - 123.5f))
            destination = new Vector3(destination.x, topLeft.y - 123.5f, destination.z);

        Vector3 vDamp = destination;
        vDamp.x = transform.position.x;
        vDamp = Vector3.SmoothDamp(transform.position, vDamp, ref velocity, vDampTime);
        Vector3 hDamp = vDamp;
        hDamp.x = destination.x;
        transform.position = Vector3.SmoothDamp(vDamp, hDamp, ref velocity, hDampTime);
    }
}
