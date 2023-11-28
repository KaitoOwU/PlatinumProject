using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PaintManager : Puzzle
{
    public Action<Paints> OnPuzzleUpdate;
    private PaintSoluce _soluce;
    private List<Paints> _paints = new();
    private bool _isComplete;
    [SerializeField] private List<NestedSpriteLIst> _imagesVar = new();
    [SerializeField]private bool _tandemDiscovered;

    public bool IsComplete { get => _isComplete; }
    public bool TandemDiscovered { get => _tandemDiscovered; }

    private void Start()
    {
        _paints = GetComponentsInChildren<Paints>().ToList();
        _soluce = GetComponentInParent<Room>().Tandem.GetComponentInChildren<PaintSoluce>();
        SetPaint();
    }
    private void Update()
    {
        if (GetComponentInParent<Room>().Tandem.PlayerInRoom() > 0 && !_tandemDiscovered)
        {
            _tandemDiscovered = true;
        }
    }
    private void OnEnable()
    {
        OnPuzzleUpdate += PuzzleUpdateCheck;
    }

    private void OnDisable()
    {
        OnPuzzleUpdate -= PuzzleUpdateCheck;
    }

    private void SetPaint()
    {
        for (int i = 0; i < _paints.Count; i++)
        {
            int rand = UnityEngine.Random.Range(0, _imagesVar[_soluce.PaintSelected].sprites.Count);
            _paints[i].GetComponentInChildren<Image>().sprite = _imagesVar[_soluce.PaintSelected].sprites[rand];
            _imagesVar[_soluce.PaintSelected].sprites.RemoveAt(rand);
        }
    }
    private void PuzzleUpdateCheck(Paints paint)
    {
        if (_paints.FindAll((paint) => paint.isTilted).Count == 1 && _paints.Find((paint) => paint.isTilted).GetComponentInChildren<Image>().sprite == _soluce.Paints[_soluce.PaintSelected]&&!IsComplete)
        {
            _isComplete = true;
            Reactive.PuzzleCompleted();
        }
    }
}
[System.Serializable]
public class NestedSpriteLIst
{
   [SerializeField] public List<Sprite> sprites;
}


