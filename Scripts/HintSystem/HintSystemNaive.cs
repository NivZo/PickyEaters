using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class HintSystemNaive
{
    // State for BFS search
    private class SearchState
    {
        public Vector2I[] EaterPositions { get; set; }  // Changed to Vector2I[] to use BoardStatePositionId
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

    // Direction vectors (matching the game's Direction.DirectionName)
    private static readonly Vector2I[] DirectionVectors = new Vector2I[]
    {
        Vector2I.Up,    // Up
        Vector2I.Down,  // Down
        Vector2I.Left,  // Left
        Vector2I.Right  // Right
    };

    /// <summary>
    /// Generates a hint for the player by finding a solution path if one exists
    /// </summary>
    /// <param name="level">The current level node containing eaters and food</param>
    /// <returns>A list of moves that solve the puzzle, or empty list if no solution exists</returns>
    public static List<HintMove> GetSolutionPath(Level level)
    {
        var eaters = level.GetEaters();
        var foods = level.GetFood();
        
        if (eaters.Count == 0 || foods.Count == 0)
            return new List<HintMove>();

        // Create initial search state using BoardStatePositionId
        var initialSearchState = new SearchState
        {
            EaterPositions = eaters.Select(e => e.BoardStatePositionId).ToArray(),
            FoodState = GetFoodStateHash(foods)
        };

        // BFS queue and visited set
        var queue = new Queue<Tuple<SearchState, List<HintMove>>>();
        var visited = new HashSet<SearchState>(new SearchStateEqualityComparer());
        
        // Add initial state to queue
        queue.Enqueue(Tuple.Create(initialSearchState, new List<HintMove>()));
        visited.Add(initialSearchState);

        // BFS search for solution
        while (queue.Count > 0)
        {
            Console.WriteLine($"Q size={queue.Count}");
            var (state, path) = queue.Dequeue();
            
            // If we've reached a goal state (no food left)
            if (state.FoodState.Count == 0)
            {
                path.ForEach(move => {
                        move.FoodAtTarget = foods.FirstOrDefault(food => food.BoardStatePositionId == move.To);
                    });
                return path;
            }

            // Try all possible moves for each eater
            for (int eaterIdx = 0; eaterIdx < eaters.Count; eaterIdx++)
            {
                var eater = eaters[eaterIdx];
                var eaterPos = state.EaterPositions[eaterIdx];
                
                // Get valid moves for this eater
                var validMoves = GetValidMoves(state, eaters, foods, eaterIdx);
                
                foreach (var movePos in validMoves)
                {
                    // Apply the move to get new state
                    var newState = ApplyMove(state, eaters, foods, eaterIdx, movePos);
                    
                    // If we haven't visited this state yet
                    if (!visited.Contains(newState))
                    {
                        visited.Add(newState);
                        
                        // Create new path with this move
                        var newPath = new List<HintMove>(path)
                        {
                            new HintMove
                            {
                                Eater = eater,
                                From = eaterPos,
                                To = movePos
                            }
                        };
                        
                        queue.Enqueue(Tuple.Create(newState, newPath));
                    }
                }
            }
        }
        
        // If we've exhausted all possibilities without finding a solution
        return new List<HintMove>();
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
        int eaterIdx)
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
                
                // Check if we're off the playable area using min/max bounds from all food positions
                if (IsOffPlayableArea(nextPos, allFoods))
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

    /// <summary>
    /// Check if a position is off the playable area using BoardStatePositionId values
    /// </summary>
    private static bool IsOffPlayableArea(Vector2I pos, List<Food> foods)
    {
        if (foods.Count == 0)
            return false;
            
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
}