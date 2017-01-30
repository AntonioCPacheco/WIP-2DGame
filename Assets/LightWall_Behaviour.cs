using UnityEngine;
using System.Collections;

public class LightWall_Behaviour : MonoBehaviour {
    
    bool v_isInLight = false;
    bool lastFrame = false;

    public bool inverted = false;
    
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(v_isInLight != lastFrame)
        {            
            lastFrame = v_isInLight;
            GetComponent<BoxCollider2D>().isTrigger = !v_isInLight;
            if (inverted)
                GetComponent<BoxCollider2D>().isTrigger = !GetComponent<BoxCollider2D>().isTrigger;
        }
    }

    public void InLight(bool l)
    {
        v_isInLight = l;
    }

    public bool IsInLight()
    {
        return v_isInLight;
    }
}
