using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    #region Helper Data Structures

    [System.Serializable]
    public class DestroyParam
    {
        public CardZoneType destroyZone;
        public CountRangeType countRangeType;
        public int destroyCount = 1;

        public override string ToString()
        {
            string str = "Destroy ";
            if (countRangeType == CountRangeType.All)
                str += "all cards";
            else
            {
                str += $"{destroyCount} card";
                if (destroyCount > 1)
                    str += "s ";
            }

            return str;
        }
    }

    #endregion

    public class DestroyFuncParam : EffectFunctionalityParameter
    {
        private DestroyParam _destroyParam = new DestroyParam();

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.Destroy
                };
            }
        }
#endif

        public DestroyParam DestroyParam
        {
            get { return _destroyParam; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _destroyParam.destroyZone = EditorUtils.DrawFoldout(_destroyParam.destroyZone);
            _destroyParam.countRangeType = EditorUtils.DrawFoldout(_destroyParam.countRangeType);
            if (_destroyParam.countRangeType == CountRangeType.Number)
            {
                if (int.TryParse(EditorGUILayout.TextField($"{_destroyParam.destroyCount}"), out int num))
                    _destroyParam.destroyCount = num;
            }
        }

        public override bool ShouldAssignCriterion()
        {
            return false;
        }

#endif
    }
}