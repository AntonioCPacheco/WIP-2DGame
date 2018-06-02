using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    public MovingPlatform platform;
    public float timeToWait = 0f;

    public Dialogue dialogue;
    public float[] npcSteps;
    public int numOfSingleLines = 0;

    public void TriggerDialogue()
    {
        StartCoroutine(waitForTime(timeToWait));
    }

    IEnumerator waitForTime(float time)
    {
        print(Time.time);
        if (platform != null)
        {
            platform.enable();
        }
        yield return new WaitForSeconds(time);
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, npcSteps, numOfSingleLines);
        this.gameObject.SetActive(false);
        print(Time.time);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            TriggerDialogue();
        }
    }
}
