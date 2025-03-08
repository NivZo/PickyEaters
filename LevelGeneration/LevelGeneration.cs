// using System;
// using System.Collections.Generic;
// using System.Linq;

// public class Cell
// {
//     public int X { get; }
//     public int Y { get; }
//     public Entity Ent { get; set; }

//     public Cell(int x, int y)
//     {
//         X = x;
//         Y = y;
//         Ent = null;
//     }

//     public bool Empty()
//     {
//         return Ent == null;
//     }
// }

// public abstract class Entity
// {
//     public string Color { get; set; }

//     public Entity(string color)
//     {
//         Color = color;
//     }
// }

// public class EaterEntity : Entity
// {
//     public EaterEntity(string color) : base(color) { }
// }

// public class FoodEntity : Entity
// {
//     public bool IsLast { get; }

//     public FoodEntity(string color, bool isLast = false) : base(color)
//     {
//         IsLast = isLast;
//     }
// }

// public class Grid
// {
//     private static readonly (int, int)[] DIRS = { (0, 1), (0, -1), (1, 0), (-1, 0) };
//     private static readonly Dictionary<string, Dictionary<string, int>> COLOR_MAP = new Dictionary<string, Dictionary<string, int>>
//     {
//         { "FoodEntity", new Dictionary<string, int> { { "White", 0 }, { "Green", 1 }, { "Blue", 2 }, { "Red", 3 }, { "Yellow", 4 }, { "Purple", 5 }, { "Pink", 6 }, { "Brown", 7 } } },
//         { "EaterEntity", new Dictionary<string, int> { { "Green", 0 }, { "Blue", 1 }, { "Red", 2 }, { "Yellow", 3 }, { "Purple", 4 }, { "Pink", 5 }, { "Brown", 6 } } }
//     };

//     private static readonly Dictionary<string, double> DEFAULT_WEIGHTS = new Dictionary<string, double>
//     {
//         { "FoodEntity_separation", 5.0 },
//         { "cluster_size", 0.33 },
//         { "clusters_count", 1.0 },
//         { "distribution", 1.0 },
//         { "distance_to_EaterEntity", 1.0 },
//         { "path_complexity", 0.8 },
//         { "interaction_complexity", 1.5 },
//         { "empty_cells", 2.0 }
//     };

//     public int Rows { get; }
//     public int Cols { get; }
//     public Cell[][] GridCells { get; }
//     public List<Dictionary<string, object>> Moves { get; }
//     public int MinSeparation { get; }
//     private Dictionary<(int, int, int, int), int> distanceCache;
//     private Dictionary<Tuple<Tuple<int, int>[], int>, int> clusterCache;
//     private Dictionary<(Tuple<int, int>, Tuple<Tuple<int, int>[]>), double> pathCache;
//     private Dictionary<string, List<(int, int)>> FoodEntityByColor;
//     private Dictionary<string, (int, int)> EaterEntityPositions;
//     public Dictionary<string, double> Weights { get; }

//     public Grid(int rows, int cols, Dictionary<string, double> weights = null)
//     {
//         Rows = rows;
//         Cols = cols;
//         GridCells = new Cell[rows][];
//         for (int x = 0; x < rows; x++)
//         {
//             GridCells[x] = new Cell[cols];
//             for (int y = 0; y < cols; y++)
//             {
//                 GridCells[x][y] = new Cell(x, y);
//             }
//         }
//         Moves = new List<Dictionary<string, object>>();
//         MinSeparation = 2;
//         distanceCache = new Dictionary<(int, int, int, int), int>();
//         clusterCache = new Dictionary<Tuple<Tuple<int, int>[], int>, int>();
//         pathCache = new Dictionary<(Tuple<int, int>, Tuple<Tuple<int, int>[]>), double>();
//         FoodEntityByColor = new Dictionary<string, List<(int, int)>>();
//         EaterEntityPositions = new Dictionary<string, (int, int)>();
//         Weights = weights != null ? new Dictionary<string, double>(weights) : new Dictionary<string, double>(DEFAULT_WEIGHTS);
//     }

//     public Dictionary<string, object> CalculateClusteringScore(int minimumQualifiedColors)
//     {
//         var colors = COLOR_MAP["FoodEntity"].Keys.ToHashSet();
//         colors.Remove("White");
//         var metrics = new Dictionary<string, Dictionary<string, object>>();

//         UpdateEntityCache();

//         foreach (var color in colors)
//         {
//             if (!FoodEntityByColor.TryGetValue(color, out var FoodEntitys) || FoodEntitys.Count == 0) continue;
//             if (!EaterEntityPositions.TryGetValue(color, out var EaterEntityPos)) continue;

//             var FoodEntitysTuple = FoodEntitys.OrderBy(f => f.Item1).ThenBy(f => f.Item2).ToArray();

//             var colorMetrics = new Dictionary<string, object>
//             {
//                 { "FoodEntity_count", FoodEntitys.Count },
//                 { "avg_FoodEntity_separation", CalculateAvgSeparation(FoodEntitysTuple) },
//                 { "largest_cluster_size", FindLargestCluster(FoodEntitysTuple) },
//                 { "clusters_count", CountClusters(FoodEntitysTuple) },
//                 { "avg_distance_to_EaterEntity", CalculateAvgDistanceToPoint(FoodEntitys, EaterEntityPos) },
//                 { "distribution_score", CalculateDistributionScore(FoodEntitys) },
//                 { "path_complexity", CalculatePathComplexity(color) }
//             };

//             if ((int)colorMetrics["largest_cluster_size"] == FoodEntitys.Count && FoodEntitys.Count > 1)
//             {
//                 colorMetrics["disqualified"] = true;
//             }
//             else
//             {
//                 colorMetrics["disqualified"] = false;
//             }

//             metrics[color] = colorMetrics;
//         }

//         if (metrics.Count == 0)
//         {
//             return new Dictionary<string, object> { { "final_score", double.PositiveInfinity }, { "color_metrics", new Dictionary<string, Dictionary<string, object>>() }, { "disqualified", true } };
//         }

//         bool disqualified = metrics.Values.Count(m => !(bool)m["disqualified"]) < minimumQualifiedColors;

//         double interactionComplexity = CalculateEaterEntityInteractions();
//         int emptyCells = CountEmpty();
//         double emptyCellsScore = CalculateEmptyCellsScore(emptyCells);

//         double finalScore = disqualified ? double.PositiveInfinity : CalculateFinalScore(metrics, interactionComplexity, emptyCellsScore);

//         return new Dictionary<string, object>
//         {
//             { "final_score", finalScore },
//             { "color_metrics", metrics },
//             { "disqualified", disqualified },
//             { "interaction_complexity", interactionComplexity },
//             { "empty_cells", emptyCells },
//             { "empty_cells_score", emptyCellsScore }
//         };
//     }

//     private double CalculateEmptyCellsScore(int emptyCells)
//     {
//         double totalCells = Rows * Cols;
//         double filledRatio = 1 - (emptyCells / totalCells);
//         return filledRatio * 100;
//     }

//     private void UpdateEntityCache()
//     {
//         FoodEntityByColor.Clear();
//         EaterEntityPositions.Clear();

//         for (int x = 0; x < Rows; x++)
//         {
//             for (int y = 0; y < Cols; y++)
//             {
//                 var cell = GridCells[x][y];
//                 if (cell.Ent is FoodEntity FoodEntity)
//                 {
//                     if (!FoodEntityByColor.ContainsKey(FoodEntity.Color))
//                     {
//                         FoodEntityByColor[FoodEntity.Color] = new List<(int, int)>();
//                     }
//                     FoodEntityByColor[FoodEntity.Color].Add((x, y));
//                 }
//                 else if (cell.Ent is EaterEntity EaterEntity)
//                 {
//                     EaterEntityPositions[EaterEntity.Color] = (x, y);
//                 }
//             }
//         }
//     }

//     private int ManhattanDistance((int, int) pos1, (int, int) pos2)
//     {
//         if (distanceCache.TryGetValue((pos1.Item1, pos1.Item2, pos2.Item1, pos2.Item2), out int dist)) return dist;
//         dist = Math.Abs(pos1.Item1 - pos2.Item1) + Math.Abs(pos1.Item2 - pos2.Item2);
//         distanceCache[(pos1.Item1, pos1.Item2, pos2.Item1, pos2.Item2)] = dist;
//         distanceCache[(pos2.Item1, pos2.Item2, pos1.Item1, pos1.Item2)] = dist;
//         return dist;
//     }

//     private double CalculateAvgSeparation(Tuple<Tuple<int, int>[]> positions)
//     {
//         if (positions.Item1.Length < 2) return double.PositiveInfinity;

//         if (distanceCache.ContainsKey((positions.Item1[0].Item1, positions.Item1[0].Item2, positions.Item1[1].Item1, positions.Item1[1].Item2)))
//         {
//             double sum = 0;
//             int count = 0;
//             for (int i = 0; i < positions.Item1.Length - 1; i++)
//             {
//                 for (int j = i + 1; j < positions.Item1.Length; j++)
//                 {
//                     sum += ManhattanDistance(positions.Item1[i], positions.Item1[j]);
//                     count++;
//                 }
//             }
//             return count > 0 ? sum / count : double.PositiveInfinity;
//         }

//         double distances = 0;
//         int distanceCount = 0;
//         for (int i = 0; i < positions.Item1.Length - 1; i++)
//         {
//             for (int j = i + 1; j < positions.Item1.Length; j++)
//             {
//                 distances += ManhattanDistance(positions.Item1[i], positions.Item1[j]);
//                 distanceCount++;
//             }
//         }
//         return distanceCount > 0 ? distances / distanceCount : double.PositiveInfinity;
//     }

//     private int CountClusters(Tuple<Tuple<int, int>[]> positions, int clusterThreshold = 2)
//     {
//         if (positions.Item1.Length == 0) return 0;

//         var clusterKey = Tuple.Create(positions, clusterThreshold);
//         if (clusterCache.TryGetValue(clusterKey, out int cachedClusters)) return cachedClusters;

//         var positionsSet = new HashSet<(int, int)>(positions.Item1);
//         int clusters = 0;

//         while (positionsSet.Count > 0)
//         {
//             clusters++;
//             var start = positionsSet.First();
//             positionsSet.Remove(start);

//             var queue = new Queue<(int, int)>();
//             queue.Enqueue(start);

//             while (queue.Count > 0)
//             {
//                 var current = queue.Dequeue();
//                 var toRemove = new List<(int, int)>();
//                 foreach (var pos in positionsSet)
//                 {
//                     if (ManhattanDistance(current, pos) <= clusterThreshold)
//                     {
//                         queue.Enqueue(pos);
//                         toRemove.Add(pos);
//                     }
//                 }
//                 foreach (var pos in toRemove)
//                 {
//                     positionsSet.Remove(pos);
//                 }
//             }
//         }

//         clusterCache[clusterKey] = clusters;
//         return clusters;
//     }

//     private int FindLargestCluster(Tuple<Tuple<int, int>[]> positions, int clusterThreshold = 2)
//     {
//         if (positions.Item1.Length == 0) return 0;

//         var clusterKey = Tuple.Create(positions, clusterThreshold);
//         if (clusterCache.TryGetValue(Tuple.Create(positions, clusterThreshold), out int cachedLargestCluster))
//         {
//             return clusterCache.TryGetValue(Tuple.Create(positions, clusterThreshold), out int largest) ? largest : 0;
//         }

//         var positionsSet = new HashSet<(int, int)>(positions.Item1);
//         int largestCluster = 0;

//         while (positionsSet.Count > 0)
//         {
//             var start = positionsSet.First();
//             positionsSet.Remove(start);

//             var queue = new Queue<(int, int)>();
//             queue.Enqueue(start);
//             int clusterSize = 1;

//             while (queue.Count > 0)
//             {
//                 var current = queue.Dequeue();
//                 var toRemove = new List<(int, int)>();
//                 foreach (var pos in positionsSet)
//                 {
//                     if (ManhattanDistance(current, pos) <= clusterThreshold)
//                     {
//                         queue.Enqueue(pos);
//                         toRemove.Add(pos);
//                         clusterSize++;
//                     }
//                 }
//                 foreach (var pos in toRemove)
//                 {
//                     positionsSet.Remove(pos);
//                 }
//             }

//             largestCluster = Math.Max(largestCluster, clusterSize);
//         }

//         clusterCache[Tuple.Create(positions, clusterThreshold)] = largestCluster;
//         return largestCluster;
//     }

//     private double CalculateAvgDistanceToPoint(List<(int, int)> positions, (int, int) target)
//     {
//         if (positions.Count == 0) return 0;
//         double distances = 0;
//         foreach (var pos in positions)
//         {
//             distances += ManhattanDistance(pos, target);
//         }
//         return distances / positions.Count;
//     }

//     private double CalculateDistributionScore(List<(int, int)> positions)
//     {
//         if (positions.Count == 0) return 0;

//         int midRow = Rows / 2;
//         int midCol = Cols / 2;
//         int[] quadrants = new int[4];

//         foreach (var (x, y) in positions)
//         {
//             int quadIdx = (x >= midRow ? 2 : 0) + (y >= midCol ? 1 : 0);
//             quadrants[quadIdx]++;
//         }

//         double avg = positions.Count / 4.0;
//         double variance = quadrants.Select(q => Math.Pow(q - avg, 2)).Sum() / 4.0;
//         return Math.Sqrt(variance);
//     }

//     private double CalculatePathComplexity(string color)
//     {
//         if (!EaterEntityPositions.TryGetValue(color, out var EaterEntityPos)) return 0;
//         if (!FoodEntityByColor.TryGetValue(color, out var FoodEntitys) || FoodEntitys.Count == 0) return 0;

//         var cacheKey = Tuple.Create(EaterEntityPos, FoodEntitys.OrderBy(f => f.Item1).ThenBy(f => f.Item2).ToArray().ToTuple());
//         if (pathCache.TryGetValue(cacheKey, out double cachedComplexity)) return cachedComplexity;

//         var currentPos = EaterEntityPos;
//         var remainingFoodEntitys = new HashSet<(int, int)>(FoodEntitys);
//         double totalDistance = 0;
//         int directionChanges = 0;
//         string prevDirection = null;

//         while (remainingFoodEntitys.Count > 0)
//         {
//             var nearestFoodEntity = remainingFoodEntitys.OrderBy(pos => ManhattanDistance(currentPos, pos)).First();
//             remainingFoodEntitys.Remove(nearestFoodEntity);

//             int dx = nearestFoodEntity.Item1 - currentPos.Item1;
//             int dy = nearestFoodEntity.Item2 - currentPos.Item2;

//             if (dx != 0 && dy != 0) directionChanges++;

//             string currentDirection = null;
//             if (Math.Abs(dx) > Math.Abs(dy)) currentDirection = "horizontal";
//             else if (Math.Abs(dy) > Math.Abs(dx)) currentDirection = "vertical";

//             if (prevDirection != null && currentDirection != null && prevDirection != currentDirection) directionChanges++;
//             prevDirection = currentDirection;

//             totalDistance += Math.Abs(dx) + Math.Abs(dy);
//             currentPos = nearestFoodEntity;
//         }

//         double complexity = totalDistance * 0.5 + directionChanges * 2.0;
//         pathCache[cacheKey] = complexity;
//         return complexity;
//     }

//     private double CalculateEaterEntityInteractions()
//     {
//         var EaterEntitys = EaterEntityPositions.ToList();
//         if (EaterEntitys.Count < 2) return 0;

//         double interactionScore = 0;

//         for (int i = 0; i < EaterEntitys.Count - 1; i++)
//         {
//             for (int j = i + 1; j < EaterEntitys.Count; j++)
//             {
//                 var (color1, pos1) = EaterEntitys[i];
//                 var (color2, pos2) = EaterEntitys[j];

//                 int distance = ManhattanDistance(pos1, pos2);
//                 if (distance < Rows + Cols / 3)
//                 {
//                     interactionScore += (Rows + Cols) / (distance + 1);
//                 }

//                 var FoodEntitys1 = FoodEntityByColor.TryGetValue(color1, out var f1) ? f1.ToHashSet() : new HashSet<(int, int)>();
//                 var FoodEntitys2 = FoodEntityByColor.TryGetValue(color2, out var f2) ? f2.ToHashSet() : new HashSet<(int, int)>();

//                 foreach (var f1Pos in FoodEntitys1)
//                 {
//                     foreach (var f2Pos in FoodEntitys2)
//                     {
//                         if (ManhattanDistance(pos1, f2Pos) + ManhattanDistance(pos2, f1Pos) <
//                             ManhattanDistance(pos1, f1Pos) + ManhattanDistance(pos2, f2Pos))
//                         {
//                             interactionScore += 5;
//                         }
//                     }
//                 }
//             }
//         }

//         foreach (var (color, pos) in EaterEntitys)
//         {
//             var otherFoodEntitys = new List<(int, int)>();
//             foreach (var (c, FoodEntitys) in FoodEntityByColor)
//             {
//                 if (c != color && c != "White")
//                 {
//                     otherFoodEntitys.AddRange(FoodEntitys);
//                 }
//             }

//             int nearbyOtherFoodEntitys = otherFoodEntitys.Count(f => ManhattanDistance(pos, f) < 3);
//             interactionScore += nearbyOtherFoodEntitys * 2;
//         }

//         return interactionScore;
//     }

//     private double CalculateFinalScore(Dictionary<string, Dictionary<string, object>> metrics, double interactionComplexity, double emptyCellsScore)
//     {
//         var scores = new List<double>();
//         foreach (var colorMetrics in metrics.Values)
//         {
//             if (colorMetrics.TryGetValue("disqualified", out object disqualifiedObj) && (bool)disqualifiedObj) continue;

//             double colorScore = (
//                 (Weights["FoodEntity_separation"] / ((double)colorMetrics["avg_FoodEntity_separation"] + 0.1)) +
//                 ((int)colorMetrics["largest_cluster_size"] * Weights["cluster_size"]) +
//                 (Weights["clusters_count"] / ((int)colorMetrics["clusters_count"] + 0.1)) +
//                 ((double)colorMetrics["distribution_score"] * Weights["distribution"]) +
//                 (Weights["distance_to_EaterEntity"] / ((double)colorMetrics["avg_distance_to_EaterEntity"] + 1)) +
//                 ((double)colorMetrics["path_complexity"] * Weights["path_complexity"])
//             );
//             scores.Add(colorScore);
//         }

//         double baseScore = scores.Count > 0 ? scores.Average() : double.PositiveInfinity;

//         return baseScore +
//                (interactionComplexity * Weights["interaction_complexity"]) +
//                (emptyCellsScore * Weights["empty_cells"]);
//     }

//     public void SetWeights(Dictionary<string, double> newWeights)
//     {
//         foreach (var (key, value) in newWeights)
//         {
//             if (Weights.ContainsKey(key))
//             {
//                 Weights[key] = value;
//             }
//         }
//     }

//     public void Place(Entity ent, int x, int y)
//     {
//         GridCells[x][y].Ent = ent;
//         if (ent is FoodEntity FoodEntity)
//         {
//             if (!FoodEntityByColor.ContainsKey(FoodEntity.Color))
//             {
//                 FoodEntityByColor[FoodEntity.Color] = new List<(int, int)>();
//             }
//             FoodEntityByColor[FoodEntity.Color].Add((x, y));
//         }
//         else if (ent is EaterEntity EaterEntity)
//         {
//             EaterEntityPositions[EaterEntity.Color] = (x, y);
//         }
//     }

//     public void Clear()
//     {
//         foreach (var row in GridCells)
//         {
//             foreach (var cell in row)
//             {
//                 cell.Ent = null;
//             }
//         }
//         distanceCache.Clear();
//         clusterCache.Clear();
//         pathCache.Clear();
//         FoodEntityByColor.Clear();
//         EaterEntityPositions.Clear();
//     }

//     public void GenPuzzle(Dictionary<string, int> EaterEntitys, int whitePct, int minFoodEntity)
//     {
//         Clear();
//         var EaterEntityPos = new List<Dictionary<string, object>>();

//         var colors = EaterEntitys.SelectMany(pair => Enumerable.Repeat(pair.Key, pair.Value)).ToList();
//         var rand = new Random();
//         colors = colors.OrderBy(a => rand.Next()).ToList();

//         foreach (var color in colors)
//         {
//             var pos = GetEmpty();
//             if (pos == null) break;
//             var (x, y) = pos.Value;
//             var e = new EaterEntity(color);
//             Place(e, x, y);
//             EaterEntityPos.Add(new Dictionary<string, object> { { "e", e }, { "x", x }, { "y", y }, { "count", 0 } });
//         }

//         var activeEaterEntitys = new List<Dictionary<string, object>>(EaterEntityPos);
//         while (activeEaterEntitys.Count > 0)
//         {
//             bool movedAny = false;
//             activeEaterEntitys = activeEaterEntitys.OrderBy(a => rand.Next()).ToList();

//             for (int i = activeEaterEntitys.Count - 1; i >= 0; i--)
//             {
//                 var einfo = activeEaterEntitys[i];
//                 if ((int)einfo["count"] >= minFoodEntity)
//                 {
//                     activeEaterEntitys.RemoveAt(i);
//                     continue;
//                 }

//                 if (MoveEaterEntityStrategically(einfo, EaterEntityPos))
//                 {
//                     movedAny = true;
//                 }
//                 else
//                 {
//                     activeEaterEntitys.RemoveAt(i);
//                 }
//             }

//             if (!movedAny) break;
//         }

//         int remainingSpaces = CountEmpty();
//         activeEaterEntitys = EaterEntityPos.Where(e => CanMoveEaterEntity(e)).ToList();
//         activeEaterEntitys = activeEaterEntitys.OrderBy(a => rand.Next()).ToList();

//         int failures = 0;
//         int maxFailures = 10;

//         while (remainingSpaces > 0 && activeEaterEntitys.Count > 0 && failures < maxFailures)
//         {
//             bool movedAny = false;

//             for (int i = activeEaterEntitys.Count - 1; i >= 0; i--)
//             {
//                 var einfo = activeEaterEntitys[i];
//                 if (MoveEaterEntityStrategically(einfo, EaterEntityPos))
//                 {
//                     movedAny = true;
//                     remainingSpaces--;
//                     failures = 0;
//                 }
//                 else
//                 {
//                     activeEaterEntitys.RemoveAt(i);
//                 }

//                 if (remainingSpaces <= 0) break;
//             }

//             if (!movedAny) failures++;

//             activeEaterEntitys = EaterEntityPos.Where(e => CanMoveEaterEntity(e)).OrderBy(a => rand.Next()).ToList();
//         }

//         WhitenFoodEntity(whitePct);
//     }

//     private bool CanMoveEaterEntity(Dictionary<string, object> einfo)
//     {
//         foreach (var (dx, dy) in DIRS)
//         {
//             int x = (int)einfo["x"];
//             int y = (int)einfo["y"];
//             var (newX, newY) = GetMaxPos(x, y, dx, dy);
//             if (newX != null && ((int)newX != x || (int)newY != y))
//             {
//                 return true;
//             }
//         }
//         return false;
//     }

//     private bool MoveEaterEntityStrategically(Dictionary<string, object> einfo, List<Dictionary<string, object>> allEaterEntitys)
//     {
//         var rand = new Random();
//         var shuffledDirs = DIRS.OrderBy(a => rand.Next()).ToList();
//         (int?, int?) bestMove = (null, null);
//         double bestScore = double.NegativeInfinity;

//         var otherEaterEntitys = allEaterEntitys.Where(e => ((EaterEntity)e["e"]).Color != ((EaterEntity)einfo["e"]).Color).Select(e => ((int)e["x"], (int)e["y"])).ToList();

//         foreach (var (dx, dy) in shuffledDirs)
//         {
//             int x = (int)einfo["x"];
//             int y = (int)einfo["y"];
//             var (newX, newY) = GetMaxPos(x, y, dx, dy);

//             if (newX == null || ((int)newX == x && (int)newY == y)) continue;

//             var oldEnt = GridCells[x][y].Ent;
//             GridCells[x][y].Ent = new FoodEntity(((EaterEntity)einfo["e"]).Color);

//             double moveScore = EvaluateFoodEntityPlacement(x, y, ((EaterEntity)einfo["e"]).Color);

//             foreach (var (ex, ey) in otherEaterEntitys)
//             {
//                 int distanceToOther = ManhattanDistance((x, y), (ex, ey));
//                 if (distanceToOther < 3) moveScore += 20;
//                 else if (distanceToOther < 5) moveScore += 10;
//             }

//             GridCells[x][y].Ent = oldEnt;

//             if (moveScore > bestScore)
//             {
//                 bestScore = moveScore;
//                 bestMove = (newX, newY);
//             }
//         }

//         if (bestMove.Item1 == null)
//         {
//             foreach (var (dx, dy) in DIRS)
//             {
//                 int x = (int)einfo["x"];
//                 int y = (int)einfo["y"];
//                 var (newX, newY) = GetMaxPos(x, y, dx, dy);
//                 if (newX != null && ((int)newX != x || (int)newY != y))
//                 {
//                     bestMove = (newX, newY);
//                     break;
//                 }
//             }
//         }

//         if (bestMove.Item1 != null)
//         {
//             int newX = (int)bestMove.Item1;
//             int newY = (int)bestMove.Item2;
//             int x = (int)einfo["x"];
//             int y = (int)einfo["y"];

//             bool isFirstFoodEntity = (int)einfo["count"] == 0;

//             GridCells[x][y].Ent = null;
//             Place((EaterEntity)einfo["e"], newX, newY);
//             Place(new FoodEntity(((EaterEntity)einfo["e"]).Color, is_last: isFirstFoodEntity), x, y);
//             einfo["count"] = (int)einfo["count"] + Math.Abs(newX - x) + Math.Abs(newY - y);

//             Moves.Add(new Dictionary<string, object>
//             {
//                 { "EaterEntity_color", ((EaterEntity)einfo["e"]).Color },
//                 { "start", (newX, newY) },
//                 { "end", (x, y) },
//                 { "FoodEntity_color", ((EaterEntity)einfo["e"]).Color },
//                 { "dist", Math.Abs(newX - x) + Math.Abs(newY - y) }
//             });

//             einfo["x"] = newX;
//             einfo["y"] = newY;

//             pathCache.Clear();
//             return true;
//         }

//         return false;
//     }

//     private double EvaluateFoodEntityPlacement(int x, int y, string color)
//     {
//         if (!FoodEntityByColor.TryGetValue(color, out var sameColorFoodEntitys)) return 100;

//         int minDistance = sameColorFoodEntitys.Min(pos => ManhattanDistance((x, y), pos));

//         var tempFoodEntitys = new List<(int, int)>(sameColorFoodEntitys) { (x, y) };
//         var tempFoodEntitysTuple = tempFoodEntitys.OrderBy(f => f.Item1).ThenBy(f => f.Item2).ToArray().ToTuple();

//         int clusterSize = FindLargestCluster(tempFoodEntitysTuple);
//         int clustersCount = CountClusters(tempFoodEntitysTuple);

//         if (clusterSize == tempFoodEntitys.Count && tempFoodEntitys.Count > 1) return -1000;

//         double separationScore = minDistance >= MinSeparation ? 50 + minDistance * 10 : 20 - (MinSeparation - minDistance) * 15;
//         double clusterScore = clustersCount * 20;
//         double pathScore = 0;

//         if (EaterEntityPositions.TryGetValue(color, out var EaterEntityPos))
//         {
//             int dx1 = Math.Abs(EaterEntityPos.Item1 - x);
//             int dy1 = Math.Abs(EaterEntityPos.Item2 - y);
//             if (dx1 > 0 && dy1 > 0) pathScore += 30;

//             int dist = ManhattanDistance(EaterEntityPos, (x, y));
//             if (3 <= dist && dist <= 6) pathScore += 25;
//             else if (dist > 6) pathScore += 15;
//         }

//         return separationScore + clusterScore + pathScore;
//     }

//     private (int?, int?) GetMaxPos(int x, int y, int dx, int dy)
//     {
//         int nx = x, ny = y;
//         while (InBounds(nx + dx, ny + dy) && GridCells[nx + dx][ny + dy].Empty())
//         {
//             nx += dx;
//             ny += dy;
//         }
//         return (nx, ny) != (x, y) ? (nx, ny) : (null, null);
//     }

//     private void WhitenFoodEntity(int pct)
//     {
//         var FoodEntity = GridCells.SelectMany(row => row.Where(c => c.Ent is FoodEntity && ((FoodEntity)c.Ent).Color != "White" && !((FoodEntity)c.Ent).IsLast).Select(c => (c.X, c.Y))).ToList();
//         var rand = new Random();
//         var toWhite = FoodEntity.OrderBy(a => rand.Next()).Take((int)(FoodEntity.Count * pct / 100.0)).ToList();
//         foreach (var (x, y) in toWhite)
//         {
//             GridCells[x][y].Ent.Color = "White";
//         }
//     }

//     private (int, int)? GetEmpty()
//     {
//         var empty = GridCells.SelectMany(row => row.Where(cell => cell.Empty()).Select(cell => (cell.X, cell.Y))).ToList();
//         if (empty.Count == 0) return null;
//         var rand = new Random();
//         return empty.OrderBy(a => rand.Next()).First();
//     }

//     private bool InBounds(int x, int y)
//     {
//         return 0 <= x && x < Rows && 0 <= y && y < Cols;
//     }

//     private int CountEmpty()
//     {
//         return GridCells.Sum(row => row.Count(cell => cell.Empty()));
//     }

//     public string ToScene(int levelId, bool fillEmpty)
//     {
//         int width = 1440, height = 2560;
//         int step = Math.Max(180, width / Cols - 24);

//         double startX = width / 2.0 - step * Cols / 2.0 + step * 0.5;
//         double startY = height / 2.0 - step * Rows / 2.0 + step * 0.5 - 40;

//         string scene = $"[gd_scene load_steps=4 format=3 uid=\"{levelId}\"]\n\n" +
//                        $"[ext_resource type=\"Script\" path=\"res://Levels/Level.cs\" id=\"lvlid\"]\n" +
//                        $"[ext_resource type=\"PackedScene\" uid=\"uid://byatslmwbvorg\" path=\"res://Entities/FoodEntity/FoodEntity.tscn\" id=\"FoodEntityid\"]\n" +
//                        $"[ext_resource type=\"PackedScene\" uid=\"uid://dd570jgysudow\" path=\"res://Entities/EaterEntity/EaterEntity.tscn\" id=\"EaterEntityid\"]\n\n" +
//                        $"[node name=\"Level\" type=\"Node\"]\n" +
//                        $"script = ExtResource(\"lvlid\")\n" +
//                        $"{SolutionMetadata()}\n\n" +
//                        $"[node name=\"FoodEntity\" type=\"Node\" parent=\".\"]\n\n" +
//                        $"[node name=\"EaterEntitys\" type=\"Node\" parent=\".\"]\n";

//         for (int x = 0; x < Rows; x++)
//         {
//             for (int y = 0; y < Cols; y++)
//             {
//                 var cell = GridCells[x][y];
//                 double posX = startX + y * step;
//                 double posY = startY + x * step;

//                 if (cell.Ent is FoodEntity FoodEntity)
//                 {
//                     scene += $"\n[node name=\"FoodEntity{x}_{y}\" parent=\"FoodEntity\" instance=ExtResource(\"FoodEntityid\")]\n" +
//                              $"position = Vector2({posX}, {posY})\n" +
//                              $"BoardStatePositionId = Vector2i({x}, {y})\n" +
//                              $"FoodEntityType = {COLOR_MAP["FoodEntity"][FoodEntity.Color]}\n" +
//                              $"IsLast = {FoodEntity.IsLast.ToString().ToLower()}\n";
//                 }
//                 else if (cell.Ent is EaterEntity EaterEntity)
//                 {
//                     scene += $"\n[node name=\"EaterEntity{x}_{y}\" parent=\"EaterEntitys\" instance=ExtResource(\"EaterEntityid\")]\n" +
//                              $"position = Vector2({posX}, {posY})\n" +
//                              $"EaterEntityType = {COLOR_MAP["EaterEntity"][EaterEntity.Color]}\n" +
//                              $"BoardStatePositionId = Vector2i({x}, {y})\n" +
//                              $"ValidFoodEntityTypes = [0, {COLOR_MAP["FoodEntity"][EaterEntity.Color]}]\n";
//                 }
//                 else if (cell.Ent == null && fillEmpty)
//                 {
//                     scene += $"\n[node name=\"FoodEntity{x}_{y}\" parent=\"FoodEntity\" instance=ExtResource(\"FoodEntityid\")]\n" +
//                              $"position = Vector2({posX}, {posY})\n" +
//                              $"BoardStatePositionId = Vector2i({x}, {y})\n" +
//                              $"FoodEntityType = 0\n" +
//                              $"IsLast = false\n";
//                 }
//             }
//         }
//         return scene;
//     }

//     private string SolutionMetadata()
//     {
//         string metadata = "metadata/solution = PackedVector2Array(";
//         foreach (var move in Moves)
//         {
//             var start = ((int, int))move["start"];
//             var end = ((int, int))move["end"];
//             metadata += $"{start.Item1}, {start.Item2}, {end.Item1}, {end.Item2}, ";
//         }
//         metadata = metadata.TrimEnd(',', ' ') + ")";
//         return metadata;
//     }

//     public static List<Dictionary<string, object>> GenBoards(int numSelect, int totalGen, int rows, int cols, Dictionary<string, int> EaterEntitys, int whitePct, int minFoodEntity, int minimumQualifiedColors, Dictionary<string, double> weights = null)
//     {
//         var boards = new List<Dictionary<string, object>>();
//         int attempts = 0;
//         int maxTries = totalGen * 100;

//         var diverseBoards = new List<List<Dictionary<string, object>>>();
//         var scoreRanges = new List<(double, double)> { (double.NegativeInfinity, 0), (0, 10), (10, 20), (20, double.PositiveInfinity) };
//         foreach (var scoreRange in scoreRanges)
//         {
//             diverseBoards.Add(new List<Dictionary<string, object>>());
//         }

//         while (boards.Count < totalGen && attempts < maxTries)
//         {
//             var g = new Grid(rows, cols, weights);
//             g.GenPuzzle(EaterEntitys, whitePct, minFoodEntity);

//             int emptyCount = g.CountEmpty();

//             if (EaterEntitys.All(pair =>
//                 g.GridCells.SelectMany(row => row).Count(cell => cell.Ent is FoodEntity FoodEntity && FoodEntity.Color == pair.Key) >= minFoodEntity))
//             {
//                 var clusteringInfo = g.CalculateClusteringScore(minimumQualifiedColors);

//                 if ((bool)clusteringInfo["disqualified"])
//                 {
//                     attempts++;
//                     continue;
//                 }

//                 var boardInfo = new Dictionary<string, object>
//                 {
//                     { "board", g },
//                     { "moves", g.Moves.ToList() },
//                     { "empty", emptyCount },
//                     { "clustering_score", clusteringInfo["final_score"] },
//                     { "metrics", clusteringInfo["color_metrics"] },
//                     { "interaction_complexity", clusteringInfo.TryGetValue("interaction_complexity", out object ic) ? ic : 0 },
//                     { "empty_cells_score", clusteringInfo.TryGetValue("empty_cells_score", out object ecs) ? ecs : 0 }
//                 };

//                 double totalScore = (double)clusteringInfo["final_score"] +
//                                     (double)(boardInfo["interaction_complexity"]) * (weights != null && weights.ContainsKey("interaction_complexity") ? weights["interaction_complexity"] : 1.5) +
//                                     (double)(boardInfo["empty_cells_score"]) * (weights != null && weights.ContainsKey("empty_cells") ? weights["empty_cells"] : 2.0);

//                 for (int i = 0; i < scoreRanges.Count; i++)
//                 {
//                     if (scoreRanges[i].Item1 <= totalScore && totalScore < scoreRanges[i].Item2)
//                     {
//                         if (diverseBoards[i].Count < numSelect / scoreRanges.Count + 1)
//                         {
//                             diverseBoards[i].Add(boardInfo);
//                         }
//                         break;
//                     }
//                 }

//                 boards.Add(boardInfo);
//             }
//             attempts++;
//         }

//         var finalSelection = new List<Dictionary<string, object>>();
//         foreach (var difficultyGroup in diverseBoards)
//         {
//             finalSelection.AddRange(difficultyGroup);
//         }

//         if (finalSelection.Count < numSelect)
//         {
//             var remainingBoards = boards.Where(b => !finalSelection.Contains(b)).ToList();
//             remainingBoards.Sort((x, y) =>
//             {
//                 double scoreX = (double)x["clustering_score"] + (double)x["interaction_complexity"] * (weights != null && weights.ContainsKey("interaction_complexity") ? weights["interaction_complexity"] : 1.5) + (double)x["empty_cells_score"] * (weights != null && weights.ContainsKey("empty_cells") ? weights["empty_cells"] : 2.0);
//                 double scoreY = (double)y["clustering_score"] + (double)y["interaction_complexity"] * (weights != null && weights.ContainsKey("interaction_complexity") ? weights["interaction_complexity"] : 1.5) + (double)y["empty_cells_score"] * (weights != null && weights.ContainsKey("empty_cells") ? weights["empty_cells"] : 2.0);
//                 return scoreX.CompareTo(scoreY);
//             });
//             finalSelection.AddRange(remainingBoards.Take(numSelect - finalSelection.Count));
//         }

//         finalSelection.Sort((x, y) =>
//         {
//             double scoreX = (double)x["clustering_score"] + (double)x["interaction_complexity"] * (weights != null && weights.ContainsKey("interaction_complexity") ? weights["interaction_complexity"] : 1.5) + (double)x["empty_cells_score"] * (weights != null && weights.ContainsKey("empty_cells") ? weights["empty_cells"] : 2.0);
//             double scoreY = (double)y["clustering_score"] + (double)y["interaction_complexity"] * (weights != null && weights.ContainsKey("interaction_complexity") ? weights["interaction_complexity"] : 1.5) + (double)y["empty_cells_score"] * (weights != null && weights.ContainsKey("empty_cells") ? weights["empty_cells"] : 2.0);
//             return scoreX.CompareTo(scoreY);
//         });
//         return finalSelection.Take(numSelect).ToList();
//     }

//     public static void Main(string[] args)
//     {
//         var DIFFICULTY = new Dictionary<string, (Dictionary<string, int>, int, int, int, int, int, int)>
//         {
//             { "tutorial", (new Dictionary<string, int> { { "Green", 1 } }, 3, 3, 5, 30, 1, 0) },
//             { "very_easy", (new Dictionary<string, int> { { "Blue", 1 }, { "Green", 1 } }, 4, 4, 15, 30, 2, 0) },
//             { "very_easy2", (new Dictionary<string, int> { { "Blue", 1 }, { "Green", 1 } }, 5, 4, 15, 30, 2, 1) },
//             { "easy", (new Dictionary<string, int> { { "Blue", 1 }, { "Green", 1 }, { "Red", 1 } }, 5, 5, 15, 30, 3, 2) },
//             { "easy2", (new Dictionary<string, int> { { "Blue", 1 }, { "Green", 1 }, { "Red", 1 } }, 6, 5, 15, 30, 3, 3) },
//             { "medium", (new Dictionary<string, int> { { "Blue", 1 }, { "Green", 1 }, { "Red", 1 }, { "Yellow", 1 } }, 6, 6, 30, 20, 3, 4) },
//             { "medium2", (new Dictionary<string, int> { { "Blue", 1 }, { "Green", 1 }, { "Red", 1 }, { "Yellow", 1 } }, 7, 6, 30, 20, 3, 4) },
//             { "hard", (new Dictionary<string, int> { { "Blue", 1 }, { "Green", 1 }, { "Red", 1 }, { "Yellow", 1 }, { "Pink", 1 } }, 7, 7, 30, 20, 3, 5) },
//             { "hard2", (new Dictionary<string, int> { { "Blue", 1 }, { "Green", 1 }, { "Red", 1 }, { "Yellow", 1}, { "Pink", 1 } }, 8, 7, 30, 20, 4, 5) },
//             { "veryhard", (new Dictionary<string, int> { { "Blue", 1 }, { "Green", 1 }, { "Red", 1 }, { "Yellow", 1 }, { "Pink", 1 }, { "Purple", 1 } }, 9, 7, 45, 20, 4, 6) },
//             { "veryhard2", (new Dictionary<string, int> { { "Blue", 1 }, { "Green", 1 }, { "Red", 1 }, { "Yellow", 1 }, { "Pink", 1 }, { "Purple", 1 } }, 10, 7, 45, 20, 4, 6) },
//             { "max", (new Dictionary<string, int> { { "Blue", 1 }, { "Green", 1 }, { "Red", 1 }, { "Yellow", 1 }, { "Pink", 1 }, { "Purple", 1 } }, 11, 7, 45, 15, 4, 7) },
//             { "max2", (new Dictionary<string, int> { { "Blue", 1 }, { "Green", 1 }, { "Red", 1 }, { "Yellow", 1 }, { "Pink", 1 }, { "Purple", 1 } }, 11, 7, 45, 10, 5, 7) },
//             { "max3", (new Dictionary<string, int> { { "Blue", 1 }, { "Green", 1 }, { "Red", 1 }, { "Yellow", 1 }, { "Pink", 1 }, { "Purple", 1 }, { "Brown", 1 } }, 11, 7, 45, 15, 4, 7) },
//             { "max4", (new Dictionary<string, int> { { "Blue", 1 }, { "Green", 1 }, { "Red", 1 }, { "Yellow", 1 }, { "Pink", 1 }, { "Purple", 1 }, { "Brown", 1 } }, 11, 7, 45, 10, 5, 7) }
//         };
//         var CUSTOM_WEIGHTS = new Dictionary<string, double>
//         {
//             { "FoodEntity_separation", 4.0 },
//             { "cluster_size", 0.33 },
//             { "clusters_count", 1.0 },
//             { "distribution", 1.0 },
//             { "distance_to_EaterEntity", 1.2 },
//             { "path_complexity", 1.5 },
//             { "interaction_complexity", 1.2 },
//             { "empty_cells", 1.3 }
//         };

//         int lvlId = 1;
//         DateTime ts = DateTime.Now;
//         Console.WriteLine($"[{ts}] Starting");
//         foreach (var (diff, (EaterEntitys, rows, cols, numSelect, whitePct, minFoodEntity, minimumQualifiedColors)) in DIFFICULTY)
//         {
//             DateTime startTime = DateTime.Now;
//             var newBoards = new List<Dictionary<string, object>>();
//             while (newBoards.Count < numSelect)
//             {
//                 newBoards.AddRange(GenBoards(numSelect - newBoards.Count, 3000, rows, cols, EaterEntitys, whitePct, minFoodEntity, minimumQualifiedColors, CUSTOM_WEIGHTS));
//             }
//             DateTime endTime = DateTime.Now;
//             double avgScore = newBoards.Average(board => (double)board["clustering_score"]);
//             double avgInteraction = newBoards.Average(board => board.TryGetValue("interaction_complexity", out object ic) ? (double)ic : 0);
//             double avgEmptyCells = newBoards.Average(board => board.TryGetValue("empty_cells", out object ec) ? (double)ec : 0);
//             Console.WriteLine("[{DateTime.Now}] Generated {newBoards.Count} new boards for difficulty {diff}");
//             Console.WriteLine("Averagescore:avgScore,Avginteraction:avgInteraction:F2,Averageemptycells:avgEmptyCells");Console.WriteLine("  Generation time: {endTime - startTime} seconds");
//             foreach (var board in newBoards)
//             {
//                 using (var writer = new System.IO.StreamWriter($"../Levels/Level{lvlId}.tscn"))
//                 {
//                     writer.Write(((Grid)board["board"]).ToScene(lvlId, false));
//                 }
//                 lvlId++;
//             }
//         }
//     }
// }