using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] float _baseSpeed;
    private float _speed;
    private Vector3 _goal;
    private bool _isMoving;
    private bool _isTurning;

    public float Speed { get => _speed; set => _speed = value; }
    public Vector3 Goal { get => _goal; set => _goal = value; }
    public bool IsTurning { get => _isTurning; set => _isTurning = value; }

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
            Vector3 targetDirection = _goal - transform.position;
            float singleStep = _speed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            if (Mathf.Abs(Vector3.Dot(targetDirection,newDirection)-(targetDirection.magnitude*newDirection.magnitude))<= 0.1)
            {
                _isTurning = false;
            }
        }
    }

    private void BallMov() 
    {
        if (_isTurning)
        {
                RotateBall();
        }
        else
        {
            _speed = _baseSpeed;
            transform.position += (_goal - transform.position).normalized * _speed * Time.deltaTime;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (other.GetComponent<Player>())
        {
            Player[] players = new Player[GameManager.Instance.PlayerList.FindAll(player => player.PlayerRef.RelativePos == other.GetComponent<Player>().RelativePos).Count];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = GameManager.Instance.PlayerList.FindAll(player => player.PlayerRef.RelativePos == other.GetComponent<Player>().RelativePos)[i].PlayerRef;
            }
            GameManager.Instance.TPPlayerPostTrap(players);
        }
    }
}
