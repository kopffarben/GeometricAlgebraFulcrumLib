# CGA Integration Möglichkeiten

## Bereits verwendete CGA Features:

1. **Circle Fitting (CircleFitter.cs)**
   - Verwendet CGA-Kreis-Encoding (IPNS/OPNS)
   - Pratt Algebraic Circle Fit Methode
   - Rückgabe: `CircleFitResult` mit Center, Radius, Normal

2. **Arc-Spline Construction (ArcSplinePrototype.cs)**
   - `ConstructCircleWithTangentConstraint`: Erstellt Kreise mit Tangentenbedingungen
   - IPNS ↔ OPNS Konvertierung
   - Verwendet CGA für geometrische Konstruktionen

## Potenzielle weitere CGA-Nutzung:

### 1. **Kurvatur-Berechnung via CGA**
```csharp
// Statt manuellem Winkel-Berechnung:
var curvature = ComputeCurvatureViaCGA(p1, p2, p3);

// CGA kann Kurvatur direkt aus Kreisen berechnen:
// κ = 1/radius
```

### 2. **Tangenten-Kontinuität-Prüfung**
```csharp
// Nutze CGA Bivector-Operationen für Tangenten-Vergleich
var tangent1 = GetTangentViaCGA(arc1);
var tangent2 = GetTangentViaCGA(arc2);
var angle = ComputeAngleBetweenVectors(tangent1, tangent2);
```

### 3. **Adaptive Resampling mit CGA-Metriken**
```csharp
// Nutze konforme Distanzen statt euklidischer Distanzen
var conformalDistance = ComputeConformalDistance(p1, p2);
```

### 4. **Input-Device Daten erweitern**

#### Maus:
- Velocity: Ableitung der Position über Zeit
- Acceleration: Zweite Ableitung

#### Pen/Stylus:
- Pressure: Druck-Sensor
- Tilt: Neigungswinkel
- Rotation: Stift-Drehung

#### VR-Controller:
- Position + Orientation (6DOF)
- Velocity: Linear + Angular
- Button States
- Trigger Value

### 5. **Vorschlag: Input-Daten-Struktur erweitern**

```csharp
public record InputPoint3D(
    double X, double Y, double Z,
    double Timestamp = 0,           // Für Velocity-Berechnung
    double Pressure = 1.0,           // Pen: 0-1, Maus: 1.0
    double VelocityMagnitude = 0,    // Computed
    InputDeviceType DeviceType = InputDeviceType.Mouse
);

public enum InputDeviceType
{
    Mouse,
    Pen,
    Touch,
    VRController
}
```

### 6. **Velocity-basierte Filterung**

Punkte mit niedriger Velocity könnten höhere Gewichtung beim Fitting bekommen:
```csharp
var weight = 1.0 / (1.0 + velocity);
fit.RmsError *= weight;
```

### 7. **CGA für Interpolation zwischen Kreisen**

Statt linearer Interpolation könnte CGA's Motor-Operatoren verwendet werden:
```csharp
// Smooth interpolation between circles using CGA motors
var interpolatedCircle = InterpolateCirclesViaCGA(circle1, circle2, t);
```

## Priorität für nächste Schritte:

1. **Hoch:** Velocity-Berechnung aus Timestamps
2. **Mittel:** Pen-Pressure Integration (falls Device unterstützt)
3. **Niedrig:** CGA-basierte Kurvatur-Metriken
4. **Research:** VR-Controller 6DOF Integration

## Offene Fragen:

- Welche Input-Devices werden primär verwendet?
- Sind Timestamps verfügbar im Frontend?
- Soll Pressure die Arc-Thickness beeinflussen?
