using UnityEngine;
using System.Collections;

public class Player_Lighting : MonoBehaviour {

	Light_Settings s_settings;
    Light_Movement s_movement;

    SpringJoint2D spJoint2D;

	// Use this for initialization
	void Start () {
        GameObject l = GameObject.Find("Light");
        if (l != null && l.activeSelf)
        {
            s_settings = l.GetComponent<Light_Settings>();
            s_movement = l.GetComponent<Light_Movement>();
            spJoint2D = GetComponent<SpringJoint2D>();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (MyInput.GetRecall()) s_movement.CallToPlayer();
        else if (s_movement!=null) s_movement.CancelRecall();
    }

    public void increaseLight(int ammount)
    {
        if(s_settings != null)
            s_settings.increaseLight(ammount);
    }

    public void decreaseLight(int ammount)
    {
        if (s_settings != null)
            s_settings.decreaseLight(ammount);
    }
}
