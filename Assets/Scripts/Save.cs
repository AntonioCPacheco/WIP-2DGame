using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    Dictionary<Vector3Ser, bool> boxes = new Dictionary<Vector3Ser, bool>(); //Maps boxes positions to wether they're enabled or not
    public Dictionary<int, bool> triggers = new Dictionary<int, bool>(); //Maps triggers' instanceIDs to triggered variables
    public List<int> doors = new List<int>(); //Maps doors' instanceIDs to open variables

    public bool playerHasBox = false;

    Vector3Ser playerPosition;
    Vector3Ser npcPosition;

    public void setBoxes(Dictionary<Vector3, bool> boxes)
    {
        foreach(KeyValuePair<Vector3, bool> item in boxes)
        {
            this.boxes.Add(new Vector3Ser(item.Key), item.Value);
        }
    }

    public Dictionary<Vector3, bool> getBoxes()
    {
        Dictionary<Vector3, bool> res = new Dictionary<Vector3, bool>();
        foreach (KeyValuePair<Vector3Ser, bool> item in this.boxes)
        {
            res.Add(item.Key.getVector3(), item.Value);
        }
        return res;
    }

    public void setPlayerPosition(Vector3 position)
    {
        playerPosition = new Vector3Ser(position);
    }

    public void setNpcPosition(Vector3 position)
    {
        npcPosition = new Vector3Ser(position);
    }

    public Vector3 getPlayerPosition()
    {
        return playerPosition.getVector3();
    }
    public Vector3 getNpcPosition()
    {
        return npcPosition.getVector3();
    }
}

[System.Serializable]
public class Vector3Ser
{
    float x, y, z;

    public Vector3Ser(Vector3 vec)
    {
        x = vec.x;
        y = vec.y;
        z = vec.z;
    }

    public Vector3Ser(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 getVector3()
    {
        return new Vector3(x, y, z);
    }
}