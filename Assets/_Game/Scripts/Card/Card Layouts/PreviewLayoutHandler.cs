using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CardLayoutHandler))]
public class PreviewLayoutHandler : MonoBehaviour
{
    [SerializeField] private GameObject _invalidOverlay;

    private CardLayoutHandler _cardLayoutHandler;
    private EventTrigger _canvasEventTrigger;

    private bool _isValid;
    private bool _isHighlighted;
    
    public Canvas Canvas
    {
        get { return _cardLayoutHandler.Canvas; }
    }

    public bool IsValid
    {
        get { return _isValid; }
    }
    
    public bool IsHighlighted
    {
        get { return _isHighlighted; }
    }

    private void Awake()
    {
        _cardLayoutHandler = GetComponent<CardLayoutHandler>();
        _canvasEventTrigger = GetComponentInChildren<EventTrigger>();
    }

    private void Start()
    {
        _canvasEventTrigger.enabled = false;
    }

    public void SetupCard(CardData cardData, CardFrameData cardFrameData)
    {
        _cardLayoutHandler.SetupCard(cardData, cardFrameData);
    }
    
    public void SetValidity(bool isValid)
    {
        _isValid = isValid;
        _invalidOverlay.SetActive(!isValid);
    }

    public void SetHighlight(bool highlight)
    {
        _isHighlighted = highlight;
        _cardLayoutHandler.SetGlow(_isHighlighted);
    }

    public void ResetStates()
    {
        _isValid = false;
        _invalidOverlay.SetActive(false);

        _isHighlighted = false;
        _cardLayoutHandler.SetGlow(false);
    }

    #region Event Trigger Methods

    public void EnableCanvasEventTrigger(bool enable)
    {
        _canvasEventTrigger.enabled = enable;
    }
    
    public void AddOnClickEvent(Action<CardObject> action, CardObject cardObj)
    {
        if (TriggerContainsOnClick() == null) 
        {
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entry.callback.AddListener(arg0 => action.Invoke(cardObj));
            _canvasEventTrigger.triggers.Add(entry);
        }
    }

    public void RemoveOnClickEvent()
    {
        EventTrigger.Entry entry = TriggerContainsOnClick();
        if (entry != null)
            _canvasEventTrigger.triggers.Remove(entry);
    }
    
    private EventTrigger.Entry TriggerContainsOnClick()
    {
        EventTrigger.Entry entry = null;
        foreach (EventTrigger.Entry entryTrigger in _canvasEventTrigger.triggers)
        {
            if (entryTrigger.eventID == EventTriggerType.PointerClick)
            {
                entry = entryTrigger;
                break;
            }
        }

        return entry;
    }

    #endregion
}
