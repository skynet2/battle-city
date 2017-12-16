using Code.Objects.Common;

namespace Code.Objects.Blocks
{
    public class Border : BaseDestroyable
    {
        public Border()
        {
            Health = int.MaxValue;
            ShouldChangeColor = false;
        }
    }
}