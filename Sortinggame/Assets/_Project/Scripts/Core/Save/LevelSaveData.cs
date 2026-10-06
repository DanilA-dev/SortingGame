using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts
{
    public enum ItemSaveState
    {
        Free = 0,
        Picked = 1,
        Sorted = 2
    }

    public class ItemSaveData
    {
        public string InfoId;
        public ItemSaveState State;
        public bool HasTransform;
        public Vector3 Position;
        public Quaternion Rotation;
        public string ShelfId;
        public int SlotIndex = -1;
        public int PickedIndex = -1;
    }

    [Serializable]
    public class LevelSaveData
    {
        #region Fields

        private const float POSITION_PRECISION = 100f;
        private const float ROTATION_PRECISION = 1000f;
        private const int TRANSFORM_SIZE = 7;
        private const int FREE_HEADER_SIZE = 1;
        private const int PICKED_HEADER_SIZE = 2;

        public List<string> InfoIds = new();
        public List<string> ShelfIds = new();
        public List<int[]> Sorted = new();
        public List<float[]> Picked = new();
        public List<float[]> Free = new();

        #endregion

        #region Public

        public static LevelSaveData Pack(List<ItemSaveData> items)
        {
            var data = new LevelSaveData();
            var infoIndexes = new Dictionary<string, int>();
            var shelfIndexes = new Dictionary<string, int>();

            foreach (var item in items)
            {
                var info = GetIndex(item.InfoId, data.InfoIds, infoIndexes);

                switch (item.State)
                {
                    case ItemSaveState.Sorted:
                        var shelf = GetIndex(item.ShelfId, data.ShelfIds, shelfIndexes);
                        data.Sorted.Add(new[] { info, shelf, item.SlotIndex });
                        break;
                    case ItemSaveState.Picked:
                        var picked = PackTransform(item, PICKED_HEADER_SIZE);
                        picked[0] = info;
                        picked[1] = item.PickedIndex;
                        data.Picked.Add(picked);
                        break;
                    default:
                        var free = PackTransform(item, FREE_HEADER_SIZE);
                        free[0] = info;
                        data.Free.Add(free);
                        break;
                }
            }

            return data;
        }

        public List<ItemSaveData> Unpack()
        {
            var items = new List<ItemSaveData>(Sorted.Count + Picked.Count + Free.Count);

            foreach (var sorted in Sorted)
            {
                if (sorted == null || sorted.Length < 3)
                    continue;

                items.Add(new ItemSaveData
                {
                    InfoId = GetId(InfoIds, sorted[0]),
                    State = ItemSaveState.Sorted,
                    ShelfId = GetId(ShelfIds, sorted[1]),
                    SlotIndex = sorted[2]
                });
            }

            foreach (var picked in Picked)
            {
                var item = UnpackTransform(picked, PICKED_HEADER_SIZE, ItemSaveState.Picked);
                if (item == null)
                    continue;

                item.PickedIndex = Mathf.RoundToInt(picked[1]);
                items.Add(item);
            }

            foreach (var free in Free)
            {
                var item = UnpackTransform(free, FREE_HEADER_SIZE, ItemSaveState.Free);
                if (item != null)
                    items.Add(item);
            }

            return items;
        }

        #endregion

        #region Private

        private ItemSaveData UnpackTransform(float[] values, int headerSize, ItemSaveState state)
        {
            if (values == null || values.Length < headerSize + TRANSFORM_SIZE)
                return null;

            var i = headerSize;
            return new ItemSaveData
            {
                InfoId = GetId(InfoIds, Mathf.RoundToInt(values[0])),
                State = state,
                HasTransform = true,
                Position = new Vector3(values[i], values[i + 1], values[i + 2]),
                Rotation = new Quaternion(values[i + 3], values[i + 4], values[i + 5], values[i + 6]).normalized
            };
        }

        private static float[] PackTransform(ItemSaveData item, int headerSize)
        {
            var values = new float[headerSize + TRANSFORM_SIZE];
            var position = item.Position;
            var rotation = item.Rotation;
            var i = headerSize;

            values[i] = Round(position.x, POSITION_PRECISION);
            values[i + 1] = Round(position.y, POSITION_PRECISION);
            values[i + 2] = Round(position.z, POSITION_PRECISION);
            values[i + 3] = Round(rotation.x, ROTATION_PRECISION);
            values[i + 4] = Round(rotation.y, ROTATION_PRECISION);
            values[i + 5] = Round(rotation.z, ROTATION_PRECISION);
            values[i + 6] = Round(rotation.w, ROTATION_PRECISION);

            return values;
        }

        private static int GetIndex(string id, List<string> ids, Dictionary<string, int> indexes)
        {
            id ??= string.Empty;

            if (indexes.TryGetValue(id, out var index))
                return index;

            index = ids.Count;
            ids.Add(id);
            indexes.Add(id, index);
            return index;
        }

        private static string GetId(List<string> ids, int index) => index >= 0 && index < ids.Count ? ids[index] : null;

        private static float Round(float value, float precision) => Mathf.Round(value * precision) / precision;

        #endregion
    }
}
