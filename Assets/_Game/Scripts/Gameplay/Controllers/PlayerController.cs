using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Helper Data Structures

    enum HoverState
    {
        None,
        Hover
    };

    #endregion

    private Camera mainCamera = null;

    private CardManager _hoveredCard;
    private HoverState _hoverState = HoverState.None;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            int iD = hit.transform.GetInstanceID();
            if (GameManager.PlayerCards.ContainsKey(iD))
            {
                CardManager card = GameManager.PlayerCards[iD];

                if (_hoveredCard != card)
                {
                    ExitHover();

                    if (_hoverState == HoverState.None)
                    {
                        _hoveredCard = card;

                        //OnMouseEnter
                        _hoveredCard.ProcessMouseEnter();
                    }

                    _hoverState = HoverState.Hover;
                }
            }
            else
            {
                ExitHover();
            }

            if (_hoverState == HoverState.Hover)
            {
                //OnMouseOver

                if (Input.GetMouseButtonDown(0))
                {
                    //OnMouseDown
                    _hoveredCard.ProcessMouseDown();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    //OnMouseOver
                }
            }
        }
    }

    private void ExitHover()
    {
        if (_hoverState == HoverState.Hover)
        {
            //OnMouseExit
            _hoveredCard.ProcessMouseExit();
            _hoveredCard = null;
        }

        _hoverState = HoverState.None;
    }
}
