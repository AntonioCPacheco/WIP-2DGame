using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Movement : MonoBehaviour
{
    //public FocusLevel FocusLevel;

    //public List<GameObject> Players;

    public Vector2 OffSet;

    float lastPlayerY; //Y value of last time player was grounded

    Transform player;
    Transform light;
    Rigidbody2D rigidbody;

    public float PositionDampTime = 1.3f;

    public float SizeMax = 200f;
    public float SizeMin = 123f;

    private Vector3 CameraPosition;
    private float CameraSize;

    public float ZoomOutXBounds = 175f;
    public float ZoomOutYBounds = 100f;
    private Bounds ZoomOutBounds;

    public float PositionXBounds = 175f;
    public float PositionYBounds = 100f;
    private Bounds PositionBounds;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player").transform;
        light = GameObject.Find("Light").transform;

        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Bounds b = new Bounds();
        Vector3 position = transform.position;
        b.Encapsulate(new Vector3(position.x - ZoomOutXBounds, position.y - ZoomOutYBounds, position.z));
        b.Encapsulate(new Vector3(position.x + ZoomOutXBounds, position.y + ZoomOutYBounds, position.z));
        ZoomOutBounds = b;

        Vector3 s = new Vector3(PositionXBounds, PositionYBounds, 2000f);
        PositionBounds = new Bounds(transform.position, s);
        if (FindObjectOfType<Player_Movement>().isGrounded()) lastPlayerY = player.position.y;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        CalculateCameraLocations();
        MoveCamera();
    }

    private void MoveCamera()
    {
        Vector3 position = gameObject.transform.position;
        if (position != CameraPosition)
        {
            //Vector3 targetPosition = Vector3.zero;
            //targetPosition.x = Mathf.MoveTowards(position.x, CameraPosition.x, PositionUpdateSpeed * Time.deltaTime);
            //targetPosition.y = Mathf.MoveTowards(position.y, CameraPosition.y, PositionUpdateSpeed * Time.deltaTime);
            //targetPosition.z = position.z;

            Vector3 v = rigidbody.velocity;
            Vector3 finalPosition = Vector3.SmoothDamp(position, CameraPosition, ref v, PositionDampTime);
            
            gameObject.transform.position = finalPosition;

            rigidbody.velocity = v;
        }

        GetComponent<Camera>().orthographicSize = CameraSize;
    }

    private void CalculateCameraLocations()
    {
        Vector3 averageCenter = Vector3.zero;
        Vector3 totalPositions = Vector3.zero;
        Bounds playerBounds = new Bounds(transform.position, new Vector3());
        
        Vector3 playerPosition = (player.transform.position);
        playerPosition.y = lastPlayerY;
        Vector3 lightPosition = (light.transform.position);

        totalPositions = playerPosition + lightPosition;
        playerBounds.Encapsulate(playerPosition);
        playerBounds.Encapsulate(lightPosition);
        
        averageCenter = 0.65f*playerPosition + 0.35f*lightPosition;

        float extents = (playerBounds.extents.x + playerBounds.extents.y);
        extents = Mathf.Clamp(extents, (ZoomOutXBounds + ZoomOutYBounds) / 3, extents);
        float lerpPercent = Mathf.InverseLerp((ZoomOutXBounds + ZoomOutYBounds) / 3, (ZoomOutXBounds + ZoomOutYBounds), extents);

        CameraSize = Mathf.Lerp(SizeMin, SizeMax, lerpPercent);
        if (!PositionBounds.Contains(averageCenter))
            CameraPosition = new Vector3(averageCenter.x, averageCenter.y, CameraPosition.z);
        else
        {
            Vector3 p = (averageCenter + CameraPosition) / 2;
            CameraPosition = new Vector3(p.x, p.y, CameraPosition.z);
        }

        CameraPosition.x += OffSet.x;
        float diffLP = (playerPosition - lightPosition).y;
        CameraPosition.y += OffSet.y;
        if (diffLP > 5)
            CameraPosition.y -= diffLP*0.5f;

        CameraPosition.z = transform.position.z;
    }

    private Vector3 ClampPosition(Vector3 pos)
    {
        if (!ZoomOutBounds.Contains(pos))
        {
            float playerX = Mathf.Clamp(pos.x, ZoomOutBounds.min.x, ZoomOutBounds.max.x);
            float playerY = Mathf.Clamp(pos.y, ZoomOutBounds.min.y, ZoomOutBounds.max.y);
            float playerZ = Mathf.Clamp(pos.z, ZoomOutBounds.min.z, ZoomOutBounds.max.z);
            pos = new Vector3(playerX, playerY, playerZ);
        }

        return pos;
    }

    void OnDrawGizmos()
    {
        Vector3 s = new Vector3(PositionXBounds, PositionYBounds, Mathf.Infinity);

        Gizmos.DrawWireCube(transform.position, s);
    }
}
