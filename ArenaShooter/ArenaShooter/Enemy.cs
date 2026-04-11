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
                if (PosX < target.X) PosX += Speed;
                if (PosX > target.X) PosX -= Speed;
                if (PosY < target.Y) PosY += Speed;
                if (PosY > target.Y) PosY -= Speed;
            }
        }

        public override void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Red, PosX, PosY, Width, Height);
        }
    }
}
