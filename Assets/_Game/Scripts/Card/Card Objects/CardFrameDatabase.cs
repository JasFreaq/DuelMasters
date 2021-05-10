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
                return frameData;
        }

        return new CardFrameData();
    }
}
