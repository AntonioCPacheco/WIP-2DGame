using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game_RoomManager : MonoBehaviour {

	public bool generateRooms = true;

    public bool randomGeneration = true;

	public Transform prefab_InitialRoom;
	public Transform prefab_NewLevelRoom;
	public Transform prefab_FinalRoom;
	public Transform prefab_Rooms;
    public Transform prefab_Platform;

    public Transform prefab_Crawler;
    public Transform prefab_Eye;

	public Transform whiteBG;
	public Transform exitSign;

	public Transform[] items;


	//level variables
	int levelNumber = 0;
	Transform activeLevel;

	bool started = false;
	Transform activeRoom;
	int numberOfRooms;

	bool generateDoors = true;

	// Use this for initialization
	void Start () {
		if(generateRooms){
			ChangeLevel (NewLevel());
			numberOfRooms = 0;
			Transform room = initialRoom ();
			ChangeActiveRoom (room);
		}
	}
	
	// Update is called once per frame
	void ForcedUpdate () {
		if (!started) {
			Start ();
			started = true;
		}
	}


	public void ChangeActiveRoom(Transform newActiveRoom) {
		if (activeRoom != null) {
			activeRoom.gameObject.SetActive (false);
		}
		if (newActiveRoom == null) {
			Debug.Log ("[RoomManager] Who's giving out null rooms???");
		}
		newActiveRoom.gameObject.SetActive (true);
		activeRoom = newActiveRoom;

		if (activeRoom.parent != activeLevel)
			ChangeLevel (activeRoom.parent);

		//Checking if final room on floor
		if (GetActiveLadder() != null) {
			Player_EnterDoors ladder = GetActiveLadder().GetComponent<Player_EnterDoors> ();
			
			ladder.currentRoom = activeRoom;
			ladder.nextRoom = NewLevelFirstRoom();
			ladder.linkedDoor = ladder.nextRoom.Find ("Entrance");
		}

		//Creating rooms for the new doors and assigning accordingly
		List<Transform> doors = GetActiveDoors ();
		//Debug.Log (doors.Count);
		for (int i = 0; i < doors.Count; i++) {
			Player_EnterDoors doorAux = doors [i].GetComponent<Player_EnterDoors> ();
			if (!doorAux.HasBeenInitialized ()) {
				if (generateDoors) {
					Debug.Log ("" + doors [i].parent.parent.gameObject.name + " - " + doors [i].parent.gameObject.name + " - " + doors [i].gameObject.name);
					doorAux.currentRoom = activeRoom;
					if (numberOfRooms == levelNumber * 3) {
						doorAux.nextRoom = FinalRoom (doors [i]);
						Transform auxExit = (Transform)Instantiate (exitSign, doorAux.transform.position + new Vector3 (0, 40, 0), doorAux.transform.rotation);
						auxExit.SetParent (doorAux.transform);
						generateDoors = false;
					} else {
						doorAux.nextRoom = NewRoom (doors [i]);
					}
					doorAux.linkedDoor = doorAux.nextRoom.Find ("InitialDoor");
	
					doorAux.Initialized ();
				} else {
					Destroy(doors [i].gameObject);
				}
			} 
		}

		List<Transform> cases = GetActiveCases ();
		for (int i = 0; i < cases.Count; i++) {
			Case_Open caseAux = cases [i].GetComponent<Case_Open> ();
			if (!caseAux.HasBeenInitialized ()) {
				int one = 1;
				int three = 3;
				caseAux.SetItem(items [items.Length - Random.Range(one, three)]);
				caseAux.Initialized ();
			} 
		}
	}

	public Transform GetActiveRoom(){
		return activeRoom;
	}

	public Transform GetInitialDoor(){
        if (activeRoom == null) return null;
		return activeRoom.Find ("InitialDoor");
	}

	public List<Transform> GetActiveDoors(){ //InitialDoor doesn't count
		
		List<Transform> res = new List<Transform>();
		if (activeRoom != null) {
			Transform doors = activeRoom.Find ("Doors");
			int numberOfDoors = doors.childCount;
			for (int i = 1; i <= numberOfDoors; i++) {
				res.Add(doors.Find ("Door " + i));
			}
		}
		return res;
	}

	public Transform GetActiveLadder(){
		if (activeRoom != null) {
			return activeRoom.Find ("Ladder");
		}
		return null;
	}

	public List<Transform> GetActiveCases(){

		List<Transform> res = new List<Transform>();
		if (activeRoom != null) {
			Transform cases = activeRoom.Find ("Cases");
			int numberOfCases = cases.childCount;
			for (int i = 1; i <= numberOfCases; i++) {
				res.Add(cases.Find ("Case " + i));
			}
		}
		return res;
	}

	public List<Transform> GetActiveItems(){
		List<Transform> res = new List<Transform>();
		if (activeRoom != null) {
			if (activeRoom.Find ("Items") == null) {
				Transform tAux = new GameObject ().transform;
				tAux.SetParent (activeRoom);
				tAux.name = "Items";
			}
				
			Transform items = activeRoom.Find ("Items");
			for (int i = 0; i < items.childCount; i++) {
				res.Add(items.GetChild(i));
			}
		}
		return res;
	}

	//--------------------------------------------------------------------------------------
	//----------------------------------Room generation-------------------------------------
	//--------------------------------------------------------------------------------------
	Transform initialRoom(){
		Transform room = (Transform) Instantiate (prefab_InitialRoom, transform.position, transform.rotation);
		numberOfRooms++;
		room.name = "Room " + numberOfRooms;
		room.SetParent (activeLevel);
		return room;
	}

	public Transform NewRoom(Transform entranceDoor){
		Transform room = (Transform)Instantiate (prefab_Rooms.Find("Room " + pickRandomRoom()), transform.position, transform.rotation);
		numberOfRooms++;
		room.name = "Room " + numberOfRooms;

		//Creating essential components
		Transform doorAux = room.Find("InitialDoor");
		doorAux.GetComponent<Player_EnterDoors> ().linkedDoor = entranceDoor;
		doorAux.GetComponent<Player_EnterDoors> ().currentRoom = room;
		doorAux.GetComponent<Player_EnterDoors> ().nextRoom = entranceDoor.GetComponent<Player_EnterDoors>().currentRoom;
		doorAux.GetComponent<Player_EnterDoors> ().Initialized ();

		room.SetParent (activeLevel);
		room.gameObject.SetActive (false);
		return room;
	}

	Transform FinalRoom(Transform entranceDoor){
		Transform room = (Transform) Instantiate (prefab_FinalRoom, transform.position, transform.rotation);
		numberOfRooms++;
		room.name = "Room " + numberOfRooms;

		Transform doorAux = room.Find("InitialDoor");
		doorAux.GetComponent<Player_EnterDoors> ().linkedDoor = entranceDoor;
		doorAux.GetComponent<Player_EnterDoors> ().currentRoom = room;
		doorAux.GetComponent<Player_EnterDoors> ().nextRoom = entranceDoor.GetComponent<Player_EnterDoors>().currentRoom;
		doorAux.GetComponent<Player_EnterDoors> ().Initialized ();

		room.SetParent (activeLevel);
		return room;
	}

	Transform NewLevelFirstRoom(){
		Transform room = (Transform) Instantiate (prefab_NewLevelRoom, transform.position, transform.rotation);
		numberOfRooms = 1;
		room.name = "Room " + numberOfRooms;
		room.SetParent (NewLevel());
		return room;
	}

	int pickRandomRoom(){
		return Random.Range(1, prefab_Rooms.childCount+1);
	}


	//--------------------------------------------------------------------------------------
	//----------------------------------Level Handling-------------------------------------
	//--------------------------------------------------------------------------------------

	Transform NewLevel(){
		levelNumber++;
		Transform auxLevel = new GameObject().transform;
		auxLevel.name = "Level " + levelNumber;
		return auxLevel;
	}

	void ChangeLevel(Transform newLevel){
		if(activeLevel != null)
			Destroy (activeLevel.gameObject);
		activeLevel = newLevel;
		generateDoors = true;
	}

}
