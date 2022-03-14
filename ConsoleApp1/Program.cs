using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
           LSTM foward = new LSTM();;
            foward.Run(1);
 float[] fwa = new float [] { 1, 2, 3 };
                        float[] fwb = new float [] { 4, 5};
            Matrix m1 = new Matrix (1,3,fwa);
           

            Matrix m2 = new Matrix (2,1,fwb);
            Matrix m3 = Matrix.OuterProduct(m1, m2);
            Console.WriteLine(m3.ToString());
        }

    }


}
