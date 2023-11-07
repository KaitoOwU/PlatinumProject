using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookshelfTrap : MonoBehaviour
{
    [SerializeField] private Room _room;
    [SerializeField] private float _cadence;
    [SerializeField] private GameObject _book;
    [SerializeField] private Transform _spawnPoint;
    private int _playerInRoom;
    private bool _isShooting;

    private void Start()
    {
        _isShooting = false;
    }

    private void Update()
    {
        _playerInRoom = 1; //PlayerInRoom();
        if (_playerInRoom > 0&& !_isShooting)
        {
            _isShooting = true;
            Debug.Log("aeaze");
            StartCoroutine(ShootBook());      
        }
        else if (_playerInRoom == 0)
        {
            _isShooting = false;
        }
    }
    private int PlayerInRoom()
    {
        int pInRoom = 0;
        foreach (PlayerInfo p in GameManager.Instance.PlayerList.FindAll(player => player.PlayerController))
        {
            if (p.PlayerRef.CurrentRoom == _room)
            {
                pInRoom++;
            }
        }
        return pInRoom;
    }
    IEnumerator ShootBook()
    {
        while (_isShooting)
        {
            Debug.Log("ah");
            yield return new WaitForSeconds(_cadence);
            GameObject projectile = Instantiate(_book, _spawnPoint);
        }
    }
}
