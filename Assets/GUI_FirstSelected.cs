using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class GUI_FirstSelected : MonoBehaviour {

    private EventSystem es;
	// Use this for initialization
	void Start () {
        es = this.GetComponent<EventSystem>();
	}
	
    public void setFirstSelected(GameObject firstSelected)
    {
        es.SetSelectedGameObject(firstSelected);
    }
}
