using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ArenaShooter 
{
    public abstract class Entity
    {
        protected float PosX;
        protected float PosY;
        protected int Width = 40;
        protected int Height = 40;
        protected float Speed;

        public Entity(float startX, float startY, float startSpeed)
        {
            PosX = startX;
            PosY = startY;
            Speed = startSpeed;
        }

        public float X => PosX;
        public float Y => PosY;

        public abstract void Update(HashSet<Keys> pressedKeys);

        public virtual void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Gray, PosX, PosY, Width, Height);
        }

        public Rectangle Bounds => new Rectangle((int)PosX, (int)PosY, Width, Height);
    }
}
