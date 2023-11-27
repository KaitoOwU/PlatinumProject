using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardGenerator : MonoBehaviour, IPuzzleReactive
{
    private GameObject _reward;
    [SerializeField] Room room;
    public void SetUp()
    {
        if(room)
        _reward = room.Reward;
    }
    public  void PuzzleCompleted()
    {
        if (_reward)
        {
            Instantiate(_reward, transform.position, transform.rotation);
        }
        room.CompletedRoom();
    }

}
