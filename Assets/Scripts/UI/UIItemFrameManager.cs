using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemFrameManager : MonoBehaviour
{
    public static UIItemFrameManager instance;
    private UIItemFrame[] _itemFrames;

    private void Awake()
    {
        if(instance != null)
            Destroy(gameObject);

        instance = this;
        _itemFrames = FindObjectsByType<UIItemFrame>(FindObjectsSortMode.InstanceID);
    }

    public void ClueAcquired(ClueData clue)
    {
        foreach (UIItemFrame itemFrame in _itemFrames)
        {
            if (itemFrame.IsFull)
                continue;
            
            itemFrame.Add(clue);
            return;
        }
    }
}
