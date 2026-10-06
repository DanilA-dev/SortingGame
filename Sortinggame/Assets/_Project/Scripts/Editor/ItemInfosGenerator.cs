using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using D_Dev.Entity;
using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using D_Dev.TagSystem;
using UnityEditor;
using UnityEngine;
using Binder = D_Dev.EntityInfoBinder.EntityInfoBinder;

namespace _Project.Scripts.Editor
{
    public static class ItemInfosGenerator
    {
        #region Fields

        private const string PrefabsFolder = "Assets/_Project/Prefabs/Entities/Items";
        private const string BasePrefabPath = PrefabsFolder + "/BaseItem_Prefab.prefab";
        private const string InfosFolder = "Assets/_Project/ScriptableObjects/Entities/Items";
        private const string TemplateInfoPath = InfosFolder + "/Burger_EntityInfo.asset";
        private const string VariablesFolder = "Assets/_Project/ScriptableObjects/Variables";
        private const string CategoriesFolder = VariablesFolder + "/Categories";
        private const string NameIdPath = VariablesFolder + "/Ids/Name_ID.asset";
        private const string DescriptionIdPath = VariablesFolder + "/Ids/Description_ID.asset";

        private static readonly Dictionary<string, string> ItemCategories = new()
        {
            { "Baguette", "Bakery" }, { "Croissant", "Bakery" }, { "WhiteBread", "Bakery" },
            { "Cheesecake", "Bakery" }, { "BlackCookie", "Bakery" }, { "WhiteCookie", "Bakery" },
            { "Donut", "Bakery" }, { "Pie", "Bakery" },
            { "Burger", "Fast Food" }, { "HotDog", "Fast Food" }, { "Pizza", "Fast Food" },
            { "Sandwich", "Fast Food" }, { "Taco", "Fast Food" }, { "Onigiri", "Fast Food" },
            { "GreenSoda", "Drinks" }, { "OrangeSoda", "Drinks" }, { "RedSoda", "Drinks" },
            { "StripedSoda", "Drinks" }, { "OrangeJuice", "Drinks" }, { "Milk", "Drinks" },
            { "BlueCreamyIceCream", "Ice Cream" }, { "GreenCreamyIceCream", "Ice Cream" },
            { "PinkCreamyIceCream", "Ice Cream" }, { "BlueFruitIce", "Ice Cream" },
            { "PinkFruitIce", "Ice Cream" }, { "RedFruitIce", "Ice Cream" },
            { "Avacado", "Fruits" }, { "Orange", "Fruits" }, { "Watermelon", "Fruits" },
            { "Tomato", "Vegetables" }, { "Pumpkin", "Vegetables" }, { "Pepper", "Vegetables" },
            { "PinkSteak", "Meat" }, { "RedSteak", "Meat" }, { "Sausage", "Meat" },
            { "Sardine", "Seafood" }, { "Shrimp", "Seafood" },
            { "Ketchup", "Sauces" }, { "Mustard", "Sauces" },
            { "PurpleChips", "Snacks" }, { "YellowChips", "Snacks" },
            { "Bowl", "Tableware" }, { "Mug", "Tableware" }, { "Plate", "Tableware" },
        };

        #endregion

        #region Menu

        [MenuItem("Tools/Items/Generate Entity Infos")]
        private static void Generate()
        {
            var template = AssetDatabase.LoadAssetAtPath<EntityInfo>(TemplateInfoPath);
            var nameId = AssetDatabase.LoadAssetAtPath<StringScriptableVariable>(NameIdPath);
            var descriptionId = AssetDatabase.LoadAssetAtPath<StringScriptableVariable>(DescriptionIdPath);

            if (template == null || nameId == null || descriptionId == null)
            {
                Debug.LogError("[ItemInfosGenerator] Template info or variable ids not found");
                return;
            }

            EnsureBinderOnBasePrefab();

            foreach (var (item, category) in ItemCategories)
            {
                var prefabPath = $"{PrefabsFolder}/{item}Item_Prefab.prefab";
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab == null)
                {
                    Debug.LogWarning($"[ItemInfosGenerator] Prefab not found: {prefabPath}");
                    continue;
                }

                var info = GetOrCreateInfo(item, template);
                var infoSO = new SerializedObject(info);
                var nameValue = FindVariableValue(infoSO, nameId);
                var descriptionValue = FindVariableValue(infoSO, descriptionId);
                if (nameValue == null || descriptionValue == null)
                {
                    Debug.LogWarning($"[ItemInfosGenerator] Name/Description variable missing in {info.name}");
                    continue;
                }

                infoSO.FindProperty("_entityPrefab._asset").objectReferenceValue = prefab;
                nameValue.managedReferenceValue = new StringConstantValue();
                descriptionValue.managedReferenceValue = new StringScriptableVariableValue();
                infoSO.ApplyModifiedPropertiesWithoutUndo();
                infoSO.Update();

                FindVariableValue(infoSO, nameId).FindPropertyRelative("_value").stringValue = ToDisplayName(item);
                FindVariableValue(infoSO, descriptionId).FindPropertyRelative("_variable").objectReferenceValue =
                    GetOrCreateCategory(category);
                infoSO.ApplyModifiedPropertiesWithoutUndo();

                BindInfoToPrefab(prefabPath, info);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("[ItemInfosGenerator] Done");
        }

        #endregion

        #region Private

        private static EntityInfo GetOrCreateInfo(string item, EntityInfo template)
        {
            var path = $"{InfosFolder}/{item}_EntityInfo.asset";
            var info = AssetDatabase.LoadAssetAtPath<EntityInfo>(path);
            if (info != null)
                return info;

            info = UnityEngine.Object.Instantiate(template);
            AssetDatabase.CreateAsset(info, path);

            var infoSO = new SerializedObject(info);
            infoSO.FindProperty("_id").stringValue = Guid.NewGuid().ToString();
            infoSO.ApplyModifiedPropertiesWithoutUndo();
            return info;
        }

        private static StringScriptableVariable GetOrCreateCategory(string category)
        {
            var path = $"{CategoriesFolder}/{category.Replace(" ", "")}_StringVariable.asset";
            var variable = AssetDatabase.LoadAssetAtPath<StringScriptableVariable>(path);
            if (variable != null)
                return variable;

            if (!AssetDatabase.IsValidFolder(CategoriesFolder))
                AssetDatabase.CreateFolder(VariablesFolder, "Categories");

            variable = ScriptableObject.CreateInstance<StringScriptableVariable>();
            AssetDatabase.CreateAsset(variable, path);

            var variableSO = new SerializedObject(variable);
            variableSO.FindProperty("_value").stringValue = category;
            variableSO.ApplyModifiedPropertiesWithoutUndo();
            return variable;
        }

        private static void EnsureBinderOnBasePrefab()
        {
            var root = PrefabUtility.LoadPrefabContents(BasePrefabPath);
            if (!root.TryGetComponent(out Binder _))
            {
                root.AddComponent<Binder>();
                PrefabUtility.SaveAsPrefabAsset(root, BasePrefabPath);
            }
            PrefabUtility.UnloadPrefabContents(root);
        }

        private static void BindInfoToPrefab(string prefabPath, EntityInfo info)
        {
            var root = PrefabUtility.LoadPrefabContents(prefabPath);

            var binderSO = new SerializedObject(root.GetComponent<Binder>());
            binderSO.FindProperty("_entityInfo").objectReferenceValue = info;
            binderSO.ApplyModifiedPropertiesWithoutUndo();

            if (root.TryGetComponent(out TagComponent tagComponent))
            {
                var tagsProperty = new SerializedObject(tagComponent).FindProperty("_tags");
                PrefabUtility.RevertPropertyOverride(tagsProperty, InteractionMode.AutomatedAction);
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        #endregion

        #region Helpers

        private static SerializedProperty FindVariableValue(SerializedObject infoSO, StringScriptableVariable id)
        {
            var variables = infoSO.FindProperty("_variables");
            for (int i = 0; i < variables.arraySize; i++)
            {
                var variable = variables.GetArrayElementAtIndex(i);
                if (variable.FindPropertyRelative("_variableID").objectReferenceValue == id)
                    return variable.FindPropertyRelative("_value");
            }
            return null;
        }

        private static string ToDisplayName(string item) => Regex.Replace(item, "(?<!^)([A-Z])", " $1");

        #endregion
    }
}
