using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    #region Helper Data Structures

    [System.Serializable]
    public class LookAtParam
    {
        public CardZoneType lookAtZone;
        public CountRangeType countRangeType;
        public int lookCount = 1;

        public override string ToString()
        {
            string str = "Look at ";
            if (countRangeType == CountRangeType.All)
                str += "all cards";
            else
            {
                str += $"{lookCount} card";
                if (lookCount > 1)
                    str += "s ";
            }

            return str;
        }
    }

    #endregion

    public class LookAtRegionFuncParam : EffectFunctionalityParameter
    {
        private LookAtParam _lookAtParam = new LookAtParam();

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.LookAtRegion
                };
            }
        }
#endif

        public LookAtParam LookAtParam
        {
            get { return _lookAtParam; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _lookAtParam.lookAtZone = EditorUtils.DrawFoldout(_lookAtParam.lookAtZone);
            _lookAtParam.countRangeType = EditorUtils.DrawFoldout(_lookAtParam.countRangeType);
            if (_lookAtParam.countRangeType == CountRangeType.Number)
            {
                if (int.TryParse(EditorGUILayout.TextField($"{_lookAtParam.lookCount}"), out int num))
                    _lookAtParam.lookCount = num;
            }
        }

        public override bool ShouldAssignCriterion()
        {
            return false;
        }

#endif
    }
}