using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerTxt;

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
    [SerializeField]
    private GameObject _fullCamera;
    [SerializeField]
    private GameObject _splitCameraLeft;
    [SerializeField]
    private GameObject _splitCameraRight;
    [SerializeField]
    private Hub _hub;

    [Header("---Events---")]
    [HideInInspector]
    public UnityEvent OnShuffleRooms;
    [HideInInspector]
    public UnityEvent OnFirstPhaseEnd;
    [HideInInspector]
    public UnityEvent OnSecondPhaseEnd;
    [HideInInspector]
    public UnityEvent OnTimerEnd;
    [HideInInspector]
    public UnityEvent OnEndPhase;
    public UnityEvent OnEachMinute;

    private SuspectData _murderer;
    private SuspectData _victim;
    private GamePhase _currentGamePhase;
    private TimerPhase _currentTimerPhase;
    private CameraState _currentCameraState;
    private float _timer;
    private bool _isTimerGoing;
    private List<PickableData> _items = new();

    private List<Clue> _foundClues = new();

    private RoomGeneration _roomGenerator;

    [SerializeField] private UnityEvent<Door> _onBackToHubRefused;
    public UnityEvent<Door> OnBackToHubRefused => _onBackToHubRefused;

    [SerializeField] private UnityEvent _onWin;
    public UnityEvent OnWin => _onWin;
    [SerializeField] private UnityEvent _onLose;
    public UnityEvent OnLose => _onLose;

    public GameObject CurrentCamera => _currentCamera;
    private GameObject _currentCamera; // A ASSIGNER

    public int CorridorChance { 
        get=> _corridorChance;
        set => _corridorChance = value;
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
        _roomGenerator = FindObjectOfType<RoomGeneration>();
        _items = Helper.GetAllItemDatas().OrderBy(value => value.ID).ToList();
    }

    void Start()
    {
        CurrentGamePhase = GamePhase.HUB;
        StartTimer();
        _onWin.AddListener(Win);
        _onLose.AddListener(Lose);
        OnEndPhase.AddListener(TPAllPlayersToHub);
    }

    private void InitGame()
    {
        _murderer = GameData.SuspectsDatas[UnityEngine.Random.Range(1, GameData.SuspectsDatas.Length)];
        _victim = GameData.SuspectsDatas[0]; //temporary
        //init game accordingly;

        CurrentClues = MurderScenarios.ToList()
            .Find(scenario => scenario.DuoSuspect == new MurderScenario.SuspectDuo(_victim, _murderer)).Clues;

        _items = Helper.GetAllItemDatas().OrderBy(value => value.ID).ToList();
    }

    public void DistributeClues()
    {
        if(CurrentClues.Count == 0)
        {
            Debug.LogError("No Clues found");
            return;
        }
        List<Clue> puzzleClues = CurrentClues.ToList(); ///
        List<Clue> furnitureClues = new();
        for(int i = 0; i < _gameData.FurnitureCluesCount; i++)
        {
            if(puzzleClues.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, puzzleClues.Count);
                furnitureClues.Add(puzzleClues[randomIndex]);
                puzzleClues.RemoveAt(randomIndex);
            }
        } 
        List<Furniture> allSearchableFurnitures = GameObject.FindObjectsOfType<Furniture>().Where(f => f.FurnitureType == Furniture.EFurnitureType.SEARCHABLE).ToList();
        foreach(Clue clue in furnitureClues)
        {
            int randomIndex = UnityEngine.Random.Range(0, allSearchableFurnitures.Count);
            allSearchableFurnitures[randomIndex].Clue = clue;
            allSearchableFurnitures.RemoveAt(randomIndex);
        }
        //ADD PUZZLE CLUES
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
        OnEndPhase.RemoveListener(TPAllPlayersToHub);
    }

    private void TPAllPlayersToHub()
    {
        SwitchCameraState(CameraState.FULL);
        foreach (Player p in PlayerList.Select(data => data.PlayerRef))
        {
            p.gameObject.transform.position = _hub.Spawnpoints[p.Index-1].position;
            p.RelativePos = HubRelativePosition.HUB;
            p.CurrentRoom = _hub;
        }
    }

    void Win() => Debug.LogError("<color:cyan> YOU WIN ! </color>");
    void Lose() => Debug.LogError("<color:cyan> YOU LOSE ! </color>");

    #region Timer
    private IEnumerator IncrementTimer()
    {
        while (_isTimerGoing)
        {
            yield return new WaitForSeconds(1f);
            if (PlayerList[0].PlayerRef.CurrentRoom != Hub)
            {
                _timer -= 1;
                _AnalyseTimer();
                
                //RETIRER APRES (faux timer UI)
                _timerTxt.text = "" + _timer;
            }

        }
    }
    private void _AnalyseTimer()
    {
        if (_timer % 60 == 0)
        {
            OnEachMinute?.Invoke();
            Debug.LogError("<color=cyan>Shuffle room </color>" + _timer);
        }
        switch (_currentTimerPhase)
        {
            case TimerPhase.FIRST_PHASE:
                if (_timer <= _gameData.TimerValues.ThirdPhaseTime + _gameData.TimerValues.SecondPhaseTime)
                {
                    OnFirstPhaseEnd?.Invoke();
                    OnEndPhase?.Invoke();
                    Debug.LogError("<color=cyan>First Phase End </color>" + _timer);
                    _currentTimerPhase = TimerPhase.SECOND_PHASE;
                }
                break;
            case TimerPhase.SECOND_PHASE:
                if (_timer <= _gameData.TimerValues.ThirdPhaseTime)
                {
                    OnSecondPhaseEnd?.Invoke();
                    OnEndPhase?.Invoke();
                    Debug.LogError("<color=cyan>Second Phase End </color>" + _timer);
                    _currentTimerPhase = TimerPhase.THIRD_PHASE;
                }
                break;
            case TimerPhase.THIRD_PHASE:
                if (_timer <= 0)
                {
                    OnTimerEnd?.Invoke();
                    OnEndPhase?.Invoke();
                    Debug.LogError("<color=cyan>Third Phase End </color>" + _timer);
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

    public MurderScenario GetMurderScenario(SuspectData victim, SuspectData murderer) => MurderScenarios.ToList()
        .FindAll(scenario => scenario.DuoSuspect.Victim == victim)
        .Find(scenario => scenario.DuoSuspect.Murderer == murderer);
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
