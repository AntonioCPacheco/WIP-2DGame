using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class DialogueManager : MonoBehaviour {
    public bool assertive = true;

    private Queue<string> sentences;
    private int sentencesDisplayed = 0;
    private float[] npcSteps;

    int numOfSingleLines = 0;
    Transform childObject;
    Lock_SuperClass linkedLock;
    GUI_FirstSelected es;
    bool waitForContinue = false;

    int numberOfPicks = 0;
    int rockPaperScissors = -1;

    AudioSource[] sources;

    IEnumerator lastCoroutine;

	// Use this for initialization
	void Start () {
        Application.targetFrameRate = 30;
        initializeTriggers();

        childObject = this.transform.Find("ChildObject");
        for(int i=0; i<childObject.childCount; i++)
        {
            childObject.GetChild(i).gameObject.SetActive(false);
        }
        sentences = new Queue<string>();
        es = GameObject.FindObjectOfType<GUI_FirstSelected>();
        sources = GetComponents<AudioSource>();
    }

    //Functions to set everything when Dialogue Trigger is triggered
    public void StartDialogue(Dialogue dialogue, float[] npcSteps, int numOfSingleLines, Lock_SuperClass linkedLock)
    {
        numberOfPicks = 0;
        sentencesDisplayed = 0;
        sentences.Clear();

        if (npcSteps != null)
            this.npcSteps = npcSteps;

        this.linkedLock = linkedLock;
        this.numOfSingleLines = numOfSingleLines;
        GameObject.Find("Player Prefab").GetComponent<Player_Movement>().startDialogue();
        GameObject.Find("NPC").GetComponent<NPC_Movement>().startDialogue();

        foreach (string sentence in dialogue.sentences){ sentences.Enqueue(sentence); }
        DisplayNextSentence();
    }

    public void StartDialogue(Dialogue dialogue, float[] npcSteps, int numOfSingleLines)
    {
        StartDialogue(dialogue, npcSteps, numOfSingleLines, null);
    }

    //Setting if rockpaper or not
    public void SetRockPaperScissors(int triggerAfterXChoices)
    {
        this.rockPaperScissors = triggerAfterXChoices;
    }

    public void SetRockPaperScissors()
    {
        this.rockPaperScissors = -1;
    }

    public void SetRockPaperScissorsResult(int choice)
    {
        string sentence = "";
        switch (choice)
        {
            case 1:
                sentence = "Rock vs. Scissors!\n You win. I'm going left.";
                break;
            case 2:
                sentence = "Paper vs. Rock!\n You win. I'm going left.";
                break;
            case 3:
                sentence = "Scissors vs. Paper!\n You win. I'm going left.";
                break;
        }

        this.rockPaperScissors = -2;
        Text toChange = getIndex(0);
        getIndex(5);
        HandleFirstSelected(5);

        StopAllCoroutines();
        StartCoroutine(TypeSingleSentence(sentence, toChange));
    }

    //Displaying Sentences
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
        if(sentences.Count == 0) HandleFirstSelected(1);
        if (sentencesDisplayed++ == 0)
        {
            if (lastCoroutine != null)
                StopCoroutine(lastCoroutine);

            lastCoroutine = TypeSentence(sentence, toChange);
            StartCoroutine(lastCoroutine);
        }
        else
        {
            DisplaySentence(sentence, toChange);
        }
        
    }

    void DisplaySentence(string sentence, Text toChange)
    {
        toChange.text = sentence;
        DisplayNextSentence();
    }

    void DisplaySingleDialogue()
    {
        Text toChange = getIndex(0);
        getIndex(5);
        HandleFirstSelected(5);

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSingleSentence(sentence, toChange));
    }

    //Typing one letter at a time
    IEnumerator TypeSingleSentence(string sentence, Text toChange)
    {
        int s = 0;
        toChange.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            playSound(this, letter, s);
            s = (s + 1) % 4;
            toChange.text += letter;
            yield return null;
        }
        sources[1].Stop();
        sources[2].Stop();
        sources[3].Stop();
        sources[4].Stop();
        waitForContinue = true;
    }

    IEnumerator TypeSentence (string sentence, Text toChange)
    {
        int s = 0;
        toChange.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            playSound(this, letter, s);
            s = (s + 1) % 4;
            toChange.text += letter;
            yield return null;
        }
        sources[1].Stop();
        sources[2].Stop();
        sources[3].Stop();
        sources[4].Stop();
        DisplayNextSentence();
    }

    //Playing sound for each letter
    static void playSound(DialogueManager dm, char c, int s)
    {
        if (c != ' ')
        {
            switch (s)
            {
                case 0:
                    dm.sources[1].Play();
                    break;
                case 1:
                    dm.sources[2].Play();
                    break;
                case 2:
                    dm.sources[3].Play();
                    break;
                case 3:
                    dm.sources[4].Play();
                    break;
            }
        }
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

    //Function called when a button is pressed
    public void pickChoice(int choice) //from 1 - 4
    {
        if (choice < 1 || choice > 4) throw new System.Exception("Choice out of range.");

        sources[0].Play();
        numberOfPicks++;
        if(rockPaperScissors > -1 && numberOfPicks == rockPaperScissors)
        {
            this.GetComponent<RockPaperScissors>().StartRPS(choice == 1);
            for (int i = 0; i < childObject.childCount; i++)
            {
                childObject.GetChild(i).gameObject.SetActive(false);
            }
            StopAllCoroutines();
            return;
        }

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

    //Returns child of "ChildObject" correspondent to index
    private Text getIndex(int index)
    {
        childObject.GetChild(index).gameObject.SetActive(true);
        return childObject.GetChild(index).GetComponentInChildren<Text>();
    }

    public void continueButton(){
        numberOfPicks++;

        getIndex(5).transform.parent.gameObject.SetActive(false);
        getIndex(0).transform.parent.gameObject.SetActive(false);

        sources[0].Play();
        waitForContinue = false;
        StopAllCoroutines();

        if (rockPaperScissors != -2)
        {
            if (numOfSingleLines == 0) HandleFirstSelected(1);
            DisplayNextSentence();
        }
        else
        {
            rockPaperScissors = -1;
            GameObject.Find("NPC").GetComponent<NPC_Movement>().setNextStep(npcSteps[0]);
            EndDialogue();
        }
    }

    //Manage which option is selected
    void HandleFirstSelected(int index)
    {
        es.setFirstSelected(childObject.GetChild(index).gameObject);
    }

    //Initialize Dialogue triggers with text from txt file
    void initializeTriggers()
    {
        string path = Application.dataPath + "/Dialogue/dialogue" + (assertive ? "Assertive" : "Submissive") + ".txt";
        if (!File.Exists(path)) return;

        StreamReader reader = new StreamReader(path);
        List<string> dialogueList = parseTriggerText(reader.ReadToEnd());
        reader.Close();

        for (int i = 1; i <= dialogueList.Count && GameObject.Find("DialogueTrigger " + i) != null; i++)
        {
            GameObject.Find("DialogueTrigger " + i).GetComponent<DialogueTrigger>().initialize(dialogueList[i-1]);
        }
    }

    List<string> parseTriggerText(string file)
    {
        List<string> res = new List<string>();
        string[] triggers = file.Split('|');
        res.AddRange(triggers);
        return res;
    }
}
