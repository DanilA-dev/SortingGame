using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace D_Dev.SheetsLoadableSystem.Writers
{
    [CreateAssetMenu(menuName = "D-Dev/Editor/Sheets/Data Exporter")]
    public class SheetsDataCsvExporter : ScriptableObject
    {
        #region Fields

        [SerializeField] private string _path;
        [SerializeField] private bool _clearOnExport;
        [SerializeField] private List<ScriptableObject> _dataToExport = new();

        #endregion

        #region Properties
        private string Path => _path + ".csv";

        #endregion

        #region Editor

         [Button]
        public void ExportData()
        {
            if(_dataToExport.Count <= 0)
                return;

            if (File.Exists(Path))
            {
                File.Delete(Path);
                Debug.Log($"<color=orange> Rewrite data {Path} </color>");
            }
            
            var allHeaders = new HashSet<string>();

            foreach (var data in _dataToExport)
            {
                var dataDictionary = data.GetData();
                foreach (var header in dataDictionary.Keys)
                    allHeaders.Add(header);
            }
            
            using (var writer = new StreamWriter(Path, false))
            {
                writer.WriteLine(string.Join(",", allHeaders));
    
                foreach (var data in _dataToExport)
                {
                    var ser = data.GetData();
                    var values = allHeaders.Select(header =>
                    {
                        if (ser.TryGetValue(header, out var value))
                        {
                            if (value is IEnumerable<float> floatList)
                                return "\"" + string.Join(";",
                                           floatList.Select(f => f.ToString(CultureInfo.InvariantCulture))) +
                                       "\"";
                            else if (value is IEnumerable<double> doubleList)
                                return "\"" + string.Join(";",
                                    doubleList.Select(d => d.ToString(CultureInfo.InvariantCulture))) + "\"";
                            else if (value is IEnumerable<int> intList)
                                return "\"" + string.Join(";", intList) + "\"";
                            else if (value is IEnumerable<object> objectList)
                                return "\"" + string.Join(";", objectList.Select(o => o.ToString())) + "\"";
                            else
                            {
                                var stringValue = value?.ToString();
                                if (stringValue != null && stringValue.Contains(","))
                                {
                                    var replaceValue = stringValue.Replace(",", ".");
                                    return "\"" + replaceValue + "\"";
                                }
                                return stringValue;
                            }
                        }
                        else
                            return "";
                    });
                        writer.WriteLine(string.Join(",", values)); 
                }
            }
            Debug.Log($"<color=yellow> Data write to file path {Path} </color>");
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
            if (_clearOnExport)
            {
                _path = "";
                _dataToExport.Clear();
            }
        }

        [MenuItem("Tools/D_Dev/Utility/CSV/Export")]
        public static void OpenExportMenu()
        {
            SheetsDataCsvExporter exporter = Resources.LoadAll<SheetsDataCsvExporter>("")[0];
            if (null == exporter)
            {
                Debug.LogError($"{exporter.name} can't be found!");
                return;
            }
            EditorUtility.OpenPropertyEditor(exporter);
        }

        #endregion
    }
}