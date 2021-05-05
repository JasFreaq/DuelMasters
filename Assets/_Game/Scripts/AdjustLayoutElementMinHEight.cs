using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class AdjustLayoutElementMinHEight : MonoBehaviour
{
    private void Awake()
    {
        LayoutElement layoutElement = GetComponent<LayoutElement>();
        if (layoutElement)
        {
            layoutElement.minHeight = layoutElement.preferredHeight;
        }
    }
}
