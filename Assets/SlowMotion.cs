using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : MonoBehaviour {

    public float slowAmount;
    public float lerpTime;
    int inside = 0;
    IEnumerator coroutine;
    // Use this for initialization
    void Awake()
    {
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            if (inside == 0)
            {
                if (coroutine != null) StopCoroutine(coroutine);
                coroutine = changeTimeScale(true, Time.realtimeSinceStartup);
                StartCoroutine(coroutine);
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
                if (coroutine != null) StopCoroutine(coroutine);
                coroutine = changeTimeScale(false, Time.realtimeSinceStartup);
                StartCoroutine(coroutine);
            }
        }
    }

    IEnumerator changeTimeScale(bool slow, float startTime)
    {
        float alpha = (Time.realtimeSinceStartup - startTime) / lerpTime;
        bool triggered = false;
        bool end = GetComponent<EndGame>() != null;
        if(end) Camera.main.GetComponent<Camera_Movement>().zoomIn(lerpTime);
        while (alpha < 1) {
            if (!triggered && alpha > 0.5f && end){
                GameObject.Find("DeathScreen").GetComponent<Animator>().SetTrigger("GameOver");
                triggered = true;
            }
            if(slow)
                Time.timeScale = Mathf.Lerp(1, slowAmount, alpha);
            else
                Time.timeScale = Mathf.Lerp(slowAmount, 1, alpha);
            alpha = (Time.realtimeSinceStartup - startTime) / lerpTime;
            yield return null;
        }
    }
}
