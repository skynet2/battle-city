using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Code.Objects.Blocks;
using Code.Objects.Common;
using UnityEngine;

namespace Assets.Code
{
    public class Bullet : MonoBehaviour
    {
        //[Header("References")] public GameObject HitEffect;
        
        [HideInInspector] public int TankId; //The tank which shot this projectile.
        [HideInInspector] public int Damage;
        [HideInInspector] public Rigidbody2D Rig;
        
        private void Awake()
        {
            Rig = GetComponent<Rigidbody2D>();
        }

        public void OnCollisionEnter2D(Collision2D col)
        {
            var obj = col.gameObject.GetComponent<BaseDestroyable>();

            if (obj != null)
            {
                obj.Damage(Damage, TankId, false);
            }

           
//            if (col.gameObject.tag.Equals("tank", StringComparison.InvariantCultureIgnoreCase))
//            {
//                var enemyTank = col.gameObject.GetComponent<TankUser>();
//
//                enemyTank.Damage(Damage, TankId);
//            }
            
            Explode();
        }
        
        /// <summary>
        /// 
        /// </summary>
        void Explode()
        {
            //Particle Effect
           // var obj = Instantiate(HitEffect, transform.position, Quaternion.identity);   //Spawn the hit particle effect at the position of impact.
         //   Destroy(obj, 1.0f);   //Destroy that effect after 1 second.
            Destroy(gameObject);        //Destroy the bullet.
        }
    }
}