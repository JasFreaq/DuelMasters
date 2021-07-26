using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CompactCardFrameDatabase : MonoBehaviour
{
    [SerializeField] private CompactCardFrameData[] _frames = new CompactCardFrameData[10];

    public CompactCardFrameData GetFrame(CardParams.Civilization[] civilization)
    {
        foreach (CompactCardFrameData frameData in _frames)
        {
            if (CardParams.IsCivilizationEqual(frameData.civilization, civilization)) 
            {
                return frameData;
            }
        }

        return new CompactCardFrameData();
    }
}
