using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardGenerator : MonoBehaviour,IPuzzleReactive
{
    private GameObject _reward;
    [SerializeField] Room room;
    private void Start()
    {
        _reward = room.Reward;
    }
    public  void GenerateItem()
    {
        if (_reward)
        {
            Instantiate(_reward, transform.position, transform.rotation);
        }
        room.OnCompletedRoom();
    }
}
