﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableFollowNPC : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player") && GameObject.Find("NPC") != null)
        {
            //Camera.main.GetComponent<Old_Camera_Movement>().followNPC = false;
        }
    }
}