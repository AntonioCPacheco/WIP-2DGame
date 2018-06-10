using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBounce : MonoBehaviour {

    Rigidbody2D rBody;
	// Use this for initialization
	void Start () {
        rBody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void addDirectionalForce(Vector2 direction, float force)
    {
        rBody.AddForce(direction * force, ForceMode2D.Impulse);
    }
}
