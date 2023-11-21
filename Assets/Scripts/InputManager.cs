using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.Serialization;
using UnityEngine.Windows;
using static GameManager;

public class InputManager : MonoBehaviour
{
    public UnityEvent OnMoveStarted
    {
        get { return onMoveStarted; }
        set { onMoveStarted = value; }
    }
    public UnityEvent OnMoveCanceled
    {
        get { return onMoveCanceled; }
        set { onMoveCanceled = value; }
    }
    public UnityEvent<Player> OnInteract
    {
        get { return onInteract; }
        set { onInteract = value; }
    }
    public UnityEvent<Player> OnPush
    {
        get { return onPush; }
        set { onPush = value; }
    }
    public UnityEvent<Player> OnPushCanceled
    {
        get { return onPushCanceled; }
        set { onPushCanceled = value; }
    }
    public UnityEvent OnUseTool
    {
        get { return onUseTool; }
        set { onUseTool = value; }
    }
    public UnityEvent OnPause
    {
        get { return onPause; }
        set { onPause = value; }
    }
    private UnityEvent onMoveStarted = new();
    private UnityEvent onMoveCanceled = new();
    private UnityEvent<Player> onInteract = new();
    private UnityEvent<Player> onPush = new();
    private UnityEvent<Player> onPushCanceled = new();
    private UnityEvent onUseTool = new();
    private UnityEvent onPause = new();

    private PlayerInput _inputs;
    [SerializeField]
    private int _index;

    // Character Select 
    private int _playerSelected;
    private Player[] _players;
    private List<Player> _nonSelectedPlayers;
    private Bubble _playerSelectedBubble;

    private Gamepad _gamepad;

    private void Start()
    {
        _inputs = GetComponent<PlayerInput>();

        //_SetupEvents();
        //_AddController();

        _index = _inputs.playerIndex;
        _SetupSelectEvents();
        //Gamepad.all[_index].SetMotorSpeeds(1f, 1f);
        //Gamepad.all[_index + 1].SetMotorSpeeds(1f, 1f);
    }

    private void OnDisable() => _CleanEvents();
    
    private void _AddController()
    {
        //Get right character depending on controller index and launch set up (= controller corresponding character)
        GameManager.Instance.PlayerList[_playerSelected].PlayerController.SetUp(this, _inputs, transform);
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
    private void _SetupSelectEvents()
    {
        _inputs.actions["Move"].started += _SwitchCharacter;
        _inputs.actions["Interact"].started += _PickCharacter;
        _players = GameManager.Instance.PlayerList.Select(p => p.PlayerRef).ToArray();
        _nonSelectedPlayers = _players.ToList();
        _playerSelected = 0;
        _playerSelectedBubble = BubbleManager.Instance.ShowPlayerIcon(_nonSelectedPlayers[_playerSelected].transform, _nonSelectedPlayers[_playerSelected], (BubbleManager.EBubblePos)_index);
    }

    private void _CleanSelectEvents()
    {
        _inputs.actions["Move"].started -= _SwitchCharacter;
        _inputs.actions["Interact"].started -= _PickCharacter;
        Destroy(_playerSelectedBubble.gameObject);
    }
    #endregion

    public float GetInputValue(string buttonName)
    {
        return _inputs.actions[buttonName].ReadValue<float>();
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
    #region Character Select Methods
    private void _SwitchCharacter(InputAction.CallbackContext obj)
    {
        if(_playerSelectedBubble != null)
            Destroy(_playerSelectedBubble.gameObject);
        if(_inputs.actions["Move"].ReadValue<Vector2>().x > 0)
            _playerSelected = (_playerSelected + 1) % 4;
        else
        {
            _playerSelected = (_playerSelected - 1) % 4;
            _playerSelected = _playerSelected < 0 ? 3 : _playerSelected;
        }
        _playerSelectedBubble = BubbleManager.Instance.ShowPlayerIcon(_nonSelectedPlayers[_playerSelected].transform, _nonSelectedPlayers[_playerSelected], (BubbleManager.EBubblePos)_index);
    }
    private void _PickCharacter(InputAction.CallbackContext obj)
    {
        _nonSelectedPlayers.RemoveAt(_playerSelected);
        if(_nonSelectedPlayers.Count<4/*_nonSelectedPlayers.Count == 0*/)
            GameManager.Instance.CurrentGamePhase = GamePhase.HUB;
        _CleanSelectEvents();
        _SetupEvents();
        _AddController();
    }
    #endregion
}
