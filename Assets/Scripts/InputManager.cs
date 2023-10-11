using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class InputManager : MonoBehaviour
{
    [Header("Events for each input")]
    public UnityEvent _OnMoveStarted;
    public UnityEvent _OnMoveCanceled;
    public UnityEvent _OnInteract;
    public UnityEvent _OnUseTool;
    public UnityEvent _OnPause;


    private PlayerInput _inputs;
    [SerializeField]
    private int index;


    private void Start()
    {
        _inputs = GetComponent<PlayerInput>();

        _SetupEvents();
        _AddController();
    }

    private void OnDisable() => _CleanEvents();
    
    private void _AddController()
    {
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        InputManager[] allControllers = FindObjectsOfType<InputManager>();

        index = allControllers.Length;

        if (index > 4)
            Debug.Log("Too many controllers : " + index);

        PlayerController player = allPlayers.FirstOrDefault(m => m.PlayerIndex == index);

        player.SetUp(this, _inputs, transform);
    }

    #region Subscription Setup & Cleanup
    private void _SetupEvents()
    {
        _inputs.actions["Move"].started += _Move_started;
        _inputs.actions["Move"].canceled += _Move_canceled;
        _inputs.actions["Move"].started += _Interact_performed;
        _inputs.actions["Tool"].started += _Tool_performed;
        _inputs.actions["Pause"].started += _Pause_performed;
    }

    private void _CleanEvents()
    {
        _inputs.actions["Move"].started -= _Move_started;
        _inputs.actions["Move"].canceled -= _Move_canceled;
        _inputs.actions["Move"].started -= _Interact_performed;
        _inputs.actions["Tool"].started -= _Tool_performed;
        _inputs.actions["Pause"].started -= _Pause_performed;
    }
    #endregion

    #region Debug Methods
    public void DebugString(string text) => Debug.Log(text);
    #endregion

    #region Invoking Methods
    private void _Move_started(InputAction.CallbackContext obj) => _OnMoveStarted?.Invoke();
    private void _Move_canceled(InputAction.CallbackContext obj) => _OnMoveCanceled?.Invoke();
    private void _Interact_performed(InputAction.CallbackContext obj) => _OnInteract?.Invoke();
    private void _Tool_performed(InputAction.CallbackContext obj) => _OnUseTool?.Invoke();
    private void _Pause_performed(InputAction.CallbackContext obj) => _OnPause?.Invoke();
    #endregion
}
