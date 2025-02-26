using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class HintSystem
{
    // State for A* search
    private class SearchState
    {
        public Vector2I[] EaterPositions { get; set; }
        public HashSet<Tuple<Vector2I, FoodType>> FoodState { get; set; }
        
        // For memoization - store best path cost to reach this state
        public int BestPathCost { get; set; } = int.MaxValue;

        public override bool Equals(object obj)
        {
            if (!(obj is SearchState other)) return false;
            
            // Compare eater positions (order matters)
            if (EaterPositions.Length != other.EaterPositions.Length) return false;
            for (int i = 0; i < EaterPositions.Length; i++)
            {
                if (EaterPositions[i] != other.EaterPositions[i])
                    return false;
            }

            // Compare food states
            return FoodState.SetEquals(other.FoodState);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                foreach (var pos in EaterPositions)
                    hash = hash * 31 + pos.GetHashCode();
                
                // Use sorted tuples for consistent hash codes
                var sortedFood = FoodState.OrderBy(f => f.Item1.X)
                    .ThenBy(f => f.Item1.Y)
                    .ThenBy(f => (int)f.Item2);
                
                foreach (var food in sortedFood)
                    hash = hash * 31 + food.GetHashCode();
                
                return hash;
            }
        }

        // Clone state
        public SearchState Clone()
        {
            return new SearchState
            {
                EaterPositions = (Vector2I[])EaterPositions.Clone(),
                FoodState = new HashSet<Tuple<Vector2I, FoodType>>(FoodState),
                BestPathCost = BestPathCost
            };
        }
    }

    // A* node with priority queue support
    private class AStarNode : IComparable<AStarNode>
    {
        public SearchState State { get; set; }
        public List<HintMove> Path { get; set; }
        public int Cost { get; set; }         // g(n): cost so far
        public int Heuristic { get; set; }    // h(n): estimated cost to goal
        public int TotalCost => Cost + Heuristic; // f(n) = g(n) + h(n)

        public int CompareTo(AStarNode other)
        {
            // Lower total cost comes first
            int result = TotalCost.CompareTo(other.TotalCost);
            if (result != 0) return result;
            
            // If total costs are equal, prioritize state with fewer food items
            return State.FoodState.Count.CompareTo(other.State.FoodState.Count);
        }
    }

    // Direction vectors
    private static readonly Vector2I[] DirectionVectors = new Vector2I[]
    {
        Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right
    };

    // Memoization cache - stores dynamic programming results
    private static Dictionary<int, List<HintMove>> solutionCache = new Dictionary<int, List<HintMove>>();

    /// <summary>
    /// Generates a hint for the player by finding a solution path using A* search with dynamic programming
    /// </summary>
    /// <param name="level">The current level node containing eaters and food</param>
    /// <returns>A list of moves that solve the puzzle, or empty list if no solution exists</returns>
    public static List<HintMove> GetSolutionPath(Level level)
    {
        var eaters = level.GetEaters();
        var foods = level.GetFood();
        
        if (eaters.Count == 0 || foods.Count == 0)
            return new List<HintMove>();

        // Create initial search state
        var initialState = new SearchState
        {
            EaterPositions = eaters.Select(e => e.BoardStatePositionId).ToArray(),
            FoodState = GetFoodStateHash(foods),
            BestPathCost = 0
        };

        // Generate a cache key for this specific puzzle
        int cacheKey = GeneratePuzzleCacheKey(initialState);
        
        // Check memo cache first
        if (solutionCache.TryGetValue(cacheKey, out var cachedSolution))
        {
            return cachedSolution;
        }

        // A* priority queue and visited dictionary (for dynamic programming)
        var openSet = new PriorityQueue<AStarNode>();
        
        // State -> Best cost to reach that state
        var visited = new Dictionary<SearchState, int>(new SearchStateEqualityComparer());
        
        // Add initial state to priority queue
        var startNode = new AStarNode
        {
            State = initialState,
            Path = new List<HintMove>(),
            Cost = 0,
            Heuristic = CalculateHeuristic(initialState, eaters)
        };
        
        openSet.Enqueue(startNode);
        visited[initialState] = 0;

        int nodesExplored = 0;
        int maxQueueSize = 1;

        // A* search
        while (openSet.Count > 0)
        {
            Console.WriteLine($"Q size={openSet.Count}");
            maxQueueSize = Math.Max(maxQueueSize, openSet.Count);
            nodesExplored++;
            
            // Log search progress every 1000 nodes
            if (nodesExplored % 1000 == 0)
            {
                Console.WriteLine($"Nodes explored: {nodesExplored}, Queue size: {openSet.Count}, Max queue: {maxQueueSize}");
            }
            
            var current = openSet.Dequeue();
            
            // If we've reached a goal state (no food left)
            if (current.State.FoodState.Count == 0)
            {
                // Add food information to the moves
                foreach (var move in current.Path)
                {
                    move.FoodAtTarget = foods.FirstOrDefault(food => food.BoardStatePositionId == move.To);
                }
                
                // Cache the solution for future use
                solutionCache[cacheKey] = current.Path;
                
                Console.WriteLine($"Solution found after exploring {nodesExplored} nodes. Solution length: {current.Path.Count}");
                return current.Path;
            }

            // Process the state only if it's the best path to this state
            if (current.Cost > visited[current.State])
                continue;

            // Try all possible moves for each eater
            for (int eaterIdx = 0; eaterIdx < eaters.Count; eaterIdx++)
            {
                var eater = eaters[eaterIdx];
                var eaterPos = current.State.EaterPositions[eaterIdx];
                
                // Get valid moves for this eater
                var validMoves = GetValidMoves(current.State, eaters, foods, eaterIdx);
                
                foreach (var movePos in validMoves)
                {
                    // Apply the move to get new state
                    var newState = ApplyMove(current.State, eaters, foods, eaterIdx, movePos);
                    int moveCost = 1;  // Base cost for each move

                    // Calculate total cost to reach this new state
                    int newCost = current.Cost + moveCost;
                    
                    // If we've found a better path to this state
                    bool isNewState = !visited.ContainsKey(newState);
                    bool isBetterPath = isNewState || newCost < visited[newState];
                    
                    if (isBetterPath)
                    {
                        // Update best cost for this state
                        visited[newState] = newCost;
                        newState.BestPathCost = newCost;
                        
                        // Create new path with this move
                        var newPath = new List<HintMove>(current.Path)
                        {
                            new HintMove
                            {
                                Eater = eater,
                                From = eaterPos,
                                To = movePos
                            }
                        };
                        
                        // Create new A* node
                        var newNode = new AStarNode
                        {
                            State = newState,
                            Path = newPath,
                            Cost = newCost,
                            Heuristic = CalculateHeuristic(newState, eaters)
                        };
                        
                        openSet.Enqueue(newNode);
                    }
                }
            }
        }
        
        // No solution found - cache empty list to avoid repeated searches for this puzzle
        solutionCache[cacheKey] = new List<HintMove>();
        Console.WriteLine($"No solution found after exploring {nodesExplored} nodes.");
        return new List<HintMove>();
    }

    /// <summary>
    /// Calculate a heuristic estimate for A* search
    /// </summary>
    private static int CalculateHeuristic(SearchState state, List<Eater> eaters)
    {
        if (state.FoodState.Count == 0)
            return 0;  // Goal state has zero heuristic
            
        int minDistance = int.MaxValue;
        
        // For each piece of food, find the closest eater that can eat it
        foreach (var foodTuple in state.FoodState)
        {
            var foodPos = foodTuple.Item1;
            var foodType = foodTuple.Item2;
            int minFoodDistance = int.MaxValue;
            
            // Find the closest compatible eater
            for (int i = 0; i < eaters.Count; i++)
            {
                var eater = eaters[i];
                
                // Skip eaters that can't eat this food type
                if (!eater.ValidFoodTypes.Contains(foodType))
                    continue;
                    
                var eaterPos = state.EaterPositions[i];
                int distance = Math.Abs(eaterPos.X - foodPos.X) + Math.Abs(eaterPos.Y - foodPos.Y);
                
                minFoodDistance = Math.Min(minFoodDistance, distance);
            }
            
            // If no eater can eat this food, the puzzle is unsolvable
            if (minFoodDistance == int.MaxValue)
                return int.MaxValue;
                
            minDistance = Math.Min(minDistance, minFoodDistance);
        }
        
        // Return food count as base heuristic plus the minimum distance
        return state.FoodState.Count + minDistance - 1;
    }

    /// <summary>
    /// Generate a unique key for the puzzle for caching solutions
    /// </summary>
    private static int GeneratePuzzleCacheKey(SearchState initialState)
    {
        // Create a hash of the initial state that uniquely identifies this puzzle
        int hash = 17;
        
        // Hash eater positions
        foreach (var pos in initialState.EaterPositions)
        {
            hash = hash * 31 + pos.GetHashCode();
        }
        
        // Hash eater types (assuming they're in same order)
        hash = hash * 31 + initialState.EaterPositions.Length;
        
        // Hash food positions and types
        var sortedFood = initialState.FoodState.OrderBy(f => f.Item1.X)
            .ThenBy(f => f.Item1.Y)
            .ThenBy(f => (int)f.Item2);
            
        foreach (var food in sortedFood)
        {
            hash = hash * 31 + food.Item1.GetHashCode() * 31 + (int)food.Item2;
        }
        
        return hash;
    }

    /// <summary>
    /// Create a hashset representation of the current food state
    /// </summary>
    private static HashSet<Tuple<Vector2I, FoodType>> GetFoodStateHash(List<Food> foods)
    {
        return new HashSet<Tuple<Vector2I, FoodType>>(
            foods.Select(food => Tuple.Create(food.BoardStatePositionId, food.FoodType))
        );
    }

    /// <summary>
    /// Get valid moves for an eater in the current state
    /// </summary>
    private static List<Vector2I> GetValidMoves(
        SearchState searchState, 
        List<Eater> eaters,
        List<Food> allFoods,
        int eaterIdx)
    {
        var validMoves = new List<Vector2I>();
        var eater = eaters[eaterIdx];
        var eaterPos = searchState.EaterPositions[eaterIdx];
        var validFoodTypes = eater.ValidFoodTypes.ToList();
        
        // Create a lookup for food positions - much faster than checking the HashSet each time
        var foodLookup = new Dictionary<Vector2I, FoodType>();
        foreach (var foodTuple in searchState.FoodState)
        {
            foodLookup[foodTuple.Item1] = foodTuple.Item2;
        }
        
        // Create a lookup for other eater positions
        var eaterPositions = new HashSet<Vector2I>();
        for (int i = 0; i < searchState.EaterPositions.Length; i++)
        {
            if (i != eaterIdx) // Exclude current eater
                eaterPositions.Add(searchState.EaterPositions[i]);
        }
        
        // Optimization: Calculate playable area bounds once
        int minX, minY, maxX, maxY;
        CalculatePlayableAreaBounds(allFoods, out minX, out minY, out maxX, out maxY);
        
        // Check in all four directions
        foreach (var dir in DirectionVectors)
        {
            Vector2I currentPos = eaterPos;
            bool foundFood = false;
            
            // Look for food in this direction
            while (true)
            {
                Vector2I nextPos = currentPos + dir;
                
                // Stop if we hit another eater
                if (eaterPositions.Contains(nextPos))
                    break;
                
                // Stop if we're off the playable area
                if (IsOffPlayableArea(nextPos, minX, minY, maxX, maxY))
                    break;
                
                // Check if there's food at this position
                if (foodLookup.TryGetValue(nextPos, out FoodType foodType))
                {
                    // Check if eater can eat this food
                    if (validFoodTypes.Contains(foodType))
                    {
                        validMoves.Add(nextPos);
                    }
                    foundFood = true;
                    break;
                }
                
                // Move to next position
                currentPos = nextPos;
            }
            
            // If no food was found, eater can move to the end position (if it's different from start)
            if (!foundFood && currentPos != eaterPos)
            {
                validMoves.Add(currentPos);
            }
        }
        
        return validMoves;
    }

    /// <summary>
    /// Apply a move to the current state to get a new state
    /// </summary>
    private static SearchState ApplyMove(
        SearchState currentState,
        List<Eater> eaters,
        List<Food> foods,
        int eaterIdx,
        Vector2I toPos)
    {
        // Clone the current state
        var newState = currentState.Clone();
        
        // Move eater
        newState.EaterPositions[eaterIdx] = toPos;
        
        // Check if we're eating food
        var foodTuples = currentState.FoodState.Where(f => f.Item1 == toPos).ToList();
        var eater = eaters[eaterIdx];
        
        foreach (var foodTuple in foodTuples)
        {
            // Make sure we can eat this food
            if (eater.ValidFoodTypes.Contains(foodTuple.Item2))
            {
                newState.FoodState.Remove(foodTuple);
            }
        }
        
        return newState;
    }

    /// <summary>
    /// Calculate the bounds of the playable area using BoardStatePositionId values
    /// </summary>
    private static void CalculatePlayableAreaBounds(List<Food> foods,
        out int minX, out int minY, out int maxX, out int maxY)
    {
        if (foods.Count == 0)
        {
            minX = minY = maxX = maxY = 0;
            return;
        }
            
        // Find the bounds of the playable area using BoardStatePositionId
        minX = foods.Min(f => f.BoardStatePositionId.X) - 1;
        minY = foods.Min(f => f.BoardStatePositionId.Y) - 1;
        maxX = foods.Max(f => f.BoardStatePositionId.X) + 1;
        maxY = foods.Max(f => f.BoardStatePositionId.Y) + 1;
    }

    /// <summary>
    /// Check if a position is off the playable area using pre-calculated bounds
    /// </summary>
    private static bool IsOffPlayableArea(Vector2I pos, int minX, int minY, int maxX, int maxY)
    {
        return pos.X < minX || pos.Y < minY || pos.X > maxX || pos.Y > maxY;
    }

    /// <summary>
    /// Equality comparer for search states
    /// </summary>
    private class SearchStateEqualityComparer : IEqualityComparer<SearchState>
    {
        public bool Equals(SearchState x, SearchState y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }

        public int GetHashCode(SearchState obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// A simple priority queue implementation for A* search
    /// </summary>
    private class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> data;

        public PriorityQueue()
        {
            data = new List<T>();
        }

        public int Count => data.Count;

        public void Enqueue(T item)
        {
            data.Add(item);
            int ci = data.Count - 1;
            while (ci > 0)
            {
                int pi = (ci - 1) / 2;
                if (data[ci].CompareTo(data[pi]) >= 0)
                    break;
                T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
                ci = pi;
            }
        }

        public T Dequeue()
        {
            int li = data.Count - 1;
            T frontItem = data[0];
            data[0] = data[li];
            data.RemoveAt(li);

            if (li > 0)
            {
                --li;
                int pi = 0;
                while (true)
                {
                    int ci = pi * 2 + 1;
                    if (ci > li) break;
                    int rc = ci + 1;
                    if (rc <= li && data[rc].CompareTo(data[ci]) < 0)
                        ci = rc;
                    if (data[pi].CompareTo(data[ci]) <= 0) break;
                    T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp;
                    pi = ci;
                }
            }
            return frontItem;
        }
    }
}