using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClueManager : MonoBehaviour
{

    public static UIClueManager current;

    [SerializeField] private GameObject _sidePrefab, _hubPrefab;
    [SerializeField] private RectTransform _leftPos, _rightPos, _hubPos;

    private UIClue _instantiatedPrefab;

    private void Awake()
    {
        if (current != null) return;
        current = this;
        
        current.ShowClue(GameManager.Instance.CurrentClues[0], HubRelativePosition.HUB);
    }

    public void ShowClue(ClueData clue, HubRelativePosition pos)
    {

        if (_instantiatedPrefab != null)
        {
            Destroy(_instantiatedPrefab.gameObject);
            _instantiatedPrefab = null;
        }
        
        switch (pos)
        {
            case HubRelativePosition.LEFT_WING:
                _instantiatedPrefab = Instantiate(_sidePrefab, _leftPos).GetComponent<UIClue>();
                break;
            
            case HubRelativePosition.RIGHT_WING:
                _instantiatedPrefab = Instantiate(_sidePrefab, _rightPos).GetComponent<UIClue>();
                break;
            
            case HubRelativePosition.HUB:
                _instantiatedPrefab = Instantiate(_hubPrefab, _hubPos).GetComponent<UIClue>();
                break;
        }
        
        _instantiatedPrefab.Init(clue);
    }
    
}
