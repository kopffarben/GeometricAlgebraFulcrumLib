# TODO – Arc‑Splines + CurveFit in 3D‑CGA (GA‑FuL, C#)

Ziel: Ein echtzeitfähiger, inkrementeller **Curve‑Fitter** für geordnete 3D‑Punktsequenzen, der mit **minimaler Segmentanzahl** eine **Arc‑Spline** (Kreisbögen + Geraden) erzeugt, gesteuert über Toleranzen (Thresholds). Grundlage ist die **3D Conformal Geometric Algebra (CGA)**; Implementierung in **C#** mit **GA‑FuL**.

---

## 0) Kontext & mathematische Prinzipien (CGA‑Essentials)

**CGA‑Modell (R^{4,1})**

- Punkte: Einbettung `X = up(x)`. Typisch (Dorst): \(up(x) = e_0 + x + \tfrac{1}{2}\|x\|^2 e_\infty\). Rückprojektion `down(X)` liefert euklidischen Punkt.
- Geraden & Kreise (einheitlich!):
  - Gerade durch A,B: \(L = up(A) \wedge up(B) \wedge e_\infty\).
  - Kreis durch A,B,C: \(K = up(A) \wedge up(B) \wedge up(C)\).
- Bewegungen als **Versoren/Motoren**: Sandwich‑Produkt \(X' = M X \tilde M\).
  - Rotation um Ebene \(\Pi\): \(R(\theta) = \exp(-\tfrac{1}{2}\,\theta\,\hat P)\) mit unit‑Bivektor \(\hat P\).
  - Translation um Vektor \(t\): \(T(t) = \exp(-\tfrac{1}{2}\, e_\infty \wedge t)\).
  - Rotation um **Punkt** C: \(M = T(C)\,R(\theta)\,T(-C)\).
- **Orbit‑Kurven** als Rotoren: \(x(t) = e^{-\tfrac{1}{2}Bt}\,x(0)\,e^{\tfrac{1}{2}Bt}\). Je nach \(B\): Gerade (\(B^2=0\)), Kreis (\(B^2<0\)), hyperbolisch/dilatativ (\(B^2>0\)).
- **Arc‑Spline in CGA**: Jeder Kreisbogen ist die Bahn eines Punktes unter einer zeitlich konstanten Rotation um ein Zentrum in einer Ebene. Geraden sind Grenzfälle (Kreise durch \(e_\infty\)).

**Warum Arc‑Splines?**

- Sehr gute Kompression glatter Kurven (oft viel weniger Segmente als Polylinien) bei kontrollierbarem Maximalfehler.
- In CGA identische Behandlung von Linie/Kreis ⇒ robuster, verzweigungsfreier Code.
- Für **Grafik/IK**: pro Segment existiert ein **Motor(t)**, der Position *und* Orientierung liefert.

> **Lesestart:** Dorst (2016) – Rotor‑Orbits & Motions; Doran (2003) – Circle‑Blending; Drysdale et al. (2008) – Minimale Anzahl Bögen; Jeon et al. (2024) – robuste G^1‑Arc‑Splines.

---

## 1) Projekt‑Setup

### 1.1 Projektstruktur (Vorschlag)

```
arc-spline-cga/
├─ src/
│  ├─ ArcSpline/                 # Library: Fitter + CGA-Helpers
│  ├─ ArcSpline.Demo/            # Kleine Console/Unity-Demo (optional)
│  └─ ArcSpline.Benchmarks/      # BenchmarkDotNet (optional)
└─ tests/
   └─ ArcSpline.Tests/           # xUnit + FluentAssertions
```

### 1.2 Anlegen & Pakete (CLI)

```bash
# Library + Tests
mkdir -p arc-spline-cga && cd arc-spline-cga

dotnet new classlib   -n ArcSpline            -f net8.0 -o src/ArcSpline
dotnet new console    -n ArcSpline.Demo       -f net8.0 -o src/ArcSpline.Demo
dotnet new console    -n ArcSpline.Benchmarks -f net8.0 -o src/ArcSpline.Benchmarks
dotnet new xunit      -n ArcSpline.Tests      -f net8.0 -o tests/ArcSpline.Tests

# Projekt-Referenzen
dotnet add src/ArcSpline.Demo       reference src/ArcSpline/ArcSpline.csproj
dotnet add src/ArcSpline.Benchmarks reference src/ArcSpline/ArcSpline.csproj

# Pakete (Lib)
dotnet add src/ArcSpline package GeometricAlgebraFulcrumLib
dotnet add src/ArcSpline package System.Numerics.Vectors
# optional (Numerik/Logging)
dotnet add src/ArcSpline package MathNet.Numerics
dotnet add src/ArcSpline package Microsoft.Extensions.Logging.Abstractions

# Pakete (Benchmarks)
dotnet add src/ArcSpline.Benchmarks package BenchmarkDotNet

# Pakete (Tests)
dotnet add tests/ArcSpline.Tests package FluentAssertions
```

> **Hinweis:** Falls GA‑FuL nicht per NuGet genutzt wird: als Git‑Submodul einbinden und Referenz als Projekt.bash

# Library + Tests

dotnet new classlib -n ArcSpline -f net8.0 -o src/ArcSpline dotnet new xunit     -n ArcSpline.Tests -f net8.0 -o tests/ArcSpline.Tests

# Pakete

dotnet add src/ArcSpline package GeometricAlgebraFulcrumLib dotnet add src/ArcSpline package System.Numerics.Vectors

# optional (Benchmarks / Numerik / Logging)

dotnet add src/ArcSpline.Benchmarks package BenchmarkDotNet dotnet add src/ArcSpline         package MathNet.Numerics dotnet add src/ArcSpline         package Microsoft.Extensions.Logging.Abstractions

dotnet add tests/ArcSpline.Tests package FluentAssertions

````

> **Hinweis:** Falls GA‑FuL nicht per NuGet genutzt wird: als Git‑Submodul einbinden und Referenz als Projekt.

### 1.3 `ArcSpline.csproj` (wichtige Flags)

```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <Nullable>enable</Nullable>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <Optimize>true</Optimize>
  <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  <InvariantGlobalization>true</InvariantGlobalization>
  <TieredCompilation>true</TieredCompilation>
  <TieredPGO>true</TieredPGO>
</PropertyGroup>
<ItemGroup>
  <PackageReference Include="GeometricAlgebraFulcrumLib" Version="*" />
  <PackageReference Include="System.Numerics.Vectors" Version="*" />
</ItemGroup>
````

### 1.4 Code‑Ordnung (Library)

```
src/ArcSpline/
├─ Cga/                 # CGA-Basis: up/down, Motors, Primitives
├─ Fit/                 # Fitter, Fenster, Pratt/Taubin, PCA
├─ Geometry/            # Segment-Modelle, Projektion, Utils
├─ Api/                 # ArcSplineFitter-API, DTOs, Serialization
└─ Diagnostics/         # DebugDraw, Logging, Counters
```

### 1.5 Build/Perf‑Einstellungen

- **Release** als Standard, PGO an (siehe csproj).
- **SIMD**: `System.Numerics.Vector<T>` für Projektion/Fehler.
- **Alloc‑Minimierung**: `Span<T>`, Ringpuffer, Objektpool für temporäre Arrays.

### 1.6 Test‑Setup

- **xUnit** + **FluentAssertions**. Kategorien: `Unit`, `Property`, `Regression`.
- **Datasets**: synthetische Kreise/Geraden, verrauschte Pfade, reale Trajektorien.

### 1.7 Sanity‑Tests (konkret)

- `UpDown_Roundtrip`: `down(up(x)) ≈ x` (1e‑9 \* scale).
- `Motor_Rotation_Preserves_Distance`: Distanz vor/nach Sandwich gleich.
- `Translator_Consistency`: `Apply(T(t), up(x))` entspricht `x+t`.
- `Line_Circle_Membership`: `X ∧ L ≈ 0`, `X ∧ K ≈ 0`.

---

## 2) GA‑FuL Integration (CGA Basics)

> Ziel: Minimal notwendige CGA‑Bausteine in GA‑FuL kapseln (unabhängig vom Rest der Library nutzbar). **Achte auf die von GA‑FuL vorgegebene Namensgebung/Basis.** Die Konzepte unten sind bibliotheks‑agnostisch; mappe sie auf die konkreten GA‑FuL‑Typen/Factories.

### 2.1 Algebra & Basis initialisieren

- **Signatur**: Conformal 3D, übliche Notation mit zwei **Nullvektoren** `n_o` (origin) und `n_∞` (infinity) sowie euklidischem 3D‑Unterraum `e1,e2,e3`.
- In GA‑FuL (je nach Version):
  - Erzeuge/verwende die vordefinierte **CGA(4,1)**‑Algebra.
  - Hole Referenzen/IDs für `e1,e2,e3,n_o,n_∞` (ggf. via Code‑Gen / Factory).
- **Prüfen** (Unit‑Test): `n_o·n_o ≈ 0`, `n_∞·n_∞ ≈ 0`, `n_o·n_∞ = -1` (häufige Konvention), `e_i·e_j = δ_ij`.

### 2.2 `up(x)` / `down(X)`

**Formeln** (Dorst‑Konvention):

- `up(x) = n_o + x + 0.5 * (‖x‖²) * n_∞`.
- `down(X)`: verwende **GA‑FuL‑Decode**, falls vorhanden. Andernfalls implementiere gemäß Dorst (z. B. Rekonstruktion über Nullbasen‑Zerlegung). **Tests** müssen `down(up(x))≈x` sichern.

**C#‑Skizze** (Bibliotheks‑agnostisch; `Mv`=Multivector):

```csharp
readonly struct CgaBasis {
    public readonly Mv e1, e2, e3, nO, nInf;
}

Mv Up(Vector3 x, in CgaBasis B)
{
    var xe = x.X * B.e1 + x.Y * B.e2 + x.Z * B.e3; // eukl. Vektor als MV
    var s  = 0.5 * (x.X*x.X + x.Y*x.Y + x.Z*x.Z);
    return B.nO + xe + s * B.nInf;
}

Vector3 Down(Mv X, in CgaBasis B)
{
    // Bevorzugt: Bibliotheks-Decode; sonst nach Dorst.
    // Pseudocode-Platzhalter – implementiere gemäß GA‑FuL Beispielen:
    // return DecodePoint(X);
    throw new NotImplementedException("Use GA‑FuL decode or implement Dorst down(X)");
}
```

### 2.3 Primitives: Linie & Kreis

```csharp
Mv Line(Vector3 A, Vector3 B, in CgaBasis Bc)
{
    var a = Up(A, Bc);
    var b = Up(B, Bc);
    return a ^ b ^ Bc.nInf; // 3‑Blade
}

Mv Circle(Vector3 A, Vector3 B, Vector3 C, in CgaBasis Bc)
{
    var a = Up(A, Bc);
    var b = Up(B, Bc);
    var c = Up(C, Bc);
    return a ^ b ^ c; // 3‑Blade (Oriented Circle)
}
```

**Mitgliedschaftstest**: Punkt X liegt auf L/K, wenn `X ∧ L ≈ 0` bzw. `X ∧ K ≈ 0` (numerische Toleranz verwenden).

### 2.4 Versoren/Motoren & Sandwich

- **Sandwich**: `Apply(M, X) = M * X * ~M` (Tilde = Reversion/Reverse).
- **Translator** um `t`:

```csharp
Mv Translator(Vector3 t, in CgaBasis B)
{
    // T(t) = exp(-0.5 * nInf ∧ t)
    var tv = t.X * B.e1 + t.Y * B.e2 + t.Z * B.e3;
    var Biv = B.nInf ^ tv; // Null‑Bivektor
    return Exp(-0.5 * Biv); // Library-Exp für Even MV / Versor
}
```

- **Rotation** in Ebene `{u,v}` um **Punkt** `C` (Zentrum):

```csharp
Mv RotorInPlane(Vector3 u, Vector3 v, double theta, in CgaBasis B)
{
    // R(θ) = exp(-0.5 * θ * P̂)  mit P̂ = normalize(u∧v) im eukl. Unterraum
    var U = u.X*B.e1 + u.Y*B.e2 + u.Z*B.e3;
    var V = v.X*B.e1 + v.Y*B.e2 + v.Z*B.e3;
    var P = (U ^ V).Normalize();
    return Exp(-0.5 * theta * P);
}

Mv RotateAroundPoint(Vector3 center, Vector3 u, Vector3 v, double theta, in CgaBasis B)
{
    var Tc  = Translator(center, B);
    var R   = RotorInPlane(u, v, theta, B);
    var Tc_ = Translator(new Vector3(-center.X, -center.Y, -center.Z), B);
    return Tc * R * Tc_; // Motor um Punkt
}
```

> **Hinweis:** Ersetze `Exp`, `Normalize`, `^`, `*`, `~` durch die echten GA‑FuL APIs. Viele GA‑FuL‑Beispiele zeigen Motor‑Konstruktion und Sandwich‑Anwendung.

### 2.5 Verifikation (konkret)

- **Fixpunkt**: `Apply(RotateAroundPoint(C,...), up(C)) == up(C)` (Toleranz).
- **Distanz‑Erhalt**: Für zwei Punkte A,B ist `‖down(Apply(M,up(A))) − down(Apply(M,up(B)))‖ ≈ ‖A−B‖` für reine Iso‑Motions.
- **Gerade/Kreis**: Für A,B,C auf Kreis liefert `Circle(A,B,C)` ein K, sodass alle `up(P) ∧ K ≈ 0`.

---

## 3) Datenstrukturen

```csharp
struct FitSettings {
    double EpsilonRadial;    // max radialer Fehler [m] (auch für Linie als orth. Abstand nutzbar)
    double EpsilonPlanar;    // max Abstand zur PCA‑Ebene [m]
    double EpsilonAngle;     // optional: max Tangenten‑Winkelabweichung [rad]
    double EpsilonLine;      // max orthogonaler Linienabstand [m] (Default: = EpsilonRadial)
    double RMax;             // Schwellwert für „praktisch gerade“ (großer Radius)
    double MinAngleRad;      // min. Bogenwinkel [rad] (z.B. 3–5°)
    double MinChordLength;   // min. Sehnenlänge für Liniensegmente [m]
    int    MinPointsSegment; // 3 für Bogen, 2 für Linie
    int    MaxWindow;        // z.B. 128–256
    int    AllowOutliers;    // erlaubte Ausreißer in Folge (z.B. 1)
    int    RefitStride;      // alle K Punkte Voll‑Refit (z.B. 32)
    bool   PreferArcs;       // Bögen bevorzugen, wenn ähnlich gut
    bool   EnforceG1;        // G¹‑Glättung nach Segmentierung
    bool   AllowBacktrack;   // Neustart mit Überlappung > 1 Punkt erlaubt
}

enum SegmentType { Line, Arc }

sealed class Segment {
    SegmentType Type;
    Vector3 P0, P1;         // Endpunkte
    // Linie
    Vector3 Dir;            // normiert
    // Bogen
    Vector3 Center;         
    Vector3 PlaneNormal;    // normiert
    double  Radius;         
    double  Theta;          // |Winkel|
    int     Sign;           // +1/-1 Drehsinn
    // optional
    Func<double, object> MotorAt; // t∈[0,1] → CGA‑Motor
}

sealed class ArcSpline { List<Segment> Segments; }
```

**Invarianten:** \(|n|=1\), \(R>0\), \(\Theta\ge 0\), \(Sign∈\{-1,+1\}\).

---

## 4) Streaming‑Pipeline (Greedy, online)

### 4.1 Zustandsautomat

- **States**: `Idle` → `GrowingSegment` → (`Finalize` | `Abort`) → `GrowingSegment` → …
- **Events**: `PushPoint(p)`, `WindowOverflow`, `ErrorExceedsThreshold`, `Flush`.
- **Invarianten**: genau **ein** aktives Fenster `W` (Ringpuffer) je Zeit; abgeschlossene Segmente sind **immutable**.

### 4.2 Datenfluss pro `PushPoint`

1. `W.Add(p)` (Ringpuffer, O(1)).
2. **FitUpdate(W)** (Abschnitt 5):
   - Online‑PCA update (Mittelwert/Kovarianz), ggf. Re‑Basis {u,v,n}.
   - 2D‑Projektion und **Kreis‑Refit** (3‑Punkt initial → Pratt/Taubin refine) **oder** Linien‑Refit.
3. **Evaluate(W, Fit, Settings)** (Abschnitt 6):
   - Planar + Radial (+ Tangenten) gegen Thresholds prüfen; Outlier/Hysterese berücksichtigen.
4. **Decision**:
   - *OK*: `ActiveModel ← Fit` (cache Params); State bleibt `GrowingSegment`.
   - *Fail*: `FinalizeSegment(W[:-1])` und starte neues Segment mit Überlappung (Abs. 4.4).

### 4.3 Ringpuffer & Speicher

- `W` speichert: `Vector3 p`, optional `double t` (Zeit), vorprojizierte `Vector2 q` (cache), sowie kumulative Maße (optional).
- **MaxWindow** begrenzt CPU/Latenz; typische Werte 128–256. Bei Overflow: erzwinge **Finalize** mit maximal gültigem Präfix und starte neu.
- **Keine GC** im Hot‑Path: Puffer pre‑alloc, Reuse kleiner temporärer Strukturen (z.B. 3×3‑Matrizen).

### 4.4 Überlappungs‑Politik (Neustart)

- **Standard**: Neues Segment startet bei `P_{k-1}` (letzter gültiger Punkt) → minimiert Lücke.
- **Alternative**: Backtrack auf `P_{k-2}` wenn `minAngle` knapp verfehlt (reduziert Kleinstsegmente), aber erhöht Latenz.
- **Realtime**: nutze Standard; Alternative nur, wenn `AllowBacktrack=true` und `LatencyBudget` vorhanden.

### 4.5 Warm‑Start & inkrementelles Re‑Fit

- **PCA** ist inkrementell (Welford).
- **Kreis‑Fit**: Warm‑Start mit 3‑Punkt‑Kreis der **letzten drei** Punkte oder vorherigem Pratt‑Center; nur wenige Iterationen zulassen.
- **Linien‑Fit**: Hauptrichtung aus PCA; Fehlerberechnung ausschließlich als **orthogonaler Abstand** zur Linie.

### 4.6 Parallelisierung & IO

- **Producer/Consumer**: `ArcSplineFitter` produziert **finalisierte Segmente** in lock‑free Queue; Renderer/IK konsumiert.
- **Snapshot()** nur auf Anfrage (teuer – kopiert Segmente).
- **Flush()**: finalisiert aktives Segment (wenn groß genug) – für Streamende/Shutdown.

### 4.7 Fehlertoleranz & Fallbacks

- NaN/Inf in Eingabe → **verwerfen** (Zähler erhöhen). Mehrfach hintereinander → finalize current.
- Sehr große Koordinaten (|x|>1e9) → skalieren oder Segment sofort schließen.

---

## 5) Fit‑Schritt je Fenster

### 5.1 Planarität (3D→2D)

- **Online‑PCA** (Welford): aktualisiere `μ, Σ` mit O(1) je Punkt.
- **Ebene**: Eigenzerlegung von `Σ` → `n` = EV zum kleinsten EW; `{u,v}` = orthonormale Ergänzung.
- **Check**: `max_i |(p_i − μ)·n| ≤ EpsilonPlanar`. Sonst **Finalize** (mit `W[:-1]`).

### 5.2 Projektion & Caches

- Projiziere alle `p_i` auf Ebene: `q_i = ( (p_i−μ)·u , (p_i−μ)·v )`.
- Cache `q_i` im Ringpuffer, damit spätere Re‑Fits ohne erneute Projektion auskommen.

### 5.3 Kreis‑Fit (2D)

- **Initial**: 3‑Punkt‑Umkreis auf den **letzten** drei Punkten (`q_{k-2},q_{k-1},q_k`) → `(c0,R0)`.
- **Refine**: Pratt/Taubin auf **allen** `q_i` in `W` mit Warm‑Start `(c0,R0)`
  - Abbruch nach `maxIter` (z.B. 5) oder wenn `Δc, ΔR` klein.
  - Liefere `(c,R)` + Fehler: `errCircle.max`, `errCircle.rms`.
- **Degeneration**: Wenn `|D|<ε` (fast kollinear) **oder** `R>R_max` **oder** `Theta<minAngle` → **Linien‑Candidate** statt Kreis.

### 5.4 Linien‑Fit (3D)

- Richtung `d` = **Hauptrichtung** von `Σ` (größter EV). Fußpunkt `p0` = Projektion von `μ` auf Linie.
- Fehler je Punkt: orthogonaler Abstand zur Linie.
- Ergebnis: `(p0,d)` + `errLine.max`, `errLine.rms`.

### 5.5 Modellwahl (Decision Tree)

1. Wenn `PreferArcs` und Kreis **gültig** (Fehler ≤ thresholds, `Theta≥minAngle`): wähle **Arc**.
2. Sonst vergleiche `errCircle` vs `errLine` (primär `max`, sekundär `rms`).
3. `R>R_max` ⇒ **Line**. Bei Gleichstand **Line** (robuster).

### 5.6 Caching & Reuse

- Speichere `(μ,u,v,n)` und `(c,R)` bzw. `(p0,d)` im Segment‑State.
- Beim nächsten `PushPoint` nur **inkrementelle** Updates; volle Neuberechnung (Safety) alle `K` Punkte (z.B. `K=32`).

---

## 6) Fehlertests & Abbruch

### 6.1 Schwellwerte & Hysterese

- Verwende **zwei** Radial‑Schwellen: `ε_r⁺` (schließen) und `ε_r⁻` (öffnen), mit `ε_r⁺ = EpsilonRadial`, `ε_r⁻ = 0.8·EpsilonRadial` → **Flattern vermeiden**.
- Linienfehler nutzt `EpsilonLine` (Default: = `EpsilonRadial`).
- Analog für Planarität optional (`ε_pl⁺`, `ε_pl⁻`).

### 6.2 Outlier‑Policy

- Erlaube `AllowOutliers` Ausreißer in Folge ohne Break; danach **Finalize**.
- Optional: **IQR/Median** pro Fenster für robusten `rms`.

### 6.3 Tangenten‑Kriterium (optional)

- Datenrichtung am Ende `t_d = normalize(P_k − P_{k-1})`.
- Modell‑Tangente `t_m` (Linie: `±d`; Arc: Tangente am `P_k`‑Winkel). Forderung: `angle(t_d,t_m) ≤ EpsilonAngle`.

### 6.4 Pseudocode

```pseudo
Evaluate(W, model, settings):
  if any |(p_i-μ)·n| > ε_pl⁺: return FAIL(Planarity)
  if model.type == ARC:
     if max_i |‖q_i - c‖ - R| > ε_r⁺: return FAIL(Radial)
     if angle(t_d, t_arc_end) > EpsilonAngle: return FAIL(Tangent)
  else:
     if max_i dist_to_line(p_i, p0,d) > EpsilonLine: return FAIL(LineError)
  return OK
```

---

## 7) Segment‑Finalisierung

### 7.1 Linie

- `P0 = first(W)`, `P1 = lastValid(W)`.
- `Dir = normalize(P1−P0)`. Achte auf Numerik bei sehr kurzer Länge; ggf. Segment verwerfen, wenn `‖P1−P0‖ < minChord`.

### 7.2 Kreisbogen

1. **3D‑Parameter**: `Center = μ + c.x·u + c.y·v`, `Radius = R`, `PlaneNormal = n` (ggf. Vorzeichen so, dass `(u×v)·n > 0`).
2. **Winkel**: `φ0 = atan2((q0−c)·v, (q0−c)·u)`, `φ1` analog; `Δφ = unwrap(φ1−φ0)`; `Theta = |Δφ|`.
3. **Drehsinn**: `Sign = sign(Δφ)` (ggf. invertieren, wenn `{u,v,n}` orientierungswidrig).
4. **Gültigkeit**: `Theta ≥ minAngle`, `Radius ≤ R_max`. Sonst **Linie**.
5. **Motor(t)** (optional):
   - Ebenen‑Bivektor `P̂ = normalize(u ∧ v)`; `T_C = Translator(Center)`.
   - `M(t) = T_C · exp(−0.5 · Sign · Theta · t · P̂) · ~T_C`.
6. **Sanity**: `down( M(1) ⟨up(P0)⟩ ) ≈ P1` (Toleranz), Arc‑Länge `R·Theta` konsistent.

### 7.3 Speicherung & Normalisierung

- Speichere `PlaneNormal` normiert, `Theta` in `[0,π]` oder vollem Bereich je Policy.
- Runde `Center`/`Radius` nicht aggressiv (erst bei Serialisierung), um Merge‑Pass zu erleichtern.

---

## 8) Post‑Processing (optional)

### 8.1 G¹‑Glättung via Biarc

- **Ziel**: Tangentiale Stetigkeit an Segment‑Nähten.
- **Eingang**: Endpunkt `P`, Nachbartangenten `T^-` (vom linken Segment) und `T^+` (vom rechten Segment).
- **Konstruktion (Planarisiert)**:
  1. Projektion auf gemeinsame Ebene (gemäß §5.1/5.2 des lokal besseren Segments).
  2. Löse für `s0,s1>0`: `Q = P − s0 T^- = P + s1 T^+` unter Nebenbedingungen, dass die Endbögen orthogonal zum Radius am Nahtpunkt sind (Meek‑Walton‑Formel). Nutze regularisierte Lösung bei Fast‑Parallelität der Tangenten.
  3. Erzeuge zwei Kreise `(C0,R0)` und `(C1,R1)`, die `P→Q` bzw. `Q→P` mit vorgegebenen Tangenten treffen.
  4. **Validierung**: Fehler ≤ Thresholds, keine Selbstüberschneidung, `minAngle` erfüllt.
- **Fallback**: Falls nicht möglich → G⁰ belassen.

### 8.2 Merge‑Pass (Segmentzahl ↓)

- **Line+Line**: Wenn |(P1−P0)×DirA| und |(P1−P0)×DirB| klein & DirA≈DirB ⇒ mergen.
- **Arc+Arc**: |CenterA−CenterB| < τ\_c und |RadiusA−RadiusB| < τ\_r und gleiche `Sign` ⇒ Winkel addieren, wenn monotone Parameterisierung.
- **Line+Arc**: Teste Re‑Fit als Arc über vereinigte Punkte; wenn Fehler ≤ Thresholds ⇒ ersetzen.

---

## 9) API‑Entwurf & Laufzeit

### 9.1 Öffentliche API

```csharp
public sealed class ArcSplineFitter {
  public ArcSplineFitter(FitSettings settings, ILogger? log = null);
  public void   Reset();
  public void   PushPoint(in Vector3 p, double t = 0);
  public bool   HasClosedSegment { get; }
  public Segment PopClosedSegment();     // FIFO, throws if none
  public ArcSpline Snapshot();           // Kopie der bisherigen Segmente
  public void   Flush();                 // Aktives Segment (falls valide) finalisieren
}
```

### 9.2 Ereignisse/Callbacks (optional)

```csharp
public interface IArcSplineConsumer {
  void OnSegmentClosed(in Segment s);
}
```

- Fitter pusht in eine lock‑free Queue; Consumer zieht in Render/IK‑Thread.

### 9.3 Thread‑Safety

- `PushPoint` nicht thread‑safe (ein Producer). Ausgabe‑Queue thread‑safe.
- `Snapshot()` nur außerhalb Hot‑Path verwenden.

### 9.4 Beispielnutzung

```csharp
var fitter = new ArcSplineFitter(new FitSettings {
  EpsilonRadial = 1e-3,
  EpsilonPlanar = 2e-3,
  EpsilonAngle  = 0.087,
  MinPointsSegment = 3,
  MaxWindow = 256,
  PreferArcs = true,
  EnforceG1 = false
});

foreach (var p in stream) {
  fitter.PushPoint(p);
  while (fitter.HasClosedSegment)
    Render(fitter.PopClosedSegment());
}

fitter.Flush();
```

---

## 10) Tests

### 10.1 Unit‑Tests (xUnit)

- **Up/Down**: 100 Zufallspunkte → `down(up(x))≈x`.
- **Sandwich**: Rotation/Translation erhalten Distanzen (innerhalb 1e‑9 \* scale).
- **Kreis‑Erkennung**: Punkte auf perfektem Bogen (versch. Ebenen/Radien) ⇒ 1 Segment.
- **Linien‑Erkennung**: Kollineare Punkte ⇒ 1 Segment.
- **Mix**: Halbkreis + Linie ⇒ genau 2 Segmente.

### 10.2 Property‑Based (FsCheck)

- **Invarianz**: Zufällige globale Motoren auf Eingang anwenden ⇒ Segmentierung unverändert (bis auf Rundung).
- **Idempotenz**: Aus Segmenten gesampelte Punkte erneut fitten ⇒ identische Parameter (Toleranzbänder).
- **Monotonie**: Kleinere Thresholds ⇒ Segmentzahl nicht kleiner.

### 10.3 Fuzz/Regression

- **Edgecases**: 2 Punkte; 3 fast kollineare; extrem kleiner/großer Radius; 180°‑Sprünge.
- **Noise Sweeps**: σ ∈ {0, 1e‑4 …} → Fehlermetriken/Segmentzahl charten.

### 10.4 Benchmarks (BenchmarkDotNet)

- Punkte/s, Latenz pro Push, Allokationen/Op.
- Datensets: glatte Bögen, Zickzack, reale Pfade (z.B. Maus‑Strokes, GPS‑Trajektorien).

---

## 11) Tuning & Heuristiken

### 11.1 Threshold‑Kopplung

- `EpsilonPlanar = k · EpsilonRadial`, Start `k=2…5`; erhöhe `k`, wenn Daten 3D‑wellig sind.
- `R_max = α · sceneScale` (Start `α=1e6`). `minAngle = 3…5°`.

### 11.2 Adaptiv nach Geschwindigkeit

- Schätze `v = ‖p_k − p_{k-1}‖ / Δt`. Setze `ε_r = clamp(ε_min, β·v, ε_max)`.
- Reduziert Segmentflattern bei schnellen Trajektorien.

### 11.3 Ausreißerbehandlung

- 1‑Ausreißer‑Regel; bei wiederholten Ausreißern verkleinere `MaxWindow` temporär und finalize früher.
- Optional gewichtete Fits (Huber/Tukey) mit Gewichten `w_i(e_i)`.

### 11.4 Stabilität

- Refit erzwingen alle `K` Punkte; numerische Resets bei schlecht konditionierter PCA (EW‑Verhältnis).

---

## 12) IK/Grafik‑Integration

### 12.1 Pfad‑Motor je Segment

- Liefere `MotorAt(t)` für `t∈[0,1]` (Linie: reiner Translator; Arc: §7.2 Motor).
- **Arc‑Length**: optional `t_s = s/Σ s_seg` global; sonst pro Segment.

### 12.2 Orientierung entlang Kurve

- **Frenet** (T,N,B): empfindlich bei Krümmung≈0.
- **Parallel Transport Frames**: stabiler. Initiale Frame `F0`; für jedes Δ entlang Segment den Minimal‑Rotation‑Rotor anwenden (rotor logarithm).
- **Roll‑Steuerung**: zusätzlicher Drehwinkel um Tangente (z.B. Blick‑Stabilisierung).

### 12.3 Unity/Engine

- Gizmos: Kreiszentren, PCA‑Ebene, Tangenten, Fehlerheatmap.
- Sampling: 32–128 Samples pro Segment (Debug); Produktion: analytische Darstellung genügt.

---

## 13) Verbesserungen (optional, später)

### 13.1 Gewichtete/Robuste Fits

- Gewichte aus Sensor‑Kovarianzen; Reliability‑basierte Kosten (vgl. aktuelle CAGD‑Arbeiten).
- M‑Schätzer (Huber/Tukey) in Pratt/Taubin.

### 13.2 G²‑Glättung

- Krümmungsstetige Übergänge (Doppel‑Biarcs oder Optimierung mit Krümmungs‑Constraints).

### 13.3 Schraubsegmente (Helices)

- Segmenttyp mit konstantem Torsions‑/Krümmungsverhältnis; Fit via nichtlinearer LSQ (5 Parameter). Für 3D‑Pfade mit starker Torsion segmentzahlarm.

### 13.4 Globale Optimierung

- Graph‑DP über zulässige Teilstücke; Dijkstra/Shortest Path minimiert Segmentzahl (offline, O(n² log n)).

### 13.5 GPU/Jobs

- PCA/Projektion/Fits als SIMD‑/Burst‑Jobs auslagern; Circle‑Fit batched.

---

## 14) Offene Entscheidungen (Default gesetzt)

1. ``**‑Konvention**: Dorst‑Standard vs. GA‑FuL‑Default. *Default: Bibliotheks‑Default.*
2. **Fehlermetrik führend**: Max‑radial + Planar. *Default: ja (Linie nutzt EpsilonLine=EpsilonRadial).*
3. **G¹ Pflicht?** Darf Biarc lokal +1 Segment kosten? *Default: optional, +1 ok.*
4. ``: 128–256. *Default: 256.*
5. **Outlier‑Policy**: `AllowOutliers = 1`. *Default: ja.*
6. **Arc vs Line Schwelle**: `RMax`, `MinAngleRad`. *Default: **`RMax = 1e6*sceneScale`**, **`MinAngleRad = deg2rad(5)`**.*
7. **Backtrack**: erlauben? *Default: **`AllowBacktrack=false`** (Realtime‑Latenz).*

---

## 15) Referenzen (Kurzkommentar + Download‑Link)

**CGA & Rotor‑Orbits**

- Dorst, L. (2016): *The Construction of 3D Conformal Motions.* Mathematics in Computer Science 10(1). (Open‑Access PDF) – Rotor‑Exponential, Orbits (Linie/Kreis/Spiral).\
  PDF: [https://link.springer.com/content/pdf/10.1007/s11786-016-0250-8.pdf](https://link.springer.com/content/pdf/10.1007/s11786-016-0250-8.pdf)\
  Mirror: [https://pure.uva.nl/ws/files/9728111/The\_Construction\_of\_3D\_Conformal\_Motions.pdf](https://pure.uva.nl/ws/files/9728111/The_Construction_of_3D_Conformal_Motions.pdf)
- Doran, C. (2003): *Circle and Sphere Blending with Conformal Geometric Algebra.* – Circle‑Blending, G^n‑Stetigkeit, einheitliche Behandlung Linie/Kreis.\
  arXiv: [https://arxiv.org/abs/cs/0310017](https://arxiv.org/abs/cs/0310017)\
  PDF‑Mirror: [https://lomont.org/math/geometric-algebra/Circle%20and%20sphere%20blending%20with%20conformal%20geometric%20algebra%20-%20Doran%20-%202003.pdf](https://lomont.org/math/geometric-algebra/Circle%20and%20sphere%20blending%20with%20conformal%20geometric%20algebra%20-%20Doran%20-%202003.pdf)

**Arc‑Splines (Minimale Segmente, G^1)**

- Drysdale, Rote, Sturm (2008): *Approximation of an Open Polygonal Curve with a Minimum Number of Circular Arcs and Biarcs.* – Optimale (globale) Lösung via Graph/Dijkstra; O(n² log n).\
  PDF: [https://page.mi.fu-berlin.de/rote/Papers/pdf/Approximation%2Bof%2Ban%2Bopen%2Bpolygonal%2Bcurve%2Bwith%2Ba%2Bminimum%2Bnumber%2Bof%2Bcircular%2Barcs%2Band%2Bbiarcs.pdf](https://page.mi.fu-berlin.de/rote/Papers/pdf/Approximation%2Bof%2Ban%2Bopen%2Bpolygonal%2Bcurve%2Bwith%2Ba%2Bminimum%2Bnumber%2Bof%2Bcircular%2Barcs%2Band%2Bbiarcs.pdf)
- Safonova & Rossignac (2003): *Compressed Piecewise‑Circular Approximations of 3D Curves.* – Starke Kompression, praxisnah.\
  PDF: [https://repository.gatech.edu/server/api/core/bitstreams/3d3c718b-5224-4b1c-be2d-00cc8e83212f/content](https://repository.gatech.edu/server/api/core/bitstreams/3d3c718b-5224-4b1c-be2d-00cc8e83212f/content)
- Jeon, Hwang, Choi (2024): *Reliability‑based G¹ Continuous Arc Spline Approximation.* CAGD – robuste G^1‑Fits mit Kovarianzen.\
  Preprint: [https://arxiv.org/abs/2401.09770](https://arxiv.org/abs/2401.09770)\
  Author‑PDF: [https://acl.kaist.ac.kr/wp-content/uploads/2024/2024CAGD\_JJH.pdf](https://acl.kaist.ac.kr/wp-content/uploads/2024/2024CAGD_JJH.pdf)
- Maier (2014): *Optimal Arc Spline Approximation.* CAGD 31(5) – Minimale Segmente, Detektion gerader Abschnitte.\
  Abstract/DOI: [https://www.sciencedirect.com/science/article/abs/pii/S0167839614000272](https://www.sciencedirect.com/science/article/abs/pii/S0167839614000272)
- Meek & Walton (1992): *Approximation of Discrete Data by G¹ Arc Splines.* CAD 24(6) – Klassiker zur G^1‑Arc‑Spline Approximation.\
  Abstract: [https://www.sciencedirect.com/science/article/pii/001044859290047E](https://www.sciencedirect.com/science/article/pii/001044859290047E)

**Praxis (Karten/Trajektorien)**

- Schindler (2012): *Generation of High Precision Digital Maps Using Circular Arc Splines.* – Anwendung, Pipeline‑Ideen.\
  PDF: [https://www.forwiss.uni-passau.de/extern/doc/IV\_2012.pdf](https://www.forwiss.uni-passau.de/extern/doc/IV_2012.pdf)

**Dual‑Quaternion / Motor‑Splines (Grafik/IK)**

- Prošková (2017): *Interpolations by Rational Motions Using Dual Quaternions.* – G^2‑Hermite‑Bewegungen.\
  PDF: [https://www.heldermann-verlag.de/jgg/jgg21/j21h1pros.pdf](https://www.heldermann-verlag.de/jgg/jgg21/j21h1pros.pdf)
- Kavan et al. (2006/2007): *Dual‑Quaternion Blending/Skinning.* – Praxisrelevante Motor‑Interpolation.\
  PDF: [https://users.cs.utah.edu/\~ladislav/kavan06dual/kavan06dual.pdf](https://users.cs.utah.edu/~ladislav/kavan06dual/kavan06dual.pdf)

**CGA in Design**

- Colapinto (2016): *Articulating Space: Geometric Algebra for Parametric Design.* – Rotor‑Kompositionen, Designs.\
  PDF: [https://escholarship.org/content/qt5m76n8tg/qt5m76n8tg.pdf](https://escholarship.org/content/qt5m76n8tg/qt5m76n8tg.pdf)

**GA‑FuL (C#)**

- GA‑FuL (GitHub): [https://github.com/ga-explorer/GeometricAlgebraFulcrumLib](https://github.com/ga-explorer/GeometricAlgebraFulcrumLib)
- GA‑FuL Paper (2024): [https://www.mdpi.com/2227-7390/12/14/2272](https://www.mdpi.com/2227-7390/12/14/2272)

> **Hinweis:** Alle Links geprüft (Stand: Erstellung dieser Datei). Bei Paywalls (Maier 2014, Meek & Walton 1992) bitte über Bibliothek/DOI beziehen.

---

## 16) Glossar (Kurz)

- **Arc‑Spline**: Folge von Kreisbögen (und Geraden) mit G^0/G^1 Stetigkeit.
- **Biarc**: Zwei Bögen, die an einer Naht tangential stetig zusammentreffen.
- **Motor/Rotor**: Even‑Versor in CGA (Rotation/Translation über Sandwich).
- **PCA‑Ebene**: Lokale Best‑Fit‑Ebene (Online‑Schätzung) zur 2D‑Projektion.
- **Pratt/Taubin‑Fit**: Algebraische Kreis‑Least‑Squares‑Verfahren.

---

## 17) Meilensteine & Checklisten

**M1 – CGA‑Basis**

-

**M2 – Fitter (ohne G¹)**

-

**M3 – G¹ & Merge**

-

**M4 – IK/Grafik**

-

---

## 18) Mathematische Details & Formeln (kompakt)

### 18.1 3‑Punkt‑Kreis (Umkreis) – robust

Gegeben A,B,C (im lokalen 2D nach Projektion):

- Vektoren: u = B − A, v = C − A.
- Determinante: D = 2·(u\_x v\_y − u\_y v\_x). Guard: |D| < ε ⇒ nahezu kollinear ⇒ Linien‑Fallback.
- Zentrum: c = A + (1/D)·(‖v‖²·[u\_y, −u\_x]^T − ‖u‖²·[v\_y, −v\_x]^T).
- Radius: R = ‖c − A‖.

### 18.2 Pratt/Taubin Kreis‑Least‑Squares (Kurz)

- Ziel: min Σ\_i (‖p\_i − c‖ − R)² via algebraischer Relaxierung.
- Pratt: verallg. Eigenproblem S a = λ C a für a der quadr. Form Ax² + By² + C xy + D x + E y + F = 0; danach c,R rekonstruieren (zuvor zentrieren und skalieren!).
- Taubin: minimiert symmetrische Distanz; ähnlich kleines Eigenproblem; oft numerisch stabil.
- Praxis: nach Zentrierung auf 2×2/3×3 Eigenproblem reduzierbar; double-Genauigkeit und Guards verwenden.

### 18.3 Online‑PCA (Welford) & Ebene

Für jeden neuen Punkt x:

- n ← n+1, δ = x − μ, μ ← μ + δ/n, Σ ← Σ + δ·(x − μ)^T.
- Ebene: Eigenzerlegung von Σ; Normal n\_plane = Eigenvektor zum kleinsten Eigenwert. Guard: schlechtes EV‑Verhältnis ⇒ EpsilonPlanar erhöhen oder Segment früher schließen.

### 18.4 Tangenten‑/Winkelberechnung

- Datenrichtung am Ende: t\_d = normalize(P\_k − P\_{k−1}).
- Bogen‑Tangente (2D): t\_a = sign · [−(p − c)\_y, (p − c)\_x] (90° gedreht, normiert). In 3D via u,v heben.
- Winkelabweichung: angle(t\_d, t\_a) (clampen gegen Rauschen).

### 18.5 Drehsinn & Unwrap

- Drehsinn sign aus Orientierung (u × v) · n\_plane und Vorzeichen von atan2 der projizierten Winkel bestimmen.
- Unwrap: Δφ in (−π, π]; Theta = |Δφ|.

### 18.6 Bogenlänge & Fehler

- s = R · Theta.
- Radialfehler e\_r(p) = |‖p − c‖ − R|.
- Ebenenfehler e\_pl(p) = |(p − μ) · n\_plane|.
- Segment gültig, wenn max e\_r ≤ ε\_r und max e\_pl ≤ ε\_pl.

### 18.7 CGA‑Motor eines Kreisbogens

- Ebenen‑Bivektor (euklidisch): P̂ = normalize(u ∧ v).
- Zentrumstranslator: T\_C = exp(−0.5 · e∞ ∧ C).
- Motor: M(t) = T\_C · exp(−0.5 · sign · Theta · t · P̂) · \~T\_C. Anwendung per Sandwich X' = M X \~M.

---

## 19) G¹‑Biarc‑Konstruktion – Schritt‑für‑Schritt

Gegeben Endpunkte P0,P1 und gewünschte Tangenten T0,T1 (Einheitsvektoren):

1. Planarisieren (wie §5.1/5.2) ⇒ lokales 2D (x,y).
2. Richtungen: t0, t1 (2D, normiert).
3. Gemeinsame Naht Q: finde s0,s1 > 0 mit Q = P0 + s0 t0 = P1 − s1 t1. Wähle s0,s1 als Lösung des 2×2‑Systems aus orthogonalen Kreisbedingungen (Meek‑Walton 1992). Guard: keine robuste Lösung ⇒ Tangenten leicht regularisieren oder Line‑Arc‑Fallback.
4. Teilbögen bestimmen: je (Endpunkte, Tangente am Ende) ⇒ Kreise K0,K1 mit Zentren C0,C1, Radien R0,R1.
5. Validieren: Fehler ≤ Thresholds, Winkelmonotonie, minAngle.
6. Anwenden: Ersetze Naht durch zwei Bögen nur wenn oben bestanden; sonst G⁰ belassen.

---

## 20) Pratt/Taubin – Pseudocode (Skizze)

```pseudo
function FitCirclePratt(pts2D):
  μ = mean(pts2D); q_i = p_i - μ
  // baue Momente S,C
  // löse (S - λ C) a = 0 (kleinstes positives λ)
  // rekonstruiere c', R' aus a
  return (c = c' + μ, R = R')
```

Engineering‑Tipps: skaliere so, dass max(|x|,|y|) ≈ 1; double; fallback auf 3‑Punkt‑Kreis bei Degeneration.

---

## 21) Online‑PCA – Pseudocode & Guards

```pseudo
state: n=0, μ=0, Σ=0
push(x):
  n ← n+1
  δ ← x − μ
  μ ← μ + δ / n
  Σ ← Σ + δ * (x − μ)^T
```

- Ebenennormal = EV(Σ) zum kleinsten EW (Jacobi 3×3 genügt).
- Guard: Verhältnis λ\_min/(λ\_max+ε) klein ⇒ starke Planarität (gut); groß ⇒ früher segmentieren.

---

## 22) Robustheit & Numerik‑Leitfaden

- Skalierung: Eingabe grob normieren (Raumskala \~ 1…1000) zur Konditionsverbesserung.
- Datentyp: durchgehend double; nur Rendering in float.
- Winkel: atan2 + Unwrap per differenziellem Tracking.
- Kollinearität: |D|<ε vor Kreisberechnung prüfen.
- Planarity: EpsilonPlanar ≈ k · EpsilonRadial (k≈2…5). Bei 3D‑Welligkeit Segment früher schließen.
- Outlier: IQR‑Filter oder 1‑Ausreißer‑Regel; optional Huber‑Gewichte im Kreis‑Fit.
- Performance: keine Allokationen im Hot‑Path; Puffer prealloc; SIMD bei Projektion.

---

## 23) Empfohlene Defaults (nach Szeneskala)

| Szene‑Skala (m) | EpsilonRadial | EpsilonPlanar | EpsilonAngle | minAngle | MaxWindow |
| --------------- | ------------- | ------------- | ------------ | -------- | --------- |
| \~1             | 1e‑3          | 2e‑3          | 5° (0.087)   | 5°       | 128       |
| \~10            | 5e‑3          | 1e‑2          | 5°           | 5°       | 192       |
| \~100           | 2e‑2          | 5e‑2          | 3° (0.052)   | 3°       | 256       |

Faustregel: EpsilonPlanar ≈ 2–3 × EpsilonRadial; minAngle bei großen Bögen senken.

---

## 24) PushPoint‑Workflow (Pseudocode, end‑to‑end)

```pseudo
PushPoint(p):
  W.add(p)
  if W.size < MinPointsSegment: return
  μ, n, {u,v} = OnlinePCA(W)
  if any |(p_i-μ)·n| > EpsilonPlanar:
     finalizeSegment(W without last)
     startNewAtOverlap()
     return
  pts2D = project(W, μ,{u,v})
  (c,R,errCircle) = FitCirclePrattOrTaubin(pts2D)
  errLine = FitLineAndError(W)
  model = choose(PreferArcs, errCircle, errLine, R, minAngle)
  if model.error > thresholds:
     finalizeSegment(W without last)
     startNewAtOverlap()
  else
     cache model params (Center, Radius, PlaneNormal, ...)
```

---

## 25) Serialisierung & Interop

- JSON Segment: {type:"arc"|"line", p0:[…], p1:[…], center:[…], n:[…], R, theta, sign}.
- Binary: kompaktes Layout (48–80 Byte/Segment) für Streaming.
- Engines: Unity‑Jobs/Burst für PCA/Fits; Gizmos/LineRenderer für Debug.

---

## 26) Performance‑ & Benchmark‑Plan

- Datensätze: (i) perfekte Bögen/Geraden, (ii) verrauschte Trajektorien, (iii) reale Pfade.
- Metriken: Punkte/s, Latenz/Push, Segmentzahl vs. Threshold, RMS/Max‑Fehler.
- Ziele: 2× Zielrate Sicherheitsmarge; 0 GC im Hot‑Path; deterministische Seeds.

---

## 27) Logging & Debugging

- Levels: OFF/ERR/WARN/INFO/TRACE. Hot‑Path nur Zähler.
- Dump: Bei Segment‑Finalisierung – W‑Punkte, Fit‑Parameter, Fehler, Abbruchgrund (Planar/Radial/Tangent).
- Visual: Fehler‑Heatmap, minAngle‑Verletzungen, PCA‑Basis.

