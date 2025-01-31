using GeoJsonRandom.Core.Models;
using GeoJsonRandom.Core.Services;
using Moq;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace TestProject1
{
    public class UnitTest_GeoJsonPrefixTreeBuilder
    {
        private readonly GeoJsonPrefixTreeBuilder _geoJsonPrefixTreeBuilder;
        private readonly Mock<IGeoJsonLoader> _mockGeoJsonLoader;
        private readonly GeometryFactory _geometryFactory;

        public UnitTest_GeoJsonPrefixTreeBuilder()
        {
            _mockGeoJsonLoader = new Mock<IGeoJsonLoader>();
            _geoJsonPrefixTreeBuilder = new GeoJsonPrefixTreeBuilder(_mockGeoJsonLoader.Object);
            _geometryFactory = new GeometryFactory();
        }

        private Geometry CreateRectangle(double p1X, double p1Y, double p2X, double p2Y)
        {
            Coordinate[] coordinates =
            [
                new Coordinate(p1X, p1Y),
                new Coordinate(p2X, p1Y),
                new Coordinate(p2X, p2Y),
                new Coordinate(p1X, p2Y),
                new Coordinate(p1X, p1Y)
            ];
            var shell = _geometryFactory.CreateLinearRing(coordinates);
            return _geometryFactory.CreatePolygon(shell);
        }

        private IAttributesTable CreateAttr(string county, string town, string city)
        {
            var at = new AttributesTable
            {
                { "COUNTYNAME", county },
                { "TOWNNAME", town },
                { "VILLNAME", city }
            };
            return at;
        }

        [SetUp]
        public void Setup()
        {
            var f1 = new Feature { Attributes = CreateAttr("CountyA", "TownA1", "VillageA11") };
            FeatureCollection fc =
            [
                new Feature { Attributes = CreateAttr("CountyA", "TownA1", "VillageA11"), Geometry = CreateRectangle(0d, 0d, 1d, 1d) },
                new Feature { Attributes = CreateAttr("CountyA", "TownA1", "VillageA12"), Geometry = CreateRectangle(2d, 2d, 3d, 3d) },
                new Feature { Attributes = CreateAttr("CountyA", "TownA2", "VillageA21"), Geometry = CreateRectangle(3d, 3d, 4d, 5d) },
                new Feature { Attributes = CreateAttr("CountyB", "TownB1", "VillageB11"), Geometry = CreateRectangle(0d, 0d, -1d, -1d) },
                new Feature { Attributes = CreateAttr("CountyB", "TownB1", "VillageB12"), Geometry = CreateRectangle(-2d, -2d, -3d, -3d) }
            ];
            _mockGeoJsonLoader.Setup(x => x.Load()).Returns(fc);
        }

        [Test]
        public void Test_Root()
        {
            // Arrange

            // Act
            GeoTreeNode root = _geoJsonPrefixTreeBuilder.GetOrBuildTree();

            // Assert
            Assert.That(root.IsLeaf == false, Is.True);
            Assert.That(root.Children.Count, Is.EqualTo(2));
            Assert.That(root.Name, Is.EqualTo("Root"));
            Assert.That(root.Geo, Is.Null);
        }

        [Test]
        public void Test_CountyA()
        {
            // Arrange

            // Act
            GeoTreeNode root = _geoJsonPrefixTreeBuilder.GetOrBuildTree();
            var countyA = root.Children["CountyA"];

            // Assert
            Assert.That(countyA.IsLeaf == false, Is.True);
            Assert.That(countyA.Children.Count, Is.EqualTo(2));
            Assert.That(countyA.Name, Is.EqualTo("CountyA"));
            Assert.That(countyA.Geo, Is.Null);
        }

        [Test]
        public void Test_CountyB()
        {
            // Arrange

            // Act
            GeoTreeNode root = _geoJsonPrefixTreeBuilder.GetOrBuildTree();
            var countyB = root.Children["CountyB"];

            // Assert
            Assert.That(countyB.IsLeaf == false, Is.True);
            Assert.That(countyB.Children.Count, Is.EqualTo(1));
            Assert.That(countyB.Name, Is.EqualTo("CountyB"));
            Assert.That(countyB.Geo, Is.Null);
        }

        [Test]
        public void Test_TownA1()
        {   
            // Arrange

            // Act
            GeoTreeNode root = _geoJsonPrefixTreeBuilder.GetOrBuildTree();
            var countyA = root.Children["CountyA"];
            var townA1 = countyA.Children["TownA1"];

            // Assert
            Assert.That(townA1.IsLeaf == false, Is.True);
            Assert.That(townA1.Children.Count, Is.EqualTo(2));
            Assert.That(townA1.Name, Is.EqualTo("TownA1"));
            Assert.That(townA1.Geo, Is.Null);
        }

        [Test]
        public void Test_VillageA11()
        {
            // Arrange

            // Act
            GeoTreeNode root = _geoJsonPrefixTreeBuilder.GetOrBuildTree();
            var countyA = root.Children["CountyA"];
            var townA1 = countyA.Children["TownA1"];
            var villageA11 = townA1.Children["VillageA11"];

            // Assert
            Assert.That(villageA11.IsLeaf == true, Is.True);
            Assert.That(villageA11.Children.Count, Is.EqualTo(0));
            Assert.That(villageA11.Name, Is.EqualTo("VillageA11"));
            Assert.That(villageA11.Geo, Is.Not.Null);
        }
    }
}