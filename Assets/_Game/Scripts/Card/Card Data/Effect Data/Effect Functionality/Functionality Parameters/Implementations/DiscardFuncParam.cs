using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    #region Helper Data Structures

    [System.Serializable]
    public class DiscardParam
    {
        public DiscardType discardType;
        public CountRangeType countRangeType;
        public int discardCount = 1;

        public override string ToString()
        {
            string str = "discards ";
            if (countRangeType == CountRangeType.All)
                str += "all cards";
            else
            {
                str += $"{discardCount} card";
                if (discardCount > 1)
                    str += "s ";

                switch (discardType)
                {
                    case DiscardType.Random:
                        str += "at random";
                        break;
                    case DiscardType.PlayerChoose:
                        str += "chosen by player";
                        break;
                    case DiscardType.OpponentChoose:
                        str += "chosen by opponent";
                        break;
                }
            }

            return str;
        }
    }

    #endregion

    public class DiscardFuncParam : EffectFunctionalityParameter
    {
        private DiscardParam _discardParam = new DiscardParam();

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.Discard
                };
            }
        }
#endif

        public DiscardParam DiscardParam
        {
            get { return _discardParam; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _discardParam.countRangeType = EditorUtils.DrawFoldout(_discardParam.countRangeType);
            if (_discardParam.countRangeType == CountRangeType.Number)
            {
                if (int.TryParse(EditorGUILayout.TextField($"{_discardParam.discardCount}"), out int num))
                    _discardParam.discardCount = num;
            }
        }

        public override bool ShouldAssignCriterion()
        {
            throw new System.NotImplementedException();
        }

#endif
    }
}