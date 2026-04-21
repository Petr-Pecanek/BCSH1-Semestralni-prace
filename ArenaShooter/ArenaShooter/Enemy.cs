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

        public Enemy(float startX, float startY, Player playerTarget) : base(startX, startY, 2.0f)
        {
            this.target = playerTarget;
        }

        public override void Update(HashSet<Keys> pressedKeys)
        {
            if (target != null)
            {
                float diffX = target.X - PosX;
                float diffY = target.Y - PosY;
                float distance = (float)Math.Sqrt(diffX * diffX + diffY * diffY);

                if (distance > 0)
                {
                    PosX += (diffX / distance) * Speed;
                    PosY += (diffY / distance) * Speed;
                }
            }
        }

        public override void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Red, PosX, PosY, Width, Height);
        }
    }
}
