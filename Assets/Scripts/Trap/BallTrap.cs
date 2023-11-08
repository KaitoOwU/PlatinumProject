using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTrap : MonoBehaviour
{
    [SerializeField]private List<Transform> _route;
    [SerializeField]private Ball _ball;
    [SerializeField]private Room _room;
    
    private Transform nextPos;
    private int _currentPoint;
    private void Start()
    {
        _room = GetComponentInParent<Room>();
        _currentPoint = 0;
        _ball = GetComponentInChildren<Ball>();
        nextPos = _route[0];
    }
    private void Update()
    {
        if (PlayerInRoom() > 0)
        {
            if (_ball.transform.position != nextPos.position)
            {
                _ball.Goal = nextPos.position;
            }
            else
            {
                NextPoint();
            }
        }
        else
        {
            _ball.transform.position = _route[0].transform.position;
        }
    }
    private void NextPoint()
    {
        if (_currentPoint >= _route.Count - 1)
        {
           _ball.Speed = 0;
        }
        else
        {
            nextPos = _route[_currentPoint + 1];
            _currentPoint++;
        }
    }
    private int PlayerInRoom()
    {
        int pInRoom = 0;
        foreach (PlayerInfo p in GameManager.Instance.PlayerList.FindAll(player => player.PlayerController != null))
        {
            if (p.PlayerRef.CurrentRoom == _room)
            {
                pInRoom++;
            }
        }
        return pInRoom;
    }
}
