using UnityEngine;

namespace Code.Objects.Common
{
    public abstract class BaseDestroyable : MonoBehaviour
    {
        [Header("Settings")] protected int Health = 0;

        [Header("References")] public Sprite NotDamaged;
        public Sprite Damaged;

        private SpriteRenderer _spriteRenderer;
        private readonly Color _damagedColor = Color.grey;
        protected bool ShouldChangeColor = true;
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public virtual bool Damage(int amount, int tankId, bool isFriendly)
        {
            if (_spriteRenderer != null)
            {
                if (Damaged != null)
                    _spriteRenderer.sprite = Damaged;
                else if (ShouldChangeColor)
                    _spriteRenderer.color = _damagedColor;
            }

            Health = Health - amount;

            if (Health > 0)
                return false;

            Destroy();

            return true;
        }

        protected virtual void Destroy()
        {
            // TOOD Addd effects
            Destroy(gameObject);
        }
    }
}