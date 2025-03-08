using System;
using Godot;

public partial class PagedScreenPrevPage : CustomButton
{
    private Action _prevPage;

    public override void _Ready()
    {
        base._Ready();
    }

    public void Setup(Action nextPage)
    {
        _prevPage = nextPage;
    }
    
    protected override void OnClick()
    {
        base.OnClick();

        AudioManager.PlayAudio(AudioType.FoodConsumed);
        _prevPage();
    }
}
