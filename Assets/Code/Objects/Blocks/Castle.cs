using System;
using Assets.Code;
using Code.Objects.Common;
using UnityEngine;

namespace Code.Objects.Blocks
{
    public class Castle : BaseDestroyable
    {
        public Castle()
        {
            Health = 1;
        }

        public override bool Damage(int amount, TankUser tank)
        {
            if (this.TeamId == tank.TeamId)
                return false;
            
            var controller = FindObjectOfType<GameController>();

            controller.IsGameRunning = false;
            
            Destroy(this.gameObject);

            if (tank.TeamId == 1)
                controller.WinText.color = Color.red;
            else 
                controller.WinText.color = Color.green;
            
            controller.WinText.text = string.Format("Team {0} Win!", tank.TeamId);
      
            return true;
        }
    }
}