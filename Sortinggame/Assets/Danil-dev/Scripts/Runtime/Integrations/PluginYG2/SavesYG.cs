#if D_DEV_YG2 && Storage_yg
using System;
using System.Collections.Generic;

namespace YG
{
    public partial class SavesYG
    {
        public List<SaveEntryYG> entries = new();
    }

    [Serializable]
    public class SaveEntryYG
    {
        public string key;
        public string value;
    }
}
#endif
