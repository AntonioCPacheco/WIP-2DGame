using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager sm;

    private static GameObject[] boxes;

    private static DialogueTrigger[] dts;
    void Start()
    {
    }

    // Use this for initialization
    void Awake()
    {
        if (sm == null)
        {
            DontDestroyOnLoad(gameObject);
            sm = this;
        }
        else if (sm != this)
        {
            Destroy(gameObject);
        }
        dts = FindObjectsOfType<DialogueTrigger>();
        boxes = GameObject.FindGameObjectsWithTag("Box");
    }

    public static void saveGame()
    {
        Debug.Log("Game is being saved!");
        Dictionary<Vector3, bool> boxes = new Dictionary<Vector3, bool>();
        Dictionary<string, bool> triggers = new Dictionary<string, bool>();
        List<string> doors = new List<string>();

        foreach (GameObject b in SaveManager.boxes)
        {
            //if (boxes.ContainsKey(b.transform.position)) boxes.Remove(b.transform.position);
            if(b.transform.parent == null || !b.transform.parent.CompareTag("Player"))
                boxes.Add(b.transform.position, b.activeSelf);
        }

        foreach (GameObject t in GameObject.FindGameObjectsWithTag("Trigger"))
        {
            triggers.Add(t.name, t.GetComponent<Lock_BoxTrigger>().triggered);
        }

        foreach (GameObject d in GameObject.FindGameObjectsWithTag("Door"))
        {
            doors.Add(d.name);
        }

        Save save = new Save();

        save.playerHasBox = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Movement>().doesPlayerHaveBox();

        save.setPlayerPosition(GameObject.FindGameObjectWithTag("Player").transform.position);
        save.setNpcPosition(GameObject.FindGameObjectWithTag("NPC").transform.position);
        save.npcGoal = FindObjectOfType<NPC_Movement>().nextStep;
        save.npcVisible = GameObject.FindGameObjectWithTag("NPC").GetComponent<SpriteRenderer>().enabled;

        save.setBoxes(boxes);
        save.triggers = triggers;
        save.doors = doors;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/saveData.dat");

        bf.Serialize(file, save);
        file.Close();
    }

    public static void loadGame()
    {
        if(File.Exists(Application.persistentDataPath + "/saveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.OpenRead(Application.persistentDataPath + "/saveData.dat");
            Save save = (Save) bf.Deserialize(file);
            file.Close();

            GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GameObject.FindWithTag("Player").transform.position = save.getPlayerPosition();
            GameObject.FindWithTag("NPC").transform.position = save.getNpcPosition();
            FindObjectOfType<NPC_Movement>().nextStep = save.npcGoal;
            GameObject.FindGameObjectWithTag("NPC").GetComponent<SpriteRenderer>().enabled = save.npcVisible;

            foreach(DialogueTrigger t in dts)
            {
                t.gameObject.SetActive(true);
            }

            int i = 0;
            if (save.playerHasBox)
            {
                Transform box = (boxes[0]).transform;
                GameObject.FindWithTag("Player").GetComponent<Player_Movement>().setBox(box);
                i++;
            }
            else
            {
                GameObject.FindWithTag("Player").GetComponent<Player_Movement>().dropBoxAnim();
            }
            foreach(KeyValuePair<Vector3, bool> item in save.getBoxes())
            {
                boxes[i].transform.parent = null;
                boxes[i].transform.position = item.Key;
                boxes[i].SetActive(item.Value);
                i++;
            }

            foreach (KeyValuePair<string, bool> item in save.triggers)
            {
                GameObject trigger = GameObject.Find(item.Key);
                trigger.GetComponent<Lock_BoxTrigger>().isTriggered = item.Value;
            }

            foreach (string item in save.doors)
            {
                GameObject door = GameObject.Find(item);

                door.GetComponent<Player_EnterDoors>().load();
            }
        }

    }
}