
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
        UIItem item = Instantiate(Resources.Load<GameObject>("Item/ItemsUI/UIItem"), transform).GetComponent<UIItem>();

        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-90f, 90f));
        item.Init(clue);

        ++_current;
    }
}
