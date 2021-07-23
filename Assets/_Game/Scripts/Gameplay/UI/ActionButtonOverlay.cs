using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonOverlay : MonoBehaviour
{
    [SerializeField] private Button _actionButton;
    [SerializeField] private TextMeshProUGUI _actionText;

    public void SetText(string text)
    {
        _actionText.text = text;
    }
}
