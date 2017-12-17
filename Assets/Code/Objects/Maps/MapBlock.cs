using UnityEngine;

namespace Code.Objects.Maps
{
    [System.Serializable]
    public class MapBlock
    {
        public MapBlockType Type;
        public GameObject ReferenceGameObject;
        public bool IsSpawn;
        public int TeamId;
    }
}