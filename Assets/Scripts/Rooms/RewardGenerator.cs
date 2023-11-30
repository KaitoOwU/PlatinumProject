using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardGenerator : MonoBehaviour, IPuzzleReactive
{
    [SerializeField]private GameObject _reward;
    [SerializeField] Room _room;
    [SerializeField] private int _enigmaCount;
    private int _currentCount;
    private void Start()
    {
        _room =GetComponentInParent<Room>();
        if (_room.Reward)
        {
            _reward = _room.Reward;
        }
        _currentCount = 0;
    }
    public  void PuzzleCompleted()
    {
        _currentCount++;
        Debug.Log(_currentCount);
        if (_enigmaCount == _currentCount)
        {
            Debug.Log(_reward.name) ;
            if (_reward)
            {
                Instantiate(_reward, transform.position, transform.rotation);
            }
            _room.CompletedRoom();
        }
    }

}
