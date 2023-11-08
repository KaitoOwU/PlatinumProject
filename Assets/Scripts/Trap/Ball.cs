using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] float _speed;
    private Vector3 _goal;

    public float Speed { get => _speed; set => _speed = value; }
    public Vector3 Goal { get => _goal; set => _goal = value; }

    private void Start()
    {
        _goal = transform.position;
    }
    private void Update()
    {
        if(transform.position!= _goal)
        {
            transform.position += ( _goal- transform.position ).normalized * _speed;
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
