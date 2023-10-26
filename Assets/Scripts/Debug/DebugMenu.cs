using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public static Data data;
    
    private DebugMenuMap _openMenu;
    [SerializeField] private GameObject _debugMenu;
    [SerializeField] private EventSystem _disableEvents;

    [SerializeField] private TextMeshProUGUI _characterName, _itemInHand, _multiplayerRestrictionsText;

    private void Awake() => _openMenu = new DebugMenuMap();

    private void Start()
    {
        data = new Data(){playerSelected = GameManager.Instance.PlayerList[0], multiplayerRestrictions = true};
        
        _openMenu.Debug.Activate.performed += DebugMenuEnabled;
        _openMenu.Debug.Navigate.started += DebugMenuNavigate;
        _openMenu.Enable();
    }

    private void OnDisable()
    {
        _openMenu.Debug.Activate.performed -= DebugMenuEnabled;
        _openMenu.Debug.Navigate.started -= DebugMenuNavigate;
        _openMenu.Disable();
    }

    private void DebugMenuEnabled(InputAction.CallbackContext callbackContext)
    {
        _disableEvents.gameObject.SetActive(false);
        _debugMenu.SetActive(true);

        UpdateValues();
    }

    private void DebugMenuDisabled()
    {
        _debugMenu.SetActive(false);
        _disableEvents.gameObject.SetActive(true);
    }

    private void UpdateValues()
    {
        _characterName.text = data.playerSelected.PlayerRef.name;
        _itemInHand.text = data.playerSelected.PlayerRef.HeldItem == null ? "null" : data.playerSelected.PlayerRef.HeldItem.Name;
        _multiplayerRestrictionsText.text = data.multiplayerRestrictions.ToString();
    }

    private void DebugMenuNavigate(InputAction.CallbackContext obj)
    {
        bool goingRight = obj.ReadValue<float>() > 0f;
        
        switch (EventSystem.current.currentSelectedGameObject.name)
        {
            case "PlayerSelectedButton":
                if (goingRight)
                {
                    data.playerSelected =
                        GameManager.Instance.PlayerList[
                            (data.playerSelected.Id) % GameManager.Instance.PlayerList.Count];
                }
                else
                {
                    try
                    {
                        data.playerSelected = GameManager.Instance.PlayerList[data.playerSelected.Id - 2];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        data.playerSelected = GameManager.Instance.PlayerList[^1];
                    }
                }
                break;
            
            case "MultiplayerRestrictionsButton":
                data.multiplayerRestrictions = !data.multiplayerRestrictions;
                break;
            
            case "ItemButton":
                break;
        }

        UpdateValues();
    }

    public struct Data
    {
        internal PlayerInfo playerSelected;
        internal bool multiplayerRestrictions;
        internal ItemData currentItem;
        
        public PlayerInfo PlayerSelected => playerSelected;

        public bool MultiplayerRestrictions => multiplayerRestrictions;

        public ItemData CurrentItem => currentItem;
        
        //Restart Scene
        //Reset All Puzzles
    }
}