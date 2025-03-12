using Godot;

public partial class SetCollectionColorButton : CustomIconButton
{
    [Export] EaterType Color;

    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        base.OnClick();
    }

}
