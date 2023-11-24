using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BubbleManager : MonoBehaviour
{
    [SerializeField] Bubble _bubblePrefab;
    [SerializeField] Transform _layout;
    private List<Bubble> _instantiatedBubbles = new();

    public void AddBubble(int triggerController, string message)
    {
        _instantiatedBubbles.Add(Instantiate(_bubblePrefab, Vector3.zero, Quaternion.identity, _layout).InitText(triggerController, message));
    }

    public void AddPlayerIcon(int targetPlayer, int targetController)
    {
        _instantiatedBubbles.Add(Instantiate(_bubblePrefab, Vector3.zero, Quaternion.identity, _layout).InitPlayer(targetPlayer, targetController));
    }
    public void AddControllerIcon(int targetController)
    {
        _instantiatedBubbles.Add(Instantiate(_bubblePrefab, Vector3.zero, Quaternion.identity, _layout).InitController(targetController));
    }

    public bool RemoveAssociatedBubble(int targetController)
    {
        Bubble target = _instantiatedBubbles.FirstOrDefault(b => b.ControllerIndexRef == targetController);
        if (target != null)
        {
            _instantiatedBubbles.Remove(target);
            Destroy(target.gameObject);
            return true;
        }
        return false;
    }

    public virtual void RemoveAllBubbles()
    {
        for(int i = _instantiatedBubbles.Count-1; i >=0; i--)
        {
            Destroy(_instantiatedBubbles[i].gameObject);
            _instantiatedBubbles.RemoveAt(i);
        }
    }
}
