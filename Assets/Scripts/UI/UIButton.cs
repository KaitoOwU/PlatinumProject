using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    private void Start()
    {
        foreach (DualSenseGamepadHID gamepad in DualSenseGamepadHID.all)
        {
            gamepad.SetMotorSpeedsAndLightBarColor(1f, 1f, Color.red);
        }
    }
}