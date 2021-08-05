using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExtendedScrollbar : Scrollbar
{
    public new bool IsPressed
    {
        get { return IsPressed(); }
    }
}
