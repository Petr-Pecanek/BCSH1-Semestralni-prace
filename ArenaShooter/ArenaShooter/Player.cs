using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ArenaShooter
{
    public class Player : Entity
    {
        private float animTimer = 0;
        public Player(float startX, float startY, Image playerImg) : base(startX, startY, 5.0f) 
        {
            this.sprite = playerImg;
            this.hitboxOffset = 5;
        }

        public override void Update(HashSet<Keys> pressedKeys)
        {
            float moveX = 0;
            float moveY = 0;

            if (pressedKeys.Contains(Keys.W)) moveY -= 1;
            if (pressedKeys.Contains(Keys.A)) moveX -= 1;
            if (pressedKeys.Contains(Keys.S)) moveY += 1;
            if (pressedKeys.Contains(Keys.D)) moveX += 1;

            float length = (float)Math.Sqrt(moveX * moveX + moveY * moveY);

            if (length > 0)
            {
                animTimer += 0.2f;
                aimOffset = (float)Math.Sin(animTimer) * 12f;

                posX += (moveX / length) * speed;
                posY += (moveY / length) * speed;
            } else
            {
                animTimer = 0; 
                aimOffset = 0;
            }
        }

        public void UpdateRotation(Point mousePos)
        {
            float diffX = mousePos.X - (posX + Width / 2);
            float diffY = mousePos.Y - (posY + Height / 2);

            rotation = (float)(Math.Atan2(diffY, diffX) * 180 / Math.PI);
        }
    }
}
