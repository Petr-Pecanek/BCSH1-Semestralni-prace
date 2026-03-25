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
        public Player(float startX, float startY) : base(startX, startY, 5.0f) 
        {

        }

        public override void Update()
        {

        }

        public override void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Blue, PosX, PosY, Width, Height);
        }
    }
}
