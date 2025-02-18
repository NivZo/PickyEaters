using Godot;

[GlobalClass]
public partial class SolutionPath : Resource
{
    [Export] public int EaterId { get; set; }
    [Export] public Godot.Collections.Array<Vector2I> Path { get; set; }
}