using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Ball : MonoBehaviour
{
    [SerializeField] float _baseSpeed;
    private float _speed;
    private Vector3 _goal;
    private bool _isMoving;
    private bool _isTurning;
    private float _rotaTimer;

    public float Speed { get => _speed; set => _speed = value; }
    public Vector3 Goal { get => _goal; set => _goal = value; }
    public bool IsTurning { get => _isTurning; set => _isTurning = value; }
    public bool IsMoving { get => _isMoving; set => _isMoving = value; }

    [HideInInspector] public UnityEvent OnBallRollingBegin;
    [HideInInspector] public UnityEvent OnBallRollingEnd;


    private void Start()
    {
        _goal = transform.position;
        _speed = _baseSpeed;
        _isTurning = false;
    }
    private void Update()
    {
        if ((transform.position - _goal).magnitude >= 0.2)
        {
            BallMov();
        }
    }
    public void RotateBall()
    {
        {
            _rotaTimer += Time.deltaTime;
            Vector3 targetDirection = _goal - transform.position;
            transform.forward= Vector3.RotateTowards(transform.forward, targetDirection, Mathf.PI*Time.deltaTime, 0.0f);
            Quaternion rotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _speed);
            if (_rotaTimer > .3f)
            {
                _isTurning = false;
                _rotaTimer = 0;
            }
        }
    }

    private void BallMov() 
    {
        if (_isTurning)
        {
                RotateBall();
        }
        else if(Speed>0&&_isMoving)
        {
            transform.position += (_goal - transform.position).normalized * _speed * Time.deltaTime;
            if (Mathf.Abs(_goal.y -transform.position.y) <= 0.4)
                transform.Rotate(_speed * Time.deltaTime * 100, 0, 0);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (other.GetComponent<Player>())
        {
            other.GetComponent<Player>().OnHit.Invoke();
            _isMoving = false;
            Player[] players = new Player[GameManager.Instance.PlayerList.FindAll(player => player.PlayerRef.RelativePos == other.GetComponent<Player>().RelativePos).Count];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = GameManager.Instance.PlayerList.FindAll(player => player.PlayerRef.RelativePos == other.GetComponent<Player>().RelativePos)[i].PlayerRef;
            }
            GameManager.Instance.TPPlayerPostTrap(players);
        }
    }
}
