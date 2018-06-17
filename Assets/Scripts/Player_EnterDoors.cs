using UnityEngine;
using System.Collections;

public class Player_EnterDoors : MonoBehaviour {

	public Player_EnterDoors linkedDoor;
	Transform nextRoom;
	Transform currentRoom;

    public Player_EnterDoors npcDoor;

    public DialogueTrigger dialogueTrigger;

    public Lock_SuperClass[] locks;

    public bool isOpen = false;

	public bool isLadder = false;

	bool beenInitialized = false;

    public bool sameRoomAsNPC = false;

    public bool isInCutscene = false;

    EndGame end;
    bool dialogueTriggerDone = false;

	// Use this for initialization
	void Start () {
        //beenInitialized = false;
        GetComponent<Animator>().SetBool("OpenDoor", isOpen);
        end = GetComponent<EndGame>();
        if (dialogueTrigger == null) dialogueTriggerDone = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (!dialogueTriggerDone && !dialogueTrigger.gameObject.activeSelf)
        {
            dialogueTriggerDone = true;
            open();
        }
	}

	public bool HasBeenInitialized(){
		return beenInitialized;
	}

	public void Initialized(){
		beenInitialized = true;
	}

    public void changeRoom()
    {
        changeRoom(true);
    }

	public void changeRoom(bool isPlayer){
        if (isPlayer)
        {
            if (linkedDoor == null)
            {
                print("Can't go in this door");
                if (end != null) end.endGame();
                return;
            }
            Camera mainCamera = Camera.main;
            bool NPCvisible = linkedDoor.GetComponent<Player_EnterDoors>().sameRoomAsNPC;
            mainCamera.GetComponent<Camera_Movement>().followNPC = NPCvisible;
            GameObject.FindObjectOfType<NPC_Movement>().setVisibility(NPCvisible);

            linkedDoor.close();

            Vector3 v3 = linkedDoor.transform.TransformVector(linkedDoor.transform.position);
            GameObject.Find("Player Prefab").GetComponent<Player_Movement>().MovePlayerTo(v3);

            if (npcDoor != null)
            {
                GameObject.Find("NPC").GetComponent<NPC_Movement>().moveNPCto(npcDoor.transform.position);
            }

            if (end != null) end.endGame();
        }
        close();
    }

    public void close()
    {
        if (!isOpen) return;
        print("close");
        isOpen = false;
        GetComponent<Animator>().SetBool("OpenDoor", false);
    }

    public void open()
    {
        if (isOpen) return;
        print("open");
        isOpen = true;
        GetComponent<Animator>().SetBool("OpenDoor", true);
    }

    public void checkLocks()
    {
        bool auxOpen = true;
        int numLocksTriggered = 0;
        for( int i = 0; i < locks.Length; i++ )
        {
            if (!locks[i].GetComponent<Lock_SuperClass>().triggered)
            {
                auxOpen = false;
            }
            else
            {
                numLocksTriggered++;
            }
        }
        if (!isInCutscene) updateIndicators(numLocksTriggered);

        if (auxOpen && !isOpen) open();
        else if (!auxOpen && isOpen) close();
    }

    public void updateIndicators(int numLocks) //Call when a new lock triggers
    {
        this.transform.GetComponentInChildren<Door_DisplayLockStates>().updateIndicators(numLocks);
    }

    public void load()
    {
        if (!dialogueTriggerDone && dialogueTrigger.gameObject.activeSelf) close();
        else checkLocks();
    }
}
