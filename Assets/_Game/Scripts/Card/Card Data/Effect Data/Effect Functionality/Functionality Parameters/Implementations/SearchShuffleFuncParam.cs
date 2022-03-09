using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    public class SearchShuffleFuncParam : EffectFunctionalityParameter
    {
        private CountRangeType _countRangeType;
        private CountQuantifierType _countQuantifier;
        private int _count;
        private SearchResultType _searchResult;

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.SearchAndShuffle
                };
            }
        }
#endif

        public SearchResultType SearchResult
        {
            get { return _searchResult; }
        }

        public CountRangeType CountRangeType
        {
            get { return _countRangeType; }
        }

        public CountQuantifierType CountQuantifier
        {
            get { return _countQuantifier; }
        }

        public int Count
        {
            get { return _count; }
        }
        
#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _searchResult = EditorUtils.DrawFoldout(_searchResult);

            _countRangeType = EditorUtils.DrawFoldout(_countRangeType);
            if (_countRangeType == CountRangeType.Number)
            {
                _countQuantifier = EditorUtils.DrawFoldout(_countQuantifier);
                if (int.TryParse(EditorGUILayout.TextField($"{_count}"), out int num))
                    _count = num;
            }
        }

        public override bool ShouldAssignCriterion()
        {
            return false;
        }

#endif
    }
}