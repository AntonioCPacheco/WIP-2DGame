using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;

public class SaveManager : MonoBehaviour
{
    public static SaveManager sm;

    private GameObject[] boxes;

    void Start()
    {
        boxes = GameObject.FindGameObjectsWithTag("Box");
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
    }

    public void saveGame()
    {
        Debug.Log(Application.persistentDataPath);
        Dictionary<Vector3, bool> boxes = new Dictionary<Vector3, bool>();
        Dictionary<int, bool> triggers = new Dictionary<int, bool>();
        List<int> doors = new List<int>();

        foreach (GameObject b in GameObject.FindGameObjectsWithTag("Box"))
        {
            if(b.transform.parent == null || !b.transform.parent.CompareTag("Player"))
                boxes.Add(b.transform.position, b.activeSelf);
        }

        foreach (GameObject t in GameObject.FindGameObjectsWithTag("Trigger"))
        {
            triggers.Add(t.GetInstanceID(), t.GetComponent<Lock_BoxTrigger>().triggered);
        }

        foreach (GameObject d in GameObject.FindGameObjectsWithTag("Door"))
        {
            doors.Add(d.GetInstanceID());
        }

        Save save = new Save();

        save.playerHasBox = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Movement>().doesPlayerHaveBox();

        save.setPlayerPosition(GameObject.FindGameObjectWithTag("Player").transform.position);
        save.setNpcPosition(GameObject.FindGameObjectWithTag("NPC").transform.position);

        save.setBoxes(boxes);
        save.triggers = triggers;
        save.doors = doors;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/saveData.dat");

        bf.Serialize(file, save);
        file.Close();
    }

    public void loadGame()
    {
        if(File.Exists(Application.persistentDataPath + "/saveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.OpenRead(Application.persistentDataPath + "/saveData.dat");
            Save save = (Save) bf.Deserialize(file);
            file.Close();

            GameObject.FindWithTag("Player").transform.position = save.getPlayerPosition();
            GameObject.FindWithTag("NPC").transform.position = save.getNpcPosition();

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
                this.boxes[i].transform.parent = null;
                this.boxes[i].transform.position = item.Key;
                this.boxes[i].SetActive(item.Value);
                i++;
            }

            foreach (KeyValuePair<int, bool> item in save.triggers)
            {
                GameObject trigger = (GameObject)EditorUtility.InstanceIDToObject(item.Key);
                trigger.GetComponent<Lock_BoxTrigger>().isTriggered = item.Value;
            }

            foreach (int item in save.doors)
            {
                GameObject door = (GameObject)EditorUtility.InstanceIDToObject(item);

                door.GetComponent<Player_EnterDoors>().load();
            }
        }
    }
}