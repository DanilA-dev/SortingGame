using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Compare
{
    public class StringValueCompare : BasePolymorphicValueCompare<string>
    {
        #region Public

        public override bool Compare(string value, string valueTo) => value == valueTo;

        #endregion
    }
}
