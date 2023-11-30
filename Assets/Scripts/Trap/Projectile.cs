using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    private Vector3 _dir;
    void Start()
    {
        _dir = transform.up * _speed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += _dir * Time.deltaTime;
        transform.Rotate(0,0,180 * _speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            other.GetComponent<Player>().OnHit.Invoke();
            Player[] players = new Player[FindObjectsOfType<Player>().ToList().FindAll(player=>player.CurrentRoom== other.GetComponent<Player>().CurrentRoom).Count];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = FindObjectsOfType<Player>().ToList().FindAll(player => player.CurrentRoom == other.GetComponent<Player>().CurrentRoom)[i]; 
            }
            GameManager.Instance.TPPlayerPostTrap(players);
        }
        Destroy(gameObject);
    }        
}
            
        
    



