using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Microsoft.Win32;

namespace MSolve.UI
{
    public class PlotOBJMesh
    {
        public Viewport3D MainViewport = new Viewport3D();
        public ModelVisual3D finalModel = new ModelVisual3D();

        public Dictionary<int, IGraphicalNode> nodes = new Dictionary<int, IGraphicalNode>();
        public Dictionary<int, Dictionary<int, int>> elementsConnectivity = new Dictionary<int, Dictionary<int, int>>();

        // The main object model group.
        private Model3DGroup MainModel3Dgroup = new Model3DGroup();

        // The camera.
        private PerspectiveCamera TheCamera;

        // The camera's current location.


        private double CameraPhi = 0; //Math.PI / 6.0;       // 30 degrees
        private double CameraTheta = 0;// Math.PI / 6.0;     // 30 degrees
#if SURFACE2
        private double CameraR = 3.0;
#else
        private double CameraR = 300.0;

#endif

        // The change in CameraPhi when you press the up and down arrows.
        private const double CameraDPhi = 0.1 * 5;

        // The change in CameraTheta when you press the left and right arrows.
        private const double CameraDTheta = 0.1 * 5;

        // The change in CameraR when you press + or -.

        private const double CameraDR = 0.1 * 100;


        // Create the scene.
        // MainViewport is the Viewport3D defined
        // in the XAML code that displays everything.
        public void Window_Loaded()
        {
            // Give the camera its initial position.
            TheCamera = new PerspectiveCamera();
            TheCamera.FieldOfView = 60;
            MainViewport.Camera = TheCamera;
            PositionCamera();

            // Define lights.
            DefineLights();

            // Create the model.
            DefineModel(MainModel3Dgroup);

            // Add the group of models to a ModelVisual3D.
            ModelVisual3D model_visual = new ModelVisual3D();
            model_visual.Content = MainModel3Dgroup;
            finalModel = model_visual;
            // Add the main visual to the viewportt.
            MainViewport.Children.Add(model_visual);
        }


        public ModelVisual3D GetModel()
        {
            // Give the camera its initial position.
            TheCamera = new PerspectiveCamera();
            TheCamera.FieldOfView = 60;
            MainViewport.Camera = TheCamera;
            PositionCamera();

            // Define lights.
            DefineLights();

            // Create the model.
            DefineModel(MainModel3Dgroup);

            // Add the group of models to a ModelVisual3D.
            ModelVisual3D model_visual = new ModelVisual3D();
            model_visual.Content = MainModel3Dgroup;
            return model_visual;
        }


        //Define the lights.
        private void DefineLights()
        {
            AmbientLight ambient_light = new AmbientLight(Colors.Gray);
            DirectionalLight directional_light =
                new DirectionalLight(Colors.Gray, new Vector3D(-1.0, -3.0, -2.0));
            MainModel3Dgroup.Children.Add(ambient_light);
            MainModel3Dgroup.Children.Add(directional_light);
        }

        // Add the model to the Model3DGroup.
        private void DefineModel(Model3DGroup model_group)
        {
            // Make a mesh to hold the surface.
            MeshGeometry3D mesh = new MeshGeometry3D();


            List<double> nodesX = new List<double>();
            List<double> nodesY = new List<double>();
            List<double> nodesZ = new List<double>();
            foreach (var node in nodes)
            {
                nodesX.Add(node.Value.XCoordinate);
                nodesY.Add(node.Value.YCoordinate);
                nodesZ.Add(node.Value.ZCoordinate);
            }
            double minX = nodesX.Min();
            double maxX = nodesX.Max();
            double centerX = (maxX + minX) / 2.0;
            double minY = nodesY.Min();
            double maxY = nodesY.Max();
            double centerY = (maxY + minY) / 2.0;
            double minZ = nodesZ.Min();
            double maxZ = nodesZ.Max();
            double centerZ = (maxZ + minZ) / 2.0;

            foreach (var element in elementsConnectivity)
            {
                int globalPointIndex1 = element.Value[1];
                int globalPointIndex2 = element.Value[2];
                int globalPointIndex3 = element.Value[3];
                int globalPointIndex4 = element.Value[4];

                double localPoint1X = nodes[globalPointIndex1].XCoordinate;
                double localPoint1Y = nodes[globalPointIndex1].YCoordinate;
                double localPoint1Z = nodes[globalPointIndex1].ZCoordinate;

                double localPoint2X = nodes[globalPointIndex2].XCoordinate;
                double localPoint2Y = nodes[globalPointIndex2].YCoordinate;
                double localPoint2Z = nodes[globalPointIndex2].ZCoordinate;

                double localPoint3X = nodes[globalPointIndex3].XCoordinate;
                double localPoint3Y = nodes[globalPointIndex3].YCoordinate;
                double localPoint3Z = nodes[globalPointIndex3].ZCoordinate;

                double localPoint4X = nodes[globalPointIndex4].XCoordinate;
                double localPoint4Y = nodes[globalPointIndex4].YCoordinate;
                double localPoint4Z = nodes[globalPointIndex4].ZCoordinate;

                Point3D point1 = new Point3D(localPoint1X - centerX, localPoint1Y - centerY, localPoint1Z - centerZ);
                Point3D point2 = new Point3D(localPoint2X - centerX, localPoint2Y - centerY, localPoint2Z - centerZ);
                Point3D point3 = new Point3D(localPoint3X - centerX, localPoint3Y - centerY, localPoint3Z - centerZ);
                Point3D point4 = new Point3D(localPoint4X - centerX, localPoint4Y - centerY, localPoint4Z - centerZ);
                AddTriangle(mesh, point1, point2, point3);
                AddTriangle(mesh, point1, point3, point4);
            }
            //Point3D point1 = new Point3D(89-70, -215+200, 34);
            //Point3D point2 = new Point3D(99-70, -200+200, 25);
            //Point3D point3 = new Point3D(79-70, -180+200, 15);
            //AddTriangle(mesh, point1, point2, point3);
            //            // Make the surface's points and triangles.
            //#if SURFACE2
            //            const double xmin = -1.5;
            //            const double xmax = 1.5;
            //            const double dx = 0.05;
            //            const double zmin = -1.5;
            //            const double zmax = 1.5;
            //            const double dz = 0.05;
            //#else
            //            const double xmin = -5;
            //            const double xmax = 5;
            //            const double dx = 0.5;
            //            const double zmin = -5;
            //            const double zmax = 5;
            //            const double dz = 0.5;
            //#endif
            //            for (double x = xmin; x <= xmax - dx; x += dx)
            //            {
            //                for (double z = zmin; z <= zmax - dz; z += dx)
            //                {
            //                    // Make points at the corners of the surface
            //                    // over (x, z) - (x + dx, z + dz).
            //                    Point3D p00 = new Point3D(x, F(x, z), z);
            //                    Point3D p10 = new Point3D(x + dx, F(x + dx, z), z);
            //                    Point3D p01 = new Point3D(x, F(x, z + dz), z + dz);
            //                    Point3D p11 = new Point3D(x + dx, F(x + dx, z + dz), z + dz);

            //                    // Add the triangles.
            //                    AddTriangle(mesh, p00, p01, p11);
            //                    AddTriangle(mesh, p00, p11, p10);
            //                }
            //            }

            Console.WriteLine(mesh.Positions.Count + " points");
            Console.WriteLine(mesh.TriangleIndices.Count / 3 + " triangles");

            // Make the surface's material using a solid orange brush.
            DiffuseMaterial surface_material = new DiffuseMaterial(Brushes.Orange);

            // Make the mesh's model.
            GeometryModel3D surface_model = new GeometryModel3D(mesh, surface_material);

            // Make the surface visible from both sides.
            surface_model.BackMaterial = surface_material;

            // Add the model to the model groups.
            model_group.Children.Add(surface_model);
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

        // Add a triangle to the indicated mesh.
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
        public void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    CameraPhi += CameraDPhi;
                    if (CameraPhi > Math.PI / 2.0) CameraPhi = Math.PI / 2.0;
                    break;
                case Key.Down:
                    CameraPhi -= CameraDPhi;
                    if (CameraPhi < -Math.PI / 2.0) CameraPhi = -Math.PI / 2.0;
                    break;
                case Key.Left:
                    CameraTheta += CameraDTheta;
                    break;
                case Key.Right:
                    CameraTheta -= CameraDTheta;
                    break;
                case Key.Add:
                case Key.OemPlus:
                    CameraR -= CameraDR;
                    if (CameraR < CameraDR) CameraR = CameraDR;
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    CameraR += CameraDR;
                    break;
            }

            // Update the camera's position.
            PositionCamera();
        }

        // Position the camera.
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



    }

}


