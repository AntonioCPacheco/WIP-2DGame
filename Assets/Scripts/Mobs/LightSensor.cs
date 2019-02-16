using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSensor : MonoBehaviour
{
    private bool isInLight = false;

    public void OnEnterLight()
    {
        isInLight = true;
    }

    public void OnExitLight()
    {
        isInLight = false;
    }

    public bool IsInLight()
    {
        return isInLight;
    }
}
