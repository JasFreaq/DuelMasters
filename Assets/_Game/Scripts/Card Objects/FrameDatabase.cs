using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class FrameDatabase : MonoBehaviour
{
    [SerializeField] private FrameData[] _frames = new FrameData[10];
    
    public Sprite GetFrame(CardParams.Civilization[] civilization)
    {
        foreach (FrameData frameData in _frames)
        {
            bool isCivilization = true;
            if (frameData.civilization.Length == civilization.Length)
            {
                for (int i = 0, n = civilization.Length; i < n; i++)
                {
                    if (frameData.civilization[i] != civilization[i])
                    {
                        isCivilization = false;
                        break;
                    }
                }
            }
            else
                isCivilization = false;

            if (isCivilization)
                return frameData.frameImage;
        }

        return null;
    }
}
