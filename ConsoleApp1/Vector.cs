using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Vector
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public Vector()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }

        public Vector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector(Vector other)
        {
            this.x = other.x;
            this.y = other.y;
            this.z = other.z;
        }

        private double GetLength()
        {
            double sql = this.x * this.x + this.y * this.y + this.z * this.z;
            double len = Math.Sqrt(sql);
            return len;
        }

        public double Length
        {
            get
            {
                return GetLength();
            }
        }

        public void Scale(double factor)
        {
            this.x *= factor;
            this.y *= factor;
            this.z *= factor;
        }

        public bool Unitize()
        {
            double len = this.GetLength();
            if (len <= 0)
            {
                return false;
            }
            this.x /= len;
            this.y /= len;
            this.z /= len;
            return true;

        }

        public void Add(Vector other)
        {
            this.x += other.x;
            this.y += other.y;
            this.z += other.z;
        }

        public override string ToString()
        {
            return $"[{this.x}, {this.y}, {this.z}]";
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return Vector.Addition(a, b);
        }
        public static double operator *(Vector a, Vector b)
        {
            return Vector.DotProduct(a, b);
        }

        public static Vector operator *(double a, Vector b)
        {
            Vector v = new Vector(b);
            v.Scale(a);
            return v;
        }
        //static methods
        public static Vector Addition(Vector a, Vector b)
        {
            double newx = a.x + b.x;
            double newy = a.y + b.y;
            double newz = a.z + b.z;
            Vector v = new Vector(newx, newy, newz);
            return v;
        }

        public static double DotProduct(Vector a, Vector b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vector CrossProduct(Vector a, Vector b)
        {
            double x = a.y * b.z - a.z * b.y;
            double y = a.z * b.x - a.x * b.z;
            double z = a.x * b.y - a.y * b.x;
            return new Vector(x, y, z);
        }
    }

}

