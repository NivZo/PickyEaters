using Godot;

public partial class Level : Node
{
    public Node Food;
    public Node Eaters;
    public override void _Ready()
    {
        base._Ready();

        Food = GetNode<Node>("Food");
        Eaters = GetNode<Node>("Eaters");
    }
}
