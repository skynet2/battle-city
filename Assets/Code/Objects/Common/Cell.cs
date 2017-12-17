using Code.Objects.Blocks;
using UnityEngine;

namespace Code.Objects.Common
{
    public class Cell
    {
        public Vector3 Pos;
        public GameObject IsOссupiedBy;

        public float Weight
        {
            get
            {
                BaseDestroyable obj = null;

                if (IsOссupiedBy != null)
                    obj = IsOссupiedBy.GetComponent<BaseDestroyable>();

                if (IsOссupiedBy == null)
                    return 1;
                else if (obj is Wood)
                    return 0.75f;
                else if (obj is Stone)
                    return 0.35f;
                else if (obj is Border)
                    return 0f;

                return 0;
            }
        }
    }
}