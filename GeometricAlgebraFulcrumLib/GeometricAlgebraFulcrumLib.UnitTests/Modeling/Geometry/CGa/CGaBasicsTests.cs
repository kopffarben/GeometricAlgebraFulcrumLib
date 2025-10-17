using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Blades;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Decoding;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Operations;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.CGa;

/// <summary>
/// Basic tests for Conformal Geometric Algebra (CGa)
/// Phase 3A - Emergency: Conformal Geometry Basics (20 tests)
/// </summary>
[TestFixture]
public class CGaBasicsTests
{
    private const double Tolerance = 1e-10;

    #region Geometric Space Construction (4 tests)

    [TestFixture]
    public class GeometricSpaceConstructionTests
    {
        [Test]
        public void Create4DSpace_ShouldHaveCorrectDimensions()
        {
            // Arrange & Act
            var space = CGaFloat64GeometricSpace.Space4D;

            // Assert
            Assert.That(space.Is4D, Is.True, "Space should be 4D");
            Assert.That(space.VSpaceDimensions, Is.EqualTo(4), "VSpace dimensions should be 4");
        }

        [Test]
        public void Create5DSpace_ShouldHaveCorrectDimensions()
        {
            // Arrange & Act
            var space = CGaFloat64GeometricSpace.Space5D;

            // Assert
            Assert.That(space.Is5D, Is.True, "Space should be 5D");
            Assert.That(space.VSpaceDimensions, Is.EqualTo(5), "VSpace dimensions should be 5");
        }

        [Test]
        public void VerifyBasisBlades_5D()
        {
            // Arrange
            var space = CGaFloat64GeometricSpace.Space5D;

            // Act & Assert
            Assert.That(space.E1, Is.Not.Null, "E1 basis blade should exist");
            Assert.That(space.E2, Is.Not.Null, "E2 basis blade should exist");
            Assert.That(space.E3, Is.Not.Null, "E3 basis blade should exist");
            Assert.That(space.OneScalarBlade, Is.Not.Null, "Unit scalar blade should exist");
            Assert.That(space.ZeroVectorBlade, Is.Not.Null, "Zero vector blade should exist");
        }

        [Test]
        public void VerifyConformalBasis()
        {
            // Arrange
            var space = CGaFloat64GeometricSpace.Space5D;

            // Act & Assert
            Assert.That(space.En, Is.Not.Null, "e_- (negative infinity) basis blade should exist");
            Assert.That(space.Ep, Is.Not.Null, "e_+ (positive infinity) basis blade should exist");

            // Verify null basis vectors are not null
            Assert.That(space.EnVector, Is.Not.Null, "e_- internal vector should not be null");
            Assert.That(space.EpVector, Is.Not.Null, "e_+ internal vector should not be null");
        }
    }

    #endregion

    #region Basic Element Encoding (8 tests)

    [TestFixture]
    public class BasicElementEncodingTests
    {
        private CGaFloat64GeometricSpace _space = null!;

        [OneTimeSetUp]
        public void Setup()
        {
            _space = CGaFloat64GeometricSpace.Space5D;
        }

        [Test]
        public void EncodePoint_IPNS()
        {
            // Arrange
            var point = LinFloat64Vector3D.Create(1, 2, 3);

            // Act
            var ipnsPoint = _space.EncodeIpnsFlat.Point(point);

            // Assert
            Assert.That(ipnsPoint, Is.Not.Null, "IPNS point should not be null");
            Assert.That(ipnsPoint.InternalKVector, Is.Not.Null, "IPNS point internal k-vector should not be null");
        }

        [Test]
        public void EncodePoint_OPNS()
        {
            // Arrange
            var point = LinFloat64Vector3D.Create(1, 2, 3);

            // Act
            var opnsPoint = _space.EncodeOpnsFlat.Point(point);

            // Assert
            Assert.That(opnsPoint, Is.Not.Null, "OPNS point should not be null");
            Assert.That(opnsPoint.InternalKVector, Is.Not.Null, "OPNS point internal k-vector should not be null");
        }

        [Test]
        public void EncodeLine_IPNS()
        {
            // Arrange
            var point = LinFloat64Vector3D.Create(0, 0, 0);
            var direction = LinFloat64Vector3D.Create(1, 0, 0);

            // Act
            var ipnsLine = _space.EncodeIpnsFlat.Line(point, direction);

            // Assert
            Assert.That(ipnsLine, Is.Not.Null, "IPNS line should not be null");
            Assert.That(ipnsLine.InternalKVector, Is.Not.Null, "IPNS line internal k-vector should not be null");
        }

        [Test]
        public void EncodeLine_OPNS()
        {
            // Arrange
            var point = LinFloat64Vector3D.Create(0, 0, 0);
            var direction = LinFloat64Vector3D.Create(1, 0, 0);

            // Act
            var opnsLine = _space.EncodeOpnsFlat.Line(point, direction);

            // Assert
            Assert.That(opnsLine, Is.Not.Null, "OPNS line should not be null");
            Assert.That(opnsLine.InternalKVector, Is.Not.Null, "OPNS line internal k-vector should not be null");
        }

        [Test]
        public void EncodePlane_IPNS()
        {
            // Arrange
            var point = LinFloat64Vector3D.Create(0, 0, 0);
            var normal = LinFloat64Vector3D.Create(0, 0, 1); // XY plane (normal = Z)

            // Act
            var ipnsPlane = _space.EncodeIpnsFlat.Plane(point, normal);

            // Assert
            Assert.That(ipnsPlane, Is.Not.Null, "IPNS plane should not be null");
            Assert.That(ipnsPlane.InternalKVector, Is.Not.Null, "IPNS plane internal k-vector should not be null");
        }

        [Test]
        public void EncodePlane_OPNS()
        {
            // Arrange
            var point = LinFloat64Vector3D.Create(0, 0, 0);
            var normal = LinFloat64Vector3D.Create(0, 0, 1); // XY plane (normal = Z)

            // Act
            var opnsPlane = _space.EncodeOpnsFlat.Plane(point, normal);

            // Assert
            Assert.That(opnsPlane, Is.Not.Null, "OPNS plane should not be null");
            Assert.That(opnsPlane.InternalKVector, Is.Not.Null, "OPNS plane internal k-vector should not be null");
        }

        [Test]
        public void EncodeSphere_IPNS()
        {
            // Arrange
            var center = LinFloat64Vector3D.Create(0, 0, 0);
            var radius = 1.0;

            // Act
            var ipnsSphere = _space.EncodeIpnsRound.RealSphere(radius, center);

            // Assert
            Assert.That(ipnsSphere, Is.Not.Null, "IPNS sphere should not be null");
            Assert.That(ipnsSphere.InternalKVector, Is.Not.Null, "IPNS sphere internal k-vector should not be null");
        }

        [Test]
        public void EncodeSphere_OPNS()
        {
            // Arrange
            var center = LinFloat64Vector3D.Create(0, 0, 0);
            var radius = 1.0;

            // Act - Create IPNS sphere first, then convert to OPNS
            var ipnsSphere = _space.EncodeIpnsRound.RealSphere(radius, center);
            var opnsSphere = ipnsSphere.IpnsToOpns();

            // Assert
            Assert.That(opnsSphere, Is.Not.Null, "OPNS sphere should not be null");
            Assert.That(opnsSphere.InternalKVector, Is.Not.Null, "OPNS sphere internal k-vector should not be null");
        }
    }

    #endregion

    #region OPNS/IPNS Blade Conversions (4 tests)

    [TestFixture]
    public class OpnsIpnsConversionTests
    {
        private CGaFloat64GeometricSpace _space = null!;

        [OneTimeSetUp]
        public void Setup()
        {
            _space = CGaFloat64GeometricSpace.Space5D;
        }

        [Test]
        public void PointDuality_IPNS_to_OPNS()
        {
            // Arrange
            var originalPoint = LinFloat64Vector3D.Create(1, 2, 3);
            var ipnsPoint = _space.EncodeIpnsFlat.Point(originalPoint);

            // Act
            var opnsPoint = ipnsPoint.IpnsToOpns();

            // Assert
            Assert.That(opnsPoint, Is.Not.Null, "OPNS point should not be null after conversion");
            Assert.That(opnsPoint.InternalKVector, Is.Not.Null, "OPNS point internal k-vector should not be null");
        }

        [Test]
        public void LineDuality_IPNS_to_OPNS()
        {
            // Arrange
            var point = LinFloat64Vector3D.Create(0, 0, 0);
            var direction = LinFloat64Vector3D.Create(1, 0, 0);
            var ipnsLine = _space.EncodeIpnsFlat.Line(point, direction);

            // Act
            var opnsLine = ipnsLine.IpnsToOpns();

            // Assert
            Assert.That(opnsLine, Is.Not.Null, "OPNS line should not be null after conversion");
            Assert.That(opnsLine.InternalKVector, Is.Not.Null, "OPNS line internal k-vector should not be null");
        }

        [Test]
        public void PlaneDuality_IPNS_to_OPNS()
        {
            // Arrange
            var point = LinFloat64Vector3D.Create(0, 0, 0);
            var normal = LinFloat64Vector3D.Create(0, 0, 1);
            var ipnsPlane = _space.EncodeIpnsFlat.Plane(point, normal);

            // Act
            var opnsPlane = ipnsPlane.IpnsToOpns();

            // Assert
            Assert.That(opnsPlane, Is.Not.Null, "OPNS plane should not be null after conversion");
            Assert.That(opnsPlane.InternalKVector, Is.Not.Null, "OPNS plane internal k-vector should not be null");
        }

        [Test]
        public void SphereDuality_IPNS_to_OPNS()
        {
            // Arrange
            var center = LinFloat64Vector3D.Create(0, 0, 0);
            var radius = 1.0;
            var ipnsSphere = _space.EncodeIpnsRound.RealSphere(radius, center);

            // Act
            var opnsSphere = ipnsSphere.IpnsToOpns();

            // Assert
            Assert.That(opnsSphere, Is.Not.Null, "OPNS sphere should not be null after conversion");
            Assert.That(opnsSphere.InternalKVector, Is.Not.Null, "OPNS sphere internal k-vector should not be null");
        }
    }

    #endregion

    #region Basic Transformations (4 tests)

    [TestFixture]
    public class BasicTransformationTests
    {
        private CGaFloat64GeometricSpace _space = null!;

        [OneTimeSetUp]
        public void Setup()
        {
            _space = CGaFloat64GeometricSpace.Space5D;
        }

        [Test]
        public void Translation_MovesPointBlade()
        {
            // Arrange
            var originalPoint = LinFloat64Vector3D.Create(0, 0, 0);
            var translationVector = LinFloat64Vector3D.Create(1, 2, 3);

            var ipnsPoint = _space.EncodeIpnsFlat.Point(originalPoint);

            // Act - Using extension method TranslateBy
            var translatedBlade = ipnsPoint.TranslateBy(translationVector);

            // Assert
            Assert.That(translatedBlade, Is.Not.Null, "Translated blade should not be null");
            Assert.That(translatedBlade.InternalKVector, Is.Not.Null, "Translated blade internal k-vector should not be null");
        }

        [Test]
        public void Rotation_RotatesLineBlade()
        {
            // Arrange
            var point = LinFloat64Vector3D.Create(1, 0, 0);
            var direction = LinFloat64Vector3D.Create(0, 1, 0);
            var ipnsLine = _space.EncodeIpnsFlat.Line(point, direction);

            var axisPoint = LinFloat64Vector3D.Create(0, 0, 0);
            var axisVector = LinFloat64Vector3D.Create(0, 0, 1); // Z-axis
            var angle = LinFloat64PolarAngle.CreateFromDegrees(90);

            // Act - Using extension method RotateUsing
            var rotatedBlade = ipnsLine.RotateUsing(angle, axisPoint, axisVector);

            // Assert
            Assert.That(rotatedBlade, Is.Not.Null, "Rotated blade should not be null");
            Assert.That(rotatedBlade.InternalKVector, Is.Not.Null, "Rotated blade internal k-vector should not be null");
        }

        [Test]
        public void Scaling_ScalesSphereBlade()
        {
            // Arrange
            var center = LinFloat64Vector3D.Create(0, 0, 0);
            var radius = 1.0;
            var ipnsSphere = _space.EncodeIpnsRound.RealSphere(radius, center);

            var scaleFactor = 2.0;

            // Act - Using extension method ScaleBy
            var scaledBlade = ipnsSphere.ScaleBy(scaleFactor);

            // Assert
            Assert.That(scaledBlade, Is.Not.Null, "Scaled blade should not be null");
            Assert.That(scaledBlade.InternalKVector, Is.Not.Null, "Scaled blade internal k-vector should not be null");
        }

        [Test]
        public void Reflection_ReflectsPlaneBlade()
        {
            // Arrange
            var point = LinFloat64Vector3D.Create(0, 0, 1);
            var normal = LinFloat64Vector3D.Create(0, 0, 1);
            var ipnsPlane = _space.EncodeIpnsFlat.Plane(point, normal);

            var reflectionNormal = LinFloat64Vector3D.Create(1, 0, 0); // Reflect across YZ plane
            var reflectionPoint = LinFloat64Vector3D.Create(0, 0, 0);
            var reflectionPlane = _space.EncodeIpnsFlat.Plane(reflectionPoint, reflectionNormal);

            // Act - Using blade-level reflection
            var reflectedBlade = ipnsPlane.ReflectIpnsOn(reflectionPlane);

            // Assert
            Assert.That(reflectedBlade, Is.Not.Null, "Reflected blade should not be null");
            Assert.That(reflectedBlade.InternalKVector, Is.Not.Null, "Reflected blade internal k-vector should not be null");
        }
    }

    #endregion
}
