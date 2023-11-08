using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Windows;

public class InputManager : MonoBehaviour
{
    [Header("Events for each input")]
    public UnityEvent OnMoveStarted;
    public UnityEvent OnMoveCanceled;
    public UnityEvent<Player> OnInteract;
    public UnityEvent<Player> OnPush;
    public UnityEvent<Player> OnPushCanceled;
    public UnityEvent OnUseTool;
    public UnityEvent OnPause;

    private PlayerInput _inputs;
    [SerializeField]
    private int _index;

    private void Start()
    {
        _inputs = GetComponent<PlayerInput>();

        _SetupEvents();
        _AddController();
    }

    private void OnDisable() => _CleanEvents();
    
    private void _AddController()
    {
        //Get right character depending on controller index and launch set up (= controller corresponding character)
        GameManager.Instance.PlayerList[_inputs.playerIndex].PlayerController.SetUp(this, _inputs, transform);
        _index = _inputs.playerIndex;
    }

    #region Subscription Setup & Cleanup
    private void _SetupEvents()
    {
        _inputs.actions["Move"].started += _Move_started;
        _inputs.actions["Move"].canceled += _Move_canceled;
        _inputs.actions["Interact"].started += _Interact_performed;
        _inputs.actions["Push"].started += _Push_performed;
        _inputs.actions["Push"].canceled += _Push_canceled;
        _inputs.actions["Tool"].started += _Tool_performed;
        _inputs.actions["Pause"].started += _Pause_performed;
    }

    private void _CleanEvents()
    {
        _inputs.actions["Move"].started -= _Move_started;
        _inputs.actions["Move"].canceled -= _Move_canceled;
        _inputs.actions["Interact"].started -= _Interact_performed;
        _inputs.actions["Push"].started -= _Push_performed;
        _inputs.actions["Push"].canceled -= _Push_canceled;
        _inputs.actions["Tool"].started -= _Tool_performed;
        _inputs.actions["Pause"].started -= _Pause_performed;
    }
    #endregion

    public T GetInputValue<T>(string buttonName) where T : struct
    {
        return _inputs.actions[buttonName].ReadValue<T>();
    }

    #region Debug Methods
    public void DebugString(string text) => Debug.Log(text);
    #endregion

    #region Invoking Methods
    private void _Move_started(InputAction.CallbackContext obj) => OnMoveStarted?.Invoke();
    private void _Move_canceled(InputAction.CallbackContext obj) => OnMoveCanceled?.Invoke();
    private void _Interact_performed(InputAction.CallbackContext obj) => OnInteract?.Invoke(GameManager.Instance.PlayerList[_index].PlayerRef);
    private void _Push_performed(InputAction.CallbackContext obj) => OnPush?.Invoke(GameManager.Instance.PlayerList[_index].PlayerRef);
    private void _Push_canceled(InputAction.CallbackContext obj) => OnPushCanceled?.Invoke(GameManager.Instance.PlayerList[_index].PlayerRef);
    private void _Tool_performed(InputAction.CallbackContext obj) => OnUseTool?.Invoke();
    private void _Pause_performed(InputAction.CallbackContext obj) => OnPause?.Invoke();
    #endregion
}
