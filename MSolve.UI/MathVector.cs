using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace MSolve.UI
{
    public static class MathVector
    {
        public static double[] VectorCrossProduct(double[] vector1, double[] vector2)
        {
            if (vector1.Length == vector2.Length && vector1.Length == 3)
            {
                double[] result = new double[3];
                result[0] = vector1[1] * vector2[2] - vector1[2] * vector2[1];
                result[1] = vector1[2] * vector2[0] - vector1[0] * vector2[2];
                result[2] = vector1[0] * vector2[1] - vector1[1] * vector2[0];
                return result;
            }
            else
            {
                throw new Exception("Vectors Dot Product: Not equally sized vectors or wrong size vector");
            }
        }

        public static double VectorNorm2(double[] vector)
        {
            double sum = 0;
            for (int row = 0; row < vector.Length; row++)
            {
                sum = sum + Math.Pow(vector[row], 2);
            }
            double norm2 = Math.Sqrt(sum);
            return norm2;
        }

        public static double[] VectorScalarProduct(double[] vector, double scalar)
        {
            double[] resultVector = new double[vector.Length];
            for (int row = 0; row < vector.Length; row++)
            {
                resultVector[row] = scalar * vector[row];
            }
            return resultVector;
        }

        public static double[] CreateVectorFromPoints(double point1X, double point1Y, double point2X, double point2Y)
        {
            return new double[] 
            { 
                point2X - point1X,
                point2Y - point1Y
            };
        }

        public static CalculateLinearQuadNormalUnitVector()
        {

        }
    }
}
