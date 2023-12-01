using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RewardGenerator : MonoBehaviour, IPuzzleReactive
{
    [HideInInspector] public UnityEvent OnClueSpawn;

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
        if (_enigmaCount == _currentCount)
        {
            if (_reward)
            {
                OnClueSpawn?.Invoke();
                Instantiate(_reward, transform.position, transform.rotation);
            }
            _room.CompletedRoom();
        }
    }

}
