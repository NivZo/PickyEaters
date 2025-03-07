using System;
using System.Linq;
using Godot;

public partial class EaterDisplay : Node2D
{
    [Export] public EaterType EaterType { get; set; }
    [Export] public EaterFace EaterFace { get; set; }
    
    public Sprite2D Body;
    public Sprite2D Face;
    public Sprite2D Thumb;
    public SelectComponent<Eater> SelectComponent;
    private AnimationPlayer _animationPlayer; 

    private float _baseScale = 1;
    private bool _isSelectable = true;


    public override void _Ready()
    {
        base._Ready();

        var collider = GetNode<Area2D>("Area2D");
        SelectComponent = new(collider, () => _isSelectable && ActionManager.IsPlayerActionAvailable());
        _animationPlayer = GetNode<AnimationPlayer>("FloatAnimationPlayer");
        StartIdleAnimation();

        
        Setup();
    }

    public void Setup()
    {
        if (!SaveManager.ActiveSave.UnlockedFaces.Contains(EaterFace))
        {
            EaterType = EaterType.Hidden;
            EaterFace = EaterFace.Hidden;
        }

        Face = GetNode<Sprite2D>("Face");
        Face.Texture = EaterFace.GetEaterFaceTexture();
        Body = GetNode<Sprite2D>("Body");
        Body.Texture = EaterType.GetEaterTypeBodyTexture();
        Thumb = GetNode<Sprite2D>("Body/HandThumb");
        Thumb.Texture = EaterType.GetEaterTypeHandThumbTexture();

        if (EaterFace != EaterFace.Hidden)
        {
            SelectComponent.Select += HandleActivate;
            SelectComponent.Deselect += HandleDeactivate;

            _baseScale = Scale.X;
        }
    }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (SelectComponent.IsSelected && @event is InputEventMouseButton inputEventMouseButton
            && inputEventMouseButton.ButtonIndex == MouseButton.Left)
        {
            if (inputEventMouseButton.IsReleased())
            {
                SelectComponent.ManualDeselection();
            }
        }
    }

    public void ToggleFinished(bool isFinished)
    {
        if (isFinished)
        {
            _isSelectable = false;
            Thumb.Visible = true;
            Face.Texture = EaterFace.GetEaterActiveFaceTexture();
            Scale = _baseScale * new Vector2(.5f, .5f);
            TweenUtils.Pop(this, _baseScale * 1.05f);
        }
        else
        {
            _isSelectable = true;
            Thumb.Visible = false;
            Face.Texture = EaterFace.GetEaterFaceTexture();
            TweenUtils.Pop(this, _baseScale);
        }
    }

    public void HandleActivate()
    {
        Face.Texture = EaterFace.GetEaterActiveFaceTexture();
        TweenUtils.Pop(this, _baseScale * 1.2f);
        TweenUtils.BoldOutline(Body, 8, 12);

        AudioManager.PlayAudio(AudioType.SelectEater);
    }

    public void HandleDeactivate()
    {
        Face.Texture = EaterFace.GetEaterFaceTexture();
        TweenUtils.Pop(this, _baseScale * 1);
        TweenUtils.BoldOutline(Body, 4, 8);

        AudioManager.PlayAudio(AudioType.DeselectEater);
    }

    private void StartIdleAnimation()
    {
        var rnd = new Random();
        _animationPlayer.Play("float_rotate");
        var offset = rnd.NextDouble() * _animationPlayer.CurrentAnimation.Length;
        _animationPlayer.Advance(offset);
    }
}
