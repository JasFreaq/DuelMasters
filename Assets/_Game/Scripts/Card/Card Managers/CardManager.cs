using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class CardManager : MonoBehaviour
{
    [SerializeField] private BoxCollider _cardLayoutCollider;
    [SerializeField] private BoxCollider _compactCardLayoutCollider;
    [SerializeField] private GameObject _visibleEyeIcon;
    [SerializeField] protected CardLayoutHandler _previewCardLayout;
    [SerializeField] protected CardLayoutHandler _cardLayoutHandler;
    [SerializeField] protected ManaCardLayoutHandler _manaCardLayoutHandler;
    
    private Card _card;
    private HoverPreviewHandler _hoverPreviewHandler;
    private DragHandler _dragHandler;

    private Action<CardManager> _onSelect;
    private Action<CardManager> _onProcessAction;
    
    protected CardZone _currentZone = 0;
    
    protected bool _isTapped = false;
    protected bool _isGlowing = false;
    private bool _isGlowSelectColor = true;
    private bool _isSelected = false;
    private bool _isVisible = false;
    private bool _canDrag = false;
    
    private bool _processAction = false;
    private bool _inPlayerHand = false;
    
    #region Properties

    public CardLayoutHandler CardLayout
    {
        get { return _cardLayoutHandler; }
    }

    public ManaCardLayoutHandler ManaLayout
    {
        get { return _manaCardLayoutHandler; }
    }

    public GameObject VisibleEyeIcon
    {
        get { return _visibleEyeIcon; }
    }

    public Card CardData
    {
        get { return _card; }
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
    
    public bool IsGlowSelectColor
    {
        get { return _isGlowSelectColor; }
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

    private void OnMouseEnter()
    {
        _hoverPreviewHandler.BeginPreviewing();
    }

    private void OnMouseDown()
    {
        if (_currentZone == CardZone.Hand) 
        {
            _onSelect.Invoke(this);
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

    private void OnMouseExit()
    {
        _hoverPreviewHandler.EndPreviewing();
        _hoverPreviewHandler.ShouldStopPreview = true;
    }

    public void SetDragOrientationOnPreviewBegin()
    {
        _dragHandler.SetOriginalOrientation(transform.localPosition, transform.localRotation);
    }

    #region Setup Methods

    public virtual void SetupCard(Card card)
    {
        _card = card;
        
        _cardLayoutHandler.SetupCard(card);
        _manaCardLayoutHandler.SetupCard(card);

        _previewCardLayout.SetupCard(card);
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

    public virtual void SetGlowColor(bool play)
    {
        _isGlowSelectColor = !play;
        Color color = play ? GameParamsHolder.Instance.PlayGlowColor 
            : GameParamsHolder.Instance.HighlightGlowColor;

        _cardLayoutHandler.SetGlowColor(color);
        _manaCardLayoutHandler.SetGlowColor(color);
    }

    public virtual void SetGlow(bool enableGlow)
    {
        _isGlowing = enableGlow;
        _cardLayoutHandler.SetGlow(_isGlowing);
        _manaCardLayoutHandler.SetGlow(_isGlowing);
    }

    public virtual void ToggleTap()
    {
        _isTapped = !_isTapped;
        _manaCardLayoutHandler.TappedOverlay.SetActive(_isTapped);

        float tapAngle = GameParamsHolder.Instance.TapAngle;
        
        Vector3 tapStateRotation = new Vector3(transform.localEulerAngles.x,
            _isTapped ? tapAngle : 0, transform.localEulerAngles.z);
        transform.DOLocalRotate(tapStateRotation, GameParamsHolder.Instance.TapTransitionTime).SetEase(Ease.OutQuint);
        
        _previewCardLayout.transform.localEulerAngles = new Vector3(_previewCardLayout.transform.localEulerAngles.x,
            _isTapped ? -tapAngle : 0, _previewCardLayout.transform.localEulerAngles.z);
    }

    public void SetCardVisible()
    {
        //TODO: Set Visible Eye Icon to Active in Opponent's Player Hand in Opponent's Client
    }
    
    #endregion
    
    #region Register Callbacks
    
    public void RegisterOnProcessAction(Action<CardManager> action)
    {
        _onProcessAction += action;
    }

    public void DeregisterOnProcessAction(Action<CardManager> action)
    {
        _onProcessAction -= action;
    }
    
    public void RegisterOnSelect(Action<CardManager> action)
    {
        _onSelect += action;
    }

    public void DeregisterOnSelect(Action<CardManager> action)
    {
        _onSelect -= action;
    }

    #endregion
}
