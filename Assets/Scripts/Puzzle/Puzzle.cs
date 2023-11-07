using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : Interactable
{
    
    [SerializeField] protected GameObject _reactive;
    
    public IPuzzleReactive Reactive => _reactive.GetComponent<IPuzzleReactive>();

}

public interface IPuzzleReactive
{
    public abstract void PuzzleCompleted();
}
