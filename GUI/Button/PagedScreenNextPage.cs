using System;
using Godot;

public partial class PagedScreenNextPage : CustomButton
{
    private Action _nextPage;

    public override void _Ready()
    {
        base._Ready();
    }

    public void Setup(Func<bool> isEnabed, Action nextPage)
    {
        IsEnabledFunc = isEnabed;
        _nextPage = nextPage;
    }
    
    protected override void OnClick()
    {
        base.OnClick();

        AudioManager.PlayAudio(AudioType.FoodConsumed);
        _nextPage();
    }
}
