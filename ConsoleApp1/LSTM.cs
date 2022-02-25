    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class LSTM
    {
        public Matrix [] x;

        public Matrix w = new Matrix (4,1);
        public Matrix wo, wa, wi, wf;
            
           
        //given
        public float uo, ua, ui, uf;
        public float bo, ba, bi, bf;

        public struct lstm
        {
             public float Out;
             public float State;
             public float o, a, i, f;

            //for backward phase
            public float dState;
        }

        public lstm[] I = new lstm[50];

        public void Initialization()
        {
            I[0].Out = 0;
            I[0].State = 0;
        }
        public void SetValues ()
        {
            //needs to be changed
            x = new Matrix[2];

            for (int i = 0; i < 2; i++)
            {
                x[i] = new Matrix(2,1);
            }

            float[] x1 = new float[] { 1f, 2f };
            float[] x2 = new float[] { 0.5f, 3f };


            x[0].Fill(x1);
            x[1].Fill(x2);

            float[] fwa = new float [] { 0.45f, 0.25f };
            float[] fwi = new float[] { 0.95f, 0.8f };
            float[] fwf = new float[] { 0.7f, 0.45f };
            float[] fwo = new float[] { 0.6f, 0.4f };
            float[] fw = new float[] { 0.45f, 0.25f, 0.95f, 0.8f,  0.7f, 0.45f, 0.6f, 0.4f };

            wa = new Matrix(1, 2, fwa);
            wi = new Matrix(1, 2, fwi);
            wf = new Matrix(1, 2, fwf);
            wo = new Matrix(1, 2, fwo);
            -
            // Matrix of Ws of a,i,f,o
            w = new Matrix(2, 4);
            w.FillVertically(fw);
            Console.WriteLine(w.ToString());
            
            ua = .15f; ui = .8f; uf = .1f; uo = .25f;
            ba = .2f; bi = .65f; bf = .15f; bo = .1f;
            //string a = 1;
            float[] inputb = new float[] { 7, 8, 9, 10, 11, 12 };
        }
        public void forward(int t)
        {
            if(t==0)
            {
                I[t].a = Tanh(wa * x[t] + ua * I[t].Out + ba);
                I[t].i = Sigmoid(wi * x[t] + ui * I[t].Out + bi);
                I[t].f = Sigmoid(wf * x[t] + uf * I[t].Out + bf);
                I[t].o = Sigmoid(wo * x[t] + uo * I[t].Out + bo);
                I[t].State = I[t].a * I[t].i + I[t].f * I[t].State;
                I[t].Out = Tanh(I[t].State) * I[t].o;
            }
            else
            {
                I[t].a = Tanh(wa * x[t] + ua * I[t-1].Out + ba);
                I[t].i = Sigmoid(wi * x[t] + ui * I[t-1].Out + bi);
                I[t].f = Sigmoid(wf * x[t] + uf * I[t-1].Out + bf);
                I[t].o = Sigmoid(wo * x[t] + uo * I[t-1].Out + bo);
                I[t].State = I[t].a * I[t].i + I[t].f * I[t-1].State;
                I[t].Out = Tanh(I[t].State) * I[t].o;
            }
            Console.WriteLine($"Iteration {t}: A {I[t].a}, I {I[t].i}, F {I[t].f}, O {I[t].o}, State {I[t].State}, Out {I[t].Out}");
           
        }

        public void backward(int t)
        {
            float dOut, dt, dA, dI, dF, dO, dX;
            float[] dG = new float[4];
            //Matrix.ToMatrix
            float ObservedOut = 1.25f;            
            dt = I[t].Out - ObservedOut; //linear
            dOut = dt + 0;///wahhh

            I[t].dState = dOut * I[t].o * (1 - Tanh2(I[t].State)) + (1 - Tanh2(I[t + 1].dState)) * I[t + 1].f;
           
            //a
            dG[0] = (float)(I[t].dState * I[t].i * (1 - Math.Pow(I[t].a, 2)));
            //i
            dG[1] = I[t].dState * I[t].a * I[t].i * (1 - I[t].i);
            //f
            dG[2] = I[t].dState * I[t-1].State * I[t].f * (1 - I[t].f);
            //o
            dG[3] = dOut * Tanh(I[t].State) * I[t].o * (1 - I[t].o);



            Matrix dGates = new Matrix(4, 1, dG);

            Console.WriteLine($"Backwards {t}: dt {dt}, dOut {dOut}, F { I[t].dState}, dGates {dGates.ToString()}, State {I[t].State}, Out {I[t].Out}");

        }
        public void Run(int t)
        {
            Initialization();
            SetValues();
            for (int i = 0; i <= t; i++)
            {
                forward(i);
            }
            backward(1);
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
