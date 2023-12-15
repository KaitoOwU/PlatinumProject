using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class UIPauseMenu : MonoBehaviour
{
    public static UIPauseMenu instance;
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private GameObject _firstSelected, _currentSelected;
    [SerializeField] private Slider _music, _sound;
    [SerializeField] private TextMeshProUGUI _player;
    [SerializeField] private Image _selector;
    
    public UnityEvent<float> OnMusicVolumeChange, OnSFXVolumeChange;

    public float MusicMultiplicator => _music.value;
    public float SFXMultiplicator => _sound.value;

    private void Awake()
    {
        if(instance != null)
            Destroy(gameObject);

        instance = this;
    }

    public void InitUI(Player player)
    {
        GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null && p.PlayerRef != player).ToList()
            .ForEach(p => p.PlayerController.Inputs.gameObject.SetActive(false));
        player.PlayerController.Inputs.InputLocked = true;

        _player.text = "Player " + player.Index;
        _group.DOFade(1f, 1.5f);
        EventSystem.current.SetSelectedGameObject(_firstSelected);
        SelectButton(_firstSelected);
    }

    public void SelectButton(GameObject obj)
    {
        Slider slider = obj.GetComponent<Slider>();
        Button button = obj.GetComponent<Button>();
        
        if(slider != null)
            slider.handleRect.DOScale(1.6f, 0.5f);

        if (button != null)
            _selector.DOColor(Color.white, 0.5f);

        UnselectButton();
        _currentSelected = obj;
    }
    
    public void UnselectButton()
    {
        if (_currentSelected == null)
            return;
        
        Slider slider = _currentSelected.GetComponent<Slider>();
        Button button = _currentSelected.GetComponent<Button>();
        
        if(slider != null)
            slider.handleRect.DOScale(1f, 0.5f);

        if (button != null)
            _selector.DOColor(new Color(1, 1, 1, 0), 0.5f);
    }

    public void Close()
    {
        GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null).ToList()
            .ForEach(p =>
            {
                p.PlayerController.Inputs.gameObject.SetActive(true);
            p.PlayerController.Inputs.InputLocked = false;
        });
        _group.DOFade(0f, 1f);
    }

    public void MusicValueChange()
    {
        OnMusicVolumeChange?.Invoke(MusicMultiplicator);
    }

    public void SFXValueChange()
    {
        OnSFXVolumeChange?.Invoke(SFXMultiplicator);
    }
}
