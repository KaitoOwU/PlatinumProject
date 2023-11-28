using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class ControllerManager : MonoBehaviour
{
    public static ControllerManager current;
    private Dictionary<Player, Gamepad> _playerGamepad = new();

    private void Awake()
    {
        if(current != null)
            Destroy(gameObject);

        current = this;
    }

    public IEnumerator HeartBeat(float duration, float maxDelta, float minDelta)
    {
        if (maxDelta < minDelta)
            throw new ArgumentException(
                $"minDelta should always be lower or equal to maxDelta (current : {maxDelta}<{minDelta})");
        duration = Mathf.Clamp(duration, 0.4f, float.MaxValue);
     
        float currentTime = 0f;
        yield return new WaitForSeconds(1);
        while (currentTime < duration)
        {
            Gamepad.all.ToList().ForEach(gamepad => gamepad.SetMotorSpeeds(0f, 0.2f));
            yield return new WaitForSecondsRealtime(0.1f);
            Gamepad.all.ToList().ForEach(gamepad => gamepad.SetMotorSpeeds(0.1f, 0f));
            yield return new WaitForSecondsRealtime(0.3f);
            Gamepad.all.ToList().ForEach(gamepad => gamepad.SetMotorSpeeds(0f, 0f));
            
            float lerp = Mathf.Lerp(maxDelta, minDelta, currentTime / duration);
            yield return new WaitForSecondsRealtime(lerp);

            currentTime += .4f + lerp;
        }
    }

    public IEnumerator Vibrate(float duration, float lightStrength = 0.5f, float hardStrength = 0.5f)
    {
        Gamepad.all.ToList().ForEach(gamepad => gamepad.SetMotorSpeeds(hardStrength, lightStrength));
        yield return new WaitForSecondsRealtime(duration);
        Gamepad.all.ToList().ForEach(gamepad => gamepad.SetMotorSpeeds(0f, 0f));
    }

    public void Link(Player player, Gamepad gamepad)
    {
        _playerGamepad[player] = gamepad;

        if (gamepad is DualSenseGamepadHID)
        {
            ((DualSenseGamepadHID)gamepad).SetLightBarColor(player.ControllerColor);
        } else if (gamepad is DualShockGamepad)
        {
            ((DualShockGamepad)gamepad).SetLightBarColor(player.ControllerColor);
        }
    }

    public void Connected(Gamepad gamepad)
    {
        if (gamepad is DualSenseGamepadHID)
        {
            ((DualSenseGamepadHID)gamepad).SetLightBarColor(Color.white);
        } else if (gamepad is DualShockGamepad)
        {
            ((DualShockGamepad)gamepad).SetLightBarColor(Color.white);
        }
    }

    private void OnDisable()
    {
        foreach (Gamepad gamepad in Gamepad.all)
        {
            if (gamepad is DualSenseGamepadHID)
            {
                ((DualSenseGamepadHID)gamepad).SetLightBarColor(Color.black);
            } else if (gamepad is DualShockGamepad)
            {
                ((DualShockGamepad)gamepad).SetLightBarColor(Color.black);
            }
        }
    }
}
