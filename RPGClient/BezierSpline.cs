using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGClient
{
    using OpenTK;

    public class BezierSpline
    {
        public List<Vector2> ResultCurve;

        public Vector2[] control;

        public BezierSpline(Vector2[] controlPoints)
        {
            control = controlPoints;
        }

        private float Factorial(float n)
        {
            float result = 1;
            for (int i = 1; i <= n; ++i)
                result *= i;
            return result;
        }

        private float Binomial(float n, float k)
        {
            return Factorial(n)/(Factorial(k)*Factorial(n-k));
        }
        public void Calculate()
        {
            ResultCurve = new List<Vector2>();
            for (float t = 0; t <= 1; t += 0.01f)
            {
                Vector2 currentPoint = new Vector2(0);
                int count = control.Length-1;
                for (int i = 0; i < control.Length; ++i)
                {
                    float binomial = Binomial(count, i);
                    float coefficient = binomial* (float)(Math.Pow(1 - t, count - i) * Math.Pow(t, i));
                    currentPoint += coefficient * control[i];
                }
                ResultCurve.Add(currentPoint);
            }
        }

    }
}
