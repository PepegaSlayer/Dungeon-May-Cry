using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public struct CollideBox
    {
        public int width, height, x, y;
        public CollideBox(int w0, int h0, int x0, int y0)
        {
            width = w0;
            height = h0;
            x = x0;
            y = y0;
        }
        public bool Intersects(CollideBox other)
        {
            if (this.x + this.width >= other.x && other.x + other.width >= this.x &&
                this.y + this.height >= other.y && other.y + other.height >= this.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public struct Attack
    {
        public string name;
        public int damage;
        public int distance;
        public Attack(string name0, int damage0, int distance0)
        {
            name = name0;
            damage = damage0;
            distance = distance0;
        }
    }
}
