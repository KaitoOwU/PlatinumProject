using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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

    public UnityEvent<Player> OnAskTPHubStarted
    {
        get => _onAskTPHubStarted;
        set => _onAskTPHubStarted = value;
    }
    
    public UnityEvent<Player> OnAskTPHubCanceled
    {
        get => _onAskTPHubCanceled;
        set => _onAskTPHubCanceled = value;
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
    public UnityEvent OnBack
    {
        get { return onBack; }
        set { onBack = value; }
    }
    public bool InputLocked
    {
        get { return _inputLocked; }
        set { _inputLocked = value; }
    }
    private UnityEvent onMoveStarted = new();
    private UnityEvent onMoveCanceled = new();
    private UnityEvent<Player> onInteract = new();
    private UnityEvent<Player> _onAskTPHubStarted = new();
    private UnityEvent<Player> _onAskTPHubCanceled = new();
    private UnityEvent<Player> onPush = new();
    private UnityEvent<Player> onPushCanceled = new();
    private UnityEvent onUseTool = new();
    private UnityEvent onPause = new();
    private UnityEvent onBack = new();

    private bool _inputLocked;
    private PlayerInput _map;
    private int _controllerIndex;

    public PlayerInput Map => _map;

    // Character Select 
    private int _playerSelectedIndex;
    private BubbleManager _playerSelectedBubbleManager;
    private Player[] _players;
    [SerializeField]
    //private static List<Player> _gm.NonSelectedPlayers;
    GameManager _gm;
    //private Bubble _playerSelectedBubble;

    private void Start()
    {
        _map = GetComponent<PlayerInput>();

        //_SetupEvents();
        //_AddController();

        _controllerIndex = _map.playerIndex;
        ControllerManager.current.Connected(Gamepad.all[_controllerIndex]);
        _SetupSelectEvents();
        //Gamepad.all[_index].SetMotorSpeeds(1f, 1f);
        //Gamepad.all[_index + 1].SetMotorSpeeds(1f, 1f);

        StartCoroutine(UIMessageGenerator.instance.Init(
            new UIMessageData("Duchesse", "WOA LA DINGUERIE C'EST UN DIALOGUE DE FOU NAN C'EST INSANE !!!!", 1f),
            new UIMessageData("Cook", "Oui en effet c'est assez technique de fou", 1f)
            ));
    }

    private void OnDisable() => _CleanEvents();

    private void _AddController()
    {
        //Get right character depending on controller index and launch set up (= controller corresponding character)
        GameManager.Instance.PlayerList[_playerSelectedIndex].PlayerController.SetUp(this, _map, transform);
    }

    #region Subscription Setup & Cleanup
    private void _SetupEvents()
    {
        _map.actions["Move"].started += _Move_started;
        _map.actions["Move"].canceled += _Move_canceled;
        _map.actions["Interact"].started += _Interact_performed;
        _map.actions["AskTPHub"].started += _AskTPHub_started;
        _map.actions["AskTPHub"].canceled += _AskTPHub_canceled;
        _map.actions["Push"].started += _Push_performed;
        _map.actions["Push"].canceled += _Push_canceled;
        _map.actions["Tool"].started += _Tool_performed;
        _map.actions["Pause"].started += _Pause_performed;
        _map.actions["Back"].started += _Back_performed;
    }

    private void _CleanEvents()
    {
        _map.actions["Move"].started -= _Move_started;
        _map.actions["Move"].canceled -= _Move_canceled;
        _map.actions["Interact"].started -= _Interact_performed;
        _map.actions["AskTPHub"].started -= _AskTPHub_started;
        _map.actions["AskTPHub"].canceled -= _AskTPHub_canceled;
        _map.actions["Push"].started -= _Push_performed;
        _map.actions["Push"].canceled -= _Push_canceled;
        _map.actions["Tool"].started -= _Tool_performed;
        _map.actions["Pause"].started -= _Pause_performed;
        _map.actions["Back"].started -= _Back_performed;
    }

    private void _SetupSelectEvents()
    {
        _gm = GameManager.Instance;
        _map.actions["Move"].started += _SwitchCharacter;
        _map.actions["Interact"].started += _PickCharacter;
        _players = _gm.PlayerList.Select(p => p.PlayerRef).ToArray();
        _playerSelectedIndex = 0;
        _playerSelectedBubbleManager = _gm.NonSelectedPlayers[_playerSelectedIndex].GetComponent<BubbleManager>();
        _playerSelectedBubbleManager?.AddControllerIcon(_controllerIndex);
    }

    private void _CleanSelectEvents()
    {
        _map.actions["Move"].started -= _SwitchCharacter;
        _map.actions["Interact"].started -= _PickCharacter;
        _playerSelectedBubbleManager?.RemoveAllBubbles();
    }
    #endregion

    public float GetInputValue(string buttonName)
    {
        return _map.actions[buttonName].ReadValue<float>();
    }

    #region Debug Methods
    public void DebugString(string text) => Debug.Log(text);
    #endregion

    #region Invoking Methods
    private void _Move_started(InputAction.CallbackContext obj)
    {
        if (_inputLocked)
            return;
        OnMoveStarted?.Invoke();
    }

    private void _Move_canceled(InputAction.CallbackContext obj)
    {
        if (_inputLocked)
            return;
        OnMoveCanceled?.Invoke();
    }

    private void _Back_performed(InputAction.CallbackContext obj)
    {
        if (_inputLocked)
            return;
        OnBack?.Invoke();
    }

    private void _Interact_performed(InputAction.CallbackContext obj)
    {
        if (_inputLocked)
            return;
        OnInteract?.Invoke(GameManager.Instance.PlayerList[_playerSelectedIndex].PlayerRef);
    }

    private void _Push_performed(InputAction.CallbackContext obj)
    {
        if (_inputLocked)
            return;
        OnPush?.Invoke(GameManager.Instance.PlayerList[_playerSelectedIndex].PlayerRef);
    }

    private void _Push_canceled(InputAction.CallbackContext obj)
    {
        if (_inputLocked)
            return;
        OnPushCanceled?.Invoke(GameManager.Instance.PlayerList[_playerSelectedIndex].PlayerRef);
    }

    private void _Tool_performed(InputAction.CallbackContext obj)
    {
        if (_inputLocked)
            return;
        OnUseTool?.Invoke();
    }

    private void _Pause_performed(InputAction.CallbackContext obj)
    {
        if (_inputLocked)
            return;
        OnPause?.Invoke();
    }

    private void _AskTPHub_canceled(InputAction.CallbackContext obj)
    {
        if (_inputLocked)
            return;
        OnAskTPHubCanceled?.Invoke(GameManager.Instance.PlayerList[_playerSelectedIndex].PlayerRef);
    }

    private void _AskTPHub_started(InputAction.CallbackContext obj)
    {
        if (_inputLocked)
            return;
        OnAskTPHubStarted?.Invoke(GameManager.Instance.PlayerList[_playerSelectedIndex].PlayerRef);
        UIHubTpManager.instance.PrintUI(_players[_playerSelectedIndex], 20, "X");
    }
    
    #endregion
    #region Character Select Methods
    private void _SwitchCharacter(InputAction.CallbackContext obj)
    {
        _playerSelectedBubbleManager?.RemoveAssociatedBubble(_controllerIndex);

        _UpdateIndex(_map.actions["Move"].ReadValue<Vector2>().x);

        _playerSelectedBubbleManager = _players[_playerSelectedIndex].GetComponent<BubbleManager>();
        _playerSelectedBubbleManager?.AddControllerIcon(_controllerIndex);
    }
    private void _UpdateIndex(float x)
    {
        if (x > 0)
            _playerSelectedIndex = (_playerSelectedIndex + 1) % 4;
        else
        {
            _playerSelectedIndex = (_playerSelectedIndex - 1) % 4;
            _playerSelectedIndex = _playerSelectedIndex < 0 ? 3 : _playerSelectedIndex;
        }
        if(!(_gm.NonSelectedPlayers.Contains(_players[_playerSelectedIndex])))
            _UpdateIndex(x);
    }
    private void _PickCharacter(InputAction.CallbackContext obj)
    {
        ControllerManager.current.Link(_players[_playerSelectedIndex], Gamepad.all[_controllerIndex]);
        _gm.NonSelectedPlayers.Remove(_players[_playerSelectedIndex]);
        if(_gm.NonSelectedPlayers.Count<4/*_gm.NonSelectedPlayers.Count == 0*/)
            GameManager.Instance.CurrentGamePhase = GamePhase.HUB;
        _CleanSelectEvents();
        _SetupEvents();
        _AddController();
    }
    #endregion
}
