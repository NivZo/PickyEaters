using System.Linq;
using Godot;

public partial class EaterShopShowcase : EaterShowcase
{
    private RichTextLabel _nameLabel;
    private RichTextLabel _rarityLabel;
    private RichTextLabel _titleLabel;
    private Sprite2D _rarityBadge;
    private CpuParticles2D _purchaseParticles;
    private HandGuidanceIndicator _tapIndicator;
    private Tween _currentTween;

    private int _tapsToReveal = 3;
    private EaterFace _newFace;
    
    public override void _Ready()
    {
        base._Ready();

        _newFace = UnlockManager.GetEaterFaceToUnlock();
        SaveManager.ActiveSave.UnlockedFaces.Add(_newFace);
        SaveManager.ActiveSave.Coins -= 1000;
        SaveManager.CommitActiveSave();

        Display.EaterFace = EaterFace.Hidden;
        Display.EaterType = EaterType.Hidden;
        Display.BaseScale = 1f;
        Display.Setup();
        Display.Scale = Vector2.One;

        _purchaseParticles = GetNode<CpuParticles2D>("PurchaseParticles");
        _titleLabel = GetNode<RichTextLabel>("Title");
        _titleLabel.Text = TextUtils.WaveString("TAP!", letterDistance: 15);
        _nameLabel = GetNode<RichTextLabel>("EaterName");
        _rarityLabel = GetNode<RichTextLabel>("EaterRarity");
        _rarityBadge = GetNode<Sprite2D>("RarityBadge");
        _nameLabel.Text = TextUtils.WaveString($"\n???", frequency: 4);
        _rarityLabel.Text = TextUtils.WaveString($"\n???", frequency: 4);
        _rarityBadge.Modulate = Rarity.Common.GetRarityColor();
        _tapIndicator = GetNode<HandGuidanceIndicator>("HandGuidanceIndicator");

        Display.SelectComponent.ClearActions();
        Display.SelectComponent.Select += HandleSelect;
        Display.SelectComponent.Deselect += HandleDeselect;

        ActionManager.StartBackgroundAction();
        Display.SelectComponent.OverrideSelectCondition(() => true);
    }

    private void HandleSelect()
    {
        _currentTween = TweenUtils.Pop(Display, 3 - .5f * _tapsToReveal - .8f);
    }

    private void HandleDeselect()
    {
        _currentTween?.Kill();
        if (_tapsToReveal >= 1)
        {
            Input.VibrateHandheld(100, (float)SaveManager.ActiveSave.ScreenShakeStrength * 0.1f);
            _currentTween = TweenUtils.Pop(Display, 3 - .5f * _tapsToReveal);
            _tapsToReveal--;
            _tapIndicator.Scale = new(.2f, .2f);
            TweenUtils.Pop(_tapIndicator, 1);

            _titleLabel.Scale = new(.5f, .5f);
            _titleLabel.Text = TextUtils.WaveString("TAP" + new string('!', 4 - _tapsToReveal), letterDistance: 15);
            TweenUtils.Pop(_titleLabel, 1, .75f);
        }

        if (_tapsToReveal == 0)
        {
            Reveal(_newFace);
        }
    }

    public void Reveal(EaterFace eaterFace)
    {
        Input.VibrateHandheld(200, (float)SaveManager.ActiveSave.ScreenShakeStrength * 0.2f);
        var resource = eaterFace.GetEaterResource();
        Display.EaterFace = eaterFace;
        Display.EaterType = EnumUtils.GetRandomValueExcluding(new EaterType[1] { EaterType.Hidden });
        _nameLabel.Text = TextUtils.WaveString($"\n{resource.EaterName}", frequency: 4);
        _rarityLabel.Text = TextUtils.WaveString($"\n{resource.EaterRarity}", frequency: 4);
        _rarityBadge.Modulate = resource.EaterRarity.GetRarityColor();
        Display.BaseScale = 2.8f;
        Display.Setup();
        _tapIndicator.Visible = false;

        _titleLabel.Scale = new(.5f, .5f);
        _titleLabel.Text = TextUtils.WaveString("MUNCHER UNLOCKED!", letterDistance: 4);
        TweenUtils.Pop(_titleLabel, 1, .75f);

        _purchaseParticles.Texture = Display.EaterType.GetFoodType().GetFoodTypeTexture(true);
        _purchaseParticles.Emitting = true;

        Display.SelectComponent.ClearActions();
        Display.EnableSelectComponent();

        ActionManager.FinishBackgroundAction();
    }
}
