using Godot;

public partial class Collection : Node
{
    public static Collection Instance;

    private Scrollable _scrollable;

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        
        _scrollable = GetNode<Scrollable>("GUILayer/Scrollable");
    }

    public void SetColor(EaterType eaterType)
    {
        foreach (var item in _scrollable.Items)
        {
            if (item is EaterDisplay eaterDisplay)
            {
                eaterDisplay.EaterType = eaterType;
                eaterDisplay.Setup();
            }
        }
    }
}
