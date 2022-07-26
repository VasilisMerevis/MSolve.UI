using LiveCharts;
using LiveCharts.Configurations;
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
        public ChartValues<ConvergenceValues> ChartValues { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }

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
        public MainWindow()
        {
            InitializeComponent();
            InitializeComponent();
            ConvergenceResults();
            Window_Loaded();
            gnuplotImage.Source = new BitmapImage(new Uri(@"C:\Users\Vasilis\source\repos\MSolve.UI\MSolve.UI\bin\Debug\mgroup.png"));
            //this.KeyDown += Window_KeyDown;
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

            // Make the surface's material using a solid orange brush.
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
    }
}
