using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint_Handler : MonoBehaviour {

    public Transform checkpoint;
    List<GameObject> checkpoints;
    public Sprite checkpointOn;

	// Use this for initialization
	void Start ()
    {
        checkpoints = new List<GameObject>();
	}

    public Vector3 getCheckpointPosition()
    {
        return checkpoint.position;
    }

    public void returnToLastCheckpoint()
    {
        FindObjectOfType<SaveManager>().loadGame();
        /*if (checkpoint != null)
            transform.position = checkpoint.position;
        else
            GameObject.Find("SceneManager").GetComponent<Game_SceneManager>().NewGame();*/
    }
}
