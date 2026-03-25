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
        public Enemy(float startX, float startY) : base(startX, startY, 2.0f)
        {

        }

        public override void Update()
        {
            
        }

        public override void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Red, PosX, PosY, Width, Height);
        }
    }
}
