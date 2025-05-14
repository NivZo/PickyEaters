using Godot;
using System;

public partial class GiftsAvailableIndicator : Node2D
{
    public override void _Ready()
    {
        base._Ready();
        var isDaily = ShopStacksManager.IsDailyRewardAvailable();
        var isHourly = ShopStacksManager.IsHourlyRewardAvailable();
        Visible = isDaily || isHourly;
        GetNode<RichTextLabel>("Amount").Text = TextUtils.WaveString($"{(isDaily ? 1 : 0) + (isHourly ? 1 : 0)}", amplitude: 2);
    }
    
}
