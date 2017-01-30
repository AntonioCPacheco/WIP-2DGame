using UnityEngine;
using System.Collections;

public class Player_EnterDoors : MonoBehaviour {

	public Transform linkedDoor;
	public Transform nextRoom;
	public Transform currentRoom;

    public Transform[] locks;

    public bool isOpen = false;

	public bool isLadder = false;

	bool beenInitialized = false;

	// Use this for initialization
	void Start () {
		//beenInitialized = false;
	}

    void Awake()
    {
        //GetComponent<Animator>().SetBool("OpenDoor", isOpen);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool HasBeenInitialized(){
		return beenInitialized;
	}

	public void Initialized(){
		beenInitialized = true;
	}

	public void changeRoom(){
		Vector3 v3 = linkedDoor.TransformVector(linkedDoor.position);
		GameObject.Find("Player Prefab").GetComponent<Player_Movement>().MovePlayerTo(v3);
		Debug.Log ("Changing to Room : " + nextRoom.gameObject.name);
		GameObject.Find ("RoomManager").GetComponent<Game_RoomManager> ().ChangeActiveRoom (nextRoom);
	}

    public void open()
    {
        isOpen = true;
        GetComponent<Animator>().SetBool("OpenDoor", true);
    }

    public void checkLocks()
    {
        bool auxOpen = true;
        for( int i = 0; i < locks.Length && auxOpen; i++ )
        {
            if (!locks[i].GetComponent<Lock_SuperClass>().triggered) auxOpen = false;
        }
        if (auxOpen && !isOpen) open();
    }
}
