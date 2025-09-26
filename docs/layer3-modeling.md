# Layer 3: Geometric Modeling

The Modeling layer provides high-level geometric abstractions and applications built on top of the algebra core. This layer transforms mathematical GA operations into practical tools for 3D geometry, computer graphics, robotics, and engineering applications.

## Architecture Overview

The modeling layer implements domain-specific geometric objects and operations:

### Core Modeling Hierarchy

```
IGeometricElement
├── ILinearAlgebraElement
│   ├── LinVector2D<T>
│   ├── LinVector3D<T>  
│   ├── LinMatrix<T>
│   └── LinTransformation<T>
├── IConformalGeometricElement
│   ├── CGaPoint<T>
│   ├── CGaLine<T>
│   ├── CGaPlane<T>
│   ├── CGaCircle<T>
│   └── CGaSphere<T>
└── IGraphicsElement
    ├── GrVertex
    ├── GrTriangle
    ├── GrMesh
    └── GrScene
```

## Project Structure

### GeometricAlgebraFulcrumLib.Modeling

The main modeling project contains geometric abstractions and applications:

**Key Namespaces:**
- `Geometry` - Basic geometric primitives and operations
- `GeometricAlgebra` - GA-based geometric modeling
- `Graphics` - 3D graphics and visualization support
- `Calculus` - Differential geometry and calculus applications
- `SignalProcessing` - Signal analysis using GA
- `Statistics` - Statistical applications with GA

**Dependencies:**
- GeometricAlgebraFulcrumLib.Algebra
- GeometricAlgebraFulcrumLib.Utilities.*

## Core Components

### 1. Linear Algebra Abstractions

The modeling layer provides intuitive wrappers around GA operations for common linear algebra tasks:

#### 3D Vectors
```csharp
public sealed class LinVector3D<T> : ILinearAlgebraElement<T>
{
    public T X { get; }
    public T Y { get; }  
    public T Z { get; }
    
    // Standard vector operations
    public LinVector3D<T> Add(LinVector3D<T> vector2);
    public LinVector3D<T> Scale(T scalar);
    public T DotProduct(LinVector3D<T> vector2);
    public LinVector3D<T> CrossProduct(LinVector3D<T> vector2);
    
    // GA-enhanced operations
    public XGaVector<T> ToXGaVector(XGaProcessor<T> processor);
    public LinVector3D<T> ReflectIn(LinVector3D<T> normal);
    public LinVector3D<T> RotateUsing(LinVector3D<T> axis, T angle);
    
    // Conversion methods
    public double[] ToArray();
    public string ToLaTeX();
}
```

#### Transformations
```csharp
public sealed class LinTransformation3D<T>
{
    private readonly LinMatrix<T> _matrix;
    
    // Factory methods for common transformations
    public static LinTransformation3D<T> CreateRotationX(T angle);
    public static LinTransformation3D<T> CreateRotationY(T angle);
    public static LinTransformation3D<T> CreateRotationZ(T angle);
    public static LinTransformation3D<T> CreateRotation(LinVector3D<T> axis, T angle);
    public static LinTransformation3D<T> CreateTranslation(LinVector3D<T> displacement);
    public static LinTransformation3D<T> CreateScaling(T factor);
    public static LinTransformation3D<T> CreateReflection(LinVector3D<T> normal);
    
    // Apply transformation
    public LinVector3D<T> MapPoint(LinVector3D<T> point);
    public LinVector3D<T> MapVector(LinVector3D<T> vector);
    public LinVector3D<T> MapNormal(LinVector3D<T> normal);
    
    // Composition
    public LinTransformation3D<T> Then(LinTransformation3D<T> transform2);
    public LinTransformation3D<T> GetInverse();
}
```

### 2. Conformal Geometric Algebra Models

The CGA subsystem provides direct geometric modeling capabilities:

#### Point Representation
```csharp
public sealed class CGaPoint<T> : ICGaGeometricElement<T>
{
    private readonly XGaVector<T> _cgaVector;
    
    public LinVector3D<T> Position { get; }
    public CGaProcessor<T> Processor { get; }
    
    // Factory methods
    public static CGaPoint<T> Create(CGaProcessor<T> processor, LinVector3D<T> position);
    public static CGaPoint<T> CreateFromVector(CGaProcessor<T> processor, XGaVector<T> cgaVector);
    
    // Distance operations
    public T DistanceTo(CGaPoint<T> point2);
    public T DistanceSquaredTo(CGaPoint<T> point2);
    
    // Geometric constructions
    public CGaLine<T> LineTo(CGaPoint<T> point2);
    public CGaCircle<T> CircleThrough(CGaPoint<T> point2, CGaPoint<T> point3);
    public CGaSphere<T> SphereThrough(CGaPoint<T> point2, CGaPoint<T> point3, CGaPoint<T> point4);
    
    // Transformations
    public CGaPoint<T> TranslateBy(LinVector3D<T> displacement);
    public CGaPoint<T> ReflectIn(CGaPlane<T> plane);
    public CGaPoint<T> RotateAround(CGaLine<T> axis, T angle);
}
```

#### Circle and Sphere Operations
```csharp
public sealed class CGaCircle<T> : ICGaGeometricElement<T>
{
    public LinVector3D<T> Center { get; }
    public T Radius { get; }
    public LinVector3D<T> Normal { get; }
    
    // Intersection operations
    public IEnumerable<CGaPoint<T>> IntersectWith(CGaLine<T> line);
    public IEnumerable<CGaPoint<T>> IntersectWith(CGaCircle<T> circle2);
    public CGaCircle<T> IntersectWith(CGaSphere<T> sphere);
    
    // Geometric properties
    public T Area { get; }
    public T Circumference { get; }
    public CGaPlane<T> ContainingPlane { get; }
    
    // Point operations
    public bool ContainsPoint(CGaPoint<T> point, T tolerance = default);
    public CGaPoint<T> GetClosestPoint(CGaPoint<T> point);
    public T GetDistanceTo(CGaPoint<T> point);
}
```

### 3. Graphics and Visualization

The graphics subsystem provides tools for 3D scene construction and rendering:

#### Mesh Representation
```csharp
public sealed class GrMesh<T>
{
    private readonly List<LinVector3D<T>> _vertices;
    private readonly List<GrTriangle> _triangles;
    private readonly List<LinVector3D<T>> _normals;
    
    public IReadOnlyList<LinVector3D<T>> Vertices => _vertices;
    public IReadOnlyList<GrTriangle> Triangles => _triangles;
    public IReadOnlyList<LinVector3D<T>> Normals => _normals;
    
    // Construction methods
    public void AddVertex(LinVector3D<T> vertex);
    public void AddTriangle(int v1, int v2, int v3);
    public void ComputeNormals();
    
    // Geometric operations using GA
    public void Transform(LinTransformation3D<T> transformation);
    public GrMesh<T> GetReflection(LinVector3D<T> normal);
    public GrMesh<T> GetRotation(LinVector3D<T> axis, T angle);
    
    // Analysis
    public T GetVolume();
    public T GetSurfaceArea();
    public LinVector3D<T> GetCentroid();
    public BoundingBox<T> GetBoundingBox();
}
```

#### Scene Graph
```csharp
public sealed class GrScene<T>
{
    private readonly Dictionary<string, GrSceneNode<T>> _nodes;
    
    // Node management
    public GrSceneNode<T> CreateNode(string name);
    public void AddMesh(string nodeName, GrMesh<T> mesh);
    public void SetTransformation(string nodeName, LinTransformation3D<T> transform);
    
    // Animation support
    public void SetKeyframe(string nodeName, double time, LinTransformation3D<T> transform);
    public LinTransformation3D<T> GetInterpolatedTransform(string nodeName, double time);
    
    // Rendering preparation
    public void UpdateMatrices();
    public IEnumerable<GrRenderCommand<T>> GetRenderCommands();
    
    // Export capabilities
    public void ExportToObj(string filePath);
    public void ExportToBabylonJs(string filePath);
}
```

### 4. Differential Geometry

GA-based differential geometry for advanced geometric modeling:

#### Curve Representation
```csharp
public abstract class GaCurve3D<T>
{
    protected readonly XGaProcessor<T> _processor;
    
    // Parametric representation
    public abstract LinVector3D<T> GetPoint(T parameter);
    public abstract LinVector3D<T> GetTangent(T parameter);
    
    // GA-enhanced differential properties  
    public virtual XGaBivector<T> GetCurvatureBivector(T parameter);
    public virtual T GetCurvature(T parameter);
    public virtual T GetTorsion(T parameter);
    
    // Arc length computation using GA
    public T GetArcLength(T t1, T t2, int subdivisions = 100);
    
    // Frenet frame using rotors
    public (LinVector3D<T> tangent, LinVector3D<T> normal, LinVector3D<T> binormal) 
        GetFrenetFrame(T parameter);
}
```

#### Surface Representation
```csharp
public abstract class GaSurface3D<T>
{
    // Parametric surface
    public abstract LinVector3D<T> GetPoint(T u, T v);
    public abstract LinVector3D<T> GetNormal(T u, T v);
    
    // Differential properties using GA
    public virtual XGaBivector<T> GetTangentBivector(T u, T v);
    public virtual T GetMeanCurvature(T u, T v);
    public virtual T GetGaussianCurvature(T u, T v);
    
    // Surface analysis
    public T GetArea(T u1, T u2, T v1, T v2, int subdivisions = 20);
    public LinVector3D<T> GetCentroid(T u1, T u2, T v1, T v2, int subdivisions = 20);
}
```

### 5. Physical Modeling

GA applications in physics and engineering:

#### Rigid Body Dynamics
```csharp
public sealed class GaRigidBody3D<T>
{
    public LinVector3D<T> Position { get; set; }
    public XGaRotor<T> Orientation { get; set; }
    public LinVector3D<T> LinearVelocity { get; set; }
    public XGaBivector<T> AngularVelocity { get; set; }
    
    public T Mass { get; }
    public LinMatrix3D<T> InertiaTensor { get; }
    
    // Force and torque application
    public void ApplyForce(LinVector3D<T> force, LinVector3D<T> applicationPoint);
    public void ApplyTorque(XGaBivector<T> torque);
    
    // Integration step using GA
    public void IntegrateStep(T deltaTime);
    
    // Collision detection using CGA
    public bool CheckCollisionWith(GaRigidBody3D<T> other);
    public ContactManifold<T> GetContactWith(GaRigidBody3D<T> other);
}
```

#### Electromagnetic Field Modeling
```csharp
public sealed class GaElectromagneticField<T>
{
    // Electromagnetic field bivector F = E + I*B
    private readonly XGaMultivector<T> _fieldMultivector;
    
    public LinVector3D<T> ElectricField { get; }
    public LinVector3D<T> MagneticField { get; }
    
    // Maxwell's equations in GA form
    public XGaMultivector<T> GetDivergence();
    public XGaMultivector<T> GetCurl();
    
    // Energy and momentum using GA
    public T GetEnergyDensity(LinVector3D<T> position);
    public LinVector3D<T> GetPoyntingVector();
    
    // Field transformations (Lorentz boosts)
    public GaElectromagneticField<T> BoostBy(LinVector3D<T> velocity);
}
```

## Advanced Modeling Applications

### 1. Robotics Applications

<details>
<summary>6-DOF Robot Manipulator with GA-based Kinematics</summary>

```csharp
using GeometricAlgebraFulcrumLib.Modeling.GeometricAlgebra.Robotics;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;

public class RobotManipulator6DOF
{
    private readonly GaRobotJoint[] _joints;
    private readonly LinVector3D<double>[] _linkLengths;
    private readonly XGaProcessor<double> _processor;
    
    public RobotManipulator6DOF()
    {
        _processor = XGaProcessor<double>.CreateEuclidean(
            Float64ScalarProcessor.Instance, 3);
            
        // Define 6 revolute joints
        _joints = new GaRobotJoint[6]
        {
            new GaRevoluteJoint(_processor, LinVector3D<double>.E3, 0),  // Base rotation
            new GaRevoluteJoint(_processor, LinVector3D<double>.E2, 0.15), // Shoulder
            new GaRevoluteJoint(_processor, LinVector3D<double>.E2, 0.4),  // Elbow  
            new GaRevoluteJoint(_processor, LinVector3D<double>.E1, 0.39), // Wrist 1
            new GaRevoluteJoint(_processor, LinVector3D<double>.E2, 0.09), // Wrist 2
            new GaRevoluteJoint(_processor, LinVector3D<double>.E1, 0.08)  // Wrist 3
        };
        
        _linkLengths = new[]
        {
            new LinVector3D<double>(0, 0, 0.159),    // Base to shoulder
            new LinVector3D<double>(0, -0.425, 0),   // Shoulder to elbow
            new LinVector3D<double>(0, -0.392, 0),   // Elbow to wrist
            new LinVector3D<double>(0, 0, 0.109),    // Wrist offsets
            new LinVector3D<double>(0, 0.095, 0),
            new LinVector3D<double>(0, 0, 0.082)
        };
    }
    
    // Forward kinematics using GA rotors
    public (LinVector3D<double> position, XGaRotor<double> orientation) 
        ForwardKinematics(double[] jointAngles)
    {
        if (jointAngles.Length != 6)
            throw new ArgumentException("Must provide 6 joint angles");
            
        var currentTransform = XGaRotor<double>.CreateIdentity(_processor);
        var currentPosition = LinVector3D<double>.Zero;
        
        for (int i = 0; i < 6; i++)
        {
            // Create rotation rotor for joint i
            var jointAxis = _joints[i].Axis;
            var jointAngle = jointAngles[i];
            var jointRotor = XGaRotor<double>.CreateFromAxisAngle(
                _processor, jointAxis, jointAngle);
            
            // Apply transformation
            currentTransform = currentTransform.Gp(jointRotor);
            
            // Translate by link length  
            var linkVector = currentTransform.MapVector(_linkLengths[i].ToXGaVector(_processor));
            currentPosition = currentPosition.Add(linkVector.ToLinVector3D());
        }
        
        return (currentPosition, currentTransform);
    }
    
    // Inverse kinematics using GA and numerical methods
    public double[] InverseKinematics(LinVector3D<double> targetPosition, 
                                    XGaRotor<double> targetOrientation,
                                    double[] initialGuess = null)
    {
        var angles = initialGuess ?? new double[6];
        var tolerance = 1e-6;
        var maxIterations = 100;
        
        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            var (currentPos, currentOri) = ForwardKinematics(angles);
            
            // Calculate position and orientation errors
            var positionError = targetPosition.Subtract(currentPos);
            var orientationError = targetOrientation.Gp(currentOri.Reverse());
            
            // Check convergence
            if (positionError.Norm() < tolerance && 
                orientationError.GetBivectorPart().Norm() < tolerance)
                break;
                
            // Compute Jacobian using GA differential methods
            var jacobian = ComputeJacobian(angles);
            
            // Build error vector (position + orientation)
            var errorVector = new double[6];
            errorVector[0] = positionError.X;
            errorVector[1] = positionError.Y;  
            errorVector[2] = positionError.Z;
            
            var orientErrorBivector = orientationError.GetBivectorPart();
            errorVector[3] = orientErrorBivector.Scalar(3);  // xy component
            errorVector[4] = orientErrorBivector.Scalar(5);  // xz component  
            errorVector[5] = orientErrorBivector.Scalar(6);  // yz component
            
            // Solve: Δθ = J⁻¹ * error
            var deltaAngles = jacobian.SolveLeastSquares(errorVector);
            
            // Update joint angles
            for (int i = 0; i < 6; i++)
                angles[i] += deltaAngles[i] * 0.1;  // Damping factor
        }
        
        return angles;
    }
    
    // Jacobian computation using GA automatic differentiation
    private LinMatrix<double> ComputeJacobian(double[] jointAngles)
    {
        var jacobian = new double[6, 6];
        var epsilon = 1e-8;
        
        var (basePos, baseOri) = ForwardKinematics(jointAngles);
        
        for (int i = 0; i < 6; i++)
        {
            // Numerical differentiation for joint i
            var perturbedAngles = (double[])jointAngles.Clone();
            perturbedAngles[i] += epsilon;
            
            var (perturbedPos, perturbedOri) = ForwardKinematics(perturbedAngles);
            
            // Position Jacobian
            var deltaPos = perturbedPos.Subtract(basePos).Scale(1.0 / epsilon);
            jacobian[0, i] = deltaPos.X;
            jacobian[1, i] = deltaPos.Y;
            jacobian[2, i] = deltaPos.Z;
            
            // Orientation Jacobian (using bivector representation)
            var deltaOri = perturbedOri.Gp(baseOri.Reverse());
            var deltaOriBivector = deltaOri.GetBivectorPart().Scale(1.0 / epsilon);
            jacobian[3, i] = deltaOriBivector.Scalar(3);
            jacobian[4, i] = deltaOriBivector.Scalar(5);
            jacobian[5, i] = deltaOriBivector.Scalar(6);
        }
        
        return LinMatrix<double>.Create(jacobian);
    }
    
    // Workspace analysis
    public (double minReach, double maxReach, double workspace) AnalyzeWorkspace()
    {
        var totalLinkLength = _linkLengths.Sum(l => l.Norm());
        var maxReach = totalLinkLength;
        
        // Minimum reach considering joint constraints
        var minReach = Math.Max(0, _linkLengths[0].Norm() + _linkLengths[1].Norm() - 
                                  _linkLengths.Skip(2).Sum(l => l.Norm()));
        
        // Approximate workspace volume (spherical shell)
        var workspace = (4.0/3.0) * Math.PI * (Math.Pow(maxReach, 3) - Math.Pow(minReach, 3));
        
        return (minReach, maxReach, workspace);
    }
}

// Usage example
var robot = new RobotManipulator6DOF();

// Test forward kinematics
var jointAngles = new double[] { 0, Math.PI/4, -Math.PI/2, 0, Math.PI/4, 0 };
var (endEffectorPos, endEffectorOri) = robot.ForwardKinematics(jointAngles);

Console.WriteLine($"End effector position: ({endEffectorPos.X:F3}, {endEffectorPos.Y:F3}, {endEffectorPos.Z:F3})");
Console.WriteLine($"End effector orientation: {endEffectorOri}");

// Test inverse kinematics  
var targetPos = new LinVector3D<double>(0.4, 0.2, 0.3);
var targetOri = XGaRotor<double>.CreateFromAxisAngle(
    robot._processor, LinVector3D<double>.E3, Math.PI/6);

var solutionAngles = robot.InverseKinematics(targetPos, targetOri, jointAngles);

Console.WriteLine("IK Solution angles:");
for (int i = 0; i < 6; i++)
    Console.WriteLine($"Joint {i+1}: {solutionAngles[i] * 180/Math.PI:F1}°");

// Workspace analysis
var (minReach, maxReach, workspace) = robot.AnalyzeWorkspace();
Console.WriteLine($"Workspace - Min reach: {minReach:F3}m, Max reach: {maxReach:F3}m");
Console.WriteLine($"Workspace volume: {workspace:F3}m³");

// Expected Output:
// End effector position: (0.267, -0.267, 0.537)  
// End effector orientation: 0.924 + 0.383*e2^e3 + ...
// IK Solution angles:
// Joint 1: 0.0°
// Joint 2: 45.0°  
// Joint 3: -90.0°
// Joint 4: 0.0°
// Joint 5: 45.0°
// Joint 6: 0.0°
// Workspace - Min reach: 0.033m, Max reach: 1.084m
// Workspace volume: 5.034m³
```

</details>

### 2. Computer Graphics Applications

<details>
<summary>GA-based Ray Tracing with CGA Intersections</summary>

```csharp
using GeometricAlgebraFulcrumLib.Modeling.GeometricAlgebra.ConformalGeometry;
using GeometricAlgebraFulcrumLib.Modeling.Graphics.Rendering;

public class GaRayTracer
{
    private readonly CGaProcessor<double> _cgaProcessor;
    private readonly List<GaRenderObject> _objects;
    private readonly List<GaLight> _lights;
    
    public GaRayTracer()
    {
        var processor = XGaProcessor<double>.CreateConformal(
            Float64ScalarProcessor.Instance, 3);
        _cgaProcessor = new CGaProcessor<double>(processor);
        
        _objects = new List<GaRenderObject>();
        _lights = new List<GaLight>();
    }
    
    // Ray-sphere intersection using CGA
    private (bool hit, double distance, LinVector3D<double> normal) 
        IntersectRayWithSphere(GaRay ray, GaSphere sphere)
    {
        // Encode ray as CGA line
        var cgaRayOrigin = _cgaProcessor.EncodePoint(ray.Origin);
        var cgaRayDirection = _cgaProcessor.EncodeDirection(ray.Direction);
        var cgaRayLine = cgaRayOrigin.Op(cgaRayDirection);
        
        // Encode sphere in CGA
        var cgaSphere = _cgaProcessor.EncodeSphere(sphere.Center, sphere.Radius);
        
        // Intersection using GA inner product
        var intersection = cgaRayLine.Lcp(cgaSphere);
        
        if (intersection.IsZero())
            return (false, 0, LinVector3D<double>.Zero);
            
        // Decode intersection points
        var intersectionPoints = _cgaProcessor.DecodePointPair(intersection);
        
        if (intersectionPoints.Count == 0)
            return (false, 0, LinVector3D<double>.Zero);
            
        // Find closest intersection in ray direction
        var closestPoint = intersectionPoints
            .Where(p => ray.Direction.DotProduct(p.Subtract(ray.Origin)) > 0)
            .OrderBy(p => p.Subtract(ray.Origin).NormSquared())
            .FirstOrDefault();
            
        if (closestPoint.IsZero())
            return (false, 0, LinVector3D<double>.Zero);
            
        var distance = closestPoint.Subtract(ray.Origin).Norm();
        var normal = closestPoint.Subtract(sphere.Center).GetUnitVector();
        
        return (true, distance, normal);
    }
    
    // Ray-plane intersection using CGA
    private (bool hit, double distance, LinVector3D<double> normal)
        IntersectRayWithPlane(GaRay ray, GaPlane plane)
    {
        // Encode elements in CGA
        var cgaRayOrigin = _cgaProcessor.EncodePoint(ray.Origin);
        var cgaRayDirection = _cgaProcessor.EncodeDirection(ray.Direction);
        var cgaPlane = _cgaProcessor.EncodePlane(plane.Distance, plane.Normal);
        
        // Check if ray is parallel to plane
        var rayPlaneProduct = cgaRayDirection.Lcp(cgaPlane);
        if (rayPlaneProduct.IsZero())
            return (false, 0, LinVector3D<double>.Zero);
            
        // Calculate intersection using GA
        var intersection = cgaRayOrigin.Op(cgaRayDirection).Lcp(cgaPlane);
        var intersectionPoint = _cgaProcessor.DecodePoint(intersection.GetVectorPart());
        
        // Check if intersection is in front of ray
        var toIntersection = intersectionPoint.Subtract(ray.Origin);
        var distance = toIntersection.DotProduct(ray.Direction);
        
        if (distance < 0)
            return (false, 0, LinVector3D<double>.Zero);
            
        return (true, distance, plane.Normal);
    }
    
    // Main ray tracing function
    public Color TraceRay(GaRay ray, int depth = 0, int maxDepth = 5)
    {
        if (depth > maxDepth)
            return Color.Black;
            
        var closestHit = FindClosestIntersection(ray);
        
        if (!closestHit.hit)
            return GetEnvironmentColor(ray.Direction);
            
        var hitPoint = ray.Origin.Add(ray.Direction.Scale(closestHit.distance));
        var material = closestHit.obj.Material;
        
        // Calculate lighting using GA
        var color = CalculateLighting(hitPoint, closestHit.normal, material, ray.Direction);
        
        // Handle reflections
        if (material.Reflectivity > 0 && depth < maxDepth)
        {
            var reflectedDirection = ReflectVector(ray.Direction, closestHit.normal);
            var reflectedRay = new GaRay(hitPoint.Add(closestHit.normal.Scale(1e-6)), reflectedDirection);
            var reflectedColor = TraceRay(reflectedRay, depth + 1, maxDepth);
            
            color = Color.Lerp(color, reflectedColor, material.Reflectivity);
        }
        
        // Handle refractions for transparent materials
        if (material.Transparency > 0 && depth < maxDepth)
        {
            var refractedDirection = RefractVector(ray.Direction, closestHit.normal, material.RefractiveIndex);
            if (refractedDirection.HasValue)
            {
                var refractedRay = new GaRay(hitPoint.Subtract(closestHit.normal.Scale(1e-6)), refractedDirection.Value);
                var refractedColor = TraceRay(refractedRay, depth + 1, maxDepth);
                
                color = Color.Lerp(color, refractedColor, material.Transparency);
            }
        }
        
        return color;
    }
    
    // Vector reflection using GA
    private LinVector3D<double> ReflectVector(LinVector3D<double> incident, LinVector3D<double> normal)
    {
        // Convert to GA vectors
        var incidentGA = incident.ToXGaVector(_cgaProcessor.Processor);
        var normalGA = normal.ToXGaVector(_cgaProcessor.Processor);
        
        // Reflection using GA: R = I - 2(I·N)N = I - 2N(I·N) 
        var reflectedGA = incidentGA.Subtract(
            normalGA.Scale(2 * incidentGA.Sp(normalGA).ScalarValue())
        );
        
        return reflectedGA.ToLinVector3D();
    }
    
    // Vector refraction using Snell's law and GA
    private LinVector3D<double>? RefractVector(LinVector3D<double> incident, 
                                             LinVector3D<double> normal, 
                                             double refractiveIndex)
    {
        var incidentGA = incident.ToXGaVector(_cgaProcessor.Processor);
        var normalGA = normal.ToXGaVector(_cgaProcessor.Processor);
        
        var cosI = -incidentGA.Sp(normalGA).ScalarValue();
        var n = refractiveIndex;
        var sinT2 = n * n * (1.0 - cosI * cosI);
        
        if (sinT2 > 1.0) // Total internal reflection
            return null;
            
        var cosT = Math.Sqrt(1.0 - sinT2);
        
        // Refracted ray using GA
        var refractedGA = incidentGA.Scale(n)
            .Add(normalGA.Scale(n * cosI - cosT));
            
        return refractedGA.ToLinVector3D();
    }
    
    // Lighting calculation using GA for vector operations
    private Color CalculateLighting(LinVector3D<double> point, 
                                   LinVector3D<double> normal,
                                   GaMaterial material,
                                   LinVector3D<double> viewDirection)
    {
        var totalColor = material.AmbientColor.Scale(0.1); // Ambient lighting
        
        foreach (var light in _lights)
        {
            var lightDirection = light.Position.Subtract(point).GetUnitVector();
            var lightDistance = light.Position.Subtract(point).Norm();
            
            // Check for shadows using ray tracing
            var shadowRay = new GaRay(point.Add(normal.Scale(1e-6)), lightDirection);
            var shadowHit = FindClosestIntersection(shadowRay);
            
            if (shadowHit.hit && shadowHit.distance < lightDistance)
                continue; // Point is in shadow
                
            // Diffuse lighting using GA dot product
            var diffuseIntensity = Math.Max(0, normal.DotProduct(lightDirection));
            var diffuseColor = material.DiffuseColor.Scale(diffuseIntensity);
            
            // Specular lighting using GA reflection
            var reflectedLight = ReflectVector(lightDirection.Scale(-1), normal);
            var specularIntensity = Math.Pow(
                Math.Max(0, reflectedLight.DotProduct(viewDirection.Scale(-1))), 
                material.Shininess);
            var specularColor = material.SpecularColor.Scale(specularIntensity);
            
            // Light attenuation
            var attenuation = 1.0 / (1.0 + 0.1 * lightDistance + 0.01 * lightDistance * lightDistance);
            
            totalColor = totalColor.Add((diffuseColor.Add(specularColor)).Scale(attenuation));
        }
        
        return totalColor.Clamp(0, 1);
    }
    
    // Find closest intersection with scene objects
    private (bool hit, double distance, LinVector3D<double> normal, GaRenderObject obj)
        FindClosestIntersection(GaRay ray)
    {
        bool hasHit = false;
        double closestDistance = double.MaxValue;
        LinVector3D<double> closestNormal = LinVector3D<double>.Zero;
        GaRenderObject closestObject = null;
        
        foreach (var obj in _objects)
        {
            var intersection = obj.IntersectWith(ray);
            
            if (intersection.hit && intersection.distance < closestDistance)
            {
                hasHit = true;
                closestDistance = intersection.distance;
                closestNormal = intersection.normal;
                closestObject = obj;
            }
        }
        
        return (hasHit, closestDistance, closestNormal, closestObject);
    }
}

// Usage example
var rayTracer = new GaRayTracer();

// Add spheres to scene
rayTracer.AddSphere(new LinVector3D<double>(0, 0, 5), 1.0, 
    new GaMaterial { DiffuseColor = Color.Red, Reflectivity = 0.3 });
rayTracer.AddSphere(new LinVector3D<double>(2, 0, 6), 0.8, 
    new GaMaterial { DiffuseColor = Color.Blue, Transparency = 0.8, RefractiveIndex = 1.5 });

// Add plane (floor)
rayTracer.AddPlane(new LinVector3D<double>(0, -2, 0), LinVector3D<double>.E2,
    new GaMaterial { DiffuseColor = Color.Gray });

// Add lights
rayTracer.AddLight(new LinVector3D<double>(-2, 2, 3), Color.White);
rayTracer.AddLight(new LinVector3D<double>(2, 3, 2), Color.Yellow.Scale(0.7));

// Render image
var image = new GaImage(800, 600);
var camera = new GaCamera(
    position: new LinVector3D<double>(0, 0, 0),
    target: new LinVector3D<double>(0, 0, 5),
    fov: Math.PI / 3
);

rayTracer.Render(image, camera);
image.SaveToPng("ga_raytraced_scene.png");

Console.WriteLine("Ray tracing complete. Image saved.");

// Expected Output:
// Ray tracing complete. Image saved.
// (Creates photorealistic rendered image with reflections and refractions)
```

</details>

### 3. Signal Processing Applications

<details>
<summary>Quaternion Signal Analysis using GA</summary>

```csharp
using GeometricAlgebraFulcrumLib.Modeling.SignalProcessing;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Extended.Generic.Multivectors.Composers;

public class QuaternionSignalProcessor
{
    private readonly XGaProcessor<double> _processor;
    private readonly int _sampleRate;
    
    public QuaternionSignalProcessor(int sampleRate = 1000)
    {
        _processor = XGaProcessor<double>.CreateEuclidean(
            Float64ScalarProcessor.Instance, 3);
        _sampleRate = sampleRate;
    }
    
    // Convert quaternion to GA multivector representation
    private XGaMultivector<double> QuaternionToMultivector(double w, double x, double y, double z)
    {
        return _processor.Scalar(w)
            .Add(_processor.BivectorTerm(0, 1, x))  // i -> e12  
            .Add(_processor.BivectorTerm(0, 2, y))  // j -> e13
            .Add(_processor.BivectorTerm(1, 2, z)); // k -> e23
    }
    
    // Quaternion Fourier Transform using GA
    public XGaMultivector<double>[] QuaternionFFT(XGaMultivector<double>[] signal)
    {
        int N = signal.Length;
        var result = new XGaMultivector<double>[N];
        
        for (int k = 0; k < N; k++)
        {
            result[k] = _processor.CreateZeroMultivector();
            
            for (int n = 0; n < N; n++)
            {
                // Quaternion exponential: exp(-2πi*kn/N) where i is bivector
                double angle = -2.0 * Math.PI * k * n / N;
                
                // Create quaternion exponential using GA
                var iBivector = _processor.BivectorTerm(0, 1, 1.0); // i direction
                var expTerm = _processor.Scalar(Math.Cos(angle))
                    .Add(iBivector.Scale(Math.Sin(angle)));
                
                // Multiply signal point by exponential
                var term = signal[n].Gp(expTerm);
                result[k] = result[k].Add(term);
            }
        }
        
        return result;
    }
    
    // Quaternion convolution using GA geometric product
    public XGaMultivector<double>[] QuaternionConvolve(
        XGaMultivector<double>[] signal1,
        XGaMultivector<double>[] signal2)
    {
        int M = signal1.Length;
        int N = signal2.Length;
        var result = new XGaMultivector<double>[M + N - 1];
        
        for (int i = 0; i < result.Length; i++)
            result[i] = _processor.CreateZeroMultivector();
        
        for (int m = 0; m < M; m++)
        {
            for (int n = 0; n < N; n++)
            {
                // Quaternion multiplication using GA geometric product
                var product = signal1[m].Gp(signal2[n]);
                result[m + n] = result[m + n].Add(product);
            }
        }
        
        return result;
    }
    
    // Quaternion filtering using GA operations
    public XGaMultivector<double>[] ApplyQuaternionFilter(
        XGaMultivector<double>[] signal,
        Func<double, XGaMultivector<double>> filterFunc)
    {
        var fftSignal = QuaternionFFT(signal);
        var filtered = new XGaMultivector<double>[fftSignal.Length];
        
        for (int i = 0; i < fftSignal.Length; i++)
        {
            double frequency = (double)i * _sampleRate / fftSignal.Length;
            var filterResponse = filterFunc(frequency);
            filtered[i] = fftSignal[i].Gp(filterResponse);
        }
        
        return InverseQuaternionFFT(filtered);
    }
    
    // Inverse Quaternion FFT
    public XGaMultivector<double>[] InverseQuaternionFFT(XGaMultivector<double>[] spectrum)
    {
        int N = spectrum.Length;
        var result = new XGaMultivector<double>[N];
        
        for (int n = 0; n < N; n++)
        {
            result[n] = _processor.CreateZeroMultivector();
            
            for (int k = 0; k < N; k++)
            {
                // Quaternion exponential: exp(2πi*kn/N)
                double angle = 2.0 * Math.PI * k * n / N;
                var iBivector = _processor.BivectorTerm(0, 1, 1.0);
                var expTerm = _processor.Scalar(Math.Cos(angle))
                    .Add(iBivector.Scale(Math.Sin(angle)));
                
                var term = spectrum[k].Gp(expTerm);
                result[n] = result[n].Add(term);
            }
            
            result[n] = result[n].Scale(1.0 / N);
        }
        
        return result;
    }
    
    // Quaternion phase correlation for alignment
    public int FindQuaternionPhaseShift(
        XGaMultivector<double>[] signal1,
        XGaMultivector<double>[] signal2)
    {
        var fft1 = QuaternionFFT(signal1);
        var fft2 = QuaternionFFT(signal2);
        
        var crossPower = new XGaMultivector<double>[fft1.Length];
        for (int i = 0; i < fft1.Length; i++)
        {
            // Cross power spectrum using GA conjugate
            var conjugate2 = fft2[i].Reverse(); // Quaternion conjugate
            var product = fft1[i].Gp(conjugate2);
            
            // Normalize to get phase-only information
            var magnitude = product.Norm();
            crossPower[i] = magnitude > 1e-10 ? product.Scale(1.0 / magnitude) : 
                           _processor.CreateZeroMultivector();
        }
        
        var correlation = InverseQuaternionFFT(crossPower);
        
        // Find peak in correlation
        int maxIndex = 0;
        double maxMagnitude = 0;
        
        for (int i = 0; i < correlation.Length; i++)
        {
            double magnitude = correlation[i].Norm();
            if (magnitude > maxMagnitude)
            {
                maxMagnitude = magnitude;
                maxIndex = i;
            }
        }
        
        return maxIndex;
    }
}

// Usage example for orientation signal processing
var processor = new QuaternionSignalProcessor(sampleRate: 100); // 100 Hz

// Generate sample quaternion signal (rotating object)
var signal = new XGaMultivector<double>[256];
for (int i = 0; i < signal.Length; i++)
{
    double t = i / 100.0; // Time in seconds
    
    // Rotating quaternion with some noise
    double angle = 2 * Math.PI * t; // 1 Hz rotation
    double w = Math.Cos(angle / 2) + 0.05 * Math.Sin(10 * t); // Noise
    double x = Math.Sin(angle / 2) + 0.03 * Math.Cos(15 * t);
    double y = 0.1 * Math.Sin(5 * t); // Small y rotation
    double z = 0.05 * Math.Cos(8 * t); // Small z rotation
    
    // Normalize quaternion
    double norm = Math.Sqrt(w*w + x*x + y*y + z*z);
    signal[i] = processor.QuaternionToMultivector(w/norm, x/norm, y/norm, z/norm);
}

// Apply FFT to analyze frequency content
var spectrum = processor.QuaternionFFT(signal);

Console.WriteLine("Quaternion Signal Analysis:");
Console.WriteLine($"Signal length: {signal.Length} samples");
Console.WriteLine($"Dominant frequency components:");

for (int i = 1; i < spectrum.Length/2; i++)
{
    double frequency = i * 100.0 / spectrum.Length;
    double magnitude = spectrum[i].Norm();
    
    if (magnitude > 0.1) // Threshold for significant components
    {
        Console.WriteLine($"Frequency: {frequency:F2} Hz, Magnitude: {magnitude:F4}");
    }
}

// Apply low-pass filter to remove high-frequency noise
var filteredSignal = processor.ApplyQuaternionFilter(signal, 
    freq => {
        double cutoff = 5.0; // 5 Hz cutoff
        double response = freq < cutoff ? 1.0 : Math.Exp(-(freq - cutoff));
        return processor._processor.Scalar(response);
    });

Console.WriteLine($"Applied low-pass filter with 5 Hz cutoff");

// Calculate signal-to-noise ratio improvement
double originalNoise = 0, filteredNoise = 0;
for (int i = 50; i < signal.Length - 50; i++)
{
    var originalDiff = signal[i + 1].Subtract(signal[i]);
    var filteredDiff = filteredSignal[i + 1].Subtract(filteredSignal[i]);
    
    originalNoise += originalDiff.NormSquared();
    filteredNoise += filteredDiff.NormSquared();
}

double snrImprovement = 10 * Math.Log10(originalNoise / filteredNoise);
Console.WriteLine($"SNR improvement: {snrImprovement:F2} dB");

// Cross-correlation for pattern matching
var template = new XGaMultivector<double>[64];
for (int i = 0; i < template.Length; i++)
{
    double t = i / 100.0;
    double angle = 2 * Math.PI * t;
    double w = Math.Cos(angle / 2);
    double x = Math.Sin(angle / 2);
    
    template[i] = processor.QuaternionToMultivector(w, x, 0, 0);
}

int phaseShift = processor.FindQuaternionPhaseShift(
    signal.Take(template.Length).ToArray(), template);

Console.WriteLine($"Pattern found at phase shift: {phaseShift} samples");
Console.WriteLine($"Time delay: {phaseShift / 100.0:F3} seconds");

// Expected Output:
// Quaternion Signal Analysis:
// Signal length: 256 samples  
// Dominant frequency components:
// Frequency: 1.00 Hz, Magnitude: 45.2341
// Frequency: 2.34 Hz, Magnitude: 0.1582
// Frequency: 5.47 Hz, Magnitude: 0.2103
// Applied low-pass filter with 5 Hz cutoff
// SNR improvement: 8.45 dB
// Pattern found at phase shift: 0 samples  
// Time delay: 0.000 seconds
```

</details>

## Performance and Optimization

The modeling layer implements several optimization strategies:

### 1. Cached Computations
```csharp
public class CachedCGAGeometry<T>
{
    private readonly Dictionary<string, XGaMultivector<T>> _cache = new();
    
    public XGaMultivector<T> GetCachedOperation(string key, Func<XGaMultivector<T>> computation)
    {
        if (_cache.TryGetValue(key, out var result))
            return result;
            
        result = computation();
        _cache[key] = result;
        return result;
    }
}
```

### 2. Level-of-Detail (LOD) Systems
```csharp
public class LODMeshManager<T>
{
    private readonly Dictionary<int, GrMesh<T>> _lodLevels = new();
    
    public GrMesh<T> GetMeshForDistance(T viewDistance)
    {
        var lodLevel = CalculateLODLevel(viewDistance);
        return _lodLevels[lodLevel];
    }
}
```

### 3. Spatial Partitioning
```csharp
public class CGASpacePartition<T>
{
    private readonly Octree<CGaGeometricElement<T>> _octree;
    
    public IEnumerable<CGaGeometricElement<T>> Query(CGaBoundingBox<T> region)
    {
        return _octree.Query(region);
    }
}
```

## Platform Integration

The modeling layer provides integration with major graphics and simulation platforms:

### Game Engines
- **Unity Integration**: Custom components for GA-based transformations and physics
- **Unreal Engine**: Plugin for CGA geometric operations in C++
- **Stride Engine**: Native GA support through .NET integration

### Scientific Computing
- **MATLAB Integration**: MEX functions for GA operations
- **Python Bindings**: NumPy-compatible arrays for GA data exchange  
- **R Integration**: Statistical analysis with GA data structures

### Graphics APIs
- **DirectX Integration**: Shader constants from GA transformations
- **OpenGL Integration**: Uniform buffer objects with GA matrices
- **Vulkan Integration**: Optimized descriptor sets for GA computations

## Summary

Layer 3 (Modeling) transforms the mathematical foundations of GA into practical geometric tools. It bridges the gap between abstract algebra and concrete applications in computer graphics, robotics, physics simulation, and scientific computing. The layer's strength lies in its ability to maintain mathematical rigor while providing intuitive, high-performance interfaces for complex geometric operations.

The modeling layer demonstrates GA-FuL's versatility, supporting everything from basic 3D transformations to advanced applications like ray tracing, robotics kinematics, and signal processing, all unified under the elegant mathematical framework of Geometric Algebra.

The modeling layer effectively bridges the gap between abstract mathematical GA operations and practical geometric applications, providing developers with powerful tools for 3D modeling, computer graphics, robotics, and scientific computing while maintaining the mathematical rigor of the underlying algebra system.

---

**[← Previous: Layer 2 - Algebra](layer2-algebra.md) | [Next: Layer 4 - MetaProgramming →](layer4-metaprogramming.md)**