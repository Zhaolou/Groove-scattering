using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GORectgroove
{

    public class Complex
    {
        public double real;//实部
        public double image;//虚部
        public double Real
        {
            get { return real; }
            set { real = value; }
        }
        public double Image
        {
            get { return image; }
            set { image = value; }
        }
        public Complex(double real, double image)
        {
            this.real = real;
            this.image = image;
        }
        public Complex() {
            real = 0;
            image = 0;
        }
        public Complex Conjugate()
        {
            return new Complex(this.real, -this.image);
        }
        public static Complex operator +(Complex C, Complex c)
        {
            return new Complex(c.real + C.real, C.image + c.image);
        }
        public Complex Add(params Complex[] complexs)
        {
            if (complexs.Length == 0)
            {
                throw new Exception("输入的参数不能为空！");
            }
            Complex com = new Complex();
            foreach (Complex c in complexs)
            {
                com = com + c;
            }
            return com;
        }

        public static Complex operator -(Complex C, Complex c)
        {
            return new Complex(C.real - c.real, C.image - c.Image);
        }
        public static bool operator ==(Complex C, Complex c)
        {
            return (C.real == c.real && C.image == c.image);
        }
        public static bool operator !=(Complex C, Complex c)
        {
            return (C.real != c.real || C.image != c.image);
        }
        public Complex Minus(params Complex[] complexs)
        {
            if (complexs.Length == 0)
            {
                throw new Exception("输入的参数不能为空！");
            }
            Complex com = complexs[0];
            for (int i = 1; i < complexs.Length; i++)
            {
                com = com - complexs[i];
            }
            return com;
        }
        public static Complex operator *(Complex c, Complex C)
        {
            return new Complex(c.real * C.real - c.image * C.image, c.real * C.image + c.image * C.real);
        }
        public Complex Multiplicative(params Complex[] complexs)
        {
            if (complexs.Length == 0)
            {
                throw new Exception("输入的参数不能为空！");
            }
            Complex com = complexs[0];
            for (int i = 1; i < complexs.Length; i++)
            {
                com += complexs[i];
            }
            return null;
        }
        public static Complex operator /(Complex C, Complex c)
        {
            if (c.real == 0 && c.image == 0)
            {
                throw new Exception("除数的虚部和实部不能同时为零(除数不能为零)");
            }
            double a, b, cc, d;
            a = C.real; b = C.image; cc = c.real; d = c.image;

            double real = (a*cc+b*d) / (cc*cc + d*d);
            double image = (-a*d+b*cc) / (cc*cc+d*d);
            return new Complex(real, image);
        }
        public Complex Divison(params Complex[] complexs)
        {
            if (complexs.Length == 0)
            {
                throw new Exception("输入的参数不能为空！");
            }
            foreach (Complex com in complexs)
            {
                if (com.image == 0 && com.real == 0)
                {
                    throw new Exception("除数的实部和虚部不能同时为零！");
                }
            }
            Complex COM = new Complex();
            COM = complexs[0];
            for (int i = 1; i < complexs.Length; i++)
            {
                COM = COM / complexs[i];
            }
            return COM;
        }
        public double Mod()
        {
            return Math.Sqrt(real * real + image * image);
        }
        public override bool Equals(object obj)
        {
            if (obj is Complex)
            {
                Complex com = (Complex)obj;
                return (com.real == this.real && com.image == this.image);
            }
            return false;
        }
        public static double GetAngle(Complex c)
        {
            return Math.Atan2(c.image, c.real);
        }
        public double GetAngle()
        {
            return Math.Atan2(image, real);
        }
        public override string ToString()
        {
            return string.Format("<{0} , {1}>", this.real, this.image);
        }
    }
    
}
