﻿using UnityEngine;
using System.Collections;

public class Lock_PressurePlate : Lock_SuperClass {

    public Sprite sprTriggered;
    public Sprite sprUnTriggered;

    Transform player;
    // Use this for initialization
    void Start () {

        if (player == null)
        {
            player = GameObject.Find("Player Prefab").transform;
        }
        untriggerable = true;
        SuperStart();
    }
	

    protected override bool isBeingTriggered()
    {
        if(GetComponent<PolygonCollider2D>().IsTouching(player.GetComponent<BoxCollider2D>()))
        {
            transform.GetComponent<SpriteRenderer>().sprite = sprTriggered;
            return true;
        }
        return false;
    }

    protected override void childUnTrigger()
    {
        transform.GetComponent<SpriteRenderer>().sprite = sprUnTriggered;
        return;
    }
}
