using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionButtonOverlay : MonoBehaviour
{
    #region Helper Data Structures

    [System.Serializable]
    struct ActionButton
    {
        public Button button;
        public TextMeshProUGUI text;

        public GameObject gameObject
        {
            get { return button.gameObject; }
        }
    }

    #endregion
    
    [SerializeField] private List<Button> _buttons = new List<Button>();

    private List<ActionButton> _actionButtons = new List<ActionButton>(2);

    private void Start()
    {
        foreach (Button button in _buttons)
        {
            ActionButton actionButton = new ActionButton
            {
                button = button,
                text = button.GetComponentInChildren<TextMeshProUGUI>()
            };
            _actionButtons.Add(actionButton);
        }
    }

    public void ActivateMayUseEffectButton(UnityAction activateAction, UnityAction cancelAction)
    {
        ActionButton activateButton = _actionButtons[0];
        if (!activateButton.gameObject.activeInHierarchy)
        {
            activateButton.button.onClick.RemoveAllListeners();
            activateButton.button.onClick.AddListener(activateAction);
            activateButton.text.text = "Activate Effect";
            activateButton.button.gameObject.SetActive(true);
        }

        ActionButton cancelButton = _actionButtons[1];
        if (!cancelButton.gameObject.activeInHierarchy)
        {
            cancelButton.button.onClick.RemoveAllListeners();
            cancelButton.button.onClick.AddListener(cancelAction);
            cancelButton.text.text = "Cancel";
            cancelButton.button.gameObject.SetActive(true);
        }
    }

    #region Card Selection Methods

    public void ActivateCardSelectionButtons(UnityAction submitAction, UnityAction cancelAction, bool mayUse)
    {
        ActionButton submitButton = _actionButtons[0];
        if (!submitButton.gameObject.activeInHierarchy)
        {
            submitButton.button.onClick.RemoveAllListeners();
            submitButton.button.onClick.AddListener(submitAction);
            submitButton.button.gameObject.SetActive(true);
        }

        ActionButton cancelButton = _actionButtons[1];
        if (mayUse && !cancelButton.gameObject.activeInHierarchy)
        {
            cancelButton.button.onClick.RemoveAllListeners();
            cancelButton.button.onClick.AddListener(cancelAction);
            cancelButton.text.text = "Cancel";
            cancelButton.button.gameObject.SetActive(true);
        }
        else if (!mayUse && cancelButton.gameObject.activeInHierarchy)
            cancelButton.button.gameObject.SetActive(false);
    }

    public void AdjustCardSelectionButtons(int lower, int upper, int selected)
    {
        ActionButton submitButton = _actionButtons[0];
        if (selected >= lower && selected <= upper)
        {
            if (!submitButton.button.interactable)
                submitButton.button.interactable = true;
        }
        else
        {
            if (submitButton.button.interactable)
                submitButton.button.interactable = false;
        }
        submitButton.text.text = $"Submit {selected}";
    }

    #endregion

    public void DeactivateButtons()
    {
        foreach (ActionButton actionButton in _actionButtons)
            actionButton.gameObject.SetActive(false);
    }
}
