using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace D_Dev.SheetsLoadableSystem
{
    public static class SheetsLoadableExtensions
    {
        public static Dictionary<string, object> GetData(this ScriptableObject sheetsLoadable)
        {
            var attDic = new Dictionary<D_Dev.SheetsLoadableSystem.SheetsLoadable, object>();
            var returnDic = new Dictionary<string, object>();
            foreach (PropertyInfo propertyInfo in sheetsLoadable.GetType().GetProperties())
            {
                var key = propertyInfo.GetCustomAttribute<D_Dev.SheetsLoadableSystem.SheetsLoadable>();
                if (key != null)
                {
                    var value = propertyInfo.GetValue(sheetsLoadable, null);
                    attDic.Add(key, value);
                }
            }

            attDic = new Dictionary<D_Dev.SheetsLoadableSystem.SheetsLoadable, object>(attDic.OrderBy(i => i.Key.Сolumn));
            foreach (var (key, value) in attDic)
                returnDic.Add(key.ValueName, value);

            return returnDic;
        }
        
        public static void SetData(this ScriptableObject sheetsLoadable, Dictionary<string, object> dataToLoad)
        {
            foreach (PropertyInfo propertyInfo in sheetsLoadable.GetType().GetProperties())
            {
                if (propertyInfo.CanWrite)
                {
                    var key = propertyInfo.GetCustomAttribute<D_Dev.SheetsLoadableSystem.SheetsLoadable>();
                    var propertyValue = propertyInfo.GetValue(sheetsLoadable, null);
                    if(key == null)
                        continue;

                    if (!dataToLoad.TryGetValue(key.ValueName, out var value))
                        continue;
                    
                    switch (propertyValue)
                    {
                        case float propertyValueFloat:
                            value = SheetsDataParser.GetDeserializedFloat(dataToLoad, key.ValueName);
                            propertyInfo.SetValue(sheetsLoadable, value);
                            break;
                        case int propertyValueInt:
                            value = SheetsDataParser.GetDeserializedInt(dataToLoad, key.ValueName);
                            propertyInfo.SetValue(sheetsLoadable, value);
                            break;
                        case double propertyValueDouble:
                            value = SheetsDataParser.GetDeserializedFloat(dataToLoad, key.ValueName);
                            propertyInfo.SetValue(sheetsLoadable, value);
                            break;
                        case float[] propertyValueFloatArray:
                            value = SheetsDataParser.GetDeserializedFloats(dataToLoad, key.ValueName);
                            propertyInfo.SetValue(sheetsLoadable, value);
                            break;
                        case int[] propertyValueIntArray:
                            value = SheetsDataParser.GetDeserializedInts(dataToLoad, key.ValueName);
                            propertyInfo.SetValue(sheetsLoadable, value);
                            break;
                        case double[] propertyValueDoubleArray:
                            value = SheetsDataParser.GetDeserializedDoubles(dataToLoad, key.ValueName);
                            propertyInfo.SetValue(sheetsLoadable, value);
                            break;
                        case string propertyValueString:
                            propertyInfo.SetValue(sheetsLoadable, (string)value);
                            break;
                    }
                }
            }
        }
    }
}
