using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField]
    private float _range;
    [SerializeField]
    private float _followDelay;
    [SerializeField]
    private float _smoothTime = 0.3f;

    private float _targetXPosition;
    private float _startXPosition;

    private Camera _camera;
    private Vector3 _velocity = Vector3.zero;
    [SerializeField]
    private ECameraBehaviourState _behaviourState;

    public enum ECameraBehaviourState
    {
        STILL,
        FOLLOW
    }

    void Awake()
    {
        _behaviourState = ECameraBehaviourState.STILL;
        _camera = gameObject.GetComponent<Camera>();
        _startXPosition = _camera.transform.position.x;
        _targetXPosition = _startXPosition;
    }

    void LateUpdate()
    {
        if(_behaviourState == ECameraBehaviourState.FOLLOW && !Mathf.Approximately(_targetXPosition, _camera.transform.position.x))
        {
            _camera.transform.position = 
                Vector3.SmoothDamp(transform.position, 
                new Vector3(_targetXPosition, _camera.transform.position.y, _camera.transform.position.z), 
                ref _velocity,
                _smoothTime);
        }
    }

    public void ChangeCameraState(ECameraBehaviourState newState, GameObject[] playersInRoom)
    {
        if (newState == _behaviourState)
            return;
        _behaviourState = newState;
        if (newState == ECameraBehaviourState.FOLLOW)
        {
            StartCoroutine(Follow(playersInRoom));
        }
        else
            _velocity = Vector3.zero;
    }

    IEnumerator Follow(GameObject[] playersInRoom)
    {
        while(_behaviourState == ECameraBehaviourState.FOLLOW)
        {
            float newTargetX = _startXPosition + (CalculateDirection(playersInRoom) * _range);
            yield return new WaitForSeconds(_followDelay); 
            _targetXPosition = newTargetX;
        }
    }

    private int CalculateDirection(GameObject[] playersInRoom)
    {
        float averageX = playersInRoom.Sum(p => p.transform.position.x)/playersInRoom.Length;
        //float averageY = playersInRoom.Sum(p => p.transform.position.x)/playersInRoom.Count;
        //if (_camera.WorldToScreenPoint(new Vector2(averageX, 0)).x < Screen.width / 2)
        if(averageX < _startXPosition)
            return -1;
        else
            return 1;
    }

    public float GetSumX(List<GameObject> playersInRoom)
    {
        return playersInRoom.Sum(p => p.transform.position.x);
    }
}
