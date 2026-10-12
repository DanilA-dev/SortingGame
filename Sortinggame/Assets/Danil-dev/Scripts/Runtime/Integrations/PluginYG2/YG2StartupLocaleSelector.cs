#if D_DEV_YG2 && EnvirData_yg
using System;
using D_Dev.LocalizationSystem;

namespace D_Dev.PluginYG2
{
    [Serializable]
    public class YG2StartupLocaleSelector : AutoAssignerStartupLocaleSelector<YG2LocaleAutoAssigner> { }
}
#endif
