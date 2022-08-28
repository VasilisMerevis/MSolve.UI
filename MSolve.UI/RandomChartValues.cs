using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MSolve.UI.MainWindow;

namespace MSolve.UI
{
    public class RandomChartValues
    {
        private int MaxIterationsPerLoadStep { get; }
        private int TotalLoadSteps { get; }
        private List<ConvergenceValues> TestConvergenceValues { get; }
        private double Tolerance { get; }
        public event EventHandler<ConvergenceValues> IterationCompleted;

        public RandomChartValues(int maxIterationsPerLoadStep, int totalLoadSteps, double tolerance)
        {
            MaxIterationsPerLoadStep = maxIterationsPerLoadStep;
            TotalLoadSteps = totalLoadSteps;
            TestConvergenceValues = new List<ConvergenceValues>();
            Tolerance = tolerance;
        }
        public void RunRandomConvergenceValuesGenerator()
        {
            double virtualLowerLimit = Tolerance * 0.9;
            double virtualUpperLimit = Tolerance * 1.2;
            
            for (int i = 0; i < TotalLoadSteps; i++)
            {
                for (int j = 0; j < MaxIterationsPerLoadStep; j++)
                {
                    Random randomNumber = new Random();
                    double norm = randomNumber.NextDouble() * (virtualUpperLimit - virtualLowerLimit) + virtualLowerLimit;
                    bool convergenceResult = false;
                   
                    TestConvergenceValues.Add(new ConvergenceValues()
                    {
                        LoadStep = i,
                        Iteration = j,
                        ResidualNorm = norm,
                        Tolerance = this.Tolerance,
                        ConvergenceResult = convergenceResult
                    });
                    
                    System.Threading.Thread.Sleep(500);
                    OnIterationCompleted(TestConvergenceValues.Last());
                    if (norm < Tolerance)
                    {
                        convergenceResult = true;
                        break;
                    }
                }
            }
        }

        protected virtual void OnIterationCompleted(ConvergenceValues e)
        {
            IterationCompleted?.Invoke(this, e);
        }
    }
}
