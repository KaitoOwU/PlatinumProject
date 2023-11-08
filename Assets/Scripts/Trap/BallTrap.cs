using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTrap : MonoBehaviour
{
    [SerializeField] List<Transform> _route;
    [SerializeField] Ball _ball;
    
    private Transform nextPos;
    private int _currentPoint;
    private void a()
    {
       
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
        }
    }
}
