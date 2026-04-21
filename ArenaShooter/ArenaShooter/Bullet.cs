using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaShooter
{
    public class Bullet : Entity
    {
        private float velocityX;
        private float velocityY;

        public Bullet(float startX, float startY, int targetX, int targetY) : base(startX, startY, 12.0f)
        {
            this.Width = 8;
            this.Height = 8;

            float diffX = targetX - startX;
            float diffY = targetY - startY;
            float distance = (float)Math.Sqrt(diffX * diffX + diffY * diffY);

            if (distance > 0)
            {
                velocityX = (diffX / distance) * Speed;
                velocityY = (diffY / distance) * Speed;
            } else
            {
                velocityX = 0;
                velocityY = -Speed;
            }
        }

        public override void Update(HashSet<Keys> pressedKeys)
        {
            PosX += velocityX;
            PosY += velocityY;
        }

        public override void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.Gold, PosX, PosY, Width, Height);
        }
    }
}
