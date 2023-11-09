using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BookshelfTrap : MonoBehaviour
{
    [SerializeField] private Room _room;
    [SerializeField] private float _cadence;
    [SerializeField] private GameObject _book;
    [SerializeField] private Transform _spawnPoint;
    private int _playerInRoom;
    private bool _isShooting;

    public UnityEvent ThrowBook;
    private void Start()
    {
        _room=GetComponentInParent<Room>();
        _isShooting = false;
        _playerInRoom = 0;
    }

    private void Update()
    {
        _playerInRoom = _room.PlayerInRoom();

        if (_playerInRoom > 0&& !_isShooting)
        {
            _isShooting = true;
            StartCoroutine(ShootBook());      
        }
        else if (_playerInRoom == 0)
        {
            _isShooting = false;
        }
    }
    IEnumerator ShootBook()
    {
        while (_isShooting)
        {
            yield return new WaitForSeconds(_cadence);
            GameObject projectile = Instantiate(_book, _spawnPoint.position,_spawnPoint.rotation);
        }
    }
}
