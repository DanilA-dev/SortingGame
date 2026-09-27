using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Compare
{
    public class BoolValueCompare : BasePolymorphicValueCompare<bool>
    {
        #region Public

        public override bool Compare(bool value, bool valueTo) => value == valueTo;

        #endregion
    }
}
