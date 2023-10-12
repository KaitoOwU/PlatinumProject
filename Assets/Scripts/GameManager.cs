using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Serializable]
    public struct Player 
    { 
        public PlayerController PlayerController => playerController;
        [SerializeField]
        private PlayerController playerController;

        public int Id => id;
        [SerializeField]
        private int id;
    }

    public List<Player> PlayerList
    {
        get => _playerList;
        set => _playerList = value;
    }
    [SerializeField]
    private List<Player> _playerList = new(4);

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
