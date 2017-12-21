using System.Collections.Generic;
using Assets.Code;
using Code.Objects.Achievements;

namespace Code.Objects.Common
{
    public class User
    {
        public TankUser Tank;
        public string Name;
        public long TimePlayed;
        public int KilledEnemies;
        public int DestroedBases;
        public int Deaths;
        public List<Achievement> Achievements;

        public User()
        {
            Achievements = new List<Achievement>();
        }
    }
}