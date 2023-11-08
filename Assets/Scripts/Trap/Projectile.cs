using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    private Vector3 _dir;
    void Start()
    {
        _dir = Vector3.forward * _speed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += _dir * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {

            Player[] players = new Player[FindObjectsOfType<Player>().ToList().FindAll(player=>player.CurrentRoom== other.GetComponent<Player>().CurrentRoom).Count];
            Debug.Log(players.Length);
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = FindObjectsOfType<Player>().ToList().FindAll(player => player.CurrentRoom == other.GetComponent<Player>().CurrentRoom)[i]; 
            }
            GameManager.Instance.TPPlayerPostTrap(players);
        }
        Destroy(gameObject);
    }        
}
            
        
    



