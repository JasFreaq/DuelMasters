using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class CardObject : CardBehaviour
{
    [SerializeField] private BoxCollider _cardLayoutCollider;
    [SerializeField] private BoxCollider _compactCardLayoutCollider;
    [SerializeField] private GameObject _visibleEyeIcon;
    [SerializeField] protected CardLayoutHandler _previewCardLayout;
    [SerializeField] protected CardLayoutHandler _cardLayoutHandler;
    [SerializeField] protected ManaCardLayoutHandler _manaCardLayoutHandler;
    
    protected CardInstance _cardInst;

    private HoverPreviewHandler _hoverPreviewHandler;
    private DragHandler _dragHandler;

    private Action<CardObject> _onDragReleaseAction;
    private Func<CardObject, Coroutine> _onSendToGraveFunc;

    private bool _isPlayer;
    protected bool _isHighlighted = false;
    protected bool _isSelected = false;
    private bool _isHighlightBaseColor = true;
    private bool _isVisible = false;
    private bool _canDrag = false;
    
    private bool _processDragRelease = false;
    private bool _inPlayerHand = false;
    
    #region Properties

    public bool IsPlayer
    {
        get { return _isPlayer;}
    }

    public CardLayoutHandler CardLayout
    {
        get { return _cardLayoutHandler; }
    }

    public ManaCardLayoutHandler ManaLayout
    {
        get { return _manaCardLayoutHandler; }
    }
    
    public CardInstance CardInst
    {
        get { return _cardInst; }
    }

    public HoverPreviewHandler HoverPreviewHandler
    {
        get { return _hoverPreviewHandler; }
    }

    public DragHandler DragHandler
    {
        get { return _dragHandler; }
    }
    
    public bool IsHighlightBaseColor
    {
        get { return _isHighlightBaseColor; }
    }
    
    public bool IsVisible
    {
        get { return _isVisible; }
        set { _isVisible = value; }
    }

    public bool CanDrag
    {
        set { _canDrag = value; }
    }

    public bool ProcessDragRelease
    {
        get { return _processDragRelease;}
        set { _processDragRelease = value; }
    }
    
    public bool InPlayerHand
    {
        set
        {
            _inPlayerHand = value;
            _hoverPreviewHandler.InPlayerHand = _inPlayerHand;
            if (_inPlayerHand) 
                _hoverPreviewHandler.RegisterOnBeginPlayerHandPreview(SetDragOrientationOnPreviewBegin);
            else
                _hoverPreviewHandler.DeregisterOnBeginPlayerHandPreview(SetDragOrientationOnPreviewBegin);
        }
    }
    
    #endregion

    private void Awake()
    {
        _hoverPreviewHandler = GetComponent<HoverPreviewHandler>();
        _dragHandler = GetComponent<DragHandler>();
    }

    public void Initialize(CardInstance cardInst, bool isPlayer)
    {
        _cardInst = cardInst;
        SetupCard();
        name = _cardInst.CardData.Name;

        _isPlayer = isPlayer;
        ActivateCardLayout();
        _cardLayoutHandler.Canvas.gameObject.SetActive(false);

        CardInst.SetCurrentZone(CardZoneType.Deck);
    }

    public void ProcessMouseEnter()
    {
        _hoverPreviewHandler.BeginPreviewing();
    }

    public virtual void ProcessMouseDown()
    {
        if (_cardInst.CurrentZone == CardZoneType.Hand)
        {
            _isSelected = !_isSelected;

            if (_canDrag)
            {
                if (_isSelected)
                {
                    _dragHandler.BeginDragging();
                }
                else
                {
                    if (_processDragRelease)
                    {
                        _onDragReleaseAction?.Invoke(this);
                    }
                    else
                    {
                        _hoverPreviewHandler.ShouldStopPreview = false;
                        _dragHandler.EndDragging();
                    }
                }
            }
        }
    }
    
    public void ProcessMouseExit()
    {
        _hoverPreviewHandler.EndPreviewing();
        _hoverPreviewHandler.ShouldStopPreview = true;
    }
    
    public void SetDragOrientationOnPreviewBegin()
    {
        _dragHandler.SetOriginalOrientation(transform.localPosition, transform.localRotation);
    }

    #region Setup Methods

    protected virtual void SetupCard()
    {
        _cardLayoutHandler.SetupCard(_cardInst.CardData);
        _manaCardLayoutHandler.SetupCard(_cardInst.CardData);

        _previewCardLayout.SetupCard(_cardInst.CardData);
    }
    
    public virtual void ActivateCardLayout()
    {
        _cardLayoutHandler.gameObject.SetActive(true);
        _manaCardLayoutHandler.gameObject.SetActive(false);

        ActivateCardCollider();
    }
    
    public virtual void ActivateManaLayout()
    {
        _cardLayoutHandler.gameObject.SetActive(false);
        _manaCardLayoutHandler.gameObject.SetActive(true);

        ActivateCompactCardCollider();
    }

    protected void ActivateCardCollider()
    {
        _cardLayoutCollider.enabled = true;
        _compactCardLayoutCollider.enabled = false;
    }

    protected void ActivateCompactCardCollider()
    {
        _cardLayoutCollider.enabled = false;
        _compactCardLayoutCollider.enabled = true;
    }

    #endregion

    #region State Methods
    
    public virtual void SetHighlightColor(bool play)
    {
        _isHighlightBaseColor = !play;
        Color color = play ? GameParamsHolder.Instance.PlayHighlightColor 
            : GameParamsHolder.Instance.BaseHighlightColor;

        _cardLayoutHandler.SetHighlightColor(color);
        _manaCardLayoutHandler.SetHighlightColor(color);
    }

    public virtual void SetHighlight(bool highlight)
    {
        _isHighlighted = highlight;
        _cardLayoutHandler.SetGlow(_isHighlighted);
        _manaCardLayoutHandler.SetGlow(_isHighlighted);
    }
    
    public virtual void ToggleTapState()
    {
        _cardInst.ToggleTapState();
        _manaCardLayoutHandler.TappedOverlay.SetActive(_cardInst.IsTapped);
        float tapAngle = GameParamsHolder.Instance.TapAngle;
        Vector3 tapStateRotation = transform.localEulerAngles;
        Vector3 previewTapStateRotation = _previewCardLayout.transform.localEulerAngles;
        PlayerDataHandler dataHandler = GameDataHandler.Instance.GetDataHandler(_isPlayer);

        if (_cardInst.IsTapped)
        {
            tapStateRotation.y = tapAngle;
            previewTapStateRotation.y = -tapAngle;
            dataHandler.TappedCards.Add(this);
        }
        else
        {
            tapStateRotation.y = 0;
            previewTapStateRotation.y = 0;
            dataHandler.TappedCards.Remove(this);
        }

        transform.DOLocalRotateQuaternion(Quaternion.Euler(tapStateRotation), GameParamsHolder.Instance.TapTransitionTime).SetEase(Ease.OutQuint);
        _previewCardLayout.transform.localRotation = Quaternion.Euler(previewTapStateRotation);
    }

    public void SetVisibleIcon(bool visible)
    {
        _visibleEyeIcon.SetActive(visible);
    }

    public Coroutine SendToGraveyard()
    {
        return _onSendToGraveFunc.Invoke(this);
    }

    public bool InZone(CardZoneType zone)
    {
        return _cardInst.CurrentZone == zone;
    }

    #endregion
    
    #region Register Callbacks
    
    public void RegisterOnDragRelease(Action<CardObject> action)
    {
        _onDragReleaseAction += action;
    }

    public void DeregisterOnDragRelease(Action<CardObject> action)
    {
        _onDragReleaseAction -= action;
    }

    public void RegisterOnSendToGrave(Func<CardObject, Coroutine> func)
    {
        _onSendToGraveFunc += func;
    }
    
    public void DeregisterOnSendToGrave(Func<CardObject, Coroutine> func)
    {
        _onSendToGraveFunc -= func;
    }

    #endregion
}