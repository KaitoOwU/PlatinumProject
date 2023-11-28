using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardGenerator : MonoBehaviour, IPuzzleReactive
{
    [SerializeField]private GameObject _reward;
    [SerializeField] Room _room;
    [SerializeField] private int _enigmaCount;
    private int _currentCount;
    public void SetUp()
    {
        if (_room)
        {
            _reward = _room.Reward;
        }
        _currentCount = 0;
    }
    private void Start()
    {
        _room =GetComponentInParent<Room>();
    }
    public  void PuzzleCompleted()
    {
        _currentCount++;
        Debug.Log(_currentCount);
        if (_enigmaCount == _currentCount)
        {
            Debug.Log("a");
            if (_reward)
            {
                Instantiate(_reward, transform.position, transform.rotation);
            }
            _room.CompletedRoom();
        }
    }

}
