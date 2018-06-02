using UnityEngine;
using System.Collections;

public class Player_EnterDoors : MonoBehaviour {

	public Transform linkedDoor;
	public Transform nextRoom;
	public Transform currentRoom;

    public Transform npcDoor;

    public Transform[] locks;

    public bool isOpen = false;

	public bool isLadder = false;

	bool beenInitialized = false;

    public bool sameRoomAsNPC = false;

    public bool isInCutscene = false;

    EndGame end;

	// Use this for initialization
	void Start () {
        //beenInitialized = false;
        GetComponent<Animator>().SetBool("OpenDoor", isOpen);
        end = GetComponent<EndGame>();
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
        if (linkedDoor == null)
        {
            print("Can't go in this door");
            if (end != null) end.endGame();
            return;
        }
        Camera mainCamera = Camera.main;
        mainCamera.GetComponent<Camera_Movement>().followNPC = linkedDoor.GetComponent<Player_EnterDoors>().sameRoomAsNPC;
		Vector3 v3 = linkedDoor.TransformVector(linkedDoor.position);
		GameObject.Find("Player Prefab").GetComponent<Player_Movement>().MovePlayerTo(v3);

        if(npcDoor != null)
        {
            GameObject.Find("NPC").GetComponent<NPC_Movement>().moveNPCto(npcDoor.position);
        }

        if (end != null) end.endGame();
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
        if (!isInCutscene) updateIndicators();
    }

    public void updateIndicators() //Call when a new lock triggers
    {
        this.transform.GetComponentInChildren<Door_DisplayLockStates>().triggerLock();
    }
}
