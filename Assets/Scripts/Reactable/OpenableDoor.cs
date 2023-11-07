using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class OpenableDoor : MonoBehaviour, IPuzzleReactive
{

    public void PuzzleCompleted()
    {
        transform.parent.DORotate(new(90f, 0f, 0f), 1.5f);
    }
}
