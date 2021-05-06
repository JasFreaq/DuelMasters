using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlavorTextObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    public void SetupFlavorText(string flavorText)
    {
        _text.text = "<indent=50>" + flavorText;
        _text.gameObject.SetActive(true);
    }
}
