## 🔴 KRITISCHER WORKFLOW: Equivalence Test Pattern und Documentation (ZWINGEND!)

**Für JEDE Klasse gilt diese Reihenfolge:**

```
1. ✅ ANALYSIERE Float64 Klasse
   ↓
2. ✅ IMPLEMENTIERE Generic<T> Klasse
   ↓
3. ✅ Stelle API-Kompartibilität zwischen Float64 und Generic<T> sicher
   ↓
4. ✅ SCHREIBE Equivalence Tests (Generic<double> vs Float64)
   ↓
5. ✅ STELLE SICHER alle Tests passing (100% Pass Rate!)
   ↓
6. ✅ Update DEDUPLICATION_ROADMAP/PHASE_3_DEDUPLICATION_TASKS.md und DEDUPLICATION_ROADMAP/DEDUPLICATION_ROADMAP.md
   ↓
7. ✅ NUR DANN Git Commit
   ↓
8. ✅ Weiter zur nächsten Klasse
```