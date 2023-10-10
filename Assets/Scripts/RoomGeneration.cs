using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGeneration : MonoBehaviour
{
    [SerializeField] private List<RoomRegion> _floor = new List<RoomRegion>();
    [SerializeField] private bool _reseting;
    private List<GameObject> _roomsInPlay = new List<GameObject>();
    SCRoomsLists _roomsLists;
    List<GameObject> _currentFloor = new List<GameObject>();

    private void Start()
    {
        _roomsLists = Resources.Load<SCRoomsLists>("ScriptableObject/Rooms");
        _reseting = false;
        Shuffle();
    }
    private void Update()
    {
        if (_reseting)
        {
            Shuffle();
            _reseting = false;
        }
    }
    private void Shuffle()
    {
        int i = 0;
        if (_roomsInPlay.Count > 0)
        {
            foreach (GameObject room in _roomsInPlay)
            {
                room.GetComponent<SelfDestruct>().Destruct();
            }
        }
        _roomsInPlay = new List<GameObject>();
        foreach (RoomRegion floor in _floor)
        {
            foreach (GameObject room in _roomsLists.Floors[i].Rooms)
            {
                _currentFloor.Add(room);
            }
            int rand = Random.Range(0, _currentFloor.Count);
            GameObject room1 = Instantiate(_currentFloor[rand], floor.FloorA.transform.position, floor.FloorA.transform.rotation);
            _currentFloor.RemoveAt(rand);
            Debug.Log(_currentFloor.Count);
            int rand2 = Random.Range(0, _currentFloor.Count);
            GameObject room2 = Instantiate(_currentFloor[rand2], floor.FloorB.transform.position, floor.FloorB.transform.rotation);
            _currentFloor.RemoveAt(rand2);
            _roomsInPlay.Add(room1);
            _roomsInPlay.Add(room2);
            i++;
            _currentFloor.Clear();
        }
    }
}
[System.Serializable]
public class RoomRegion
{
    [SerializeField] private GameObject _floorA;
    [SerializeField] private GameObject _floorB;

    public GameObject FloorA { get => _floorA; }
    public GameObject FloorB { get => _floorB; }
}
