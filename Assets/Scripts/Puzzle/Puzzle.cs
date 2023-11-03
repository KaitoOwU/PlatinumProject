using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : Interactable
{

    [SerializeField] protected Transform _cluePosition;
    [SerializeField] private IPuzzleReactive _reactive;

    public void SpawnClue(Clue clue)
    {
        Instantiate(clue, _cluePosition.position, Quaternion.identity);
    }

}
public interface IPuzzleReactive
{
    public abstract void GenerateItem();
}
