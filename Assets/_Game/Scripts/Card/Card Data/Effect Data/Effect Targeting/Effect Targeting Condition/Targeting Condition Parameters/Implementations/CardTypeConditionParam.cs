using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.TargetingCondition.Parameters
{
    public class CardTypeConditionParam : SingleEnumConditionParam, ICardIntrinsicParam
    {
        [SerializeReference] private CardParams.CardType _cardTypeCondition;

#if UNITY_EDITOR

        public override int CompareValue
        {
            get { return 1; }
        }

#endif

        public override bool IsConditionSatisfied(CardInstance cardInstToCheck)
        {
            bool result = true;
            CardData cardData = cardInstToCheck.CardData;

            if (_assignedParameter)
            {
                switch (_cardTypeCondition)
                {
                    case CardParams.CardType.EvolutionCreature:
                        switch (cardData.CardType)
                        {
                            case CardParams.CardType.Creature:
                            case CardParams.CardType.Spell:
                                result = false;
                                break;
                        }

                        break;

                    case CardParams.CardType.Creature:
                        switch (cardData.CardType)
                        {
                            case CardParams.CardType.Spell:
                                result = false;
                                break;
                        }

                        break;

                    case CardParams.CardType.Spell:
                        switch (cardData.CardType)
                        {
                            case CardParams.CardType.EvolutionCreature:
                            case CardParams.CardType.Creature:
                                result = false;
                                break;
                        }

                        break;
                }
            }

            return result;
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            GUILayout.BeginHorizontal();

            if (_assignedParameter)
            {
                GUILayout.Label("Card Type:", EditorStatics.EffectTargetingConditionParamLabelStyle);
                _cardTypeCondition = EditorUtils.DrawFoldout(_cardTypeCondition, 1);

                if (GUILayout.Button("Remove Card Type"))
                    _assignedParameter = false;
            }
            else if (GUILayout.Button("Add Card Type"))
                _assignedParameter = true;

            GUILayout.EndHorizontal();
        }

        public override string GetEditorRepresentationString()
        {
            return $"\nCard Type is{ToString()}";
        }

#endif

        public override string GetGameRepresentationString()
        {
            return ToString();
        }

        public override string ToString()
        {
            return $" {CardParams.StringFromCardType(_cardTypeCondition)}";
        }
    }
}