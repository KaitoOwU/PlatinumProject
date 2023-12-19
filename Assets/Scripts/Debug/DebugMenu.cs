using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
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
        _openMenu.Enable();
    }

    private void OnDisable()
    {
        _openMenu.Debug.Activate.performed -= DebugMenuEnabled;
        _openMenu.Disable();
    }

    public void DebugMenuEnabled(InputAction.CallbackContext callbackContext)
    {
        _openMenu.Debug.Navigate.started += DebugMenuNavigate;
        
        _disableEvents.gameObject.SetActive(false);
        _debugMenu.SetActive(true);

        UpdateValues();
    }

    public void DebugMenuDisabled()
    {
        _openMenu.Debug.Navigate.started -= DebugMenuNavigate;
        
        _debugMenu.SetActive(false);
        _disableEvents.gameObject.SetActive(true);
    }

    public void RestartScene()
    {
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetAllPuzzles()
    {
        FindObjectsOfType<MonoBehaviour>().OfType<IResettable>().ToList().ForEach(value => value.ResetAsDefault());
    }

    private void UpdateValues()
    {
        _characterName.text = data.playerSelected.PlayerRef.name;
        _itemInHand.text = data.playerSelected.PlayerRef.HeldPickable == null ? "null" : data.playerSelected.PlayerRef.HeldPickable.Name;
        _multiplayerRestrictionsText.text = data.multiplayerRestrictions.ToString();
    }

    public void UnlockInputs()
    {
        GameManager.Instance.PlayerList.Where(p => p.PlayerController.Inputs != null).ToList().ForEach(p => p.PlayerController.Inputs.InputLocked = false);
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
                if (data.currentPickable == null)
                {
                    data.currentPickable = GameManager.Instance.Items[0];
                }
                else
                {
                    if (data.currentPickable.ID + 1 < GameManager.Instance.Items.Count)
                    {
                        data.currentPickable = GameManager.Instance.Items[data.currentPickable.ID + 1];
                    }
                    else
                    {
                        data.currentPickable = null;
                    }
                }

                data.playerSelected.PlayerRef.HeldPickable = data.currentPickable;
                break;
        }

        UpdateValues();
    }

    public struct Data
    {
        internal PlayerInfo playerSelected;
        internal bool multiplayerRestrictions;
        internal PickableData currentPickable;
        
        public PlayerInfo PlayerSelected => playerSelected;

        public bool MultiplayerRestrictions => multiplayerRestrictions;

        public PickableData CurrentPickable => currentPickable;
        
        //Restart Scene
        //Reset All Puzzles
    }
}