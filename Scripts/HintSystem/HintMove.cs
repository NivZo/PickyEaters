using Godot;

public class HintMove
    {
        public Eater Eater { get; set; }
        public Vector2I From { get; set; }
        public Vector2I To { get; set; }
        public Food FoodAtTarget { get; set; }
    }