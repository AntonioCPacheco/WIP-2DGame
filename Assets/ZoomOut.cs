using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomOut : MonoBehaviour {

    public Vector2 focusPosition;
    public float newSize = 185;
    Camera_Movement mainCam;
    int inside = 0;
    // Use this for initialization
    void Awake () {
        mainCam = Camera.main.GetComponent<Camera_Movement>();
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            if (inside == 0)
            {
                mainCam.zoomOut(focusPosition, newSize);
            }
            inside++;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            inside--;
            if (inside == 0)
            {
                mainCam.zoomIn();
            }
        }
    }
}
