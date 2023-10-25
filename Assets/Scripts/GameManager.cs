using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public GamePhase CurrentGamePhase
    {
        get => _currentGamePhase; 
        set => _currentGamePhase = value;
    }

    public CameraState CurrentCameraState
    {
        get; private set;
    }

    public List<PlayerInfo> PlayerList
    {
        get => _playerList;
        set => _playerList = value;
    }
    public GameData GameData => _gameData;
    public float Timer => _timer;
    public SuspectData Murderer => _murderer;
    public SuspectData Victim => _victim;
    public TimerPhase CurrentTimerPhase => _currentTimerPhase;
    
    public IReadOnlyList<PlayerInfo> RightPlayers =>
        PlayerList.FindAll(player => player.PlayerRef.RelativePos == HubRelativePosition.RIGHT_WING);
    
    public IReadOnlyList<PlayerInfo> LeftPlayers =>
        PlayerList.FindAll(player => player.PlayerRef.RelativePos == HubRelativePosition.LEFT_WING);

    [Header("---Constants---")]
    [SerializeField]
    private GameData _gameData;

    [Header("---References---")]
    [SerializeField]
    private List<PlayerInfo> _playerList = new(4);
    [SerializeField]
    private GameObject _fullCamera;
    [SerializeField]
    private GameObject _splitCameraLeft;
    [SerializeField]
    private GameObject _splitCameraRight;

    [Header("---Events---")]
    public UnityEvent OnShuffleRooms;
    public UnityEvent OnFirstPhaseEnd;
    public UnityEvent OnSecondPhaseEnd;
    public UnityEvent OnTimerEnd;
    public UnityEvent OnEachMinute;

    private SuspectData _murderer;
    private SuspectData _victim;
    private GamePhase _currentGamePhase;
    private TimerPhase _currentTimerPhase;
    private CameraState _currentCameraState;
    private float _timer;
    private bool _isTimerGoing;

    [SerializeField] private UnityEvent<Door> _onBackToHubRefused;
    public UnityEvent<Door> OnBackToHubRefused => _onBackToHubRefused;

    [SerializeField] private UnityEvent _onWin;
    public UnityEvent OnWin => _onWin;
    [SerializeField] private UnityEvent _onLose;
    public UnityEvent OnLose => _onLose;

    public enum GamePhase
    {
        MENU,
        HUB,
        GAME,
        GUESS,
        END,
    }
    public enum TimerPhase
    {
        FIRST_PHASE,
        SECOND_PHASE,
        THIRD_PHASE,
        END,
    }
    public enum CameraState
    {
        FULL,
        SPLIT,
    }

    #region Singleton
    private static GameManager instance = null;
    public static GameManager Instance => instance;

    private void InitSingleton()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    private void Awake()
    {
        InitSingleton();
        InitGame();
    }

    void Start()
    {
        CurrentGamePhase = GamePhase.HUB;
        StartTimer();
    }

    private void InitGame()
    {
        _murderer = GameData.SuspectsDatas[UnityEngine.Random.Range(1, GameData.SuspectsDatas.Length)];
        _victim = GameData.SuspectsDatas[0]; //temporary
        //init game accordingly;
    }
    private void OnEnable()
    {
        _onWin.AddListener(Win);
        _onLose.AddListener(Lose);
    }    
    private void OnDisable()
    {
        _onWin.RemoveListener(Win);
        _onLose.RemoveListener(Lose);
    }

    void Win() => Debug.Log("<color:cyan> YOU WIN ! </color>");
    void Lose() => Debug.Log("<color:cyan> YOU LOSE ! </color>");

    #region Timer
    private IEnumerator IncrementTimer()
    {
        while (_isTimerGoing)
        {
            yield return new WaitForSeconds(1f);
            _timer -= 1;
            _AnalyseTimer();
        }
    }
    private void _AnalyseTimer()
    {
        if (_timer % 60 == 0)
        {
            OnEachMinute?.Invoke();
            Debug.Log("<color=cyan>Shuffle room </color>" + _timer);
        }
        switch (_currentTimerPhase)
        {
            case TimerPhase.FIRST_PHASE:
                if (_timer <= _gameData.TimerValues.ThirdPhaseTime + _gameData.TimerValues.SecondPhaseTime)
                {
                    OnFirstPhaseEnd?.Invoke();
                    OnShuffleRooms?.Invoke();
                    Debug.Log("<color=cyan>First Phase End </color>" + _timer);
                    _currentTimerPhase = TimerPhase.SECOND_PHASE;
                }
                break;
            case TimerPhase.SECOND_PHASE:
                if (_timer <= _gameData.TimerValues.ThirdPhaseTime)
                {
                    OnSecondPhaseEnd?.Invoke();
                    OnShuffleRooms?.Invoke();
                    Debug.Log("<color=cyan>Second Phase End </color>" + _timer);
                    _currentTimerPhase = TimerPhase.THIRD_PHASE;
                }
                break;
            case TimerPhase.THIRD_PHASE:
                if (_timer <= 0)
                {
                    OnTimerEnd?.Invoke();
                    OnShuffleRooms?.Invoke();
                    Debug.Log("<color=cyan>Third Phase End </color>" + _timer);
                    _isTimerGoing = false;
                    _timer = 0;
                    _currentTimerPhase = TimerPhase.END;
                }
                break;
            case TimerPhase.END:
                break;
        }
    }

    public void StartTimer() 
    {
        _currentTimerPhase = TimerPhase.FIRST_PHASE;
        _timer = _gameData.TimerValues.FirstPhaseTime
                + _gameData.TimerValues.SecondPhaseTime
                + _gameData.TimerValues.ThirdPhaseTime;
        _isTimerGoing = true;
        StartCoroutine(IncrementTimer());
    }
    #endregion

    #region Camera
    public void SwitchCameraState(CameraState targetState)
    {
        if(_currentCameraState == targetState)
            return;
        switch(targetState)
        {
            case CameraState.FULL:
                _fullCamera.SetActive(true);
                _splitCameraLeft.SetActive(false);
                _splitCameraRight.SetActive(false);
                CurrentCameraState = targetState;
                return;
            case CameraState.SPLIT:
                _fullCamera.SetActive(false);
                _splitCameraLeft.SetActive(true);
                _splitCameraRight.SetActive(true);
                CurrentCameraState = targetState;
                return;
        }
    }

    public void TP_LeftCamera(Transform newValues) => TP_Camera(_splitCameraLeft, newValues);
    public void TP_RightCamera(Transform newValues) => TP_Camera(_splitCameraRight, newValues);

    private void TP_Camera(GameObject camera, Transform newValues)
    {
        camera.transform.position = newValues.position;
        camera.transform.rotation = newValues.rotation;
    }
    #endregion
}

[Serializable]
public struct PlayerInfo
{ 
    public PlayerController PlayerController => playerController;
    [SerializeField]
    private PlayerController playerController;

    public int Id => id;
    [SerializeField]
    private int id;

    public Player PlayerRef => _playerRef;
    [SerializeField] private Player _playerRef;
}
