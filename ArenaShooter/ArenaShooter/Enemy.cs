using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ArenaShooter
{
    public class Enemy : Entity
    {
        private Player target;
        private float animTimer = 0;

        public Enemy(float startX, float startY, Player playerTarget, Image enemyImg) : base(startX, startY, 2.0f)
        {
            this.target = playerTarget;
            this.sprite = enemyImg;
            this.hitboxOffset = 5;
        }

        public override void Update(HashSet<Keys> pressedKeys)
        {
            if (target != null)
            {
                float diffX = target.X - posX;
                float diffY = target.Y - posY;
                float distance = (float)Math.Sqrt(diffX * diffX + diffY * diffY);

                if (distance > 0)
                {
                    rotation = (float)(Math.Atan2(diffY, diffX) * 180 / Math.PI);

                    animTimer += 0.15f;
                    aimOffset = (float)Math.Sin(animTimer) * 15f;

                    posX += (diffX / distance) * speed;
                    posY += (diffY / distance) * speed;
                }
            }
        }
    }
}
