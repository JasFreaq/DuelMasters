using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldsLayoutHandler : MonoBehaviour
{
    [SerializeField] private Shield _shieldPrefab;
    [SerializeField] private float _shieldAreaWidth = 25;
    [SerializeField] private float _maxShieldWidth = 10;
    [SerializeField] private float _shieldScale = 12.5f;
    [SerializeField] private Transform _holderTransform;
    
    public List<Shield> Initialize(List<Shield> shields)
    {
        for (int i = 0; i < 5; i++)
        {
            Shield shield = Instantiate(_shieldPrefab, _holderTransform);
            shields.Add(shield);
        }

        ArrangeShields();
        return shields;
    }

    private void ArrangeShields()
    {
        int n = _holderTransform.childCount;
        float shieldWidth = Mathf.Min((_shieldAreaWidth * 2) / n, _maxShieldWidth);
        float sizeRatio = (shieldWidth / _maxShieldWidth) * _shieldScale;

        float startOffset = (n % 2) * shieldWidth;
        if (n % 2 == 0)
            startOffset += shieldWidth / 2;
        Vector3 startPos = new Vector3(_holderTransform.localPosition.x - startOffset,
            _holderTransform.localPosition.y, _holderTransform.localPosition.z);

        for (int i = 0; i < n; i++)
        {
            Transform shieldTransform = _holderTransform.GetChild(i);
            Vector3 shieldPos = new Vector3(startPos.x + (i - n / 2 + 1) * shieldWidth, startPos.y, startPos.z);

            shieldTransform.localPosition = shieldPos;
            shieldTransform.localScale = new Vector3(sizeRatio, sizeRatio, sizeRatio);
        }
    }

    public List<Shield> AddShield(List<Shield> shields)
    {
        Shield shield = Instantiate(_shieldPrefab, _holderTransform);
        shields.Add(shield);
        ArrangeShields();
        return shields;
    }

    public void RemoveShield(int shieldIndex)
    {
        int n = _holderTransform.childCount;
        if (n > 5 && shieldIndex < n)
        {
            Transform shieldTransform = _holderTransform.GetChild(shieldIndex);
            shieldTransform.parent = transform;
            Destroy(shieldTransform.gameObject);
            ArrangeShields();
        }
    }
}
