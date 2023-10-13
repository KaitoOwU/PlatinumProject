using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GamePhase CurrentGamePhase
    {
        get => _currentGamePhase; 
        set => _currentGamePhase = value;
    }
    private GamePhase _currentGamePhase;

    public CameraState CurrentCameraState
    {
        get; private set;
    }
    private CameraState _currentCameraState;

    public List<PlayerInfo> PlayerList
    {
        get => _playerList;
        set => _playerList = value;
    }

    [SerializeField]
    private List<PlayerInfo> _playerList = new(4);
    [SerializeField]
    private GameObject _fullCamera;
    [SerializeField]
    private GameObject _splitCameraLeft;
    [SerializeField]
    private GameObject _splitCameraRight;

    public enum GamePhase
    {
        MENU,
        HUB,
        GAME,
        GUESS,
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

    private void Awake()
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

    // Start is called before the first frame update
    void Start()
    {
        CurrentGamePhase = GamePhase.HUB;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    [Serializable]
    public struct PlayerInfo
    { 
        public PlayerController PlayerController => playerController;
        [SerializeField]
        private PlayerController playerController;

        public int Id => id;
        [SerializeField]
        private int id;
    }
}
