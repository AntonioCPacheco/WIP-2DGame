using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.SceneManagement;

public class Game_PauseManager : MonoBehaviour {

	bool _paused;
	GameObject auxObj;
    bool alreadyPressed = false;
	// Use this for initialization
	void Start () {
        Time.timeScale = 0;
		_paused = false;
		
		auxObj = transform.GetChild(0).gameObject;
		auxObj.SetActive (false);

        transform.GetChild(1).gameObject.SetActive(true);

        bool isFile = File.Exists(Application.persistentDataPath + "/saveData.dat");
        GameObject.Find("Continue").SetActive(isFile);
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
		_paused = true;
		auxObj.SetActive(true);
        GameObject.Find("EventSystem").GetComponent<GUI_FirstSelected>().setFirstSelected(GameObject.Find("Pause_Resume"));
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void restartLevel()
    {
        if(_paused) Unpause();
        SaveManager.loadGame();
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        GameObject.Find("IntroScreen").SetActive(false);
        GameObject.Find("DeathScreen").GetComponent<Animator>().SetTrigger("StartGame");
        playAtmosphericAndMusic();
    }

    public void ContinueFromSave()
    {
        StartCoroutine(muteAndWait());
        GameObject.Find("IntroScreen").SetActive(false);
        SaveManager.loadGame();
        GameObject.Find("DeathScreen").GetComponent<Animator>().SetTrigger("StartGame");
        playAtmosphericAndMusic();
    }

    IEnumerator muteAndWait()
    {
        AudioListener.volume = 0;
        Time.timeScale = 1;
        yield return new WaitForSeconds(1.7f);
        float alpha = 0;
        float start = Time.realtimeSinceStartup;
        while (alpha < 1)
        {
            AudioListener.volume = Mathf.Lerp(0, 1, alpha);
            alpha = (Time.realtimeSinceStartup - start) / 3;
            yield return null;
        }
    }

    void playAtmosphericAndMusic()
    {
        foreach (AudioSource s in Camera.main.GetComponents<AudioSource>())
        {
            if(!s.isPlaying && s.clip.name != "FinalSceneMusic")
                s.Play();
        }
    }
}
