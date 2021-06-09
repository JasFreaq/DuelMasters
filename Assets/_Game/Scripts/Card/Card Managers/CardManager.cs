using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class CardManager : MonoBehaviour
{
    public const float TAP_ANGLE = 15f;
    private const float TAP_TRANSITION_TIME = 0.5f;

    protected static readonly Color HIGHLIGHT_GLOW_COLOR = new Color(0f, 1f, 1f, 1f);
    protected static readonly Color PLAY_GLOW_COLOR = new Color(1f, 0.4117647f, 0f, 1f);

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
    
    protected bool _isTapped = false;
    protected bool _isGlowing = false;
    private bool _isGlowSelectColor = true;
    private bool _isSelected = false;

    public CardLayoutHandler CardLayout
    {
        get { return _cardLayoutHandler; }
    }

    public ManaCardLayoutHandler ManaLayout
    {
        get { return _manaCardLayoutHandler; }
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

    public bool IsTapped
    {
        get { return _isTapped; }
    }
    
    public bool IsGlowSelectColor
    {
        get { return _isGlowSelectColor; }
    }

    private void Awake()
    {
        _hoverPreviewHandler = GetComponent<HoverPreviewHandler>();
        _dragHandler = GetComponent<DragHandler>();
    }

    private void OnMouseEnter()
    {
        _hoverPreviewHandler.BeginPreviewing();
    }

    public void OnMouseDown()
    {
        _onSelect.Invoke(this);

        _dragHandler.BeginDragging();
    }

    private void OnMouseUp()
    {
        _dragHandler.EndDragging();

        _onProcessAction.Invoke(this);
    }

    private void OnMouseExit()
    {
        _hoverPreviewHandler.EndPreviewing();
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
        Color color = play ? PLAY_GLOW_COLOR : HIGHLIGHT_GLOW_COLOR;

        _cardLayoutHandler.SetGlowColor(color);
        _manaCardLayoutHandler.SetGlowColor(color);
    }

    public virtual void ToggleGlow()
    {
        _isGlowing = !_isGlowing;
        _cardLayoutHandler.SetGlow(_isGlowing);
        _manaCardLayoutHandler.SetGlow(_isGlowing);
    }

    public virtual void ToggleTap()
    {
        _isTapped = !_isTapped;
        _manaCardLayoutHandler.TappedOverlay.SetActive(_isTapped);
        
        Vector3 tapStateRotation = new Vector3(transform.localEulerAngles.x,
            _isTapped ? TAP_ANGLE : 0, transform.localEulerAngles.z);
        transform.DOLocalRotate(tapStateRotation, TAP_TRANSITION_TIME).SetEase(Ease.OutQuint);
        
        _previewCardLayout.transform.localEulerAngles = new Vector3(_previewCardLayout.transform.localEulerAngles.x,
            _isTapped ? -TAP_ANGLE : 0, _previewCardLayout.transform.localEulerAngles.z);
    }

    public void SetCardVisible()
    {
        //TODO: Set Visible Eye Icon to Active in Opponent's Player Hand in Opponent's Client
    }

    public void Select(bool selected)
    {
        _isSelected = selected;
        ToggleGlow();
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
