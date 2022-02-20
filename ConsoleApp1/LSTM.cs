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

        public Matrix wo = new Matrix(1,2);
        public Matrix wa = new Matrix(1,2);
        public Matrix wi = new Matrix(1,2);
        public Matrix wf = new Matrix(1,2);

        public float o, a, i, f;

        public float uo, ua, ui, uf;

        public float bo, ba, bi, bf;

        public struct lstm
        {
             public float Out;
             public float State;       
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

            float[] fwa = new float[] { 0.45f, 0.25f };
            float[] fwi = new float[] { 0.95f, 0.8f };
            float[] fwf = new float[] { 0.7f, 0.45f };
            float[] fwo = new float[] { 0.6f, 0.4f };

            wa = new Matrix(1, 2, fwa);
            wi = new Matrix(1, 2, fwi);
            wf = new Matrix(1, 2, fwf);
            wo = new Matrix(1, 2, fwo);

            ua = .15f; ui = .8f; uf = .1f; uo = .25f;
            ba = .2f; bi = .65f; bf = .15f; bo = .1f;

            float[] inputb = new float[] { 7, 8, 9, 10, 11, 12 };
        }
        public void Iteration(int t)
        {
            if(t==0)
            {
                a = Tanh(wa * x[t] + ua * I[t].Out + ba);
                i = Sigmoid(wi * x[t] + ui * I[t].Out + bi);
                f = Sigmoid(wf * x[t] + uf * I[t].Out + bf);
                o = Sigmoid(wo * x[t] + uo * I[t].Out + bo);
                I[t].State = a * i + f * I[t].State;
                I[t].Out = Tanh(I[t].State) * o;
            }
            else
            {
                a = Tanh(wa * x[t] + ua * I[t-1].Out + ba);
                i = Sigmoid(wi * x[t] + ui * I[t-1].Out + bi);
                f = Sigmoid(wf * x[t] + uf * I[t-1].Out + bf);
                o = Sigmoid(wo * x[t] + uo * I[t-1].Out + bo);
                I[t].State = a * i + f * I[t-1].State;
                I[t].Out = Tanh(I[t].State) * o;
            }
            Console.WriteLine($"Iteration {t}: A {a}, I {i}, F {f}, O {o}, State {I[t].State}, Out {I[t].Out}");
        }

        public void Run(int t)
        {
            Initialization();
            SetValues();
            for (int i = 0; i <= t; i++)
            {
                Iteration(i);
            }
             
        }

        public static float Sigmoid(float value)
        {
            return 1.0f / (1.0f + (float) Math.Exp(-value));
        }

        public static float Tanh (float x)
        {
            return (float)((Math.Exp(2 * x) - 1) / (Math.Exp(2 * x) + 1));
        }

    }

}
