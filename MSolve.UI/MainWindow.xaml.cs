﻿using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MSolve.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Dictionary<int, IGraphicalNode> nodes = new Dictionary<int, IGraphicalNode>();
        private Dictionary<int, Dictionary<int, int>> elementsConnectivity = new Dictionary<int, Dictionary<int, int>>();
        private PerspectiveCamera TheCamera;
        public event PropertyChangedEventHandler PropertyChanged;
        private Model3DGroup MainModel3Dgroup = new Model3DGroup();
        private GFECMesh mesh;
        
        public ChartValues<ConvergenceValues> ChartValues { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }
        int kTemp;

        // The camera's current location.
        private double CameraPhi = 0; //Math.PI / 6.0;       // 30 degrees
        private double CameraTheta = 0;// Math.PI / 6.0;     // 30 degrees
        private double CameraR = 300.0 / 100;

        // The change in CameraPhi when you press the up and down arrows.
        private const double CameraDPhi = 0.1;

        // The change in CameraTheta when you press the left and right arrows.
        private const double CameraDTheta = 0.1;

        // The change in CameraR when you press + or -.
        private const double CameraDR = 0.1 * 100 * 10;
        private double _axisMax;
        private double _axisMin;

        public MainWindow()
        {
            InitializeComponent();
            
            ConvergenceResults();
            Window_Loaded();
            //gnuplotImage.Source = new BitmapImage(new Uri(@"C:\Users\Vasilis\source\repos\MSolve.UI\MSolve.UI\bin\Debug\mgroup.png"));
            //this.KeyDown += Window_KeyDown;
            //DataContext = this;
        }

        private void Window_Loaded()
        {
            // Give the camera its initial position.
            TheCamera = new PerspectiveCamera();
            TheCamera.FieldOfView = 60;
            ViewportGraphics.Camera = TheCamera;
            PositionCamera();

            // Define lights.
            DefineLights();

            // Create the model.
            DefineModel(MainModel3Dgroup);

            // Add the group of models to a ModelVisual3D.
            ModelVisual3D model_visual = new ModelVisual3D();
            model_visual.Content = MainModel3Dgroup;

            // Add the main visual to the viewportt.
            ViewportGraphics.Children.Add(model_visual);
        }

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

        private void DefineLights()
        {
            AmbientLight ambient_light = new AmbientLight(Colors.Gray);
            DirectionalLight directional_light =
                new DirectionalLight(Colors.Gray, new Vector3D(-1.0, -3.0, -2.0));
            MainModel3Dgroup.Children.Add(ambient_light);
            MainModel3Dgroup.Children.Add(directional_light);
        }

        private void DefineModel(Model3DGroup model_group)
        {
            // Make a mesh to hold the surface.
            MeshGeometry3D mesh = new MeshGeometry3D();

            // Make the surface's points and triangles.
#if SURFACE2
            const double xmin = -1.5;
            const double xmax = 1.5;
            const double dx = 0.05;
            const double zmin = -1.5;
            const double zmax = 1.5;
            const double dz = 0.05;
#else
            const double xmin = -5;
            const double xmax = 5;
            const double dx = 0.5;
            const double zmin = -5;
            const double zmax = 5;
            const double dz = 0.5;
#endif
            for (double x = xmin; x <= xmax - dx; x += dx)
            {
                for (double z = zmin; z <= zmax - dz; z += dx)
                {
                    // Make points at the corners of the surface
                    // over (x, z) - (x + dx, z + dz).
                    Point3D p00 = new Point3D(x, F(x, z), z);
                    Point3D p10 = new Point3D(x + dx, F(x + dx, z), z);
                    Point3D p01 = new Point3D(x, F(x, z + dz), z + dz);
                    Point3D p11 = new Point3D(x + dx, F(x + dx, z + dz), z + dz);

                    // Add the triangles.
                    AddTriangle(mesh, p00, p01, p11);
                    AddTriangle(mesh, p00, p11, p10);
                }
            }
            Console.WriteLine(mesh.Positions.Count + " points");
            Console.WriteLine(mesh.TriangleIndices.Count / 3 + " triangles");

            // Make the surface's material using a solid orange brush..
            DiffuseMaterial surface_material = new DiffuseMaterial(Brushes.BlueViolet);

            // Make the mesh's model.
            GeometryModel3D surface_model = new GeometryModel3D(mesh, surface_material);

            // Make the surface visible from both sides.
            surface_model.BackMaterial = surface_material;

            // Add the model to the model groups.
            model_group.Children.Add(surface_model);
        }

        private void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            // Get the points' indices.
            int index1 = AddPoint(mesh.Positions, point1);
            int index2 = AddPoint(mesh.Positions, point2);
            int index3 = AddPoint(mesh.Positions, point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index2);
            mesh.TriangleIndices.Add(index3);
        }

        // Create the point and return its new index.
        private int AddPoint(Point3DCollection points, Point3D point)
        {
            // Create the point and return its index.
            points.Add(point);
            return points.Count - 1;
        }

        // Adjust the camera's position.

        private void MoveCameraLeft(object sender, EventArgs e)
        {
            CameraTheta += CameraDTheta;
            PositionCamera();
        }

        private void MoveCameraRight(object sender, EventArgs e)
        {
            CameraTheta -= CameraDTheta;
            PositionCamera();
        }

        private void MoveCameraUp(object sender, EventArgs e)
        {
            CameraPhi += CameraDPhi;
            if (CameraPhi > Math.PI / 2.0) CameraPhi = Math.PI / 2.0;
            PositionCamera();
        }

        private void MoveCameraDown(object sender, EventArgs e)
        {
            CameraPhi -= CameraDPhi;
            if (CameraPhi < -Math.PI / 2.0) CameraPhi = -Math.PI / 2.0;
            PositionCamera();
        }

        private void ZoomInCamera(object sender, EventArgs e)
        {
            CameraR -= CameraDR;
            if (CameraR < CameraDR) CameraR = CameraDR;
            PositionCamera();
        }

        private void ZoomOutCamera(object sender, EventArgs e)
        {
            CameraR += CameraDR;
            PositionCamera();
        }

        // The function that defines the surface we are drawing.
        private double F(double x, double z)
        {
#if SURFACE2
            const double two_pi = 2 * 3.14159265;
            double r2 = x * x + z * z;
            double r = Math.Sqrt(r2);
            double theta = Math.Atan2(z, x);
            return Math.Exp(-r2) * Math.Sin(two_pi * r) * Math.Cos(3 * theta);
#else
            double r2 = x * x + z * z;
            return 8 * Math.Cos(r2 / 2) / (2 + r2);
#endif
        }
       

        private void PositionCamera()
        {
            // Calculate the camera's position in Cartesian coordinates.
            double y = CameraR * Math.Sin(CameraPhi);
            double hyp = CameraR * Math.Cos(CameraPhi);
            double x = hyp * Math.Cos(CameraTheta);
            double z = hyp * Math.Sin(CameraTheta);
            TheCamera.Position = new Point3D(x, y, z);

            // Look toward the origin.
            TheCamera.LookDirection = new Vector3D(-x, -y, -z);

            // Set the Up direction.
            TheCamera.UpDirection = new Vector3D(0, 1, 0);

            // Console.WriteLine("Camera.Position: (" + x + ", " + y + ", " + z + ")");
        }

        private void Button_Plot_OBJ(object sender, RoutedEventArgs e)
        {
            PlotOBJMesh plotMesh = new PlotOBJMesh();
            plotMesh.elementsConnectivity = elementsConnectivity;
            plotMesh.nodes = nodes;
            ViewportGraphics.Children.Clear();
            ViewportGraphics.InvalidateVisual();

            ViewportGraphics.Children.Add(plotMesh.GetModel());
        }

        private void Button_Run_Simulation(object sender, RoutedEventArgs e)
        {
            Task task1 = new Task(() => Test());
            task1.Start();
            DataContext = this;
            //task1.Wait();
            
            
        }

        private void Test()
        {
            RandomChartValues values = new RandomChartValues(100, 10, 1e-4);
            values.IterationCompleted += WriteValuesOnLogTool;
            values.RunRandomConvergenceValuesGenerator();
        }

        public void WriteValuesOnLogTool(object sender, ConvergenceValues e)
        { 
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                LogTool.Text = "Load Step " + e.LoadStep + "-Iteration " + e.Iteration + " : Convergence State: " + e.ConvergenceResult + " with residual " + e.ResidualNorm +" kTemp  "+ kTemp;
                if (e.Iteration==0)
                {
                    //ChartValues.Clear();
                }
                ChartValues.Add(
                    new ConvergenceValues()
                    {
                        Iteration = kTemp,//e.Iteration,
                        ResidualNorm = e.ResidualNorm,
                    });
            }));
            SetAxisLimits(kTemp + 1);
            kTemp = kTemp + 1;
            //SetAxisLimits(e.Iteration+1);

            if (ChartValues.Count > 50) ChartValues.RemoveAt(0);
            //LogTool.Text = e.ConvergenceResult.ToString();
        }

        private void SetAxisLimits(int now)
        {
            AxisMax = now+2;
            AxisMin = now-10;
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private void ImportOBJFile(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog1 = new OpenFileDialog();
                if (dialog1.ShowDialog() == true)
                {
                    string selectedFilePath = dialog1.FileName;
                    List<string> allLines = new List<string>(File.ReadAllLines(selectedFilePath));

                    //List<string> lines = new List<string>(file.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                    allLines.RemoveRange(0, 4);
                    int nodeIndex = 0;
                    int connectivityIndex = 0;
                    foreach (var line in allLines)
                    {
                        // in case of first line ...
                        string separator = " ";
                        string[] fields = line.Split(separator.ToCharArray()); //(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                        if (fields[0] == "v")
                        {
                            nodeIndex = nodeIndex + 1;
                            var node = new GraphicalNode(double.Parse(fields[1], CultureInfo.InvariantCulture), double.Parse(fields[2], CultureInfo.InvariantCulture), double.Parse(fields[3], CultureInfo.InvariantCulture));
                            nodes[nodeIndex] = node;
                        }
                        else if (fields[0] == "f")
                        {
                            connectivityIndex = connectivityIndex + 1;
                            string separatorForNode = "/";
                            int[] elementNodes = new int[4];
                            for (int i = 0; i < 4; i++)
                            {
                                string[] fieldsForNode = fields[i + 1].Split(separatorForNode.ToCharArray());
                                elementNodes[i] = Int16.Parse(fieldsForNode[0]);
                            }
                            elementsConnectivity[connectivityIndex] = new Dictionary<int, int>() { { 1, elementNodes[0] }, { 2, elementNodes[1] }, { 3, elementNodes[2] }, { 4, elementNodes[3] } };
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void PlotResultsClick(object sender, RoutedEventArgs e)
        {
            SeriesCollection SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 4, 6, 5, 2 ,7 }
                },
                new LineSeries
                {
                    Title = "Series 2",
                    Values = new ChartValues<double> { 6, 7, 3, 4 ,6 }
                }
            };

            //Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
            //YFormatter = value => value.ToString("C");

            //modifying the series collection will animate and update the chart
            SeriesCollection.Add(new LineSeries
            {
                Values = new ChartValues<double> { 5, 3, 2, 4 },
                LineSmoothness = 0 //straight lines, 1 really smooth lines
            });

            //modifying any series values will also animate and update the chart
            SeriesCollection[2].Values.Add(5d);

            DispChart.Series = SeriesCollection;
        }

        private void Import_GFEC_Results(object sender, RoutedEventArgs e)
        {
            mesh = new GFECMesh();
            string result = mesh.ReadData();
			LogTool.Text = "Importing operation completed!";
		}

        private void ExportParaviewModel(object sender, RoutedEventArgs e)
        {
            mesh.ExportParaviewXML("C:/Users/Public/Documents/TEST/paraviewData.vtu");
            LogTool.Text = "Operation completed!";
        }

        private void ExportMeshToCSV(object sender, RoutedEventArgs e)
        {
            var importedMeshFromAnsys = new AnsysMesh();
            importedMeshFromAnsys.ImportMesh(@"C:/Users/Public/Documents/AnsysMesh");

            Mesh newMesh = new Mesh();
            newMesh.Nodes = importedMeshFromAnsys.Nodes;
            newMesh.Elements = importedMeshFromAnsys.Elements;
            Mesh offsetMesh = newMesh.CreateNewOffsetMesh(0.005);
            offsetMesh.Elements = newMesh.Elements;
            List<string> dispVector = new List<string>();
            for (int i = 0; i < newMesh.Nodes.Length*2; i++)
            {
                dispVector.Add("0 0 0\n");
            }

            Mesh mergedMesh = new Mesh();
            mergedMesh.CreateMergedMesh(newMesh, offsetMesh);
            //ParaviewModel exportableModel = new ParaviewModel(offsetMesh, dispVector);
            //exportableModel.ExportParaviewXML(@"C:\Users\Public\Documents\ExportedToParaview\", "geometryAnsys.vtu");
            //ParaviewModel exportableModel2 = new ParaviewModel(newMesh, dispVector);
            //exportableModel2.ExportParaviewXML(@"C:\Users\Public\Documents\ExportedToParaview\", "geometryAnsys2.vtu");



            //IGraphicalNode[] nodes = new IGraphicalNode[8];
            //IGraphicalNode node1 = new GraphicalNode(0.0, 1.0, 0.0);
            //IGraphicalNode node2 = new GraphicalNode(1.0, 0.0, 0.0);
            //IGraphicalNode node3 = new GraphicalNode(2.0, 0.0, 0.0);
            //IGraphicalNode node4 = new GraphicalNode(3.0, 1.0, 0.0);

            //IGraphicalNode node5 = new GraphicalNode(0.0, 1.0, 1.0);
            //IGraphicalNode node6 = new GraphicalNode(1.0, 0.0, 1.0);
            //IGraphicalNode node7 = new GraphicalNode(2.0, 0.0, 1.0);
            //IGraphicalNode node8 = new GraphicalNode(3.0, 1.0, 1.0);

            //nodes[0] = node1;
            //nodes[1] = node2;
            //nodes[2] = node3;
            //nodes[3] = node4;

            //nodes[4] = node5;
            //nodes[5] = node6;
            //nodes[6] = node7;
            //nodes[7] = node8;

            //nodes[0].GlobalIndex = 0;
            //nodes[1].GlobalIndex = 1;
            //nodes[2].GlobalIndex = 2;
            //nodes[3].GlobalIndex = 3;

            //nodes[4].GlobalIndex = 4;
            //nodes[5].GlobalIndex = 5;
            //nodes[6].GlobalIndex = 6;
            //nodes[7].GlobalIndex = 7;

            //IGraphicalElement[] elements = new IGraphicalElement[3];
            //elements[0] = new QuadElement(nodes[0], nodes[1], nodes[5], nodes[4]);
            //elements[1] = new QuadElement(nodes[1], nodes[2], nodes[6], nodes[5]);
            //elements[2] = new QuadElement(nodes[2], nodes[3], nodes[7], nodes[6]);

            //Mesh newMesh = new Mesh();
            //newMesh.Nodes = nodes;
            //newMesh.Elements = elements;

            //Mesh offsetMesh = newMesh.CreateNewOffsetMesh(1.0);
            //offsetMesh.Elements = newMesh.Elements;

            //Mesh mergedMesh = new Mesh();
            //mergedMesh.CreateMergedMesh(newMesh, offsetMesh);

            //List<string> dispVector = new List<string>();
            //for (int i = 0; i < mergedMesh.Nodes.Length; i++)
            //{
            //    dispVector.Add("0 0 0\n");
            //}
            ParaviewModel exportableModel = new ParaviewModel(mergedMesh, dispVector);
            exportableModel.ExportParaviewXML(@"C:\Users\Public\Documents\ExportedToParaview\", "exportedSimpleExample.vtu");

            MSolveModel exportedMsolveModel = new MSolveModel(mergedMesh.Nodes, mergedMesh.Elements);
            exportedMsolveModel.ExportToTXT(@"C:\Users\Public\Documents\ExportedToParaview\", "exportedForMsolve");
        }

		private void Button_Convert(object sender, RoutedEventArgs e)
		{
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.ShowDialog();
            List<string> files = new List<string>(fileDialog.FileNames);
            var folderPath = System.IO.Path.GetDirectoryName(files[0]);
            var dir = folderPath + @"\Converted";

			if (!Directory.Exists(dir)) // if it doesn't exist, create
				Directory.CreateDirectory(dir);

			foreach (string s in files)
            {
				var fileName = System.IO.Path.GetFileName(s);
				string[] importedFile = File.ReadAllLines(s);
                List<string> convertedFile = new List<string>(importedFile[0].Split(new char[] { ' ' }));
                convertedFile.RemoveAll(string.IsNullOrEmpty);
                File.WriteAllLines(System.IO.Path.Combine(dir, fileName), convertedFile);
			}
            LogTool.Text = "Conversion operation completed!";
        }

		private void ImportAnsysClick(object sender, RoutedEventArgs e)
		{
			var importedMeshFromAnsys = new AnsysMesh();
			importedMeshFromAnsys.ImportMesh(@"C:\Users\vasil\Downloads\AnsysNewResultsForVideo");
			importedMeshFromAnsys.ExportParaviewXML("C:/Users/Public/Documents/AnsysVideo/paraviewData.vtu");
			LogTool.Text = "Operation completed!";
		}
    }

    
}
