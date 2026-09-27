using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace D_Dev.SheetsLoadableSystem
{
    public static class SheetsDataParser
    {
        public static float GetDeserializedFloat(Dictionary<string, object> data, string key)
        {
            float returnFloat = 0;
            if (data.TryGetValue(key, out var obj) && obj != null)
            {
                if (obj is float floatValue)
                    returnFloat = floatValue;

                if (obj is string stringValue)
                {
                    var sValue = stringValue;
                    if(float.TryParse(sValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                        returnFloat = result;
                    else
                        Debug.LogError($"Can't parse {sValue} of {key} into float");
                }
            }
            return returnFloat;
        }
        
        public static double GetDeserializedDouble(Dictionary<string, object> data, string key)
        {
            double returnDouble = 0;
            if (data.TryGetValue(key, out var obj) && obj != null)
            {
                if (obj is double doubleValue)
                    returnDouble = doubleValue;

                if (obj is string stringValue)
                {
                    var sValue = stringValue;
                    if(double.TryParse(sValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                        returnDouble = result;
                    else
                        Debug.LogError($"Can't parse {sValue} of {key} into double");
                }
            }
            return returnDouble;
        }
        
        public static int GetDeserializedInt(Dictionary<string, object> data, string key)
        {
            int returnInt = 0;
            if (data.TryGetValue(key, out var obj) && obj != null)
            {
                if (obj is int intValue)
                {
                    returnInt = intValue;
                }

                if (obj is string stringValue)
                {
                    
                    var sValue = stringValue;
                    if(int.TryParse(sValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                        returnInt = result;
                    else
                        Debug.LogError($"Can't parse {sValue} of {key} into int");
                }
            }
            return returnInt;
        }
        
        public static float[] GetDeserializedFloats(Dictionary<string, object> data, string key)
        {
            float[] floats = new float[0];
            if (data.TryGetValue(key, out var obj) && obj != null)
            {
                if (obj is float[] floatArray)
                {
                    floats = floatArray;
                }
                else if (obj is string stringValue)
                {
                    var splitValues = stringValue
                        .Split(';')  
                        .Where(i => !string.IsNullOrWhiteSpace(i))
                        .ToArray();

                    var parsedFloats = new List<float>();
                    foreach (var splitValue in splitValues)
                    {
                        if(float.TryParse(splitValue.Trim(), NumberStyles.Float,
                               CultureInfo.InvariantCulture, out var floatRes))
                            parsedFloats.Add(floatRes);
                        else
                            Debug.LogError($"Can't parse {splitValue} of {key} into float");
                    }

                    floats = parsedFloats.ToArray();
                }
            }
            return floats;
        }
        
        public static double[] GetDeserializedDoubles(Dictionary<string, object> data, string key)
        {
            double[] doubles = new double[0];
            if (data.TryGetValue(key, out var obj) && obj != null)
            {
                if (obj is double[] floatArray)
                {
                    doubles = floatArray;
                }
                else if (obj is string stringValue)
                {
                    var splitValues = stringValue
                        .Split(';')  
                        .Where(i => !string.IsNullOrWhiteSpace(i))
                        .ToArray();

                    var parsedDoubles = new List<double>();
                    foreach (var splitValue in splitValues)
                    {
                        if(double.TryParse(splitValue.Trim(), NumberStyles.Float,
                               CultureInfo.InvariantCulture, out var doubleRes))
                            parsedDoubles.Add(doubleRes);
                        else
                            Debug.LogError($"Can't parse {splitValue} of {key} into double");
                    }

                    doubles = parsedDoubles.ToArray();
                }
            }
            return doubles;
        }
        
        public static int[] GetDeserializedInts(Dictionary<string, object> data, string key)
        {
            int[] ints = new int[0];
            if (data.TryGetValue(key, out var obj) && obj != null)
            {
                if (obj is int[] intArray)
                {
                    ints = intArray;
                }
                else if (obj is string stringValue)
                {
                    var splitValues = stringValue
                        .Split(';')  
                        .Where(i => !string.IsNullOrWhiteSpace(i))
                        .ToArray();

                    var parsedInts = new List<int>();
                    foreach (var splitValue in splitValues)
                    {
                        if(int.TryParse(splitValue.Trim(), NumberStyles.Integer,
                               CultureInfo.InvariantCulture, out var intRes))
                            parsedInts.Add(intRes);
                        else
                            Debug.LogError($"Can't parse {splitValue} of {key} into int");
                    }

                    ints = parsedInts.ToArray();
                }
            }
            return ints;
        }
    }
}
