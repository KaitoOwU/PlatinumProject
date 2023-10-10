using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput _inputs;

    [Header("Events for each input")]
    public UnityEvent _OnMoveStarted;
    public UnityEvent _OnMoveCanceled;
    public UnityEvent _OnInteract;
    public UnityEvent _OnUseTool;
    public UnityEvent _OnPause;

    public PlayerInput Inputs => _inputs;


    private void Start()
    {
        SetupEvents();
    }

    private void OnDisable()
    {
        WipeEvents();
    }

    #region Subscription Setup
    void SetupEvents()
    {
        _inputs = new PlayerInput();

        _inputs.Gameplay.Move.started += Move_performed;
        _inputs.Gameplay.Move.canceled += Move_canceled;
        _inputs.Gameplay.Interact.performed += Interact_performed;
        _inputs.Gameplay.Tool.performed += Tool_performed;
        _inputs.Gameplay.Pause.performed += Pause_performed;

        _inputs.Enable();
    }

    void WipeEvents()
    {
        _inputs.Gameplay.Move.performed -= Move_performed;
        _inputs.Gameplay.Move.canceled -= Move_canceled;
        _inputs.Gameplay.Interact.performed -= Interact_performed;
        _inputs.Gameplay.Tool.performed -= Tool_performed;
        _inputs.Gameplay.Pause.performed -= Pause_performed;

        _inputs.Disable();
    }
    #endregion

    #region Debug Methods
    public void DebugString(string text) => Debug.Log(text);
    #endregion

    #region Invoking Methods

    private void Move_performed(InputAction.CallbackContext obj) => _OnMoveStarted?.Invoke();
    private void Move_canceled(InputAction.CallbackContext obj) => _OnMoveCanceled?.Invoke();
    private void Interact_performed(InputAction.CallbackContext obj) => _OnInteract?.Invoke();
    private void Tool_performed(InputAction.CallbackContext obj) => _OnUseTool?.Invoke();
    private void Pause_performed(InputAction.CallbackContext obj) => _OnPause?.Invoke();
    #endregion
}
