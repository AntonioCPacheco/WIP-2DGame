﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyInput {

    private static int controllerType = 0;

    public static int getControllerType()
    {
        return controllerType;
    }
    
    public static bool GetPickUp()
    {
        switch (MyInput.getControllerType())
        {
            case (-1):
                return Input.GetButtonDown("PickUpKeyboard");
            case (0):
                return Input.GetButton("PickUpXbox");
            case (1):
                return Input.GetButton("PickUpPS");
            default:
                return false;
        }
    }

    public static bool GetEnterDoor()
    {
        switch (MyInput.getControllerType())
        {
            case (-1):
                return Input.GetButtonDown("EnterDoorKeyboard");
            case (0):
                return Input.GetButton("EnterDoorXbox");
            case (1):
                return Input.GetButton("EnterDoorPS");
            default:
                return false;
        }
    }

    public static bool GetJump()
    {
        switch (MyInput.getControllerType())
        {
            case (-1):
                return Input.GetButton("JumpKeyboard");
            case (0):
                return Input.GetButton("JumpXbox");
            case (1):
                return Input.GetButton("JumpPS");
            default:
                return false;
        }
    }

    public static bool GetPause()
    {
        switch (MyInput.getControllerType())
        {
            case (-1):
                return Input.GetButtonDown("PauseKeyboard");
            case (0):
                return Input.GetButton("PauseXbox");
            case (1):
                return Input.GetButton("PausePS");
            default:
                return false;
        }
    }

    public static bool GetUnpause()
    {
        switch (MyInput.getControllerType())
        {
            case (-1):
                return Input.GetButtonDown("CancelKeyboard");
            case (0):
                return Input.GetButton("CancelXbox");
            case (1):
                return Input.GetButton("CancelPS");
            default:
                return false;
        }
    }


    public static bool GetRecall()
    {
        switch (MyInput.getControllerType())
        {
            case (-1):
                return Input.GetButton("RecallKeyboard");
            case (0):
                return Input.GetButton("RecallXbox");
            case (1):
                return Input.GetButton("RecallPS4");
            default:
                return false;
        }
    }

    public static float GetRightHorizontal()
    {
        switch (MyInput.getControllerType())
        {
            case (-1):
                return Input.GetAxis("KeyboardRightHorizontal");
            case (0):
                return Input.GetAxis("XboxRightHorizontal");
            case (1):
                return Input.GetAxis("PS4RightHorizontal");
            default:
                return 0;
        }
    }

    public static float GetRightVertical()
    {
        switch (MyInput.getControllerType())
        {
            case (-1):
                return Input.GetAxis("KeyboardRightVertical");
            case (0):
                return Input.GetAxis("XboxRightVertical");
            case (1):
                return Input.GetAxis("PS4RightVertical");
            default:
                return 0;
        }
    }



    public static void updateController()
    {
        if (Input.GetJoystickNames().Length != 0)
        {
            string c = Input.GetJoystickNames()[0];
            bool isXbox = false;
            bool isKeyboard = true;

            for(int i=0; i<Input.GetJoystickNames().Length; i++)
            {
                string joystickName = Input.GetJoystickNames()[i];
                if (joystickName.Length > 0) isKeyboard = false;
                if (joystickName.Contains("XBOX") || joystickName.Contains("Xbox"))
                {
                    isXbox = true;
                    break;
                }
            }

            if (isXbox)
            {
                MyInput.controllerType = 0;
            }
            else if(isKeyboard)
            {
                MyInput.controllerType = -1;
            }
            else
            {
                MyInput.controllerType = 1;
            }
        }
        else
        {
            MyInput.controllerType = -1;
        }
    }
}
