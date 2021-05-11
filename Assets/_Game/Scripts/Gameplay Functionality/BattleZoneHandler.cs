using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleZoneHandler : MonoBehaviour
{
    [SerializeField] private float _cardAreaWidth = 24;
    [SerializeField] private float _maxCardWidth = 8;
    [SerializeField] private int _battleZoneSortingLayerFloor;

    private Dictionary<int, CompactCardObject> _cardsInBattleZone = new Dictionary<int, CompactCardObject>();
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0, n = transform.childCount; i < n; i++)
            {
                CompactCardObject card = transform.GetChild(i).GetComponent<CompactCardObject>();
                if (!_cardsInBattleZone.ContainsKey(transform.GetChild(i).GetInstanceID()))
                    _cardsInBattleZone.Add(transform.GetChild(i).GetInstanceID(), card);
            }

            ArrangeCards();
        }
    }

    void ArrangeCards()
    {
        int n = transform.childCount;
        float cardWidth = Mathf.Min((_cardAreaWidth * 2) / n, _maxCardWidth);
        float sizeRatio = cardWidth / _maxCardWidth;

        float startOffset = (n % 2) * cardWidth;
        if (n % 2 == 0)
            startOffset += cardWidth / 2;
        Vector3 startPos = new Vector3(transform.position.x - startOffset, transform.position.y, transform.position.z);

        for (int i = 0; i < n; i++)
        {
            Transform cardTransform = transform.GetChild(i);
            _cardsInBattleZone[cardTransform.GetInstanceID()].Canvas.sortingOrder = _battleZoneSortingLayerFloor + i;
            Vector3 cardPos = new Vector3(startPos.x + (i - n / 2 + 1) * cardWidth, startPos.y, startPos.z);

            cardTransform.position = cardPos;
            cardTransform.localScale = new Vector3(sizeRatio, sizeRatio, sizeRatio);
        }
    }
}
