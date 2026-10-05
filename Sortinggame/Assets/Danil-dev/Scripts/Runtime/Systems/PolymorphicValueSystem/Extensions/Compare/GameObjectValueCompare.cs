using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Compare
{
    public class GameObjectValueCompare : BasePolymorphicValueCompare<GameObject>
    {
        #region Public

        public override bool Compare(GameObject value, GameObject valueTo) => value == valueTo;

        #endregion
    }
}
