using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class GUI_FirstSelected : MonoBehaviour {

    private EventSystem es;
	// Use this for initialization
	void Start () {
        es = this.GetComponent<EventSystem>();
	}
	
    public void setFirstSelected(GameObject firstSelected)
    {
        print(firstSelected.name);
        //firstSelected.GetComponent<Button>(). = firstSelected.GetComponent<Button>().spriteState.highlightedSprite;
        //if (es.currentSelectedGameObject != firstSelected) 
            es.SetSelectedGameObject(firstSelected);
        
    }
}
