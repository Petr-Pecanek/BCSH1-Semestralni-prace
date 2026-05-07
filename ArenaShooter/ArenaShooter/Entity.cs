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
        protected float posX;
        protected float posY;
        protected int width = 40;
        protected int height = 40;
        protected float speed;
        protected Image sprite;
        protected float rotation = 0;
        protected float aimOffset = 0;
        protected int hitboxOffset = 0;

        public Entity(float startX, float startY, float startSpeed)
        {
            posX = startX;
            posY = startY;
            speed = startSpeed;
        }

        public float X { get => posX; set => posX = value; }
        public float Y { get => posY; set => posY = value; }
        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }

        public abstract void Update(HashSet<Keys> pressedKeys);

        public virtual void Draw(Graphics g)
        {
            if (sprite == null) return;

            var state = g.Save();
            g.TranslateTransform(posX + width / 2, posY + height / 2);
            g.RotateTransform(rotation + aimOffset);
            g.DrawImage(sprite, -width / 2, -height / 2, width, height);
            g.Restore(state);
        }

        public virtual Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)posX + hitboxOffset,
                    (int)posY + hitboxOffset,
                    width - (hitboxOffset * 2),
                    height - (hitboxOffset * 2)
                );
            }
        }
    }
}
