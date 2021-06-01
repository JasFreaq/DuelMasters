using System;
using System.Collections;
using System.Collections.Generic;
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

    private Action<CardManager> _onProcessAction;

    private bool _isGlowing = false;

    public CardLayoutHandler CardLayout
    {
        get { return _cardLayoutHandler; }
    }

    public ManaCardLayoutHandler ManaLayout
    {
        get { return _manaCardLayoutHandler; }
        set { _manaCardLayoutHandler = value; }
    }

    public Card Card
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

    public bool IsGlowing
    {
        set
        {
            _isGlowing = value;
            SetGlow(_isGlowing);
        }
    }

    #region Static Data Members

    private static CardManager _CurrentlySelected;

    public static CardManager CurrentlySelected
    {
        get { return _CurrentlySelected; }
    }

    #endregion

    private void Awake()
    {
        _hoverPreviewHandler = GetComponent<HoverPreviewHandler>();
        _dragHandler = GetComponent<DragHandler>();
    }

    private void OnEnable()
    {
        if (_dragHandler)
            _dragHandler.RegisterOnDragRelease(ProcessAction);
    }

    private void OnMouseEnter()
    {
        if (!(_isGlowing || _CurrentlySelected))
            SetGlow(true);
    }

    private void OnMouseUpAsButton()
    {
        Select();
    }

    private void OnMouseExit()
    {
        if (!(_isGlowing || _CurrentlySelected))
            SetGlow(false);
    }
    
    private void OnDisable()
    {
        if (_dragHandler)
            _dragHandler.DeregisterOnDragRelease(ProcessAction);
    }

    #region Setup Methods

    public virtual void SetupCard(Card card, bool considerAsDataObject = false)
    {
        _card = card;

        if (!considerAsDataObject) 
        {
            _cardLayoutHandler.SetupCard(card);
            _manaCardLayoutHandler.SetupCard(card);

            _previewCardLayout.SetupCard(card);
        }
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

    public void Select()
    {
        if (_CurrentlySelected != this)
        {
            _isGlowing = true;
            if (_CurrentlySelected)
                DeselectCurrentSelection();
            _CurrentlySelected = this;
        }
        else if (_CurrentlySelected)
            DeselectCurrentSelection();
    }

    public void SetCardVisible()
    {
        //TODO: Set Visible Eye Icon to Active in Opponent's Player Hand in Opponent's Client
    }

    protected virtual void SetGlow(bool enableGlow)
    {
        _cardLayoutHandler.SetGlow(enableGlow);
        _manaCardLayoutHandler.SetGlow(enableGlow);
    }
    
    #endregion

    private void ProcessAction()
    {
        _onProcessAction.Invoke(this);
    }

    public void RegisterOnProcessAction(Action<CardManager> action)
    {
        _onProcessAction += action;
    }

    public void DeregisterOnProcessAction(Action<CardManager> action)
    {
        _onProcessAction -= action;
    }

    #region Static Methods

    private static void DeselectCurrentSelection()
    {
        _CurrentlySelected.IsGlowing = false;
        _CurrentlySelected = null;
    }

    #endregion
}
