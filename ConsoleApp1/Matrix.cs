using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Matrix
    {
        public int row { get; set; }
        public int col { get; set; }
        public float[,] matrixData;

        public Matrix(int r, int c)
        {
            if (c < 1 || r < 1)
            {
                throw new Exception($"Invalid Number of Rows or Columns in Matrix");
            }

            this.col = c;
            this.row = r;
            matrixData = new float[row, col];
        }
        public Matrix(int r, int c, params float[] data)
        {
            if (c < 1 || r < 1)
            {
                throw new Exception($"Invalid Number of Rows or Columns in Matrix");
            }

            this.col = c;
            this.row = r;
            matrixData = new float[row, col];

            int datasize = data.Length;
            int count = 0;
            while (count < datasize)
            {
                for (int m = 0; m < row; m++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        matrixData[m, j] = data[count];
                        count++;
                    }
                }
            }
        }

       
        //needs work
        public Matrix(Matrix other)
        {
            this.col = other.col;
            this.row = other.row;
        }
        //Sets individual values in the matrix
        public void SetCell(int r, int c, int val)
        {
            matrixData[r, c] = val;
        }

      
        //Fills matrix values using an array
        //neds an error message
        public void Fill(params float[] data)
        {
            int datasize = data.Length;
            int count = 0;
            while (count < datasize)
            {
                for (int m = 0; m < row; m++)
                {
                    for (int j = 0; j < col; j++)
                    {
                         matrixData[m,j] = data[count];
                         count++;
                    }
                }
            }
        }
         public void FillVertically(params float[] data)
        {
            int datasize = data.Length;
            int count = 0;
            while (count < datasize)
            {
                for (int m = 0; m < col; m++)
                {
                    for (int j = 0; j < row; j++)
                    {
                         matrixData[j,m] = data[count];
                         count++;
                    }
                }
            }
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    str += matrixData[i, j] + "\t";
                }
                str += "\n";
            }

            return str;
        }

      
        public static float operator * (Matrix a, Matrix b)
        {
            return Matrix.DotProductValue(a, b);
        }
        public static Matrix operator +(Matrix a, Matrix b)
        {
            return Matrix.Concatenation(a, b);
        }
        public static Matrix ToMatrix(float x)
        {
            Matrix temp = new Matrix(1,1);
            temp.matrixData[0, 0] = x;
            return temp;
        }

        public static Matrix Sigmoid(Matrix x)
        {
            Matrix temp = new Matrix(x.row, x.col);
            for (int i = 0; i < x.row; i++)
            {
                for (int j = 0; j < x.col; j++)
                {
                    temp.matrixData[i, j] = (float)(1.0f / (1.0f + Math.Exp(-x.matrixData[i, j])));
                }
            }

            return temp;
        }
      
        public static Matrix Tanh(Matrix x)
        {
            Matrix temp = new Matrix(x.row, x.col);
            for (int i = 0; i < x.row; i++)
            {
                for (int j = 0; j < x.col; j++)
                {
                    temp.matrixData[i, j] = (float)Math.Tanh(x.matrixData[i, j]);
                }
            }

            return temp;
        }

        public static Matrix Concatenation(Matrix x, Matrix y)
        {
            if (x.row != y.row)
            {
                throw new Exception("Cannot concatenate, uneven rows:\n");
            }
            Matrix temp = new Matrix(x.row, x.col + y.col);
            // x.matrixData.CopyTo(temp.matrixData, 0);
            //     x.matrixData.
            // y.matrixData.CopyTo(temp.matrixData, x.matrixData.Length);

            for (int i = 0; i < x.row; i++)
            {
                for (int j = 0; j < x.col; j++)
                {
                    temp.matrixData[i, j] = x.matrixData[i, j];
                }
            }

            for (int i = 0; i < y.row; i++)
            {
                for (int j = 0; j < y.col; j++)
                {

                    temp.matrixData[i, j + x.col] = y.matrixData[i, j];
                }
            }
            return temp;
        }

       
        public static Matrix HadmardProduct(Matrix x, Matrix y)
        {
            if (x.row != y.row || x.col != y.col)
            {
                throw new Exception("Cannot compute Hadamard product, dimensions do not match:\n");
            }
            Matrix temp = new Matrix(x.row, x.col);

            for (int i = 0; i < temp.row; i++)
            {
                for (int j = 0; j < temp.col; j++)
                {
                    temp.matrixData[i, j] = x.matrixData[i, j] * y.matrixData[i, j];
                }
            }

            return temp;
        }

      
        //dot product that returns the matrix
        //needs work
        public static Matrix DotProductMatrix(Matrix x, Matrix y)
        {          
            if (x.col != y.row)
            {
                throw new Exception("Cannot compute Dot product, dimensions do not match:\n");
            }

            Matrix RowTemp = new Matrix(x.row, 1);
            Matrix ColTemp = new Matrix(x.row, 1);
            Matrix temp = new Matrix(x.row, 1);


            float value = 0.0f;

            for (int i = 0; i < temp.row; i++)
            {
                Console.WriteLine("i"+i);
                for (int j = 0; j < temp.col; j++)
                {
                    Console.WriteLine("j"+j);
                }

            }

            return temp;
        }

        //dot product that returns single value
        public static float DotProductValue(Matrix x, Matrix y)
        {
            if (x.col != y.row)
            {
                throw new Exception("Cannot compute Dot product, dimensions do not match:\n");
            }

            Matrix RowTemp = new Matrix(x.col, 1);
            Matrix ColTemp = new Matrix(y.row, 1);

            float value = 0.0f;
            int r = 0;

            for (int i = 0; i < x.row; i++)
            {
                //Get Matrix x Rows
                for (int j = 0; j < x.col; j++)
                {
                    RowTemp.matrixData [r,0] = x.matrixData [i,j];
                    r++;
                }
                r = 0;
                //Gets Matrix y Rows and multiplies by x row
                for (int m = 0; m < y.col; m++)
                {
                    for (int p = 0; p < y.row; p++)
                    {
                        ColTemp.matrixData[r, 0] = y.matrixData[p, m];
                        r++;
                    }
                    r = 0;
                   value += DotProductAssistant(RowTemp,ColTemp);
                }

                r = 0; 
            }

            return value;
        }

        private static float DotProductAssistant (Matrix x, Matrix y)
        {
            float temp = 0.0f;
            for (int i = 0; i < x.row; i++)
            {
                temp += x.matrixData[i, 0] * y.matrixData[i,0];
            }

            return temp;
        }
            
        public static double MOuterProduct(Vector a, Vector b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

    }

}
