using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Movement : MonoBehaviour
{
    //public FocusLevel FocusLevel;

    //public List<GameObject> Players;

    public Vector2 OffSet;

    float lastPlayerY; //Y value of last time player was grounded

    Transform playerTransform;
    Transform lightTransform;
    Rigidbody2D rbody2D;

    public float PositionDampTime = 0.5f;

    public float SizeMax = 8f;
    public float SizeMin = 5f;

    private Vector3 CameraPosition;
    private float CameraSize;

    public float ZoomOutXBounds = 20f;
    public float ZoomOutYBounds = 10f;
    private Bounds ZoomOutBounds;

    public float PositionXBounds = 20f;
    public float PositionYBounds = 10f;
    private Bounds PositionBounds;

    [SerializeField]
    private bool FollowTheLight = true;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        
        rbody2D = this.GetComponent<Rigidbody2D>();
        if (GameObject.Find("Light") == null || !GameObject.Find("Light").activeSelf) FollowTheLight = false;
        else lightTransform = GameObject.Find("Light").transform;
    }

    void Update()
    {
        Bounds b = new Bounds();
        Vector3 position = transform.position;
        b.Encapsulate(new Vector3(position.x - ZoomOutXBounds, position.y - ZoomOutYBounds, position.z));
        b.Encapsulate(new Vector3(position.x + ZoomOutXBounds, position.y + ZoomOutYBounds, position.z));
        ZoomOutBounds = b;

        Vector3 s = new Vector3(PositionXBounds, PositionYBounds, 20f);
        PositionBounds = new Bounds(transform.position, s);
        if (FindObjectOfType<Player_Movement>().isGrounded()) lastPlayerY = playerTransform.position.y;
        else print("NOT ON THE GROUND");
    }

    private void LateUpdate()
    {
        CalculateCameraLocations();
        print(CameraPosition);
        MoveCamera();
        //keepInlineWithTiles();
    }

    private void MoveCamera()
    {
        Vector3 position = gameObject.transform.position;
        if (position != CameraPosition)
        {
            Vector3 v = GetComponent<Rigidbody2D>().velocity;
            Vector3 finalPosition = Vector3.SmoothDamp(position, CameraPosition, ref v, PositionDampTime);
            
            gameObject.transform.position = finalPosition;

            GetComponent<Rigidbody2D>().velocity = v;
        }

        GetComponent<Camera>().orthographicSize = CameraSize;
    }

    private void CalculateCameraLocations()
    {
        Vector3 averageCenter = Vector3.zero;
        Bounds playerBounds = new Bounds(transform.position, new Vector3());
        
        Vector3 playerPosition = (playerTransform.transform.position);
        playerPosition.y = lastPlayerY;
        Vector3 lightPosition = FollowTheLight ? lightTransform.transform.position : playerPosition;
        
        if (!FollowTheLight || Light_Movement.onPlayer)
        {
            playerBounds.Encapsulate(playerPosition);
            averageCenter = playerPosition;
        }
        else
        { 
            playerBounds.Encapsulate(playerPosition);
            playerBounds.Encapsulate(lightPosition);

            averageCenter = (playerPosition + lightPosition) * 0.5f;
        }
        float extents = (playerBounds.extents.x + playerBounds.extents.y);
        extents = Mathf.Clamp(extents, (ZoomOutXBounds + ZoomOutYBounds) / 3, extents);
        float lerpPercent = Mathf.InverseLerp((ZoomOutXBounds + ZoomOutYBounds) / 3, (ZoomOutXBounds + ZoomOutYBounds), extents);

        CameraSize = Mathf.Lerp(SizeMin, SizeMax, lerpPercent);

        if (PositionBounds.Contains(averageCenter))
            CameraPosition = new Vector3(averageCenter.x, averageCenter.y, CameraPosition.z);
        else
        {
            Vector3 p = (averageCenter + CameraPosition) / 2;
            CameraPosition = new Vector3(p.x, p.y, CameraPosition.z);
        }

        CameraPosition.x += (Mathf.Sign(playerTransform.localScale.x) * OffSet.x);
        CameraPosition.y += OffSet.y;

        if (FollowTheLight)
        {
            float diffLP = (playerPosition - lightPosition).y;
            if (diffLP > 5)
                CameraPosition.y -= diffLP * 0.5f;
        }

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

    private void keepInlineWithTiles()
    {
        Vector3 pos = transform.position;
        pos.x = (int)(pos.x / 0.01f) * 0.01f;
        pos.y = (int)(pos.y / 0.01f) * 0.01f;
        transform.position = pos; 
    }
}
