using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Portrait : Interactable
{
    [Header("---References---")]
    [SerializeField] private GuessManager _guessManager;
    [SerializeField] private Image _imageComponent, _blood;
    [SerializeField] private GameObject[] _votesIndicators;
    private SuspectData _suspectData;
    public SuspectData SuspectData => _suspectData;


    public void InitPortrait(SuspectData suspect)
    {
        _suspectData = suspect;
        _imageComponent.sprite = suspect.Image;
    }
    public void UpdateToVictim()
    {
        _blood.gameObject.SetActive(true);
        _imageComponent.color = Color.gray;
    }

    public void UpdateVote(int playerIndex, bool isVoted) => _votesIndicators[playerIndex].SetActive(isVoted);
    

    protected override void OnInteract(Player player)
    {
        base.OnInteract(player);
        _guessManager.OnChoseSuspect?.Invoke(player, _suspectData);
    }

}
