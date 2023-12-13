using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CameraBehaviour;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public GamePhase CurrentGamePhase
    {
        get => _currentGamePhase; 
        set => _currentGamePhase = value;
    }    
    public CameraState CurrentCameraState
    {
        get => _currentCameraState; 
        set => _currentCameraState = value;
    }

    public List<PlayerInfo> PlayerList
    {
        get => _playerList;
        set => _playerList = value;
    }
    public int CorridorChance
    { 
        get => _corridorChance; 
        set => _corridorChance = value;
    }
    
    public List<Clue> FoundClues
    {
        get => _foundClues;
        set => _foundClues = value;
    }
    public GameData GameData => _gameData;
    public PlayerConstants PlayerConstants => _playerConstants;
    public float Timer => _timer;
    public SuspectData Murderer => _murderer;
    public SuspectData Victim => _victim;
    public Hub Hub => _hub;
    public Vestibule Vestibule => _vestibule;
    public TimerPhase CurrentTimerPhase => _currentTimerPhase;
    
    public IReadOnlyList<PlayerInfo> RightPlayers =>
        PlayerList.FindAll(player => player.PlayerRef.RelativePos == HubRelativePosition.RIGHT_WING);
    
    public IReadOnlyList<PlayerInfo> LeftPlayers =>
        PlayerList.FindAll(player => player.PlayerRef.RelativePos == HubRelativePosition.LEFT_WING);

    public IReadOnlyList<PickableData> Items => _items;
    public IReadOnlyList<MurderScenario> MurderScenarios => Resources.LoadAll<MurderScenario>("Clues");
    public IReadOnlyList<Clue> CurrentClues { get; private set; }

    [Header("---Constants---")]
    [SerializeField]
    private GameData _gameData;
    [SerializeField]
    private PlayerConstants _playerConstants;

    [Header("---References---")]
    [SerializeField]
    private List<PlayerInfo> _playerList = new(4);
    public Camera FullCamera { get => _fullCamera; }
    [SerializeField]
    private Camera _fullCamera;

    public Camera CameraLeft => _splitCameraLeft; 
    [SerializeField]
    private Camera _splitCameraLeft;
    private CameraBehaviour _splitCameraLeftBehaviour;
    public CameraBehaviour SplitCameraLeftBehaviour { get => _splitCameraLeftBehaviour; set => _splitCameraLeftBehaviour = value; }

    public Camera CameraRight => _splitCameraRight;
    [SerializeField]
    private Camera _splitCameraRight;
    private CameraBehaviour _splitCameraRightBehaviour;
    public CameraBehaviour SplitCameraRightBehaviour { get => _splitCameraRightBehaviour; set => _splitCameraRightBehaviour = value; }

    [SerializeField]
    private Hub _hub;
    [SerializeField] private UIRoomTransition _transitions;
    [SerializeField] private TMP_Text _timerUI;
    public UIRoomTransition Transitions => _transitions;
    private Vestibule _vestibule;

    //Public Unity Events
    [HideInInspector]
    public UnityEvent OnShuffleRooms;
    [HideInInspector]
    public UnityEvent OnFirstPhaseEnd;
    [HideInInspector]
    public UnityEvent OnSecondPhaseEnd;
    [HideInInspector]
    public UnityEvent OnTimerEnd;
    [HideInInspector]
    public UnityEvent OnEachEndPhase;
    [HideInInspector]
    public UnityEvent OnEachMinute;
    [HideInInspector]
    public UnityEvent OnChangeToSplitScreen;
    [HideInInspector]
    public UnityEvent OnChangeToFullScreen;
    [HideInInspector]
    public UnityEvent OnTPToHubAfterTrap;
    [HideInInspector]
    public UnityEvent OnTPToHub;



    private SuspectData _murderer;
    private SuspectData _victim;
    private GamePhase _currentGamePhase;
    private TimerPhase _currentTimerPhase;
    private CameraState _currentCameraState;
    private int _shuffleCount=0;
    private float _timer;
    private bool _isTimerGoing=false;
    private List<PickableData> _items = new();
    public List<Player> NonSelectedPlayers { get=> _nonSelectedPlayers; set=> _nonSelectedPlayers = value; }
    private List<Player> _nonSelectedPlayers = new();

    private List<Clue> _foundClues = new();

   [SerializeField] private RoomGeneration _roomGenerator;

    [SerializeField] private UnityEvent<Door> _onBackToHubRefused;
    public UnityEvent<Door> OnBackToHubRefused => _onBackToHubRefused;
    public bool IsTimerGoing { get => _isTimerGoing;}

    [SerializeField] private UnityEvent _onWin;
    public UnityEvent OnWin => _onWin;
    [SerializeField] private UnityEvent _onLose;
    private UnityEvent _onFadeMusic;
    public UnityEvent OnLose => _onLose;
    public UnityEvent OnFadeMusic => _onLose;

    private int _currentPlayersCount;
    public int CurrentPlayersCount
    {
        get => _currentPlayersCount;
        set => _currentPlayersCount = value;
    }
    
    int _corridorChance = 0;
    public int ValidatedRooom
    {
        get => _validatedRooom;
        set => _validatedRooom = value;
    }
    int _validatedRooom = 0;

    public enum GamePhase
    {
        INTRO,
        SELECT_CHARACTER,
        HUB,
        GAME,
        EARLY_GUESS,
        GUESS,
        END
    }
    public enum TimerPhase
    {
        FIRST_PHASE,
        SECOND_PHASE,
        THIRD_PHASE,
        END_THIRD_PHASE,
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
        _validatedRooom = 0;
        _corridorChance = 10;
        _vestibule = FindObjectOfType<Vestibule>();
        _items = Helper.GetAllItemDatas().OrderBy(value => value.ID).ToList();
        _nonSelectedPlayers = PlayerList.Select(p => p.PlayerRef).ToList();
        OnEachEndPhase.AddListener(TPAllPlayersToHub);
    }

    private void RandomMessage()
    {
        Debug.Log(_shuffleCount);
        switch (_shuffleCount)
        {
            case 0:
                StartCoroutine(UIMessageGenerator.instance.Init(false,
                    new UIMessageData("The Manor", "Not so fast! Let me shuffle the rooms around a bit for you so it's not too easy. Wouldn't want you to get bored to death...", 0.03f, 5f)));
                break;
            
            case 1:
                StartCoroutine(UIMessageGenerator.instance.Init(false,
                    new UIMessageData("The Manor", "Aaand let me shuffle the rooms again. You only got 5 minutes left to find the murderer, sounds good for me..", 0.03f, 5f)));
                break;
            
            case 2:
                StartCoroutine(UIMessageGenerator.instance.Init(false,
                    new UIMessageData("The Manor", "Time's up! You can't explore the manor anymore so look at what you've found and then enter the vestibule to give me your final answer.", 0.03f, 5f)));
                break;
        }
        _shuffleCount++;
    }

    void Start()
    {
        _roomGenerator = FindObjectOfType<RoomGeneration>();
        _hub = FindObjectOfType<Hub>();
        SwitchCameraState(CameraState.FULL);
        StartCoroutine(VestibuleMessages());
        TP_Camera(_fullCamera.gameObject, Vestibule.CameraPoint);
        StartCoroutine(StartTimer());
        _onWin.AddListener(Win);
        _onLose.AddListener(Lose);
        
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    private void InitGame()
    {
        CurrentGamePhase = GamePhase.INTRO;

        //_murderer = GameData.SuspectsDatas[UnityEngine.Random.Range(1, GameData.SuspectsDatas.Length)];
        //_victim = GameData.SuspectsDatas[0]; //temporary
        _murderer = GameData.SuspectsDatas[0];
        _victim = GameData.SuspectsDatas[2]; //temporary
        //init game accordingly;
        
        CurrentClues = MurderScenarios.ToList()
            .Find(scenario => scenario.DuoSuspect == new MurderScenario.SuspectDuo(_victim, _murderer)).Clues;

        _items = Helper.GetAllItemDatas().OrderBy(value => value.ID).ToList();

        _splitCameraLeftBehaviour = _splitCameraLeft.GetComponent<CameraBehaviour>();
        _splitCameraRightBehaviour = _splitCameraRight.GetComponent<CameraBehaviour>();
    }

    public void DistributeClues()
    {
        if(CurrentClues.Count == 0)
        {
            Debug.LogError("No Clues found");
            return;
        }
        List<Furniture> allSearchableFurnitures = new List<Furniture>();
        List<Clue> puzzleClues = CurrentClues.ToList(); ///
        Debug.Log(puzzleClues.Count);
        foreach(Furniture f in FindObjectsOfType<Furniture>())
        {
            if (f.FurnitureType == Furniture.EFurnitureType.SEARCHABLE)
            {
                allSearchableFurnitures.Add(f);
            }
        }
        //Debug.Log(puzzleClues.Count);
        if (FindObjectsOfType<Furniture>().Length > 0)
        {
            List<Clue> furnitureClues = new();
            for (int i = 0; i < _gameData.FurnitureCluesCount; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, puzzleClues.Count);
                furnitureClues.Add(puzzleClues[randomIndex]);
                puzzleClues.RemoveAt(randomIndex);
                if (puzzleClues.Count == 0||furnitureClues.Count== FindObjectsOfType<Furniture>().Length)
                {
                    break;
                }
            }
            foreach (Furniture f in FindObjectsOfType<Furniture>())
            {
                if (f.FurnitureType == Furniture.EFurnitureType.SEARCHABLE)
                {
                    allSearchableFurnitures.Add(f);
                }
            }
            for (int i= furnitureClues.Count-1; i>=0; i--) 
            {
                int randomIndex = UnityEngine.Random.Range(0, allSearchableFurnitures.Count);
                allSearchableFurnitures[randomIndex].Clue = furnitureClues[i];
                allSearchableFurnitures.RemoveAt(randomIndex);
            }
        }
        Debug.Log(puzzleClues.Count);
        _roomGenerator.SetRoomsRewards(puzzleClues);
    }

    private void OnEnable()
    {
        //_onWin.AddListener(Win);
        //_onLose.AddListener(Lose);
        //OnEndPhase.AddListener(TPAllPlayersToHub);
    }    
    private void OnDisable()
    {
        _onWin.RemoveListener(Win);
        _onLose.RemoveListener(Lose);
        OnEachEndPhase.RemoveListener(TPAllPlayersToHub);
    }

    public void TPAllPlayersToHub()
    {
        StartCoroutine(CR_TPAllPlayersToHub());
    }

    private IEnumerator CR_TPAllPlayersToHub()
    {
        PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = true);
        _hub.Doors[0].IsLocked = false;
        _hub.Doors[1].IsLocked = false;
        StartCoroutine(_transitions.StartTransition(_transitions.RightTransition));
        yield return StartCoroutine(_transitions.StartTransition(_transitions.LeftTransition));
        
        SwitchCameraState(CameraState.FULL);
        int i = 0;
        foreach (Player p in PlayerList.Select(data => data.PlayerRef))
        {
            p.gameObject.transform.position = _hub.Spawnpoints[i].position;
            p.RelativePos = HubRelativePosition.HUB;
            p.CurrentRoom = _hub;
            i++;
        }
        
        StartCoroutine(_transitions.EndTransition(_transitions.RightTransition));
        yield return StartCoroutine(_transitions.EndTransition(_transitions.LeftTransition));
        
        PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = false);
        //A VOIR SI ILS PEUVENT REBOUGER AVANT LA FIN DU FADE OUT
    }
    
    public void TPPlayerPostTrap(Player[] players)
    {
        StartCoroutine(CR_TPPlayerPostTrap(players));
        foreach(Player p in players)
        {
            StartCoroutine(p.PlayerController.Fall());
        }
    }

    private IEnumerator CR_TPPlayerPostTrap(Player[] players)
    {
        Image imgToCancel;
        
        if (players[0].RelativePos == HubRelativePosition.RIGHT_WING)
        {
             yield return StartCoroutine(_transitions.StartTransition(_transitions.RightTransition));
             imgToCancel = _transitions.RightTransition;
            
            _hub.Doors[0].IsLocked = true;
            TP_RightCamera(_hub.CameraPoint);
            _splitCameraRight.GetComponent<CameraBehaviour>().ChangeCameraState(ECameraBehaviourState.FOLLOW, players.Select(p => p.gameObject).ToArray());
        }
        else
        {
            yield return StartCoroutine(_transitions.StartTransition(_transitions.LeftTransition));
            imgToCancel = _transitions.LeftTransition;
            
            _hub.Doors[1].IsLocked = true;
            TP_LeftCamera(_hub.CameraPoint);
            _splitCameraLeft.GetComponent<CameraBehaviour>().ChangeCameraState(ECameraBehaviourState.FOLLOW, players.Select(p => p.gameObject).ToArray());
        }
        for (int i = 0; i < players.Length; i++)
        {
            players[i].gameObject.transform.position = _hub.Spawnpoints[i].position;
            players[i].RelativePos = HubRelativePosition.HUB;
            players[i].CurrentRoom = _hub;
        }

        yield return StartCoroutine(_transitions.EndTransition(imgToCancel));
    }

    void Win() => Debug.LogError("<color:cyan> YOU WIN ! </color>");
    void Lose() => Debug.LogError("<color:cyan> YOU LOSE ! </color>");

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
    private IEnumerator ShuffleTimer()
    {
        yield return new WaitForSeconds(1f);
        OnShuffleRooms?.Invoke();
    }
    private void _AnalyseTimer()
    {
        _timerUI.text = _timer.ToString();
        if (_timer % 60 == 0)
        {
            OnEachMinute?.Invoke();
        }
        switch (_currentTimerPhase)
        {
            case TimerPhase.FIRST_PHASE:
                if (Math.Abs(_timer - (_gameData.TimerValues.ThirdPhaseTime + _gameData.TimerValues.SecondPhaseTime) - 15f) < 0.1f)
                {
                    StartCoroutine(ControllerManager.current.HeartBeat(13f, 1f, .1f));
                }
                
                if (_timer <= _gameData.TimerValues.ThirdPhaseTime + _gameData.TimerValues.SecondPhaseTime)
                {
                    CurrentGamePhase = GamePhase.HUB;
                    OnFirstPhaseEnd?.Invoke();
                    OnEachEndPhase?.Invoke();
                    StartCoroutine(ShuffleTimer());
                    Debug.LogError("<color=cyan>First Phase End </color>" + _timer);
                    _currentTimerPhase = TimerPhase.SECOND_PHASE;
                    RandomMessage();
                }
                break;
            case TimerPhase.SECOND_PHASE:
                if (Math.Abs(_timer - _gameData.TimerValues.ThirdPhaseTime - 15f) < 0.1f)
                {
                    StartCoroutine(ControllerManager.current.HeartBeat(13f, 1f, .1f));
                }
                
                if (_timer <= _gameData.TimerValues.ThirdPhaseTime)
                {
                    CurrentGamePhase = GamePhase.HUB;
                    _currentTimerPhase = TimerPhase.THIRD_PHASE;
                    OnSecondPhaseEnd?.Invoke();
                    OnEachEndPhase?.Invoke();
                    StartCoroutine(ShuffleTimer());
                    Debug.LogError("<color=cyan>Second Phase End </color>" + _timer);
                    _currentTimerPhase = TimerPhase.THIRD_PHASE;
                    RandomMessage();
                }
                break;
            case TimerPhase.THIRD_PHASE:
                if (Math.Abs(_timer - 15f) < 0.1f)
                {
                    StartCoroutine(ControllerManager.current.HeartBeat(13f, 1f, .1f));
                }
                
                if (_timer <= 3)
                {
                    _onFadeMusic?.Invoke();
                    _currentTimerPhase = TimerPhase.END_THIRD_PHASE;
                }
                break;
            case TimerPhase.END_THIRD_PHASE:
                if (_timer <= 0)
                {
                    CurrentGamePhase = GamePhase.EARLY_GUESS;
                    Vestibule.Doors[0].IsLocked = false;
                    Hub.RoomDoorLeft.IsLocked = true;
                    Hub.RoomDoorRight.IsLocked = true;
                    _currentTimerPhase = TimerPhase.END;
                    OnTimerEnd?.Invoke();
                    OnEachEndPhase?.Invoke();
                    Debug.LogError("<color=cyan>Third Phase End </color>" + _timer);
                    _isTimerGoing = false;
                    _timer = 0;
                    _currentTimerPhase = TimerPhase.END;
                    RandomMessage();
                }
                break;
            case TimerPhase.END:
                break;
        }
    }

    private IEnumerator StartTimer() 
    {
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => PlayerList.Any(p => p.PlayerRef.CurrentRoom != Hub) == true);
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
                OnChangeToFullScreen?.Invoke();
                _fullCamera.gameObject.SetActive(true);
                _splitCameraLeft.gameObject.SetActive(false);
                _splitCameraRight.gameObject.SetActive(false);
                CurrentCameraState = targetState;
                return;
            case CameraState.SPLIT:
                OnChangeToSplitScreen?.Invoke();
                _fullCamera.gameObject.SetActive(false);
                _splitCameraLeft.gameObject.SetActive(true);
                _splitCameraRight.gameObject.SetActive(true);
                CurrentCameraState = targetState;
                return;
        }
    }

    public void TP_LeftCamera(Transform newValues) => TP_Camera(_splitCameraLeft.gameObject, newValues);
    public void TP_RightCamera(Transform newValues) => TP_Camera(_splitCameraRight.gameObject, newValues);

    public void TP_Camera(GameObject camera, Transform newValues)
    {
        camera.transform.position = newValues.position;
        camera.transform.rotation = newValues.rotation;
    }
    #endregion

    public MurderScenario GetMurderScenario(SuspectData victim, SuspectData murderer) => MurderScenarios.ToList()
        .FindAll(scenario => scenario.DuoSuspect.Victim == victim)
        .Find(scenario => scenario.DuoSuspect.Murderer == murderer);

    IEnumerator VestibuleMessages()
    {
        UIMessageGenerator.instance.messages = StartCoroutine(UIMessageGenerator.instance.Init(true,
            new UIMessageData("The Manor", "You, who dare disturb my sleep, pay the price for your imprudence!", 0.0f, 2f),
            new UIMessageData("The Manor",
                "Explore the manor in which I've spent all my lonely life and uncover the truth behind the story I've created for you.",
                0.02f, 2f),
            new UIMessageData("The Manor",
                "You'll have to find clues about the murder that took place here and give me the culprit before midnight strikes. It has to be one of the four people painted here.",
                0.02f, 2f),
            new UIMessageData("The Manor", "If you fail, you'll be stuck with me forever, so I'll never be alone again !", 0.02f,
                2f),
            new UIMessageData("The Manor", "But remember, I won't make it easy for you...", 0.02f, 3f),
            new UIMessageData("The Manor", "You can't be more than 3 in a wing so you'll have to split into teams.", 0.02f, 3f),
            new UIMessageData("The Manor", "The upper hall in the hall is blocked until midnight.", 0.02f, 3f)
        ));
        yield return UIMessageGenerator.instance.messages;

        yield return new WaitForSecondsRealtime(2f);
        yield return UIRoomTransition.current.StartTransition(UIRoomTransition.current.HubTransition);
        TP_Camera(_fullCamera.gameObject, Hub.CameraPoint);
        yield return UIRoomTransition.current.EndTransition(UIRoomTransition.current.HubTransition);
        FindObjectsOfType<InputManager>().ToList().ForEach(i => i.InputLocked = false);
        CurrentGamePhase = GamePhase.SELECT_CHARACTER;
        foreach (Player p in Vestibule.GetComponentsInChildren<Player>())
        {
            Destroy(p.gameObject);
        }

        StartCoroutine(UIMessageGenerator.instance.Init(false,
            new UIMessageData("", "Press any key and use your Left Joystick to select your puppet.<br>Press A to awake it.", 0.01f, 10f)));
    } 
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

public static class Helper
{
    public static List<PickableData> GetAllItemDatas()
    {
        return Resources.LoadAll<PickableData>("Item/ItemsData").ToList();
    }
}
