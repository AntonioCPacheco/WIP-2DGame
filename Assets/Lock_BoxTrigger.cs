using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock_BoxTrigger : Lock_SuperClass {

    public Sprite sprTriggered;
    public Sprite sprUnTriggered;

    public GameObject triggeredCollider;

    public bool isTriggered;

    Transform player;
    // Use this for initialization
    void Start()
    {
        triggeredCollider.SetActive(false);
        if (player == null)
        {
            player = GameObject.Find("Player Prefab").transform;
        }
        untriggerable = false;
        SuperStart();
    }

    protected override bool isBeingTriggered()
    {
        return isTriggered;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            other.gameObject.SetActive(false);
            trigger();
            isTriggered = true;
            transform.GetComponent<SpriteRenderer>().sprite = sprTriggered;
            triggeredCollider.SetActive(true);
        }
    }

    protected override void childUnTrigger()
    {
        transform.GetComponent<SpriteRenderer>().sprite = sprUnTriggered;
        isTriggered = false;
        return;
    }
}
