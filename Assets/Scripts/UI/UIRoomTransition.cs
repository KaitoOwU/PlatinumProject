using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomTransition : MonoBehaviour
{
    public static UIRoomTransition current;
    [SerializeField] private Image _leftTransition, _rightTransition, _hubTransition;
    public Image LeftTransition => _leftTransition;
    public Image RightTransition => _rightTransition;
    public Image HubTransition => _hubTransition;

    private void Awake()
    {
        if(current != null)
            Destroy(gameObject);

        current = this;
    }

    public IEnumerator StartTransition(Image img)
    {
        yield return img.DOColor(new(0, 0, 0, 1), 1f).WaitForCompletion();
        yield return this;
    }

    public IEnumerator EndTransition(Image img)
    {
        yield return img.DOColor(new Color(0, 0, 0, 0), 1f).WaitForCompletion();
        yield return this;
    }
}
