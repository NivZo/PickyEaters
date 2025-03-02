using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class HintSystemSingle
{
    // State for A* search
    private class SearchState
    {
        public Vector2I[] EaterPositions { get; set; }
        public HashSet<Tuple<Vector2I, FoodType, bool>> FoodState { get; set; } // Added IsLast flag
        public float Cost { get; set; }
        public float Heuristic { get; set; }
        public float TotalCost => Cost + Heuristic;
        public HintMove LastMove { get; set; }
        public SearchState Parent { get; set; }

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
    /// Returns the next move to take that would lead to a solution
    /// </summary>
    public static HintMove GetNextMove(Level level)
    {
        var eaters = level.GetEaters();
        var foods = level.GetFood();
        
        if (eaters.Count == 0 || foods.Count == 0)
            return null;

        // Calculate playable area bounds once
        var bounds = CalculatePlayableAreaBounds(foods);

        // Create initial search state
        var initialState = new SearchState
        {
            EaterPositions = eaters.Select(e => e.BoardStatePositionId).ToArray(),
            FoodState = GetFoodStateHash(foods),
            Cost = 0,
            Heuristic = CalculateHeuristic(eaters, foods),
            LastMove = null,
            Parent = null
        };

        // Initialize A* search
        var openSet = new PriorityQueue<SearchState, float>();
        var closedSet = new Dictionary<string, SearchState>();
        
        openSet.Enqueue(initialState, initialState.TotalCost);
        closedSet[StateToString(initialState)] = initialState;

        // Maximum iterations to prevent infinite loops
        int maxIterations = 10000;
        int iterations = 0;
        
        // A* search
        while (openSet.Count > 0 && iterations < maxIterations)
        {
            iterations++;
            
            var currentState = openSet.Dequeue();
            
            // Check if we've reached a goal state (no food left)
            if (currentState.FoodState.Count == 0)
            {
                // Trace back to the first move
                return TraceBackToFirstMove(currentState, foods);
            }
            
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
                    
                    // Skip invalid states (where we've eaten non-last food when last food exists)
                    if (newState == null)
                        continue;
                        
                    newState.Parent = currentState;
                    newState.Cost = currentState.Cost + 1;
                    newState.Heuristic = CalculateHeuristic(eaters, newState);
                    
                    newState.LastMove = new HintMove
                    {
                        Eater = eater,
                        From = eaterPos,
                        To = movePos,
                        FoodAtTarget = foods.FirstOrDefault(food => food.BoardStatePositionId == movePos)
                    };
                    
                    string newStateStr = StateToString(newState);
                    
                    // If we're already at goal state, return this move immediately
                    if (newState.FoodState.Count == 0)
                    {
                        return TraceBackToFirstMove(newState, foods);
                    }
                    
                    // If we haven't visited this state or found a better path
                    if (!closedSet.TryGetValue(newStateStr, out var existingState) || 
                        newState.Cost < existingState.Cost)
                    {
                        closedSet[newStateStr] = newState;
                        openSet.Enqueue(newState, newState.TotalCost);
                    }
                }
            }
        }
        
        // If we couldn't find a complete solution, find the state with the least remaining food
        if (closedSet.Count > 1)
        {
            var bestPartialState = closedSet.Values
                .Where(s => s != initialState)
                .OrderBy(s => s.FoodState.Count)
                .ThenBy(s => s.Cost)
                .FirstOrDefault();
                
            if (bestPartialState != null)
            {
                return TraceBackToFirstMove(bestPartialState, foods);
            }
        }
        
        return null;
    }
    
    private static HintMove TraceBackToFirstMove(SearchState state, List<Food> foods)
    {
        // Go back to the first move
        while (state.Parent != null && state.Parent.Parent != null)
        {
            state = state.Parent;
        }
        
        return state.LastMove;
    }
    
    private static float CalculateHeuristic(List<Eater> eaters, List<Food> foods)
    {
        // Count of food items, with penalty for unprocessed "last" items
        float score = foods.Count;
        
        // Group foods by type
        var foodGroups = foods.GroupBy(f => f.FoodType).ToList();
        
        foreach (var group in foodGroups)
        {
            // Check if there's a "last" food in this group
            bool hasLast = group.Any(f => f.IsLast);
            if (hasLast)
            {
                // Count non-last food of this type (these must be eaten first)
                int nonLastCount = group.Count(f => !f.IsLast);
                // Add more weight to groups with last food that have other foods remaining
                score += nonLastCount * 2;
            }
        }
        
        return score;
    }
    
    private static float CalculateHeuristic(List<Eater> eaters, SearchState state)
    {
        // Base score is count of remaining food
        float score = state.FoodState.Count;
        
        // Group foods by type
        var foodGroups = state.FoodState.GroupBy(f => f.Item2).ToList();
        
        foreach (var group in foodGroups)
        {
            // Check if there's a "last" food in this group
            bool hasLast = group.Any(f => f.Item3);
            if (hasLast)
            {
                // Count non-last food of this type (these must be eaten first)
                int nonLastCount = group.Count(f => !f.Item3);
                // Add more weight to groups with last food that have other foods remaining
                score += nonLastCount * 2;
            }
        }
        
        return score;
    }

    private static string StateToString(SearchState state)
    {
        string posStr = string.Join(",", state.EaterPositions.Select(p => $"{p.X},{p.Y}"));
        string foodStr = string.Join(",", state.FoodState.OrderBy(f => f.Item1.X).ThenBy(f => f.Item1.Y)
            .Select(f => $"{f.Item1.X},{f.Item1.Y},{(int)f.Item2},{(f.Item3?1:0)}"));
        return $"{posStr}|{foodStr}";
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

    private static HashSet<Tuple<Vector2I, FoodType, bool>> GetFoodStateHash(List<Food> foods)
    {
        var foodState = new HashSet<Tuple<Vector2I, FoodType, bool>>();
        
        foreach (var food in foods)
        {
            foodState.Add(Tuple.Create(food.BoardStatePositionId, food.FoodType, food.IsLast));
        }
        
        return foodState;
    }

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
        var foodLookup = new Dictionary<Vector2I, Tuple<FoodType, bool>>();
        foreach (var foodTuple in searchState.FoodState)
        {
            foodLookup[foodTuple.Item1] = Tuple.Create(foodTuple.Item2, foodTuple.Item3);
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
                if (foodLookup.TryGetValue(nextPos, out var foodInfo))
                {
                    FoodType foodType = foodInfo.Item1;
                    bool isLast = foodInfo.Item2;
                    
                    // Check if eater can eat this food
                    if (validFoodTypes.Contains(foodType))
                    {
                        // Check if this is a valid move considering the "last" constraint
                        bool canEat = true;
                        
                        // If this is not a "last" food, it's always valid
                        if (isLast)
                        {
                            // If it's a "last" food, make sure there are no other non-last foods of this type
                            canEat = !searchState.FoodState.Any(f => f.Item2 == foodType && !f.Item3);
                        }
                        
                        if (canEat)
                        {
                            validMoves.Add(nextPos);
                        }
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
            FoodState = new HashSet<Tuple<Vector2I, FoodType, bool>>(currentState.FoodState)
        };
        
        // Move eater
        newState.EaterPositions[eaterIdx] = toPos;
        
        // Check if we're eating food
        var foodTuples = currentState.FoodState.Where(f => f.Item1 == toPos).ToList();
        var eater = eaters[eaterIdx];
        
        foreach (var foodTuple in foodTuples)
        {
            FoodType foodType = foodTuple.Item2;
            bool isLast = foodTuple.Item3;
            
            // Make sure we can eat this food
            if (eater.ValidFoodTypes.Contains(foodType))
            {
                // Check if this is a valid food to eat considering the "last" constraint
                if (isLast)
                {
                    // If it's a "last" food, make sure there are no other non-last foods of this type
                    bool otherNonLastExists = currentState.FoodState.Any(f => 
                        f != foodTuple && f.Item2 == foodType && !f.Item3);
                    
                    if (otherNonLastExists)
                    {
                        // Invalid move - can't eat "last" food when non-last food exists
                        return null;
                    }
                }
                
                newState.FoodState.Remove(foodTuple);
            }
        }
        
        return newState;
    }
}