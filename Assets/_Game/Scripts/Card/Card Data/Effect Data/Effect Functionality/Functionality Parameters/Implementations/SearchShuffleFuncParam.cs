using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    public class SearchShuffleFuncParam : EffectFunctionalityParameter
    {
        private NumericParamsHolder _numericParams = new NumericParamsHolder();
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

        public NumericParamsHolder NumericParams
        {
            get { return _numericParams; }
        }
        
#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _searchResult = EditorUtils.DrawFoldout(_searchResult);

            _numericParams.DrawInspector();
        }

        public override bool ShouldAssignCriterion()
        {
            return false;
        }

#endif
    }
}