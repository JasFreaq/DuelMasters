using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldsLayoutHandler : MonoBehaviour
{
    [SerializeField] private Shield _shieldPrefab;
    [SerializeField] private float _shieldAreaWidth = 24;
    [SerializeField] private float _maxShieldWidth = 8;
    [SerializeField] private float _shieldScale = 12.5f;
    [SerializeField] private Transform _holderTransform;
    [SerializeField] private Transform _tempShield;

    private ShieldsManager _shieldsManager;

    public void Initialize(ShieldsManager shieldsManager)
    {
        _shieldsManager = shieldsManager;

        for (int i = 0; i < 5; i++)
        {
            Shield shield = Instantiate(_shieldPrefab, _holderTransform);
            shieldsManager.Shields.Add(shield);
        }

        ArrangeShields();
    }

    private void ArrangeShields()
    {
        int n = _holderTransform.childCount;
        if (n >= 5) 
        {
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
                _shieldsManager.Shields[i].UpdateHolderScale();
            }
        }
    }

    public void AddShield()
    {
        Shield shield = Instantiate(_shieldPrefab, _holderTransform);
        _shieldsManager.Shields.Add(shield);
        ArrangeShields();
    }

    public void RemoveShield(int shieldIndex)
    {
        int n = _holderTransform.childCount;
        if (n > 5 && shieldIndex < n) 
        {
            Destroy(_holderTransform.GetChild(shieldIndex).gameObject);
        }
    }
}
