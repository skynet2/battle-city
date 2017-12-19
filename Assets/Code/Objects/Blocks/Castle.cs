using Assets.Code;
using Code.Objects.Common;

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

            return true;
        }
    }
}