using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : Interactable
{

    [SerializeField] private Transform _cluePosition;

    public void SpawnClue(Clue clue)
    {
        Instantiate(clue, _cluePosition.position, Quaternion.identity);
    }

}

public interface IPuzzleReactive
{
    public abstract void GenerateItem(GameObject objToSpawn);
}
