using System;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.StateMachineBehaviour
{
    [System.Serializable]
    public class StateEvent
    {
        [field: SerializeReference]
        public PolymorphicValue<string> State { get; private set; } = new StringConstantValue();
        [FoldoutGroup("Events")]
        public UnityEvent<string> OnStateEnter;
        [FoldoutGroup("Events")]
        public UnityEvent<string> OnStateExit;
    }
}
