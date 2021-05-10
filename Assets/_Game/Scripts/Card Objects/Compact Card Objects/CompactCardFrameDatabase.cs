using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompactCardFrameDatabase : MonoBehaviour
{
    [SerializeField] private CompactCardFrameData[] _frames = new CompactCardFrameData[10];

    public CompactCardFrameData GetFrame(CardParams.Civilization[] civilization)
    {
        foreach (CompactCardFrameData frameData in _frames)
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

        return new CompactCardFrameData();
    }
}
