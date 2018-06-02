using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
    private Queue<string> sentences;
    private int sentencesDisplayed = 0;
    private float[] npcSteps;

    int numOfSingleLines = 0;

    Transform childObject;
    Lock_SuperClass linkedLock;

    bool waitForContinue = false;

	// Use this for initialization
	void Start () {
        childObject = this.transform.Find("ChildObject");
        for(int i=0; i<childObject.childCount; i++)
        {
            childObject.GetChild(i).gameObject.SetActive(false);
        }
        sentences = new Queue<string>();
	}


    public void StartDialogue(Dialogue dialogue, float[] npcSteps, int numOfSingleLines, Lock_SuperClass linkedLock)
    {
        sentencesDisplayed = 0;
        sentences.Clear();
        if (npcSteps != null)
            this.npcSteps = npcSteps;
        this.linkedLock = linkedLock;
        this.numOfSingleLines = numOfSingleLines;
        GameObject.Find("Player Prefab").GetComponent<Player_Movement>().startDialogue();
        GameObject.Find("NPC").GetComponent<NPC_Movement>().startDialogue();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public void StartDialogue(Dialogue dialogue, float[] npcSteps, int numOfSingleLines)
    {
        StartDialogue(dialogue, npcSteps, numOfSingleLines, null);
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0 || waitForContinue)
        {
            return;
        }

        if (numOfSingleLines > 0)
        {
            numOfSingleLines--;
            DisplaySingleDialogue();
            return;
        }
        Text toChange = getIndex(sentencesDisplayed);

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence, toChange));

        sentencesDisplayed++;
    }

    void DisplaySingleDialogue()
    {
        Text toChange = getIndex(0);
        getIndex(5);

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSingleSentence(sentence, toChange));
    }

    IEnumerator TypeSingleSentence(string sentence, Text toChange)
    {
        toChange.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            toChange.text += letter;
            yield return null;
        }
        waitForContinue = true;
    }

    IEnumerator TypeSentence (string sentence, Text toChange)
    {
        toChange.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            toChange.text += letter;
            yield return null;
        }
        DisplayNextSentence();
    }

    void EndDialogue()
    {
        for (int i = 0; i < childObject.childCount; i++)
        {
            childObject.GetChild(i).gameObject.SetActive(false);
        }
        GameObject.Find("Player Prefab").GetComponent<Player_Movement>().stopDialogue();
        GameObject.Find("NPC").GetComponent<NPC_Movement>().stopDialogue();
    }

    public void pickChoice(int choice) //from 0 - 3
    {
        if (choice < 1 || choice > 4) throw new System.Exception("Choice out of range.");
        if (npcSteps == null || npcSteps.Length == 0)
        {
            EndDialogue();
            StopAllCoroutines();
            print("No steps for the NPC to take.");
            return;
        }
        //Log choice
        print(getIndex(choice).text);
        GameObject.Find("NPC").GetComponent<NPC_Movement>().setNextStep(npcSteps[choice - 1]);
        EndDialogue();
        StopAllCoroutines();
    }

    private Text getIndex(int index)
    {
        childObject.GetChild(index).gameObject.SetActive(true);
        return childObject.GetChild(index).GetComponentInChildren<Text>();
    }

    public void continueButton(){
        waitForContinue = false;
        getIndex(5).transform.parent.gameObject.SetActive(false);
        getIndex(0).transform.parent.gameObject.SetActive(false);
        StopAllCoroutines();
        DisplayNextSentence();
    }
}
