using System;
using System.Collections.Generic;
using System.IO;
using Code.Objects.Achievements;
using Code.Objects.Common;
using UnityEngine;

namespace Assets.Code
{
    public class AchievementService
    {
        private static List<Achievement> _achievements;

        private static List<Achievement> GetAvailable()
        {
            if (_achievements != null)
                return _achievements;

            _achievements = JsonUtility.FromJson<AchievementContainer>(
                File.ReadAllText(@"C:\ExtraSSD\Git\Github\battle-city\Assets\Code\Map\achevements.json")).Items; // TODO

            return _achievements;
        }

        public List<Achievement> ApplyAchievements(User user)
        {
            if (user == null)
                return new List<Achievement>();

            var toApply = new List<Achievement>();

            foreach (var ac in GetAvailable())
            {
                if (user.Achievements == null)
                    user.Achievements = new List<Achievement>();

                if (user.Achievements.Contains(ac))
                    continue;

                switch (ac.ConditionType)
                {
                    case AchievementCondition.KilledEnemies:
                        if (ac.Value <= user.KilledEnemies)
                            toApply.Add(ac);
                        break;
                    case AchievementCondition.DestroedBases:
                        if (ac.Value <= user.DestroedBases)
                            toApply.Add(ac);
                        break;
                    case AchievementCondition.TimePlayed:
                        if (ac.Value <= user.TimePlayed)
                            toApply.Add(ac);
                        break;
                    case AchievementCondition.Deaths:
                        if (ac.Value <= user.Deaths)
                            toApply.Add(ac);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            user.Achievements.AddRange(toApply);

            return toApply;
        }
    }
}