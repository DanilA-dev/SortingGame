using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.PolymorphicValueSystem.Compare
{
    public abstract class BasePolymorphicValueCompare<T> : MonoBehaviour
    {
        #region Classes

        [System.Serializable]
        public class CompareValuePair
        {
            #region Fields

            [SerializeReference] public PolymorphicValue<T> CompareValue;
            [SerializeReference] public PolymorphicValue<T> CompareValueTo;

            [FoldoutGroup("Events"), PropertyOrder(100)]
            [SerializeField] public UnityEvent OnValueCompareTrue;
            [FoldoutGroup("Events"), PropertyOrder(100)]
            [SerializeField] public UnityEvent OnValueCompareFalse;

            #endregion
        }

        #endregion

        #region Fields

        [SerializeField, PropertyOrder(-1)] protected bool _checkOnStart;
        [SerializeField] protected List<CompareValuePair> _comparePairs = new();

        #endregion

        #region Public

        public abstract bool Compare(T value, T valueTo);

        public void CheckValues()
        {
            foreach (CompareValuePair pair in _comparePairs)
            {
                if (pair?.CompareValue == null || pair.CompareValueTo == null)
                    continue;

                T value = pair.CompareValue.Value;
                T valueTo = pair.CompareValueTo.Value;

                if (Compare(value, valueTo))
                    pair.OnValueCompareTrue?.Invoke();
                else
                    pair.OnValueCompareFalse?.Invoke();
            }
        }

        #endregion

        #region Monobehaviour

        private void Start()
        {
            if (_checkOnStart)
                CheckValues();
        }

        #endregion
    }
}
