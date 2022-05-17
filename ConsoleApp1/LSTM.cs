using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class LSTM
    {
        public Matrix [] x;

        public Matrix warr, warrold;
        public Matrix dGates;
        Matrix dx, dw;
        public Matrix wo, wa, wi, wf, wv, wmainvalue;

        public Matrix u;
        public float uo, ua, ui, uf, uv;

        public float bo, ba, bi, bf, bv;

        //backward values 
        float[] dOut;
        float[] ObservedOut;

        //file values
        public static int lineCounter;

        public struct lstm
        {
             public float Out;
             public float State;
             public float o, a, i, f, v;

            //for backward phase
            public float dState;
        }

        public lstm[] I = new lstm[200];



        //Steps that needs to run in order to perform the LSTM as a whole

        public void Run(int t)
        {
            Initialization();
            int batchSize = 10;//the amount of items in a batch
            for (int m = 0; m < 1; m++)
            {
                int tempbatchSize = batchSize;
                List<int> batchRandOrder = DecideBatches(batchSize);
                int TotalLines = lineCounter - 10;//m check if batchsize matches the amount of lines left
                Console.WriteLine("Iteration" + m);

                for (int i = 0; i < batchRandOrder.Count; i++)
                {
                    Console.WriteLine("Batch Number " + batchRandOrder[i]);
                    if (TotalLines < batchRandOrder[i])
                    {
                        tempbatchSize = lineCounter - batchRandOrder[i];
                        SetValues(tempbatchSize, batchRandOrder[i]);
                        forward(0, tempbatchSize);
                        backward(tempbatchSize - 1);
                        
                        updateWUB(m);
                        if (dw.Greaterthan(.01f))
                        {
                            Console.WriteLine("DONE");

                            Console.WriteLine(dw.ToString());
                            break;
                        }
                        tempbatchSize = batchSize;
                    }
                    else
                    {
                        SetValues(tempbatchSize, batchRandOrder[i]);
                        forward(0, tempbatchSize);
                        backward(tempbatchSize - 1);
                        updateWUB(m);
                        if (dw.Greaterthan(.01f))
                        {
                            Console.WriteLine("DONE");
                            Console.WriteLine(dw.ToString());
                            break;
                        }
                    }

                    Console.WriteLine(m);

                    // batchRandOrder.RemoveAt(i);
                }
            }
/*      
            Initialization();
            SetValues(2);
            for (int i = 0; i <= t; i++)
            {
                forward(i);
            }

            backward(1);    */    
        }

        /* Divides input file into batches & makes a random selection of the batch order */
        private static List<int> DecideBatches(int sample)
        {
            List<int> batchOrder = new List<int>();
            double samplesize = sample;

            //First we determine file length 
            lineCounter = 0;
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\Input.txt" );
            using (StreamReader sr = File.OpenText(fileName))
            {
                sr.ReadLine();//ignoring first line
                while (sr.ReadLine() != null)
                {
                    lineCounter++;
                }
            }
            //Then the number of batches there will be
            int TotalBatches = (int) Math.Ceiling(lineCounter / samplesize);
            Console.WriteLine(TotalBatches);    
            //Then randomly pick based on TotalBatches
            Random rnd = new Random();
            while (batchOrder.Count != TotalBatches)
            {
                int t = (int) ((rnd.Next(1, TotalBatches + 1) - 1) * samplesize);
                if (!batchOrder.Contains(t))
                {
                    batchOrder.Add(t);
                }
            }

            //Printing out list if need be
           /* foreach (var item in batchOrder)
            {
                Console.Write(item + "\n");
            }*/

            return batchOrder;
        }

        public void Initialization()
        {
            I[0].Out = 0;
            I[0].State = 0;

            //Setting the Ws
            float[] fwa = new float[] { 0.45f, 0.25f, 0.45f, 0.25f, 0.45f };
            float[] fwi = new float[] { 0.95f, 0.8f, 0.45f, 0.25f, 0.45f };
            float[] fwf = new float[] { 0.7f, 0.45f, 0.45f, 0.25f, 0.45f };
            float[] fwo = new float[] { 0.6f, 0.4f, 0.45f, 0.25f, 0.45f };
            float[] fwv = new float[] { 0.6f, 0.4f, 0.45f, 0.25f, 0.45f };

            wa = new Matrix(1, 5, fwa);
            wi = new Matrix(1, 5, fwi);
            wf = new Matrix(1, 5, fwf);
            wo = new Matrix(1, 5, fwo);
            wv = new Matrix(1, 5, fwv);

            wmainvalue = Matrix.FillWs(wa, wi, wf, wo, wv);

            //Setting the Us
            ua = .15f; ui = .8f; uf = .1f; uo = .25f; uv = .2f;
            u = new Matrix(1, 5);
            float[] fu = new float[] { ua, ui, uf, uo, uv };
            u.Fill(fu);

            //Setting the Bs
            ba = .2f; bi = .65f; bf = .15f; bo = .1f; bv = .2f;
        }
        

        public void SetValues (int batchsize, int fileline)
        {
            int BatchSize = batchsize, size = 0;
            int lineCount = 0, lineLength;
            string text = "";
            string[] lines;
            string[] stringSeparators = new string[] { "\n", "\t", " " };

            //setting X 
            x = new Matrix[batchsize];
            //setting Observed Out & its delta
            ObservedOut = new float[batchsize];
            dOut = new float[batchsize + 1];//needs to demonstrate -1 hence the addition of one item

            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\Input.txt");
            using (StreamReader sr = File.OpenText(fileName))
            {
                sr.ReadLine();//ignore first line

                //First go to correct line of file, based of random batch list (fileline)
                while (lineCount != fileline) 
                {
                    text = sr.ReadLine();
                    if (lineCount!=fileline) lineCount++;
                }
                text = sr.ReadLine();

                //initializing lineLength
                lines = text.Trim().Split('\t', (char)StringSplitOptions.RemoveEmptyEntries);
                lineLength = lines.Length -1;

                //Then read the file and assign values for the batchsize

                //File must be formatted as follows: first values must be Input Vectors	(can be any size, dont forget to change w).
                //                                   followed by Observed Value ( Y)
                while (BatchSize>0)
                {   
                    lines = text.Trim().Split('\t', (char)StringSplitOptions.RemoveEmptyEntries);

                    //Setting X
                    x[size] = new Matrix(lineLength, 1);
                    for (int i = 0; i < lineLength; i++)
                    {
                        x[size].SetCell(i, 0, float.Parse(lines[i]));
                    }
                   
                    //Setting Observed Out
                    ObservedOut[size] = float.Parse(lines[lineLength]);
                    
                   
                    text = sr.ReadLine();
                    BatchSize--; size++;


                }


            }
        }

        /*public void SetInitialValues()
        {

            x = new Matrix[2];

            for (int i = 0; i < 2; i++)
            {
                x[i] = new Matrix(2, 1);
            }

            float[] x1 = new float[] { 1f, 2f };
            float[] x2 = new float[] { 0.5f, 3f };


            x[0].Fill(x1);
            x[1].Fill(x2);

            float[] fwa = new float[] { 0.45f, 0.25f };
            float[] fwi = new float[] { 0.95f, 0.8f };
            float[] fwf = new float[] { 0.7f, 0.45f };

            float[] fwo = new float[] { 0.6f, 0.4f };
            float[] fw = new float[] { 0.45f, 0.25f, 0.95f, 0.8f, 0.7f, 0.45f, 0.6f, 0.4f };

            wa = new Matrix(1, 2, fwa);
            wi = new Matrix(1, 2, fwi);
            wf = new Matrix(1, 2, fwf);
            wo = new Matrix(1, 2, fwo);

            // Matrix of Ws of a,i,f,o
            //make it the last value of input
            w = new Matrix[2];
            w[1] = Matrix.FillW(wa, wi, wf, wo);

            ua = .15f; ui = .8f; uf = .1f; uo = .25f;
            float[] fu = new float[] { ua, ui, uf, uo };
            u.Fill(fu);

            ba = .2f; bi = .65f; bf = .15f; bo = .1f;
            //string a = 1;

            //backwards
            ObservedOut = new float[2] { .5f, 1.25f };
            dOut = new float[2 + 1];//needs to demonstrate -1
        }*/
    
        public void forward(int start, int size)
        {
            int t = start;
            if(t==0)
            {
                I[t].a = Tanh(wa * x[t] + ua * I[t].Out + ba);
                I[t].i = Sigmoid(wi * x[t] + ui * I[t].Out + bi);
                I[t].f = Sigmoid(wf * x[t] + uf * I[t].Out + bf);
                I[t].o = Sigmoid(wo * x[t] + uo * I[t].Out + bo);
                I[t].v = Sigmoid(wv * x[t] + uo * I[t].Out + bv);//HERE
                I[t].State = I[t].a * I[t].i + I[t].f * I[t].State;
                I[t].Out = Tanh(I[t].State) * I[t].o;
            }
            else
            {
                I[t].a = Tanh(wa * x[t] + ua * I[t-1].Out + ba);
                I[t].i = Sigmoid(wi * x[t] + ui * I[t-1].Out + bi);
                I[t].f = Sigmoid(wf * x[t] + uf * I[t-1].Out + bf);
                I[t].o = Sigmoid(wo * x[t] + uo * I[t-1].Out + bo);
                I[t].v = Sigmoid(wv * x[t] + uo * I[t].Out + bv);//HERE
                I[t].State = I[t].a * I[t].i + I[t].f * I[t-1].State;
                I[t].Out = Tanh(I[t].State) * I[t].o;
            }
           // Console.WriteLine($"Iteration {t}: A {I[t].a}, I {I[t].i}, F {I[t].f}, O {I[t].o}, V {I[t].v}, State {I[t].State}, Out {I[t].Out}");
            t += 1;
            if (t < size)
            {
                forward(t, size);
            }
        }

        public void backward(int t)
        {           
            float dt;
          
            float[] dG = new float[5];

            dt = I[t].Out - ObservedOut[t]; //linear

            if (dOut[t+1]==0) dOut[t+1] = dt + dOut[t+1];

            I[t].dState = dOut[t+1] * I[t].o * (1 - Tanh2(I[t].State)) + (1 - Tanh2(I[t + 1].dState)) * I[t + 1].f;
           
            //GATES
            //a
            dG[0] = (float)(I[t].dState * I[t].i * (1 - Math.Pow(I[t].a, 2)));                
            //i
            dG[1] = I[t].dState * I[t].a * I[t].i * (1 - I[t].i);
            //f
            if (t== 0) dG[2] = I[t].dState * I[0].State * I[t].f * (1 - I[t].f);
            else dG[2] = I[t].dState * I[t - 1].State * I[t].f * (1 - I[t].f);
            //o
            dG[3] = dOut[t+1] * Tanh(I[t].State) * I[t].o * (1 - I[t].o);
            //v HERE
            dG[4] = dOut[t + 1] * Tanh(I[t].State) * I[t].v * (1 - I[t].v);


            dGates = new Matrix(5,1,dG);
           // dGates = Matrix.OuterProduct(dGates, x[t]);
            //update w vector

            //updating u vector
            //updating b vector;';;;
            //learning rate
            //  Console.WriteLine(w2[t].ToString);
            dx = Matrix.DotProductMatrix(wmainvalue,dGates);

            /*Matrix dwold, dnew ;
            if (dw != null)
            {
                dwold = dw;
            }*/
           
            if (dw==null)
            {
                dw = Matrix.OuterProduct(dGates, x[t]);
            }
            else
            {
                dw = dw + Matrix.OuterProduct(dGates, x[t]);
            }
          

            //dnew = dwold - dw;
           //Console.WriteLine(dw.ToString());
            //  dx = Matrix.DotProductValue(dGates, dGates);
            // Matrix.DotProductValue(a, b);
            //    dx = w[t] * dGates;
            //  w[t] = Matrix.OuterProduct(dGates,x[t]);
            dOut[t] = u * dGates;//out t-1
            // Console.WriteLine($"\n\nBackwards {t}: dt {dt}, dOut {dOut[t+1]}, F { I[t].dState},\nGates: \n{dGates.ToString()}dx:\n{dx}dOut-1 {dOut[t]} ");
           // Console.WriteLine("hello"+dw.ToString());
            t -= 1;         
            if (t>=0)
			{
                //if (dw.Greaterthan(0.01f) == true)
                {
                    int i = t;
                    backward(i);
                }
			}
        }

        public void updateWUB(int m)
        {
            /* x[size] = new Matrix(lineLength, 1);
             for (int i = 0; i < lineLength; i++)
             {
                 x[size].SetCell(i, 0, float.Parse(lines[i]));
             }*/
            /*     warrold = warr;
                 warr = new Matrix(w.row,w.col);*/
            /* 

              if (m == 0)
              {
                  warr = w;
                  *//*for (int i = 0; i < warr.row; i++)
                  {
                      for (int j  = 0; j < warr.col; j++)
                      {
                          warr.matrixData[i, j] = 0f;
                      }
                  }*//*
              }
              else
              {
                  w = Matrix.Substract(w, dw);
              }*/

            Console.WriteLine("W beofre updating is \n" + wmainvalue.ToString());
            wmainvalue = Matrix.Substract(wmainvalue, Matrix.MultiplybyAValue(dw,0.1f));
            Console.WriteLine("W after updating is \n" + wmainvalue.ToString());
            Console.WriteLine("Dw is "+ dw.ToString());
            Console.WriteLine("DW average now is : " + dw.Summation() / 10);
            // Console.WriteLine("Patch ended, w is: \n" + warr.ToString());
            /* Console.WriteLine("Patch ended, wnew is: \n" + w.ToString());
             Console.WriteLine("DW is "+ dw);

             Console.WriteLine("DW average now is : " + dw.Summation()/10);
 */
            for (int i = 0; i < dw.row; i++)
            {
                for (int j = 0; j < dw.col; j++)
                {
                   dw.matrixData[i,j] = 0f;
                }
            }
           // warrold = dw;
        }

        public static float Sigmoid(float value)
        {
            return 1.0f / (1.0f + (float) Math.Exp(-value));
        }

        public static float Tanh (float x)
        {
            return (float)((Math.Exp(2 * x) - 1) / (Math.Exp(2 * x) + 1));
        }
        public static float Tanh2(float x)
        {
            return (float)(Math.Pow((Math.Exp(2 * x) - 1) / (Math.Exp(2 * x) + 1),2));
        }
    }

}
