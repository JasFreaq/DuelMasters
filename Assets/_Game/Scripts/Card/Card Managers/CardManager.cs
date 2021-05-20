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
    private HoverPreview _hoverPreview;

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

    public HoverPreview HoverPreview
    {
        get { return _hoverPreview; }
    }

    private void Awake()
    {
        _hoverPreview = GetComponent<HoverPreview>();
    }

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

    public void SetCardVisible()
    {

        //TODO: Set Visible Eye Icon to Active in Opponent's Player Hand in Opponent's Client
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
}
