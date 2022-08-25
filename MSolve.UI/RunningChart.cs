using LiveCharts;
using LiveCharts.Configurations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSolve.UI
{
    public class RunningChart
    {
        public ChartValues<ConvergenceValues> ChartValues { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }
        private double _axisMax;
        private double _axisMin;
        public event PropertyChangedEventHandler PropertyChanged;

        public void ConvergenceResults()
        {
            var mapper = Mappers.Xy<ConvergenceValues>()
                .X(model => model.Iteration)   //use DateTime.Ticks as X
                .Y(model => model.ResidualNorm);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<ConvergenceValues>(mapper);

            //the values property will store our values array
            ChartValues = new ChartValues<ConvergenceValues>();

            //lets set how to display the X Labels
            //DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");

            //AxisStep forces the distance between each separator in the X axis
            AxisStep = 1;// TimeSpan.FromSeconds(1).Ticks;
            //AxisUnit forces lets the axis know that we are plotting seconds
            //this is not always necessary, but it can prevent wrong labeling
            AxisUnit = 100;// TimeSpan.TicksPerSecond;
            //DataContext = this;
        }

        private void SetAxisLimits(int now)
        {
            AxisMax = now + 2;
            AxisMin = now - 10;
        }
        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged("AxisMax");
            }
        }
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged("AxisMin");
            }
        }
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
