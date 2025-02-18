using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;

public static class LevelGenerator
{
    private static string DATA = "{\"rows\": 11, \"cols\": 7, \"cells\": [{\"x\": 0, \"y\": 0, \"entity\": {\"type\": \"Food\", \"color\": \"Purple\"}}, {\"x\": 0, \"y\": 1, \"entity\": {\"type\": \"Food\", \"color\": \"Yellow\"}}, {\"x\": 0, \"y\": 2, \"entity\": {\"type\": \"Food\", \"color\": \"Green\"}}, {\"x\": 0, \"y\": 3, \"entity\": {\"type\": \"Food\", \"color\": \"Green\"}}, {\"x\": 0, \"y\": 4, \"entity\": {\"type\": \"Food\", \"color\": \"Pink\"}}, {\"x\": 0, \"y\": 5, \"entity\": {\"type\": \"Food\", \"color\": \"White\"}}, {\"x\": 0, \"y\": 6, \"entity\": {\"type\": \"Food\", \"color\": \"White\"}}, {\"x\": 1, \"y\": 0, \"entity\": {\"type\": \"Food\", \"color\": \"Purple\"}}, {\"x\": 1, \"y\": 1, \"entity\": {\"type\": \"Food\", \"color\": \"Purple\"}}, {\"x\": 1, \"y\": 2, \"entity\": {\"type\": \"Food\", \"color\": \"Purple\"}}, {\"x\": 1, \"y\": 3, \"entity\": {\"type\": \"Food\", \"color\": \"Green\"}}, {\"x\": 1, \"y\": 4, \"entity\": {\"type\": \"Food\", \"color\": \"Pink\"}}, {\"x\": 1, \"y\": 5, \"entity\": {\"type\": \"Food\", \"color\": \"Pink\"}}, {\"x\": 1, \"y\": 6, \"entity\": {\"type\": \"Food\", \"color\": \"White\"}}, {\"x\": 2, \"y\": 0, \"entity\": {\"type\": \"Food\", \"color\": \"Purple\"}}, {\"x\": 2, \"y\": 1, \"entity\": {\"type\": \"Eater\", \"color\": \"Purple\"}}, {\"x\": 2, \"y\": 2, \"entity\": {\"type\": \"Food\", \"color\": \"Purple\"}}, {\"x\": 2, \"y\": 3, \"entity\": {\"type\": \"Food\", \"color\": \"Green\"}}, {\"x\": 2, \"y\": 4, \"entity\": {\"type\": \"Eater\", \"color\": \"Pink\"}}, {\"x\": 2, \"y\": 5, \"entity\": {\"type\": \"Eater\", \"color\": \"Green\"}}, {\"x\": 2, \"y\": 6, \"entity\": {\"type\": \"Food\", \"color\": \"Pink\"}}, {\"x\": 3, \"y\": 0, \"entity\": {\"type\": \"Food\", \"color\": \"Yellow\"}}, {\"x\": 3, \"y\": 1, \"entity\": {\"type\": \"Food\", \"color\": \"Yellow\"}}, {\"x\": 3, \"y\": 2, \"entity\": {\"type\": \"Food\", \"color\": \"Yellow\"}}, {\"x\": 3, \"y\": 3, \"entity\": {\"type\": \"Food\", \"color\": \"Pink\"}}, {\"x\": 3, \"y\": 4, \"entity\": {\"type\": \"Food\", \"color\": \"Pink\"}}, {\"x\": 3, \"y\": 5, \"entity\": {\"type\": \"Food\", \"color\": \"White\"}}, {\"x\": 3, \"y\": 6, \"entity\": {\"type\": \"Food\", \"color\": \"Pink\"}}, {\"x\": 4, \"y\": 0, \"entity\": {\"type\": \"Food\", \"color\": \"White\"}}, {\"x\": 4, \"y\": 1, \"entity\": {\"type\": \"Eater\", \"color\": \"Yellow\"}}, {\"x\": 4, \"y\": 2, \"entity\": {\"type\": \"Food\", \"color\": \"Yellow\"}}, {\"x\": 4, \"y\": 3, \"entity\": {\"type\": \"Food\", \"color\": \"Blue\"}}, {\"x\": 4, \"y\": 4, \"entity\": {\"type\": \"Food\", \"color\": \"Blue\"}}, {\"x\": 4, \"y\": 5, \"entity\": {\"type\": \"Food\", \"color\": \"Pink\"}}, {\"x\": 4, \"y\": 6, \"entity\": {\"type\": \"Food\", \"color\": \"White\"}}, {\"x\": 5, \"y\": 0, \"entity\": {\"type\": \"Food\", \"color\": \"Yellow\"}}, {\"x\": 5, \"y\": 1, \"entity\": {\"type\": \"Food\", \"color\": \"Yellow\"}}, {\"x\": 5, \"y\": 2, \"entity\": {\"type\": \"Food\", \"color\": \"Blue\"}}, {\"x\": 5, \"y\": 3, \"entity\": {\"type\": \"Food\", \"color\": \"Blue\"}}, {\"x\": 5, \"y\": 4, \"entity\": {\"type\": \"Food\", \"color\": \"Blue\"}}, {\"x\": 5, \"y\": 5, \"entity\": {\"type\": \"Food\", \"color\": \"Blue\"}}, {\"x\": 5, \"y\": 6, \"entity\": {\"type\": \"Food\", \"color\": \"Blue\"}}, {\"x\": 6, \"y\": 0, \"entity\": {\"type\": \"Food\", \"color\": \"Purple\"}}, {\"x\": 6, \"y\": 1, \"entity\": {\"type\": \"Food\", \"color\": \"White\"}}, {\"x\": 6, \"y\": 2, \"entity\": {\"type\": \"Food\", \"color\": \"Yellow\"}}, {\"x\": 6, \"y\": 3, \"entity\": {\"type\": \"Food\", \"color\": \"Blue\"}}, {\"x\": 6, \"y\": 4, \"entity\": {\"type\": \"Eater\", \"color\": \"Blue\"}}, {\"x\": 6, \"y\": 5, \"entity\": {\"type\": \"Food\", \"color\": \"Yellow\"}}, {\"x\": 6, \"y\": 6, \"entity\": {\"type\": \"Food\", \"color\": \"White\"}}, {\"x\": 7, \"y\": 0, \"entity\": {\"type\": \"Food\", \"color\": \"Purple\"}}, {\"x\": 7, \"y\": 1, \"entity\": {\"type\": \"Food\", \"color\": \"Yellow\"}}, {\"x\": 7, \"y\": 2, \"entity\": {\"type\": \"Food\", \"color\": \"Red\"}}, {\"x\": 7, \"y\": 3, \"entity\": {\"type\": \"Food\", \"color\": \"White\"}}, {\"x\": 7, \"y\": 4, \"entity\": {\"type\": \"Food\", \"color\": \"Blue\"}}, {\"x\": 7, \"y\": 5, \"entity\": {\"type\": \"Food\", \"color\": \"Blue\"}}, {\"x\": 7, \"y\": 6, \"entity\": {\"type\": \"Food\", \"color\": \"White\"}}, {\"x\": 8, \"y\": 0, \"entity\": {\"type\": \"Food\", \"color\": \"Purple\"}}, {\"x\": 8, \"y\": 1, \"entity\": {\"type\": \"Food\", \"color\": \"Purple\"}}, {\"x\": 8, \"y\": 2, \"entity\": {\"type\": \"Food\", \"color\": \"Red\"}}, {\"x\": 8, \"y\": 3, \"entity\": {\"type\": \"Eater\", \"color\": \"Red\"}}, {\"x\": 8, \"y\": 4, \"entity\": {\"type\": \"Food\", \"color\": \"Red\"}}, {\"x\": 8, \"y\": 5, \"entity\": {\"type\": \"Food\", \"color\": \"Blue\"}}, {\"x\": 8, \"y\": 6, \"entity\": {\"type\": \"Food\", \"color\": \"White\"}}, {\"x\": 9, \"y\": 0, \"entity\": {\"type\": \"Food\", \"color\": \"Red\"}}, {\"x\": 9, \"y\": 1, \"entity\": {\"type\": \"Food\", \"color\": \"Red\"}}, {\"x\": 9, \"y\": 2, \"entity\": {\"type\": \"Food\", \"color\": \"Red\"}}, {\"x\": 9, \"y\": 3, \"entity\": {\"type\": \"Food\", \"color\": \"Green\"}}, {\"x\": 9, \"y\": 4, \"entity\": {\"type\": \"Food\", \"color\": \"Red\"}}, {\"x\": 9, \"y\": 5, \"entity\": null}, {\"x\": 9, \"y\": 6, \"entity\": {\"type\": \"Food\", \"color\": \"Blue\"}}, {\"x\": 10, \"y\": 0, \"entity\": {\"type\": \"Food\", \"color\": \"Red\"}}, {\"x\": 10, \"y\": 1, \"entity\": {\"type\": \"Food\", \"color\": \"White\"}}, {\"x\": 10, \"y\": 2, \"entity\": {\"type\": \"Food\", \"color\": \"White\"}}, {\"x\": 10, \"y\": 3, \"entity\": {\"type\": \"Food\", \"color\": \"Green\"}}, {\"x\": 10, \"y\": 4, \"entity\": {\"type\": \"Food\", \"color\": \"White\"}}, {\"x\": 10, \"y\": 5, \"entity\": {\"type\": \"Food\", \"color\": \"Pink\"}}, {\"x\": 10, \"y\": 6, \"entity\": {\"type\": \"Food\", \"color\": \"Pink\"}}]}";
    public static void GenerateLevel(Level level)
    {
        var board = JsonSerializer.Deserialize<Board>(DATA);
        board.DeserializeGrid();
        
        var cellSize = 184;
        var center = new Vector2(720, 1280);
        var pos = center - new Vector2(cellSize * board.cols/2, cellSize * board.rows/2) + new Vector2(cellSize / 2, cellSize / 2);

        for (int x = 0; x < board.rows; x++)
        {
            for (int y = 0; y < board.cols; y++)
            {
                Cell cell = board.Grid[x, y];
                if (cell.entity is EaterData eaterData)
                {
                    var eater = CreateEater(eaterData);
                    eater.Position = pos;
                    level.Eaters.AddChild(eater);
                }
                else if (cell.entity is FoodData foodData)
                {
                    var food = CreateFood(foodData);
                    food.Position = pos;
                    level.Food.AddChild(food);
                }
                else
                {
                    var food = CreateFood(new() { color = "White" });
                    food.Position = pos;
                    level.Food.AddChild(food);
                }

                pos += new Vector2(cellSize, 0);
            }

            pos += new Vector2(-board.cols * cellSize, cellSize);
        }
    }

    private static Eater CreateEater(EaterData eaterData)
    {
        var eater = GD.Load<PackedScene>("res://Entities/Eater/Eater.tscn").Instantiate<Eater>();
        eater.EaterType = Enum.Parse<EaterType>(eaterData.color);
        eater.ValidFoodTypes = new Godot.Collections.Array<FoodType>() { FoodType.White, Enum.Parse<FoodType>(eaterData.color) };
        return eater;
    }

    private static Food CreateFood(FoodData foodData)
    {
        var food = GD.Load<PackedScene>("res://Entities/Food/Food.tscn").Instantiate<Food>();
        food.FoodType = Enum.Parse<FoodType>(foodData.color);
        return food;
    }

    public class Board
    {
        public int rows { get; set; }
        
        public int cols { get; set; }

        public List<CellData> cells { get; set; }

        public Cell[,] Grid { get; set; }

        public void DeserializeGrid()
        {
            Grid = new Cell[rows, cols];
            foreach (var cellData in cells)
            {
                var cell = new Cell
                {
                    x = cellData.x,
                    y = cellData.y,
                    entity = null
                };

                if (cellData.entity != null)
                {
                    if (cellData.entity.type == "Eater")
                    {
                        cell.entity = new EaterData
                        {
                            color = cellData.entity.color
                        };
                    }
                    else if (cellData.entity.type == "Food")
                    {
                        cell.entity = new FoodData
                        {
                            color = cellData.entity.color
                        };
                    }
                }

                Grid[cellData.x, cellData.y] = cell;
            }
        }
    }

    public class CellData
    {
        public int x { get; set; }
        public int y { get; set; }
        public EntityData entity { get; set; }
    }

    public class EntityData
    {
        public string type { get; set; }
        public string color { get; set; }
    }

    public class Cell
    {
        public int x { get; set; }
        public int y { get; set; }
        public IEntity entity { get; set; }
    }

    public interface IEntity
    {
        string color { get; set; }
    }

    public class EaterData : IEntity
    {
        public string color { get; set; }
    }

    public class FoodData : IEntity
    {
        public string color { get; set; }
    }
}