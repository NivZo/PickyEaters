using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class HintSystemBiDi
{
    // State for BFS search
    private class SearchState
    {
        public Vector2I[] EaterPositions { get; set; }
        public HashSet<Tuple<Vector2I, FoodType>> FoodState { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is SearchState other)) return false;
            
            // Compare eater positions
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
            int hash = 17;
            foreach (var pos in EaterPositions)
                hash = hash * 31 + pos.GetHashCode();
            
            foreach (var food in FoodState)
                hash = hash * 31 + food.GetHashCode();
            
            return hash;
        }
    }

    // Direction vectors
    private static readonly Vector2I[] DirectionVectors = new Vector2I[]
    {
        Vector2I.Up,
        Vector2I.Down,
        Vector2I.Left,
        Vector2I.Right
    };

    /// <summary>
    /// Generates a hint using bidirectional search
    /// </summary>
    public static List<HintMove> GetSolutionPath(Level level)
    {
        var eaters = level.GetEaters();
        var foods = level.GetFood();
        
        if (eaters.Count == 0 || foods.Count == 0)
            return new List<HintMove>();

        // Calculate playable area bounds once
        var bounds = CalculatePlayableAreaBounds(foods);

        // Create initial search state
        var initialState = new SearchState
        {
            EaterPositions = eaters.Select(e => e.BoardStatePositionId).ToArray(),
            FoodState = GetFoodStateHash(foods)
        };

        // Create goal state (no food)
        var goalState = new SearchState
        {
            EaterPositions = new Vector2I[eaters.Count], // Will be filled during backward search
            FoodState = new HashSet<Tuple<Vector2I, FoodType>>()
        };

        // Forward search from initial state
        var forwardQueue = new Queue<Tuple<SearchState, List<HintMove>>>();
        var forwardVisited = new Dictionary<string, Tuple<SearchState, List<HintMove>>>();
        
        // Backward search from goal state
        var backwardQueue = new PriorityQueue<Tuple<SearchState, List<HintMove>>, int>();
        var backwardVisited = new Dictionary<string, Tuple<SearchState, List<HintMove>>>();

        // Initialize forward search
        forwardQueue.Enqueue(Tuple.Create(initialState, new List<HintMove>()));
        forwardVisited[StateToString(initialState)] = Tuple.Create(initialState, new List<HintMove>());

        // Initialize backward search with multiple potential goal states
        InitializeBackwardSearch(eaters, foods, goalState, backwardQueue, backwardVisited, bounds);

        // Track best solution found
        List<HintMove> bestSolution = null;
        int bestSolutionLength = int.MaxValue;

        // Bidirectional search
        while (forwardQueue.Count > 0 && backwardQueue.Count > 0)
        {
            // Forward search step
            if (forwardQueue.Count > 0)
            {
                var (forwardState, forwardPath) = forwardQueue.Dequeue();
                string forwardStateStr = StateToString(forwardState);

                // Check if we've found a connection to backward search
                if (backwardVisited.ContainsKey(forwardStateStr))
                {
                    var (backState, backPath) = backwardVisited[forwardStateStr];
                    var completePath = MergePaths(forwardPath, backPath);

                    if (completePath.Count < bestSolutionLength)
                    {
                        bestSolution = completePath;
                        bestSolutionLength = completePath.Count;
                    }
                }

                // Continue forward search if we haven't reached max depth
                if (forwardPath.Count < 20)  // Prevent infinite paths
                {
                    ExpandForwardSearch(forwardState, forwardPath, eaters, foods, forwardQueue, 
                        forwardVisited, bounds);
                }
            }

            // Backward search step
            if (backwardQueue.Count > 0)
            {
                var (backwardState, backwardPath) = backwardQueue.Dequeue();
                string backwardStateStr = StateToString(backwardState);

                // Check if we've found a connection to forward search
                if (forwardVisited.ContainsKey(backwardStateStr))
                {
                    var (forwardState, forwardPath) = forwardVisited[backwardStateStr];
                    var completePath = MergePaths(forwardPath, backwardPath);

                    if (completePath.Count < bestSolutionLength)
                    {
                        bestSolution = completePath;
                        bestSolutionLength = completePath.Count;
                    }
                }

                // Continue backward search if we haven't reached max depth
                if (backwardPath.Count < 20)  // Prevent infinite paths
                {
                    ExpandBackwardSearch(backwardState, backwardPath, eaters, foods, backwardQueue, 
                        backwardVisited, bounds);
                }
            }

            // If we found a solution and the queues are getting too large, terminate early
            if (bestSolution != null && (forwardQueue.Count > 10000 || backwardQueue.Count > 10000))
                break;
        }

        // Link moves with food information
        if (bestSolution != null)
        {
            bestSolution.ForEach(move => {
                move.FoodAtTarget = foods.FirstOrDefault(food => food.BoardStatePositionId == move.To);
            });
        }

        return bestSolution ?? new List<HintMove>();
    }

    private static string StateToString(SearchState state)
    {
        string posStr = string.Join(",", state.EaterPositions.Select(p => $"{p.X},{p.Y}"));
        string foodStr = string.Join(",", state.FoodState.OrderBy(f => f.Item1.X).ThenBy(f => f.Item1.Y)
            .Select(f => $"{f.Item1.X},{f.Item1.Y},{(int)f.Item2}"));
        return $"{posStr}|{foodStr}";
    }

    private static List<HintMove> MergePaths(List<HintMove> forwardPath, List<HintMove> backwardPath)
    {
        // Combine forward path with reversed backward path
        var result = new List<HintMove>(forwardPath);
        for (int i = backwardPath.Count - 1; i >= 0; i--)
        {
            result.Add(backwardPath[i]);
        }
        return result;
    }

    private static void InitializeBackwardSearch(
        List<Eater> eaters, 
        List<Food> foods,
        SearchState goalState,
        PriorityQueue<Tuple<SearchState, List<HintMove>>, int> backwardQueue,
        Dictionary<string, Tuple<SearchState, List<HintMove>>> backwardVisited,
        (int minX, int minY, int maxX, int maxY) bounds)
    {
        // Group foods by type
        var foodByType = foods.GroupBy(f => f.FoodType).ToDictionary(g => g.Key, g => g.ToList());
        
        // For each eater, identify what types of food they can eat
        foreach (var eater in eaters)
        {
            var validTypes = eater.ValidFoodTypes.ToList();
            foreach (var type in validTypes)
            {
                if (!foodByType.ContainsKey(type) || foodByType[type].Count == 0)
                    continue;
                
                // Try each food of this type as a possible last target
                foreach (var food in foodByType[type])
                {
                    // Clone the goal state for this food/eater combination
                    var candidateGoalState = new SearchState
                    {
                        EaterPositions = (Vector2I[])goalState.EaterPositions.Clone(),
                        FoodState = new HashSet<Tuple<Vector2I, FoodType>>(goalState.FoodState)
                    };
                    
                    // Position the current eater at this food's position
                    int eaterIdx = eaters.IndexOf(eater);
                    for (int i = 0; i < eaters.Count; i++)
                    {
                        candidateGoalState.EaterPositions[i] = i == eaterIdx ? 
                            food.BoardStatePositionId : GetValidInitialPosition(bounds, candidateGoalState);
                    }
                    
                    var path = new List<HintMove>
                    {
                        new HintMove
                        {
                            Eater = eater,
                            From = food.BoardStatePositionId, // Starting from food in backward search
                            To = food.BoardStatePositionId    // Target is also food in backward search
                        }
                    };
                    
                    string stateStr = StateToString(candidateGoalState);
                    if (!backwardVisited.ContainsKey(stateStr))
                    {
                        backwardVisited[stateStr] = Tuple.Create(candidateGoalState, path);
                        backwardQueue.Enqueue(Tuple.Create(candidateGoalState, path), path.Count);
                    }
                }
            }
        }
    }

    private static Vector2I GetValidInitialPosition((int minX, int minY, int maxX, int maxY) bounds, SearchState state)
    {
        // Find a position that doesn't collide with other eaters or food
        for (int x = bounds.minX; x <= bounds.maxX; x++)
        {
            for (int y = bounds.minY; y <= bounds.maxY; y++)
            {
                var pos = new Vector2I(x, y);
                
                // Check if this position has food
                bool hasFood = state.FoodState.Any(f => f.Item1 == pos);
                if (hasFood)
                    continue;
                
                // Check if this position has an eater
                bool hasEater = state.EaterPositions.Any(p => p == pos);
                if (hasEater)
                    continue;
                
                return pos;
            }
        }
        
        // Fallback to a position outside the bounds
        return new Vector2I(bounds.minX - 2, bounds.minY - 2);
    }

    private static void ExpandForwardSearch(
        SearchState currentState,
        List<HintMove> currentPath,
        List<Eater> eaters,
        List<Food> foods,
        Queue<Tuple<SearchState, List<HintMove>>> queue,
        Dictionary<string, Tuple<SearchState, List<HintMove>>> visited,
        (int minX, int minY, int maxX, int maxY) bounds)
    {
        // Try all possible moves for each eater
        for (int eaterIdx = 0; eaterIdx < eaters.Count; eaterIdx++)
        {
            var eater = eaters[eaterIdx];
            var eaterPos = currentState.EaterPositions[eaterIdx];
            
            // Get valid moves for this eater
            var validMoves = GetValidMoves(currentState, eaters, foods, eaterIdx, bounds);
            
            foreach (var movePos in validMoves)
            {
                // Apply the move to get new state
                var newState = ApplyMove(currentState, eaters, foods, eaterIdx, movePos);
                string newStateStr = StateToString(newState);
                
                // If we haven't visited this state yet
                if (!visited.ContainsKey(newStateStr))
                {
                    // Create new path with this move
                    var newPath = new List<HintMove>(currentPath)
                    {
                        new HintMove
                        {
                            Eater = eater,
                            From = eaterPos,
                            To = movePos
                        }
                    };
                    
                    visited[newStateStr] = Tuple.Create(newState, newPath);
                    queue.Enqueue(Tuple.Create(newState, newPath));
                }
            }
        }
    }

    private static void ExpandBackwardSearch(
        SearchState currentState,
        List<HintMove> currentPath,
        List<Eater> eaters,
        List<Food> foods,
        PriorityQueue<Tuple<SearchState, List<HintMove>>, int> queue,
        Dictionary<string, Tuple<SearchState, List<HintMove>>> visited,
        (int minX, int minY, int maxX, int maxY) bounds)
    {
        // Prioritize eating food that matches the eater's preference
        // In backward search, we're "placing" food and "moving" eaters to previous positions
        for (int eaterIdx = 0; eaterIdx < eaters.Count; eaterIdx++)
        {
            var eater = eaters[eaterIdx];
            var eaterPos = currentState.EaterPositions[eaterIdx];
            var validFoodTypes = eater.ValidFoodTypes.ToList();
            
            // Try each direction
            foreach (var dir in DirectionVectors)
            {
                Vector2I prevPos = eaterPos;
                
                // Move in the opposite direction of the move (backward search)
                while (true)
                {
                    Vector2I nextPos = prevPos - dir; // Move in reverse direction
                    
                    // Stop if we hit another eater
                    if (currentState.EaterPositions.Any(p => p == nextPos && currentState.EaterPositions.ToList().IndexOf(p) != eaterIdx))
                        break;
                    
                    // Check if the position is within bounds
                    if (IsOffPlayableArea(nextPos, bounds))
                        break;
                    
                    // Try placing the eater here
                    var newState = new SearchState
                    {
                        EaterPositions = (Vector2I[])currentState.EaterPositions.Clone(),
                        FoodState = new HashSet<Tuple<Vector2I, FoodType>>(currentState.FoodState)
                    };
                    
                    newState.EaterPositions[eaterIdx] = nextPos;
                    
                    // Check if we can add food at the eater's current position
                    foreach (var foodType in validFoodTypes)
                    {
                        // Check if this is a valid food addition
                        if (IsValidFoodAddition(currentState, eaterPos, foodType))
                        {
                            var stateWithFood = new SearchState
                            {
                                EaterPositions = newState.EaterPositions,
                                FoodState = new HashSet<Tuple<Vector2I, FoodType>>(newState.FoodState)
                            };
                            
                            stateWithFood.FoodState.Add(Tuple.Create(eaterPos, foodType));
                            
                            string stateStr = StateToString(stateWithFood);
                            if (!visited.ContainsKey(stateStr))
                            {
                                var newPath = new List<HintMove>(currentPath)
                                {
                                    new HintMove
                                    {
                                        Eater = eater,
                                        From = nextPos,  // In backward search, "from" is the previous position
                                        To = eaterPos    // "to" is the current position
                                    }
                                };
                                
                                visited[stateStr] = Tuple.Create(stateWithFood, newPath);
                                queue.Enqueue(Tuple.Create(stateWithFood, newPath), newPath.Count + CountRemainingFood(stateWithFood));
                            }
                        }
                    }
                    
                    // Try just moving the eater without adding food
                    string newStateStr = StateToString(newState);
                    if (!visited.ContainsKey(newStateStr))
                    {
                        var newPath = new List<HintMove>(currentPath)
                        {
                            new HintMove
                            {
                                Eater = eater,
                                From = nextPos,
                                To = eaterPos
                            }
                        };
                        
                        visited[newStateStr] = Tuple.Create(newState, newPath);
                        queue.Enqueue(Tuple.Create(newState, newPath), newPath.Count + CountRemainingFood(newState));
                    }
                    
                    prevPos = nextPos;
                }
            }
        }
    }

    private static int CountRemainingFood(SearchState state)
    {
        return state.FoodState.Count;
    }

    private static bool IsValidFoodAddition(SearchState state, Vector2I pos, FoodType foodType)
    {
        // Check if there's already food at this position
        if (state.FoodState.Any(f => f.Item1 == pos))
            return false;
        
        // Don't place food on top of eaters
        if (state.EaterPositions.Contains(pos))
            return false;
        
        return true;
    }

    private static (int minX, int minY, int maxX, int maxY) CalculatePlayableAreaBounds(List<Food> foods)
    {
        if (foods.Count == 0)
            return (0, 0, 0, 0);
            
        // Find the bounds of the playable area using BoardStatePositionId
        int minX = foods.Min(f => f.BoardStatePositionId.X);
        int minY = foods.Min(f => f.BoardStatePositionId.Y);
        int maxX = foods.Max(f => f.BoardStatePositionId.X);
        int maxY = foods.Max(f => f.BoardStatePositionId.Y);
        
        // Add a margin of one cell
        minX -= 1;
        minY -= 1;
        maxX += 1;
        maxY += 1;
        
        return (minX, minY, maxX, maxY);
    }

    private static bool IsOffPlayableArea(Vector2I pos, (int minX, int minY, int maxX, int maxY) bounds)
    {
        return pos.X < bounds.minX || pos.Y < bounds.minY || pos.X > bounds.maxX || pos.Y > bounds.maxY;
    }

    /// <summary>
    /// Create a hashset representation of the current food state
    /// </summary>
    private static HashSet<Tuple<Vector2I, FoodType>> GetFoodStateHash(List<Food> foods)
    {
        var foodState = new HashSet<Tuple<Vector2I, FoodType>>();
        
        foreach (var food in foods)
        {
            foodState.Add(Tuple.Create(food.BoardStatePositionId, food.FoodType));
        }
        
        return foodState;
    }

    /// <summary>
    /// Get valid moves for an eater in the current state
    /// </summary>
    private static List<Vector2I> GetValidMoves(
        SearchState searchState, 
        List<Eater> eaters,
        List<Food> allFoods,
        int eaterIdx,
        (int minX, int minY, int maxX, int maxY) bounds)
    {
        var validMoves = new List<Vector2I>();
        var eater = eaters[eaterIdx];
        var eaterPos = searchState.EaterPositions[eaterIdx];
        var validFoodTypes = eater.ValidFoodTypes.ToList();
        
        // Create a lookup for food positions
        var foodLookup = new Dictionary<Vector2I, FoodType>();
        foreach (var foodTuple in searchState.FoodState)
        {
            foodLookup[foodTuple.Item1] = foodTuple.Item2;
        }
        
        // Create a lookup for eater positions
        var eaterPositions = new HashSet<Vector2I>();
        for (int i = 0; i < searchState.EaterPositions.Length; i++)
        {
            if (i != eaterIdx) // Exclude current eater
                eaterPositions.Add(searchState.EaterPositions[i]);
        }
        
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
                
                // Check if we're off the playable area
                if (IsOffPlayableArea(nextPos, bounds))
                    break;
                
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
        // Create new search state
        var newState = new SearchState
        {
            EaterPositions = (Vector2I[])currentState.EaterPositions.Clone(),
            FoodState = new HashSet<Tuple<Vector2I, FoodType>>(currentState.FoodState)
        };
        
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
}