using System.Collections.Generic;

namespace Code.Objects.Achievements
{
    [System.Serializable]
    public class Achievement
    {
        public int Id;
        public AchievementCondition ConditionType;
        public string Title;
        public int Value;
    }

    public enum AchievementCondition
    {
        KilledEnemies = 0,
        DestroedBases = 1,
        TimePlayed = 2,
        Deaths = 3
    }

    [System.Serializable]
    public class AchievementContainer
    {
        public List<Achievement> Items;
    }
}