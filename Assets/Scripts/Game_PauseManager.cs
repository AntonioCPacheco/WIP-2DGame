using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Game_PauseManager : MonoBehaviour {

	bool _paused;
	GameObject auxObj;
    bool alreadyPressed = false;
	// Use this for initialization
	void Start () {
        Time.timeScale = 0;
		_paused = false;
		
		auxObj = GameObject.Find("AuxiliaryGameObject");
		auxObj.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if ((!alreadyPressed && Input.GetAxisRaw("Pause")>0.1) || (Input.GetAxisRaw("unPause")>0.1 && _paused))
        {
            alreadyPressed = true;
			if (_paused) {
				Unpause ();
			} else {
				Pause ();
			}
		}
        else if(Input.GetAxisRaw("Pause") < 0.8)
        {
            alreadyPressed = false;
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
        GameObject.Find("EventSystem").GetComponent<GUI_FirstSelected>().setFirstSelected(GameObject.Find("Resume"));
        Cursor.visible = true;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void restartLevel()
    {
        Time.timeScale = 1;
        SaveManager.loadGame();
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        GameObject.Find("IntroScreen").SetActive(false);
    }
}
