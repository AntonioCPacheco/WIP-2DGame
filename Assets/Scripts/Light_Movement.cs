using UnityEngine;
using System.Collections;

public class Light_Movement : MonoBehaviour {

    Vector3 mouseposition;
    public float mouseZ { get; set; }

	// Use this for initialization
	void Start () {
        mouseZ = -62.3f;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.timeScale != 0f)
        {
            mouseposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseposition.z = mouseZ;
            transform.position = mouseposition;
        }
    }
}
