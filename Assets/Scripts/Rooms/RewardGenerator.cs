using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardGenerator : MonoBehaviour, IPuzzleReactive
{
    private GameObject _reward;
    [SerializeField] Room room;

    //GameObject IPuzzleReactive.ObjToSpawn { get; set; }

    //private void Start()
    //{
    //    _reward = room.Reward;
    //}
    //public void GenerateItem()
    //{
    //    if (_reward)
    //    {
    //        Instantiate(_reward, transform.position, transform.rotation);
    //    }
    //    room.OnCompletedRoom();
    //}

    public void PuzzleCompleted()
    {
        throw new System.NotImplementedException();
    }

    void IPuzzleReactive.PuzzleCompleted()
    {
        throw new System.NotImplementedException();
    }

    //public void PuzzleCompleted()
    //{
    //    throw new System.NotImplementedException();
    //}
}
