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

    [Serializable]
    public class ItemSaveData
    {
        public string InfoId;
        public ItemSaveState State;
        public Vector3 Position;
        public Quaternion Rotation;
        public string ShelfId;
        public int SlotIndex = -1;
        public int PickedIndex = -1;
    }

    [Serializable]
    public class LevelSaveData
    {
        public List<ItemSaveData> Items = new();
    }
}
