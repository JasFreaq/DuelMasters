using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class CardInstanceObject : CardBehaviour
{
    [SerializeField] private BoxCollider _cardLayoutCollider;
    [SerializeField] private BoxCollider _compactCardLayoutCollider;
    [SerializeField] private GameObject _visibleEyeIcon;
    [SerializeField] protected CardLayoutHandler _previewCardLayout;
    [SerializeField] protected CardLayoutHandler _cardLayoutHandler;
    [SerializeField] protected ManaCardLayoutHandler _manaCardLayoutHandler;
    
    private CardData _cardData;
    private HoverPreviewHandler _hoverPreviewHandler;
    private DragHandler _dragHandler;

    private Action<CardInstanceObject> _onProcessAction;

    private bool _isPlayer;
    protected CardZone _currentZone = 0;
    
    protected bool _isTapped = false;
    protected bool _isHighlighted = false;
    protected bool _isSelected = false;
    private bool _isHighlightBaseColor = true;
    private bool _isVisible = false;
    private bool _canDrag = false;
    
    private bool _processAction = false;
    private bool _inPlayerHand = false;
    
    #region Properties

    public bool IsPlayer
    {
        set { _isPlayer = value; }
    }

    public CardLayoutHandler CardLayout
    {
        get { return _cardLayoutHandler; }
    }

    public ManaCardLayoutHandler ManaLayout
    {
        get { return _manaCardLayoutHandler; }
    }
    
    public CardData CardData
    {
        get { return _cardData; }
    }

    public HoverPreviewHandler HoverPreviewHandler
    {
        get { return _hoverPreviewHandler; }
    }

    public DragHandler DragHandler
    {
        get { return _dragHandler; }
    }

    public CardZone CurrentZone
    {
        set { _currentZone = value; }
    }

    public bool IsTapped
    {
        get { return _isTapped; }
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

    public bool ProcessAction
    {
        get { return _processAction;}
        set { _processAction = value; }
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

    public void ProcessMouseEnter()
    {
        _hoverPreviewHandler.BeginPreviewing();
    }

    public virtual void ProcessMouseDown()
    {
        if (_currentZone == CardZone.Hand)
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
                    if (_processAction)
                    {
                        _onProcessAction?.Invoke(this);
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

    public virtual void SetupCard(CardData cardData)
    {
        _cardData = cardData;
        
        _cardLayoutHandler.SetupCard(cardData);
        _manaCardLayoutHandler.SetupCard(cardData);

        _previewCardLayout.SetupCard(cardData);
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
    
    public virtual void ToggleTap()
    {
        _isTapped = !_isTapped;
        _manaCardLayoutHandler.TappedOverlay.SetActive(_isTapped);
        float tapAngle = GameParamsHolder.Instance.TapAngle;
        Vector3 tapStateRotation = transform.localEulerAngles;
        Vector3 previewTapStateRotation = _previewCardLayout.transform.localEulerAngles;
        PlayerDataHandler dataHandler = GameDataHandler.Instance.GetDataHandler(_isPlayer);

        if (_isTapped)
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

    public void DestroyCard()
    {
        gameObject.SetActive(false);
        ActivateCardLayout();

        PlayerManager manager = GameManager.Instance.GetManager(_isPlayer);
        manager.GraveyardManager.AddCard(this);
        switch (_currentZone)
        {
            case CardZone.BattleZone:
                GameDataHandler.Instance.GetDataHandler(_isPlayer).CardsInBattle.Remove(transform.GetInstanceID());
                GameManager.Instance.GetManager(_isPlayer).BattleZoneManager.ArrangeCards();
                break;
        }

        gameObject.SetActive(true);
    }

    public bool InZone(CardZone zone)
    {
        return _currentZone == zone;
    }

    #endregion
    
    #region Register Callbacks
    
    public void RegisterOnProcessAction(Action<CardInstanceObject> action)
    {
        _onProcessAction += action;
    }

    public void DeregisterOnProcessAction(Action<CardInstanceObject> action)
    {
        _onProcessAction -= action;
    }
    
    #endregion
}
