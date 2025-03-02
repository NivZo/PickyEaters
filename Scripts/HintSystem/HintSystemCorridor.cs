using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class HintSystemCorridor
{
    // State for A* search
    private class SearchState
    {
        public Vector2I[] EaterPositions { get; set; }
        public HashSet<Tuple<Vector2I, FoodType>> FoodState { get; set; }
        public Dictionary<FoodType, Vector2I> LastFoodPositions { get; set; }
        public int BestPathCost { get; set; } = int.MaxValue;
        public bool IsReversed { get; set; } = false; // Flag for bidirectional search

        public override bool Equals(object obj)
        {
            if (!(obj is SearchState other)) return false;
            
            if (EaterPositions.Length != other.EaterPositions.Length) return false;
            for (int i = 0; i < EaterPositions.Length; i++)
                if (EaterPositions[i] != other.EaterPositions[i]) return false;

            if (!FoodState.SetEquals(other.FoodState)) return false;
                
            if (LastFoodPositions.Count != other.LastFoodPositions.Count) return false;
            foreach (var pair in LastFoodPositions)
                if (!other.LastFoodPositions.TryGetValue(pair.Key, out Vector2I otherPos) || otherPos != pair.Value)
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                foreach (var pos in EaterPositions)
                    hash = hash * 31 + pos.GetHashCode();
                
                var sortedFood = FoodState.OrderBy(f => f.Item1.X)
                    .ThenBy(f => f.Item1.Y)
                    .ThenBy(f => (int)f.Item2);
                
                foreach (var food in sortedFood)
                    hash = hash * 31 + food.GetHashCode();
                
                var sortedLastFood = LastFoodPositions.OrderBy(f => (int)f.Key);
                foreach (var food in sortedLastFood)
                    hash = hash * 31 + food.Key.GetHashCode() * 31 + food.Value.GetHashCode();
                
                return hash;
            }
        }

        public SearchState Clone()
        {
            return new SearchState
            {
                EaterPositions = (Vector2I[])EaterPositions.Clone(),
                FoodState = new HashSet<Tuple<Vector2I, FoodType>>(FoodState),
                LastFoodPositions = new Dictionary<FoodType, Vector2I>(LastFoodPositions),
                BestPathCost = BestPathCost,
                IsReversed = IsReversed
            };
        }
    }

    private class AStarNode : IComparable<AStarNode>
    {
        public SearchState State { get; set; }
        public List<HintMove> Path { get; set; }
        public int Cost { get; set; }
        public int Heuristic { get; set; }
        public int TotalCost => Cost + Heuristic;
        public int MoveIndex { get; set; } = 0; // For solution corridor

        public int CompareTo(AStarNode other)
        {
            int result = TotalCost.CompareTo(other.TotalCost);
            if (result != 0) return result;
            return State.FoodState.Count.CompareTo(other.State.FoodState.Count);
        }
    }

    private static readonly Vector2I[] DirectionVectors = new Vector2I[]
    {
        Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right
    };

    private static Dictionary<int, List<HintMove>> solutionCache = new Dictionary<int, List<HintMove>>();

    // APPROACH 2: SOLUTION CORRIDOR
    public static List<HintMove> GetSolutionPathWithCorridor(Level level)
    {
        // Get known solution if available
        var knownSolution = GetKnownSolution(level);
        if (knownSolution == null || knownSolution.Count == 0)
            return GetSolutionPath(level); // Fall back to standard A*
            
        var eaters = level.GetEaters();
        var foods = level.GetFood();
        
        if (eaters.Count == 0 || foods.Count == 0)
            return new List<HintMove>();

        var initialState = CreateInitialState(eaters, foods);
        int cacheKey = GeneratePuzzleCacheKey(initialState);
        
        if (solutionCache.TryGetValue(cacheKey, out var cachedSolution))
            return cachedSolution;

        var openSet = new PriorityQueue<AStarNode>();
        var visited = new Dictionary<SearchState, int>(new SearchStateEqualityComparer());
        
        var startNode = new AStarNode
        {
            State = initialState,
            Path = new List<HintMove>(),
            Cost = 0,
            Heuristic = CalculateHeuristic(initialState, eaters),
            MoveIndex = 0
        };
        
        openSet.Enqueue(startNode);
        visited[initialState] = 0;

        int corridorWidth = 2; // How many moves we allow to deviate from known solution
        int nodesExplored = 0;

        while (openSet.Count > 0)
        {
            nodesExplored++;
            var current = openSet.Dequeue();
            
            if (current.State.FoodState.Count == 0)
            {
                foreach (var move in current.Path)
                    move.FoodAtTarget = foods.FirstOrDefault(food => food.BoardStatePositionId == move.To);
                
                solutionCache[cacheKey] = current.Path;
                Console.WriteLine($"Solution found after exploring {nodesExplored} nodes. Solution length: {current.Path.Count}");
                return current.Path;
            }

            if (current.Cost > visited[current.State])
                continue;

            for (int eaterIdx = 0; eaterIdx < eaters.Count; eaterIdx++)
            {
                var eater = eaters[eaterIdx];
                var eaterPos = current.State.EaterPositions[eaterIdx];
                var validMoves = GetValidMoves(current.State, eaters, foods, eaterIdx);
                
                foreach (var movePos in validMoves)
                {
                    var newState = ApplyMove(current.State, eaters, foods, eaterIdx, movePos);
                    int newCost = current.Cost + 1;
                    
                    bool isNewState = !visited.ContainsKey(newState);
                    bool isBetterPath = isNewState || newCost < visited[newState];
                    
                    // CORRIDOR APPROACH: Check if we're within solution corridor
                    int nextMoveIndex = Math.Min(current.MoveIndex, knownSolution.Count - 1);
                    if (!IsWithinSolutionCorridor(newState, knownSolution, nextMoveIndex, corridorWidth))
                        continue; // Skip states outside corridor
                    
                    if (isBetterPath)
                    {
                        visited[newState] = newCost;
                        newState.BestPathCost = newCost;
                        
                        var newPath = new List<HintMove>(current.Path)
                        {
                            new HintMove { Eater = eater, From = eaterPos, To = movePos }
                        };
                        
                        // Check if we're making a move matching the solution
                        int newMoveIndex = current.MoveIndex;
                        if (nextMoveIndex < knownSolution.Count && 
                            knownSolution[nextMoveIndex].Eater == eater && 
                            knownSolution[nextMoveIndex].To == movePos)
                        {
                            newMoveIndex = current.MoveIndex + 1;
                        }
                        
                        var newNode = new AStarNode
                        {
                            State = newState,
                            Path = newPath,
                            Cost = newCost,
                            Heuristic = CalculateHeuristic(newState, eaters),
                            MoveIndex = newMoveIndex
                        };
                        
                        openSet.Enqueue(newNode);
                    }
                }
            }
        }
        
        solutionCache[cacheKey] = new List<HintMove>();
        return new List<HintMove>();
    }

    // APPROACH 3: BIDIRECTIONAL SEARCH
    public static List<HintMove> GetSolutionPathBidirectional(Level level)
    {
        var knownSolution = GetKnownSolution(level);
        if (knownSolution == null || knownSolution.Count == 0)
            return GetSolutionPath(level);
            
        var eaters = level.GetEaters();
        var foods = level.GetFood();
        
        if (eaters.Count == 0 || foods.Count == 0)
            return new List<HintMove>();

        var initialState = CreateInitialState(eaters, foods);
        int cacheKey = GeneratePuzzleCacheKey(initialState);
        
        if (solutionCache.TryGetValue(cacheKey, out var cachedSolution))
            return cachedSolution;

        // Create goal state from known solution
        var goalState = DeriveGoalState(initialState, knownSolution, eaters, foods);
        
        // Forward search (from initial state)
        var forwardOpenSet = new PriorityQueue<AStarNode>();
        var forwardVisited = new Dictionary<SearchState, AStarNode>(new SearchStateEqualityComparer());
        
        // Backward search (from goal state)
        var backwardOpenSet = new PriorityQueue<AStarNode>();
        var backwardVisited = new Dictionary<SearchState, AStarNode>(new SearchStateEqualityComparer());
        
        // Initialize searches
        var forwardNode = new AStarNode
        {
            State = initialState,
            Path = new List<HintMove>(),
            Cost = 0,
            Heuristic = CalculateHeuristic(initialState, eaters)
        };
        
        var backwardNode = new AStarNode
        {
            State = goalState,
            Path = new List<HintMove>(),
            Cost = 0,
            Heuristic = CalculateReverseHeuristic(goalState, eaters, initialState)
        };
        goalState.IsReversed = true;
        
        forwardOpenSet.Enqueue(forwardNode);
        backwardOpenSet.Enqueue(backwardNode);
        
        forwardVisited[initialState] = forwardNode;
        backwardVisited[goalState] = backwardNode;
        
        int meetingPointCost = int.MaxValue;
        List<HintMove> finalPath = null;
        
        int nodesExplored = 0;
        int maxIterations = 50000; // Safety limit
        
        while (forwardOpenSet.Count > 0 && backwardOpenSet.Count > 0 && nodesExplored < maxIterations)
        {
            nodesExplored++;
            
            // Check if searches have met
            foreach (var fwdState in forwardVisited.Keys)
            {
                if (backwardVisited.TryGetValue(fwdState, out var bwdNode))
                {
                    var fwdNode = forwardVisited[fwdState];
                    int totalCost = fwdNode.Cost + bwdNode.Cost;
                    
                    if (totalCost < meetingPointCost)
                    {
                        meetingPointCost = totalCost;
                        // Combine paths: forward path + reversed backward path
                        finalPath = CombinePaths(fwdNode.Path, bwdNode.Path, eaters, foods);
                    }
                }
            }
            
            // If we found a meeting point, we can finish
            if (finalPath != null)
                break;
                
            // Expand forward search
            if (forwardOpenSet.Count > 0 && nodesExplored % 2 == 0)
            {
                ExpandBidirectionalSearch(forwardOpenSet, forwardVisited, backwardVisited, 
                    eaters, foods, false, ref meetingPointCost, ref finalPath);
            }
            
            // Expand backward search
            if (backwardOpenSet.Count > 0 && nodesExplored % 2 == 1)
            {
                ExpandBidirectionalSearch(backwardOpenSet, backwardVisited, forwardVisited, 
                    eaters, foods, true, ref meetingPointCost, ref finalPath);
            }
        }
        
        if (finalPath != null)
        {
            foreach (var move in finalPath)
                move.FoodAtTarget = foods.FirstOrDefault(food => food.BoardStatePositionId == move.To);
                
            solutionCache[cacheKey] = finalPath;
            Console.WriteLine($"Bidirectional solution found after exploring {nodesExplored} nodes. Solution length: {finalPath.Count}");
            return finalPath;
        }
        
        // Fall back to known solution if bidirectional search failed
        Console.WriteLine($"Bidirectional search failed after {nodesExplored} nodes. Using known solution.");
        return knownSolution;
    }

    private static void ExpandBidirectionalSearch(
        PriorityQueue<AStarNode> openSet, 
        Dictionary<SearchState, AStarNode> visited,
        Dictionary<SearchState, AStarNode> oppositeVisited,
        List<Eater> eaters, 
        List<Food> foods,
        bool isReversed,
        ref int meetingPointCost,
        ref List<HintMove> finalPath)
    {
        var current = openSet.Dequeue();
        
        for (int eaterIdx = 0; eaterIdx < eaters.Count; eaterIdx++)
        {
            var eater = eaters[eaterIdx];
            var eaterPos = current.State.EaterPositions[eaterIdx];
            var validMoves = GetValidMoves(current.State, eaters, foods, eaterIdx);
            
            foreach (var movePos in validMoves)
            {
                // Apply move differently based on search direction
                var newState = ApplyMove(current.State, eaters, foods, eaterIdx, movePos);
                newState.IsReversed = isReversed;
                
                int newCost = current.Cost + 1;
                
                // Check if this is a better path
                if (!visited.TryGetValue(newState, out var existingNode) || newCost < existingNode.Cost)
                {
                    // Create path
                    var newPath = new List<HintMove>(current.Path);
                    newPath.Add(new HintMove { Eater = eater, From = eaterPos, To = movePos });
                    
                    var newNode = new AStarNode
                    {
                        State = newState,
                        Path = newPath,
                        Cost = newCost,
                        Heuristic = isReversed 
                            ? CalculateReverseHeuristic(newState, eaters, null) 
                            : CalculateHeuristic(newState, eaters)
                    };
                    
                    // Update visited and open set
                    visited[newState] = newNode;
                    openSet.Enqueue(newNode);
                    
                    // Check if we've met the opposite search
                    if (oppositeVisited.TryGetValue(newState, out var oppositeNode))
                    {
                        int totalCost = newCost + oppositeNode.Cost;
                        if (totalCost < meetingPointCost)
                        {
                            meetingPointCost = totalCost;
                            finalPath = CombinePaths(
                                isReversed ? oppositeNode.Path : newPath,
                                isReversed ? newPath : oppositeNode.Path,
                                eaters, foods);
                        }
                    }
                }
            }
        }
    }

    // Function to check if state is within corridor of known solution
    private static bool IsWithinSolutionCorridor(
        SearchState state, 
        List<HintMove> knownSolution, 
        int moveIndex, 
        int corridorWidth)
    {
        // If we're past solution length, allow any state
        if (moveIndex >= knownSolution.Count)
            return true;
            
        // Calculate "distance" between current state and expected state
        int stateDistance = CalculateStateDistance(state, knownSolution, moveIndex);
        
        // Only explore states within corridor width
        return stateDistance <= corridorWidth;
    }

    // Calculate distance between current state and state expected at solution point
    private static int CalculateStateDistance(
        SearchState state,
        List<HintMove> solution,
        int moveIndex)
    {
        int distance = 0;
        
        // Get expected move at this point
        if (moveIndex < solution.Count)
        {
            var expectedMove = solution[moveIndex];
            
            // Find eater index
            int eaterIdx = -1;
            for (int i = 0; i < state.EaterPositions.Length; i++)
            {
                if (state.EaterPositions[i] == expectedMove.From)
                {
                    eaterIdx = i;
                    break;
                }
            }
            
            if (eaterIdx >= 0)
            {
                // Calculate Manhattan distance to expected position
                var currentPos = state.EaterPositions[eaterIdx];
                var expectedPos = expectedMove.To;
                distance = Math.Abs(currentPos.X - expectedPos.X) + Math.Abs(currentPos.Y - expectedPos.Y);
            }
        }
        
        return distance;
    }

    // Derive goal state from known solution
    private static SearchState DeriveGoalState(
        SearchState initialState,
        List<HintMove> solution,
        List<Eater> eaters,
        List<Food> foods)
    {
        // Clone initial state
        var goalState = initialState.Clone();
        
        // Apply all moves to get final state
        for (int i = 0; i < solution.Count; i++)
        {
            var move = solution[i];
            int eaterIdx = eaters.FindIndex(e => e == move.Eater);
            goalState = ApplyMove(goalState, eaters, foods, eaterIdx, move.To);
        }
        
        return goalState;
    }

    // Combine forward and backward paths
    private static List<HintMove> CombinePaths(
        List<HintMove> forwardPath,
        List<HintMove> backwardPath,
        List<Eater> eaters,
        List<Food> foods)
    {
        var result = new List<HintMove>(forwardPath);
        
        // Reverse backward path and add
        for (int i = backwardPath.Count - 1; i >= 0; i--)
        {
            result.Add(backwardPath[i]);
        }
        
        return result;
    }

    // Calculate heuristic for backward search (estimate to initial state)
    private static int CalculateReverseHeuristic(
        SearchState state,
        List<Eater> eaters,
        SearchState initialState)
    {
        if (initialState == null)
            return state.FoodState.Count; // Simple food count heuristic
            
        int totalDistance = 0;
        
        // Calculate total Manhattan distance between eater positions
        for (int i = 0; i < state.EaterPositions.Length; i++)
        {
            var currentPos = state.EaterPositions[i];
            var initialPos = initialState.EaterPositions[i];
            totalDistance += Math.Abs(currentPos.X - initialPos.X) + Math.Abs(currentPos.Y - initialPos.Y);
        }
        
        return totalDistance;
    }

    // Rest of your existing methods (CalculateHeuristic, GetValidMoves, ApplyMove, etc.)
    // are assumed to be available but not included for brevity

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
        var foodTuple = currentState.FoodState.FirstOrDefault(f => f.Item1 == toPos);
        if (foodTuple != null && foodTuple.Item1 == toPos)
        {
            var eater = eaters[eaterIdx];
            FoodType foodType = foodTuple.Item2;
            
            // Make sure we can eat this food
            if (eater.ValidFoodTypes.Contains(foodType))
            {
                // Check if this is a "last" food
                bool isLastFood = currentState.LastFoodPositions.TryGetValue(foodType, out Vector2I lastPos) && 
                                  lastPos == toPos;
                
                // Count how many of this type exist
                int sameTypeCount = currentState.FoodState.Count(f => f.Item2 == foodType);
                
                // For last food, make sure it's the only one left
                if (isLastFood && sameTypeCount == 1)
                {
                    newState.FoodState.Remove(foodTuple);
                    newState.LastFoodPositions.Remove(foodType);
                }
                // For regular food, just remove it
                else if (!isLastFood)
                {
                    newState.FoodState.Remove(foodTuple);
                }
            }
        }
        
        return newState;
    }

    private static List<HintMove> GetKnownSolution(Level level)
    {
        return HintSystem.GetHints(level).ToList();
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
    /// Create the initial search state from the game objects
    /// </summary>
    private static SearchState CreateInitialState(List<Eater> eaters, List<Food> foods)
    {
        // Positions of "last" food items by type
        var lastFoodPos = new Dictionary<FoodType, Vector2I>();
        
        // Find all "last" food items
        foreach (var food in foods)
        {
            if (food.IsLast)
            {
                lastFoodPos[food.FoodType] = food.BoardStatePositionId;
            }
        }
        
        return new SearchState
        {
            EaterPositions = eaters.Select(e => e.BoardStatePositionId).ToArray(),
            FoodState = GetFoodStateHash(foods),
            LastFoodPositions = lastFoodPos,
            BestPathCost = 0
        };
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
        
        // Hash "last" food positions
        foreach (var lastFood in initialState.LastFoodPositions)
        {
            hash = hash * 31 + lastFood.Key.GetHashCode() * 31 + lastFood.Value.GetHashCode();
        }
        
        return hash;
    }

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
        var initialState = CreateInitialState(eaters, foods);

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
                
                // Get valid moves for this eater (considering the IsLast constraint)
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
        
        // Create a lookup for food positions and count food by type
        var foodLookup = new Dictionary<Vector2I, FoodType>();
        var foodCountByType = new Dictionary<FoodType, int>();
        
        foreach (var foodTuple in searchState.FoodState)
        {
            foodLookup[foodTuple.Item1] = foodTuple.Item2;
            
            // Count foods by type
            if (!foodCountByType.ContainsKey(foodTuple.Item2))
            {
                foodCountByType[foodTuple.Item2] = 0;
            }
            foodCountByType[foodTuple.Item2]++;
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
                        // Check if this is a "last" food position
                        bool isLastFood = searchState.LastFoodPositions.TryGetValue(foodType, out Vector2I lastPos) && 
                                        lastPos == nextPos;
                        
                        // For last food, make sure it's the only one of its type left
                        if (isLastFood)
                        {
                            if (foodCountByType[foodType] == 1)
                            {
                                validMoves.Add(nextPos);
                            }
                        }
                        // For non-last food, always valid 
                        else
                        {
                            validMoves.Add(nextPos);
                        }
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

    private static bool IsOffPlayableArea(Vector2I pos, int minX, int minY, int maxX, int maxY)
    {
        return pos.X < minX || pos.Y < minY || pos.X > maxX || pos.Y > maxY;
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
            bool isLastFood = state.LastFoodPositions.TryGetValue(foodType, out Vector2I lastPos) && 
                             lastPos == foodPos;
            
            // Skip last food in distance calculation unless it's the only one of its type
            if (isLastFood && state.FoodState.Count(f => f.Item2 == foodType) > 1)
                continue;
                
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