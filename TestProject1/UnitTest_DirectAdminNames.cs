using GeoJsonRandom.Core.Models;
using GeoJsonRandom.Core.Services;
using Moq;

namespace TestProject1
{
    public class UnitTest_DirectAdminNames
    {
        private readonly Mock<IGeoJsonPrefixTreeBuilder> _mockGeoJsonPrefixTreeBuilder;
        private readonly GeoJsonRandomPointGenerator _geoJsonRandomPointGenerator;

        public UnitTest_DirectAdminNames()
        {
            _mockGeoJsonPrefixTreeBuilder = new Mock<IGeoJsonPrefixTreeBuilder>();
            _geoJsonRandomPointGenerator = new GeoJsonRandomPointGenerator(_mockGeoJsonPrefixTreeBuilder.Object);
        }

        [SetUp]
        public void Setup()
        {
            var root = new GeoTreeNode();
            var ca = new GeoTreeNode { Name = "CountyA", County = "CountyA" };
            var cb = new GeoTreeNode { Name = "CountyB", County = "CountyB" };
            var ta1 = new GeoTreeNode { Name = "TownA1", County = "CountyA", Town = "TownA1" };
            var ta2 = new GeoTreeNode { Name = "TownA2", County = "CountyA", Town = "TownA2" };
            var tb1 = new GeoTreeNode { Name = "TownB1", County = "CountyB", Town = "TownB1" };
            var va11 = new GeoTreeNode { Name = "VillageA11", County = "CountyA", Town = "TownA1", Village = "VillageA11" };
            var va12 = new GeoTreeNode { Name = "VillageA12", County = "CountyA", Town = "TownA1", Village = "VillageA11" };
            root.Children.Add(ca.Name, ca);
            root.Children.Add(cb.Name, cb);
            ca.Children.Add(ta1.Name, ta1);
            ca.Children.Add(ta2.Name, ta1);
            cb.Children.Add(tb1.Name, tb1);
            ta1.Children.Add(va11.Name, va11);
            ta1.Children.Add(va12.Name, va12);
            _mockGeoJsonPrefixTreeBuilder.Setup(x => x.GetOrBuildTree()).Returns(root);
        }

        [Test]
        public void Test_County()
        {
            // Arrange

            // Act
            var result = _geoJsonRandomPointGenerator.GetDirectAdminNames([]).ToList();

            // Assert
            Assert.True(result.Count == 2);
            Assert.Contains("CountyA", result);
            Assert.Contains("CountyB", result);
        }

        [Test]
        public void Test_Town_of_CountyA()
        {
            // Arrange

            // Act
            var result = _geoJsonRandomPointGenerator.GetDirectAdminNames(["CountyA"]).ToList();

            // Assert
            Assert.That(result.Count == 2, Is.True);
            Assert.That(result, Does.Contain("TownA1"));
            Assert.That(result, Does.Contain("TownA2"));
        }

        [Test]
        public void Test_Town_of_CountyB()
        {
            // Arrange

            // Act
            var result = _geoJsonRandomPointGenerator.GetDirectAdminNames(["CountyB"]).ToList();

            // Assert
            Assert.That(result.Count == 1, Is.True);
            Assert.That(result, Does.Contain("TownB1"));
        }

        [Test]
        public void Test_Village_of_TownA1()
        {
            // Arrange

            // Act
            var result = _geoJsonRandomPointGenerator.GetDirectAdminNames(["CountyA", "TownA1"]).ToList();

            // Assert
            Assert.That(result.Count == 2, Is.True);
            Assert.That(result, Does.Contain("VillageA11"));
            Assert.That(result, Does.Contain("VillageA12"));
        }

        [Test]
        public void Test_Village_of_TownA1_NotFound()
        {
            // Arrange

            // Act
            var result = _geoJsonRandomPointGenerator.GetDirectAdminNames(["CountyXXX", "TownA1"]).ToList();

            // Assert
            Assert.That(result.Count == 0, Is.True);
        }

        [Test]
        public void Test_Empty_Hierarchy()
        {
            // Arrange

            // Act
            var result = _geoJsonRandomPointGenerator.GetDirectAdminNames([]).ToList();

            // Assert
            Assert.That(result.Count == 2, Is.True);
            Assert.That(result, Does.Contain("CountyA"));
            Assert.That(result, Does.Contain("CountyB"));
        }
    }
}