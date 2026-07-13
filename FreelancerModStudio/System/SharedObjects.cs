using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixEngine;
using HelixEngine.Meshes;

namespace FreelancerModStudio.SystemPresenter
{
    public static class SharedMaterials
    {
        public static readonly Material Zone = CreateZoneMaterial(Color.FromRgb(30, 30, 30));
        public static readonly Material ZoneExclusion = MaterialHelper.CreateEmissiveMaterial(Color.FromRgb(30, 15, 0));
        public static readonly Material CmpModel = MaterialHelper.CreateMaterial(Brushes.SlateGray);

        public static readonly Color ConnectionJumpHole = Colors.Orange;
        public static readonly Color ConnectionJumpGate = Colors.SlateGray;
        public static readonly Color ConnectionJumpGateAndHole = Colors.Snow;
        public static readonly Color ConnectionNone = Colors.Black;

        public static readonly Color Selection = Colors.Yellow;
        public static readonly Color TrackedLine = Colors.LightYellow;
        public static readonly Color ManipulatorX = Colors.OrangeRed;
        public static readonly Color ManipulatorY = Colors.LawnGreen;
        public static readonly Color ManipulatorZ = Color.FromRgb(0, 120, 255);

        public static Material CreateZoneMaterial(Color color)
        {
            return MaterialHelper.CreateEmissiveMaterial(Color.FromArgb(255, color.R, color.G, color.B));
        }

        public static Material CreateNebulaZoneMaterial(Color color)
        {
            return MaterialHelper.CreateEmissiveMaterial(Color.FromArgb(Helper.Settings.Data.Data.General.OverlayOpacity, color.R, color.G, color.B));
        }
    }

    public static class SharedMeshes
    {
        public static readonly MeshGeometry3D Sphere = GetMesh(new SphereMesh
            {
                Slices = 18,
                Stacks = 14
            });

        public static readonly MeshGeometry3D Box = GetMesh(new BoxMesh());

        public static readonly MeshGeometry3D Cylinder = GetMesh(new CylinderMesh
            {
                Slices = 18
            });

        public static readonly MeshGeometry3D Pyramid = GetMesh(new PyramidMesh());

        public static readonly MeshGeometry3D Octahedron = GetMesh(new OctahedronMesh());

        public static readonly MeshGeometry3D Surface = GetMesh(new SurfaceMesh());

        public static readonly MeshGeometry3D SurfaceCylinder = GetMesh(new SurfaceMesh
            {
                Width = 2
            });

        static MeshGeometry3D GetMesh(MeshGeneratorBase mesh)
        {
            MeshGeometry3D geometry = mesh.Geometry;
            geometry.Freeze();
            return geometry;
        }
    }

    public static class SharedGeometries
    {
        public static readonly GeometryModel3D System =
            GetGeometry(SharedMeshes.Sphere, MaterialHelper.CreateMaterial(Brushes.LightYellow));

        public static readonly GeometryModel3D LightSource =
            GetGeometry(SharedMeshes.Octahedron, MaterialHelper.CreateEmissiveMaterial(Color.FromRgb(120, 120, 0)));

        public static GeometryModel3D Construct =
            GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(Brushes.Fuchsia));

        public static GeometryModel3D Depot =
            GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(Brushes.SlateGray));

        public static GeometryModel3D DockingRing =
            GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(Brushes.DimGray));

        public static GeometryModel3D JumpGate =
            GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(Brushes.Green));

        public static GeometryModel3D JumpHole =
            GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(Brushes.Firebrick));

        public static GeometryModel3D Planet =
            GetGeometry(SharedMeshes.Sphere, MaterialHelper.CreateMaterial(Color.FromRgb(0, 60, 120)));

        public static GeometryModel3D Satellite =
            GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(Brushes.BlueViolet));

        public static GeometryModel3D Ship =
            GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(Brushes.Gold));

        public static GeometryModel3D Station =
            GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(Brushes.OrangeRed));

        public static GeometryModel3D Sun =
            GetGeometry(SharedMeshes.Sphere, MaterialHelper.CreateMaterial(Brushes.Orange));

        public static GeometryModel3D TradeLane =
            GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(Brushes.Cyan));

        public static GeometryModel3D WeaponsPlatform =
            GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(Brushes.BurlyWood));

        public static readonly GeometryModel3D ZoneSphereOrEllipsoid =
            GetGeometry(SharedMeshes.Sphere, SharedMaterials.Zone);

        public static readonly GeometryModel3D ZoneBox =
            GetGeometry(SharedMeshes.Box, SharedMaterials.Zone);

        public static readonly GeometryModel3D ZoneCylinderOrRing =
            GetGeometry(SharedMeshes.Cylinder, SharedMaterials.Zone);

        public static readonly GeometryModel3D ZoneSphereOrEllipsoidExclusion =
            GetGeometry(SharedMeshes.Sphere, SharedMaterials.ZoneExclusion);

        public static readonly GeometryModel3D ZoneBoxExclusion =
            GetGeometry(SharedMeshes.Box, SharedMaterials.ZoneExclusion);

        public static readonly GeometryModel3D ZoneCylinderExclusion =
            GetGeometry(SharedMeshes.Cylinder, SharedMaterials.ZoneExclusion);

        public static GeometryModel3D ZoneVignette =
            GetGeometry(SharedMeshes.Sphere, MaterialHelper.CreateEmissiveMaterial(Color.FromRgb(0, 30, 15)));

        public static readonly GeometryModel3D ZonePath =
            GetGeometry(SharedMeshes.SurfaceCylinder, SharedMaterials.Zone);

        public static GeometryModel3D ZonePathTrade =
            GetGeometry(SharedMeshes.SurfaceCylinder, MaterialHelper.CreateEmissiveMaterial(Color.FromRgb(30, 0, 30)));

        public static GeometryModel3D ZonePathTradeLane =
            GetGeometry(SharedMeshes.Surface, MaterialHelper.CreateEmissiveMaterial(Color.FromRgb(0, 30, 30)));

        public static GeometryModel3D GetGeometry(Geometry3D geometry, Material material)
        {
            GeometryModel3D model = new GeometryModel3D(geometry, material);
            model.Freeze();
            return model;
        }

        public static GeometryModel3D GetNebulaZoneGeometry(Geometry3D geometry, Color fogColor)
        {
            return GetGeometry(geometry, SharedMaterials.CreateNebulaZoneMaterial(fogColor));
        }

        public static void LoadColors(FreelancerModStudio.Data.Settings.ColorBox colors)
        {
            Construct = GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(ToMediaColor(colors.Construct)));
            Depot = GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(ToMediaColor(colors.Depot)));
            DockingRing = GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(ToMediaColor(colors.DockingRing)));
            JumpGate = GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(ToMediaColor(colors.JumpGate)));
            JumpHole = GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(ToMediaColor(colors.JumpHole)));
            Planet = GetGeometry(SharedMeshes.Sphere, MaterialHelper.CreateMaterial(ToMediaColor(colors.Planet)));
            Satellite = GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(ToMediaColor(colors.Satellite)));
            Ship = GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(ToMediaColor(colors.Ship)));
            Station = GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(ToMediaColor(colors.Station)));
            Sun = GetGeometry(SharedMeshes.Sphere, MaterialHelper.CreateMaterial(ToMediaColor(colors.Sun)));
            TradeLane = GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(ToMediaColor(colors.TradeLane)));
            WeaponsPlatform = GetGeometry(SharedMeshes.Pyramid, MaterialHelper.CreateMaterial(ToMediaColor(colors.WeaponsPlatform)));
            byte opacity = Helper.Settings.Data.Data.General.OverlayOpacity;
            ZoneVignette = GetGeometry(SharedMeshes.Sphere, CreateOverlayMaterial(colors.ZoneVignette, opacity));
            ZonePathTrade = GetGeometry(SharedMeshes.SurfaceCylinder, CreateOverlayMaterial(colors.ZonePathTrade, opacity));
            ZonePathTradeLane = GetGeometry(SharedMeshes.Surface, CreateOverlayMaterial(colors.ZonePathTradeLane, opacity));
        }

        static Material CreateOverlayMaterial(System.Drawing.Color color, byte opacity)
        {
            Color mediaColor = ToMediaColor(color);
            return MaterialHelper.CreateEmissiveMaterial(Color.FromArgb(opacity, mediaColor.R, mediaColor.G, mediaColor.B));
        }

        static Color ToMediaColor(System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
