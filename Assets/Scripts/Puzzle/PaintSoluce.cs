using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintSoluce : MonoBehaviour
{
    [SerializeField] private List<Sprite> _paints = new List<Sprite>();
    [SerializeField] private int _paintSelected;

    public int PaintSelected { get => _paintSelected; }
    public List<Sprite> Paints { get => _paints; }
    private void Awake()
    {
        _paintSelected = Random.Range(0, _paints.Count);
    }
    // Start is called before the first frame update
    public void Set()
    {
        GetComponentInChildren<Image>().sprite = _paints[_paintSelected];
        GetComponentInParent<Room>().Tandem.GetComponentInChildren<PaintManager>().SetPaint();
    }
}
