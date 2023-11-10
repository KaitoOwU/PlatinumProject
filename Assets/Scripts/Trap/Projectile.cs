using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _rotaSpeed;
    private Vector3 _dir;
    private float _rota;
    void Start()
    {
        _dir = Vector3.forward * _speed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += _dir * Time.deltaTime;
        _rota = _rotaSpeed * Time.deltaTime;
        transform.rotation = new Quaternion(_rota, 0, 0, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            Player[] players = new Player[GameManager.Instance.PlayerList.FindAll(player => player.PlayerRef.RelativePos == other.GetComponent<Player>().RelativePos).Count];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = GameManager.Instance.PlayerList.FindAll(player => player.PlayerRef.RelativePos == other.GetComponent<Player>().RelativePos)[i].PlayerRef; 
            } 
        }
        Destroy(gameObject);
    }        
}
            
        
    



