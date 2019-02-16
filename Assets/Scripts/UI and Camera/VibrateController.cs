using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class VibrateController : MonoBehaviour {

    public const float NONE = 0.0f;
    public const float SUPER_SOFT = 0.1f;
    public const float SOFT = 0.2f;
    public const float MEDIUM = 0.4f;
    public const float MEDIUM_HARD = 0.5f;
    public const float HARD = 0.7f;
    public const float HARD_AF = 1f;

    static float seconds = 0;
    IEnumerator lastCoroutine;

    public static void vibrateControllerForXSeconds(float seconds, float left, float right)
    {
        VibrateController.seconds = seconds;
        GamePad.SetVibration(PlayerIndex.One, left, right);
    }

    public static void vibrateControlUntilToldOtherwise(float left, float right)
    {
        vibrateControllerForXSeconds(1000f, left, right);
    }

    void FixedUpdate()
    {
        if(seconds != 0)
        {
            if (lastCoroutine != null) StopCoroutine(lastCoroutine);
            lastCoroutine = waitUntilTurnOff(seconds);
            StartCoroutine(lastCoroutine);
            seconds = 0;
        }
    }

    IEnumerator waitUntilTurnOff(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GamePad.SetVibration(PlayerIndex.One, 0, 0);
    }

    public static void stopVibrations()
    {
        GamePad.SetVibration(PlayerIndex.One, 0, 0);
    }

    public enum VIBRATION_INTENSITY
    {
        NONE,
        SUPER_SOFT,
        SOFT,
        MEDIUM,
        MEDIUM_HARD,
        HARD,
        HARD_AF
    }
}
