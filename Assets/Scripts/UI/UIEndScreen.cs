using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIEndScreen : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Button _button;
    [SerializeField] private Image _selection;
    [SerializeField] private CanvasGroup _group;

    [SerializeField] private TextMeshProUGUI _clues, _culprit;
    [SerializeField] private CanvasGroup _cluesG, _culpritG, _backG;

    public void Init(bool isWon)
    {
        _group.DOFade(1f, 1.5f);
        _title.text = isWon ? "You Escaped" : "Eternal Prisoner";
        _title.alpha = 0f;
        _culprit.text = GameManager.Instance.Murderer.Name;
        StartCoroutine(InitAnimation());
    }

    private IEnumerator InitAnimation()
    {
        yield return _title.DOFade(1f, 1f).WaitForCompletion();
        _cluesG.DOFade(1f, 1f);
        for(int i = 0; i <= GameManager.Instance.FoundClues.Count; i++)
        {
            _clues.text = i + "<size=60><#FFFFFF>/10";
            if (i == 10)
            {
                _clues.DOColor(new Color(1, 0.7f, 0), 1f);
            }
            yield return new WaitForSecondsRealtime(.1f);
        }
        yield return new WaitForSecondsRealtime(.5f);
        yield return _culpritG.DOFade(1f, 1f).WaitForCompletion();

        EventSystem.current.SetSelectedGameObject(_button.gameObject);
        yield return _backG.DOFade(1f, 1f).WaitForCompletion();
    }

    public void GoMainMenu()
    {
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
