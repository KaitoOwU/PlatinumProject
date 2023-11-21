using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIClue : MonoBehaviour
{
    [SerializeField] private Image _clueVisual;
    [SerializeField] private TextMeshProUGUI _clueText, _clueDescription;
    
    public Image ClueVisual => _clueVisual;
    public TextMeshProUGUI ClueText => _clueText;
    public TextMeshProUGUI ClueDescription => _clueDescription;

    public void Init(Sprite clueVisual, string clueDesc, string clueText = "")
    {
        _clueVisual.sprite = clueVisual;
        _clueDescription.text = clueDesc;
        _clueText.text = clueText;
    }
    
    public void Init(ClueData clue)
    {
        _clueDescription.text = clue.Description;
        _clueText.text = clue.Content;
        
        if(clue.Sprite != null)
            _clueVisual.sprite = clue.Sprite;
    }
}
