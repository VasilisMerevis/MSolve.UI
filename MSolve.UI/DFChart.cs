using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Configurations;

namespace MSolve.UI
{
    public class DFChart
    {
        public ChartValues<DFChartValues> ValuesToDrawChart { get; set; }
        
        public DFChart()
        {
            var mapper = Mappers.Xy<DFChartValues>()
               .X(model => model.Displacement) 
               .Y(model => model.Force);
            Charting.For<ConvergenceValues>(mapper);

            ValuesToDrawChart = new ChartValues<DFChartValues>();
        }

        public void CreateTestValuesForChart()
        {
            ValuesToDrawChart.Add(new DFChartValues() { Displacement = 0, Force = 0 });
            ValuesToDrawChart.Add(new DFChartValues() { Displacement = 2, Force = 4 });
            ValuesToDrawChart.Add(new DFChartValues() { Displacement = 4, Force = 5 });
            ValuesToDrawChart.Add(new DFChartValues() { Displacement = 6, Force = 7 });
        }
    }
}
