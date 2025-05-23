using Mirror;
using UnityEngine;

namespace ScriptableObjects
{
    public abstract class ItemData : ScriptableObject
    {
        [Header("GENERAL")]
        [ReadOnly] public string id => name;
        public GameObject prefab;

        public bool isStackable = true;
    }
}