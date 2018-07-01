using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    public MovingPlatform platform;
    public float timeToWait = 0f;

    public Dialogue dialogue;
    public float[] npcSteps;
    public int numOfSingleLines = 0;
    public bool onlyPlayerTrigger = true;

    bool triggeredPlayer = false;
    bool triggeredNPC = false;

    public void initialize(string dialogueLines)
    {
        string[] lines = dialogueLines.Split('-');
        dialogue = new Dialogue(lines);
    }

    public void TriggerDialogue()
    {
        StartCoroutine(waitForTime(timeToWait));
    }

    IEnumerator waitForTime(float time)
    {
        print(Time.time);
        
        yield return new WaitForSeconds(time/2);
        if (platform != null)
        {
            platform.enable();
        }
        yield return new WaitForSeconds(time/2);
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, npcSteps, numOfSingleLines);
        if (GetComponent<RockPaperScissorsTrigger>() != null)
        {
            int triggerAfterXChoices = GetComponent<RockPaperScissorsTrigger>().triggerAfterXChoices;
            FindObjectOfType<DialogueManager>().SetRockPaperScissors(triggerAfterXChoices);
        }
        this.gameObject.SetActive(false);
        print(Time.time);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (onlyPlayerTrigger || triggeredNPC)
                TriggerDialogue();
            else
                triggeredPlayer = true;
        }
        else if (other.CompareTag("NPC"))
        {
            if (triggeredPlayer)
                TriggerDialogue();
            else
                triggeredNPC = true;
        }
    }
}
