using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace D_Dev.PositionRotationConfig.Editor
{
    /// <summary>
    /// Draws random spawn areas of every BasePositionSettings ([SerializeReference]) found on selected GameObjects.
    /// </summary>
    [InitializeOnLoad]
    public static class PositionSettingsSceneDrawer
    {
        #region Fields

        private static readonly List<BasePositionSettings> SettingsCache = new();
        private static readonly List<Vector3> PositionsBuffer = new();
        private static bool _isDirty = true;

        #endregion

        #region Constructors

        static PositionSettingsSceneDrawer()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            Selection.selectionChanged += MarkDirty;
            Undo.undoRedoPerformed += MarkDirty;
            ObjectChangeEvents.changesPublished += OnChangesPublished;
        }

        #endregion

        #region Private

        private static void MarkDirty() => _isDirty = true;

        private static void OnChangesPublished(ref ObjectChangeEventStream stream) => MarkDirty();

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            if (_isDirty)
                CollectSettings();

            if (SettingsCache.Count == 0)
                return;

            var prevColor = Handles.color;
            var prevMatrix = Handles.matrix;

            foreach (var settings in SettingsCache)
                Draw(settings);

            Handles.color = prevColor;
            Handles.matrix = prevMatrix;
        }

        private static void CollectSettings()
        {
            _isDirty = false;
            SettingsCache.Clear();

            foreach (var gameObject in Selection.gameObjects)
            {
                foreach (var component in gameObject.GetComponents<MonoBehaviour>())
                {
                    if (component == null)
                        continue;

                    using var serializedObject = new SerializedObject(component);
                    var property = serializedObject.GetIterator();
                    var enterChildren = true;

                    while (property.Next(enterChildren))
                    {
                        enterChildren = property.hasChildren && property.propertyType != SerializedPropertyType.String;

                        if (property.propertyType == SerializedPropertyType.ManagedReference
                            && property.managedReferenceValue is BasePositionSettings settings
                            && !SettingsCache.Contains(settings))
                            SettingsCache.Add(settings);
                    }
                }
            }
        }

        private static void Draw(BasePositionSettings settings)
        {
            if (settings == null || !settings.IsRandom || !settings.DrawGizmos)
                return;

            PositionsBuffer.Clear();
            settings.GetGizmoPositions(PositionsBuffer);

            Handles.color = settings.GizmoColor;
            var axisScale = GetAxisScale(settings.Axis);

            foreach (var position in PositionsBuffer)
            {
                Handles.matrix = Matrix4x4.TRS(position, Quaternion.identity, axisScale);

                switch (settings.RandomMode)
                {
                    case RandomPositionMode.Sphere:
                        Handles.DrawWireDisc(Vector3.zero, Vector3.up, settings.RandomRadius);
                        Handles.DrawWireDisc(Vector3.zero, Vector3.right, settings.RandomRadius);
                        Handles.DrawWireDisc(Vector3.zero, Vector3.forward, settings.RandomRadius);
                        break;
                    case RandomPositionMode.Box:
                        Handles.DrawWireCube(Vector3.zero, settings.RandomBoxSize);
                        break;
                }
            }
        }

        // Disabled axes have no random offset, so the area is flattened along them
        private static Vector3 GetAxisScale(AxisUpdate axis) => new(
            axis.HasFlag(AxisUpdate.X) ? 1 : 0,
            axis.HasFlag(AxisUpdate.Y) ? 1 : 0,
            axis.HasFlag(AxisUpdate.Z) ? 1 : 0);

        #endregion
    }
}
