using Godot;

public partial class Title : Node2D
{
    private bool _readyToTransition = false;
    private bool _transitioned = false;

    private Node2D _content;
    private ColorRect _bg;
    private ColorRect _fade;
    private RichTextLabel _tapToStartLabel;
    private Area2D _tapToStartArea;

    private readonly Vector2 _contentTargetPosition = new(720, 0);
    private readonly int _contentTargetScale = 1;
    public override void _Ready()
    {
        base._Ready();

        _content = GetNode<Node2D>("TitleContent");
        _bg = GetNode<ColorRect>("BackgroundEffectTiled");
        _fade = GetNode<ColorRect>("Fade");
        _tapToStartLabel = GetNode<RichTextLabel>("TapToStartLabel");
        _tapToStartArea = GetNode<Area2D>("TapToStartArea");
        _tapToStartArea.InputEvent += OnInput;
        AudioManager.PlayTitleBackgroundMusic();

        if (Main.PlayedIntro)
        {
            ClearPostAnimation();
        }
        else
        {
            StartupAnimation();
            Main.PlayedIntro = true;
        }
    }

    private void StartupAnimation()
    {
        _fade.Visible = true;
        _content.Scale = new(1.5f, 1.5f);
        _content.GlobalPosition = new(720, 432);
        (_bg.Material as ShaderMaterial).SetShaderParameter("progress", 0);
        
        var munch = _content.GetNode<RichTextLabel>("Munch");
        var bunch = _content.GetNode<RichTextLabel>("Bunch");
        var topEater = _content.GetNode<EaterShowcase>("TopEaterShowcase");
        var bottomEater = _content.GetNode<EaterShowcase>("BottomEaterShowcase");

        var munchTargetPosition = munch.GlobalPosition;
        var bunchTargetPosition = bunch.GlobalPosition;
        munch.GlobalPosition = new(munch.GlobalPosition.X - 2*munch.Size.X, munch.GlobalPosition.Y);
        bunch.GlobalPosition = new(bunch.GlobalPosition.X + 2*bunch.Size.X, bunch.GlobalPosition.Y);
;
        topEater.Scale = Vector2.Zero;
        bottomEater.Scale = Vector2.Zero;
        CutsceneManager.Play(new()
        {
            new(() => TweenUtils.Color(_fade, new Color(0, 0, 0, 0), 1f, Tween.TransitionType.Linear), 0),
            new(() => { TweenUtils.Pop(topEater, 1.2f); AudioManager.PlaySoundEffect(AudioType.SelectEater); }, 0.5f),
            new(() => { TweenUtils.Travel(munch, munchTargetPosition, .5f, Tween.TransitionType.Spring); AudioManager.PlaySoundEffect(AudioType.Swoosh); }, 0.5f),
            new(() => { TweenUtils.Pop(bottomEater, 1.2f); AudioManager.PlaySoundEffect(AudioType.SelectEater); }, 0.2f),
            new(() => { TweenUtils.Travel(bunch, bunchTargetPosition, .5f, Tween.TransitionType.Spring); AudioManager.PlaySoundEffect(AudioType.Swoosh); }, 0.5f),
            new(() => {
                _readyToTransition = true;
                _bg.MouseFilter = Control.MouseFilterEnum.Pass;
                }, 0),
            new(() => {
                _tapToStartLabel.Visible = true;
                _tapToStartLabel.Scale = Vector2.Zero;
                TweenUtils.Pop(_tapToStartLabel, 1f);
            }, 0.5f),
        });
    }

    private void ClearPostAnimation()
    {
        _tapToStartLabel.QueueFree();
        _tapToStartArea.InputEvent -= OnInput;
        _tapToStartArea.QueueFree();
        _bg.QueueFree();
        _fade.QueueFree();
        _content.GlobalPosition = _contentTargetPosition;
        _content.Scale = new(_contentTargetScale, _contentTargetScale);
    }

    private void OnInput(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (!_transitioned && _readyToTransition && @event is InputEventMouseButton inputEventMouseButton && inputEventMouseButton.IsReleased())
        {
            AudioManager.PlayBackgroundMusic();
            _transitioned = true;
            CutsceneManager.Play(new() {
                new(() => TweenUtils.Pop(_tapToStartLabel, 0f, 0.3f, Tween.TransitionType.Cubic), 0),
                new(() => {
                    TweenUtils.Pop(_content, _contentTargetScale, 1.5f);
                    TweenUtils.Travel(_content, _contentTargetPosition, 1.5f);
                }, 0.3f),
                new(() => TweenUtils.MethodTween(_bg, val => (_bg.Material as ShaderMaterial).SetShaderParameter("progress", val), 0f, 1f, 1, Tween.TransitionType.Linear).Finished += ClearPostAnimation, 0),
            });
        }
    }

}
