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
    
    private bool _activeInCardBrowser, _isHighlighted;
    
    public Canvas Canvas
    {
        get { return _cardLayoutHandler.Canvas; }
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

    public void SetActiveInBrowser()
    {
        _activeInCardBrowser = true;
    }

    public void SetInvalidOverlay(bool isValid)
    {
        if (_activeInCardBrowser)
            _invalidOverlay.SetActive(!isValid);
    }

    public void SetHighlight(bool highlight)
    {
        _isHighlighted = highlight;
        _cardLayoutHandler.SetGlow(_isHighlighted);
    }

    public void ResetStates()
    {
        if (_activeInCardBrowser)
        {
            _activeInCardBrowser = false;
            _invalidOverlay.SetActive(false);

            _isHighlighted = false;
            _cardLayoutHandler.SetGlow(false);
        }
    }

    #region Event Trigger Methods

    public void EnableCanvasEventTrigger(bool enable)
    {
        _canvasEventTrigger.enabled = enable;
    }

    public void AddCanvasEventTriggers(Action<CardObject> enterAction, Action<CardObject> clickAction, Action<CardObject> exitAction,
        CardObject cardObj)
    {
        AddOnEnterEvent(enterAction, cardObj);
        AddOnClickEvent(clickAction, cardObj);
        AddOnExitEvent(exitAction, cardObj);
    }

    public void RemoveCanvasEventTriggers()
    {
        RemoveOnEnterEvent();
        RemoveOnClickEvent();
        RemoveOnExitEvent();
    }

    private void AddOnEnterEvent(Action<CardObject> action, CardObject cardObj)
    {
        if (TriggerContainsOnEnter() == null)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entry.callback.AddListener(arg0 => action.Invoke(cardObj));
            _canvasEventTrigger.triggers.Add(entry);
        }
    }

    private void AddOnClickEvent(Action<CardObject> action, CardObject cardObj)
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

    private void AddOnExitEvent(Action<CardObject> action, CardObject cardObj)
    {
        if (TriggerContainsOnExit() == null)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            entry.callback.AddListener(arg0 => action.Invoke(cardObj));
            _canvasEventTrigger.triggers.Add(entry);
        }
    }

    private void RemoveOnEnterEvent()
    {
        EventTrigger.Entry entry = TriggerContainsOnEnter();
        if (entry != null)
            _canvasEventTrigger.triggers.Remove(entry);
    }

    private void RemoveOnClickEvent()
    {
        EventTrigger.Entry entry = TriggerContainsOnClick();
        if (entry != null)
            _canvasEventTrigger.triggers.Remove(entry);
    }

    private void RemoveOnExitEvent()
    {
        EventTrigger.Entry entry = TriggerContainsOnExit();
        if (entry != null)
            _canvasEventTrigger.triggers.Remove(entry);
    }
    
    private EventTrigger.Entry TriggerContainsOnEnter()
    {
        EventTrigger.Entry entry = null;
        foreach (EventTrigger.Entry entryTrigger in _canvasEventTrigger.triggers)
        {
            if (entryTrigger.eventID == EventTriggerType.PointerEnter)
            {
                entry = entryTrigger;
                break;
            }
        }

        return entry;
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

    private EventTrigger.Entry TriggerContainsOnExit()
    {
        EventTrigger.Entry entry = null;
        foreach (EventTrigger.Entry entryTrigger in _canvasEventTrigger.triggers)
        {
            if (entryTrigger.eventID == EventTriggerType.PointerExit)
            {
                entry = entryTrigger;
                break;
            }
        }

        return entry;
    }

    #endregion
}
