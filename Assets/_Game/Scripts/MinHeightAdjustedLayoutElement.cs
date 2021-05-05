using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinHeightAdjustedLayoutElement : LayoutElement
{
    public override void CalculateLayoutInputVertical()
    {
        base.CalculateLayoutInputVertical();

        minHeight = preferredHeight;
    }
}
