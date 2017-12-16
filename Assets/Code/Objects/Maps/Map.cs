using System.Collections.Generic;

namespace Code.Objects.Maps
{
    [System.Serializable]
    public class Map
    {
        public List<MapRow> MapRow;
    }
    
    [System.Serializable]
    public class MapRow
    {
        public List<MapBlock> MapData;
    }
}