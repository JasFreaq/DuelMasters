using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CardFrameDatabase : MonoBehaviour
{
    [SerializeField] private CardFrameData[] _frames = new CardFrameData[10];

    public CardFrameData GetFrame(CardParams.Civilization[] civilization)
    {
        foreach (CardFrameData frameData in _frames)
        {
            if (CardParams.IsCivilizationEqual(frameData.civilization, civilization))
            {
                return frameData;
            }
        }

        return new CardFrameData();
    }
}
