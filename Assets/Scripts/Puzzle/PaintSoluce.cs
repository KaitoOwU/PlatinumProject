using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintSoluce : MonoBehaviour
{
    [SerializeField] private List<Sprite> _paints = new List<Sprite>();
   [SerializeField] private List<int> _paintSelected = new List<int>();

    public List<int> PaintSelected { get => _paintSelected; }

    // Start is called before the first frame update
    void Start()
    {
        List<Sprite> paint = new List<Sprite>();
        foreach(Sprite spr in _paints)
        {
            paint.Add(spr);
        }
        foreach (Image p in GetComponentsInChildren<Image>())
        {
            int rand = Random.Range(0, paint.Count);
            p.sprite = paint[rand];
            paint.Remove(paint[rand]);          
            _paintSelected.Add(_paints.IndexOf(p.sprite));
            
        }
    }
}
