
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIItemFrame : MonoBehaviour
{
    [SerializeField] private int _limit;
    private int _current = 0;

    public bool IsFull => _current >= _limit;

    public void Add(ClueData clue)
    {
        UIItem item = Instantiate(Resources.Load<GameObject>("Item/ItemsUI/UIItem"), transform).transform.GetChild(1).GetComponent<UIItem>();

        item.transform.localPosition = Vector3.zero;
        item.Init(clue);

        ++_current;
    }
}
