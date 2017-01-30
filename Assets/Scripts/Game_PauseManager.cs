using UnityEngine;
using System.Collections;

public class Game_PauseManager : MonoBehaviour {

	bool _paused;
	GameObject auxObj;
	// Use this for initialization
	void Start () {
		_paused = false;
		
		auxObj = GameObject.Find("AuxiliaryGameObject");
		auxObj.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (_paused) {
				Unpause ();
			} else {
				Pause ();
			}
		}
	}

	public void Unpause(){
		Time.timeScale = 1f;
		_paused = false;
		auxObj.SetActive(false);
        Cursor.visible = false;
	}

	public void Pause(){
		Time.timeScale = 0f;
		_paused = true;
		auxObj.SetActive(true);
        Cursor.visible = true;
    }
}
