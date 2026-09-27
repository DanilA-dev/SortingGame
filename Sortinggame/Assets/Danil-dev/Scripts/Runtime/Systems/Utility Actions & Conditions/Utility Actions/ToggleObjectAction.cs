using D_Dev.Base;
using UnityEngine;

namespace D_Dev.Actions
{
    [System.Serializable]
    public class ToggleObjectAction : BaseAction
    {
        #region Fields

        [SerializeField] private GameObject _targetObject;
        [SerializeField] private bool _enable = true;

        #endregion

        #region IAction


        public override void Execute()
        {
            if (_targetObject != null)
                _targetObject.SetActive(_enable);

            IsFinished = true;
        }

        #endregion
    }
}
