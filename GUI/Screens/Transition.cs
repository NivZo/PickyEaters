using Godot;
using System;

public partial class Transition : Node
{
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        base._Ready();

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.AnimationFinished += animationName =>
        {
            if (animationName == "fade_out")
            {
                EventManager.InvokeFadeOutTransitionFinished();
            }
        };
    }

    public void FadeIn() => _animationPlayer.Play("fade_in");
    public void FadeOut() => _animationPlayer.Play("fade_out");
}
