using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paints : Interactable
{
    private PaintManager _manager;
    public bool isTilted;



    private void Start()
    {
        _manager = GetComponentInParent<PaintManager>();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            if (other.GetComponent<PlayerController>() == null)
                return;
            PlayerController p = other.GetComponent<PlayerController>();
            if (p.Inputs == null)
                return;
            _playersInRange.Add(GameManager.Instance.PlayerList[p.PlayerIndex - 1].PlayerRef);
            _onPlayerEnterRange?.Invoke();
            p.Inputs.OnInteract.AddListener(OnInteract);
        }
    }
    protected override void OnTriggerExit(Collider other)
    {
        //Debug.Log("Trigger exit " + other.gameObject.name);
        if (other.GetComponent<PlayerController>() == null)
            return;
        PlayerController p = other.GetComponent<PlayerController>();
        if (p.Inputs == null)
            return;
        _playersInRange.Remove(GameManager.Instance.PlayerList[p.PlayerIndex - 1].PlayerRef);
        p.Inputs.OnInteract?.RemoveListener(OnInteract);
        _onPlayerExitRange?.Invoke();
    }
    protected override void OnInteract(Player player)
    {
        if (!_manager.IsComplete&&_manager.TandemDiscovered)
        {
            if (isTilted)
            {
                isTilted = false;
                transform.Rotate(0, 0, 30f);
            }
            else
            {
                isTilted = true;
                transform.Rotate(0, 0, -30f);
            }
        }
        _manager.OnPuzzleUpdate?.Invoke(this);
    }
}

