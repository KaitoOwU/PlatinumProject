using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomTransition : MonoBehaviour
{
    [SerializeField] private Image _leftTransition, _rightTransition;
    public Image LeftTransition => _leftTransition;
    public Image RightTransition => _rightTransition;

    public IEnumerator StartTransition(Image img)
    {
        yield return img.DOColor(new(0, 0, 0, 1), 1f).WaitForCompletion();
    }

    public IEnumerator EndTransition(Image img)
    {
        yield return img.DOColor(new Color(0, 0, 0, 0), 1f).WaitForCompletion();
    }
}
