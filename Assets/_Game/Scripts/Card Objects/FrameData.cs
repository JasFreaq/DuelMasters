using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CardFrameData
{
    public CardParams.Civilization[] civilization;
    public float cardTypePosY;
    public Sprite frameImage;
}

[System.Serializable]
public struct CompactCardFrameData
{
    public CardParams.Civilization[] civilization;
    public Sprite frameImage;
    public Material frameBGMaterial;
}