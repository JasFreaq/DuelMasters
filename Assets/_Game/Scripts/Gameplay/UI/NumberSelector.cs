using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberSelector : MonoBehaviour
{
    [SerializeField] private GameObject _holderCanvas;
    [SerializeField] private TextMeshProUGUI _numText;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;

    private int _selectionNum, _lowerBound, _upperBound;
    private bool _selectionMade;

    private void Start()
    {
        _holderCanvas.SetActive(false);
    }
    
    public IEnumerator GetSelectionRoutine(int lower, int upper)
    {
        _selectionNum = _lowerBound = lower;
        _upperBound = upper;
        _numText.text = _selectionNum.ToString();
        _leftButton.interactable = false;
        _rightButton.interactable = true;
        _holderCanvas.SetActive(true);

        while (!_selectionMade) 
        {
            yield return new WaitForEndOfFrame();
        }

        _selectionMade = false;
        yield return _selectionNum;
    }

    public void DecreaseSelection()
    {
        _selectionNum = Mathf.Max(_selectionNum - 1, _lowerBound);
        _numText.text = _selectionNum.ToString();

        if (_selectionNum == _lowerBound && _leftButton.IsInteractable())
            _leftButton.interactable = false;

        if (!_rightButton.IsInteractable())
            _rightButton.interactable = true;
    }
    
    public void IncreaseSelection()
    {
        _selectionNum = Mathf.Min(_selectionNum + 1, _upperBound);
        _numText.text = _selectionNum.ToString();

        if (_selectionNum == _upperBound && _rightButton.IsInteractable())
            _rightButton.interactable = false;

        if (!_leftButton.IsInteractable())
            _leftButton.interactable = true;
    }

    public void MakeSelection()
    {
        _holderCanvas.SetActive(false);
        _selectionMade = true;
    }
}
