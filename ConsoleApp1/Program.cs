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
           LSTM food = new LSTM();;
            food.Run(1);

            float[] inputb = new float[] {7,8,9,10,11,12};
            float[] inputa = new float[] {1,2,3,4,5,6};
            /*
                        int[] myIntArray = new int[5] { 1, 2, 3, 4, 5 };
                        int[,] myIntArray2d = new int[2, 1] { { 2}, { 5 } };
                        int[,] myIntArray2d2 = new int[7, 8] ;
                        int[,] array2Da = new int[4, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };

            */

            Matrix m3 = new Matrix(2, 3, inputa);
            Matrix m4 = new Matrix(3, 2);
       

  

   
           float extra = m3 * m4;              
         //   float tries = Matrix.DotProductValue(m3, m4);
        //    Console.WriteLine("dot :\n" + tries);
///Console.WriteLine("dot :\n" + extra);




            //hanmard product doesnt work for matrixes of various sizes concatenation?                             
        }

        public static float Sigmoid(double value)
        {
            return 1.0f / (1.0f + (float)Math.Exp(-value));
        }
        public static void PrintValues (int[] myArr)
        {
            foreach (int i in myArr)
            {
                Console.WriteLine("\t{0}", i);
            }
            Console.WriteLine();
        }
        public static void PrintValues2d(int[] myArr)
        {
            foreach (int i in myArr)
            {
                Console.WriteLine("\t{0}", i);
            }
            Console.WriteLine();
        }
    }


}
