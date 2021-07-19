using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeywordPrefabHolder : MonoBehaviour
{
    [SerializeField] private List<KeywordLayoutHandler> _keywordLayoutPrefabs = new List<KeywordLayoutHandler>();

    private Dictionary<KeywordLayoutType, KeywordLayoutHandler> _keywordPrefabDict = new Dictionary<KeywordLayoutType, KeywordLayoutHandler>();

    #region Static Data Members

    private static KeywordPrefabHolder _Instance = null;

    public static KeywordPrefabHolder Instance
    {
        get
        {
            if (!_Instance)
                _Instance = FindObjectOfType<KeywordPrefabHolder>();
            return _Instance;
        }
    }

    #endregion

    public IReadOnlyDictionary<KeywordLayoutType, KeywordLayoutHandler> KeywordPrefabDict
    {
        get { return _keywordPrefabDict; }
    }

    private void Awake()
    {
        int count = FindObjectsOfType<KeywordPrefabHolder>().Length;
        if (count > 1)
            Destroy(gameObject);
        else
            _Instance = this;

        foreach (KeywordLayoutHandler keyword in _keywordLayoutPrefabs)
        {
            _keywordPrefabDict[keyword.Type] = keyword;
        }
    }
}
