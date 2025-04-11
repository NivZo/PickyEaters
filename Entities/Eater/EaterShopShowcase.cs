using System.Linq;
using Godot;

public partial class EaterShopShowcase : EaterShowcase
{
    private RichTextLabel _nameText;
    private RichTextLabel _rarityText;
    private Sprite2D _rarityBadge;
    private CpuParticles2D _purchaseParticles;
    private HandGuidanceIndicator _tapIndicator;
    private Tween _currentTween;

    private int _tapsToReveal = 3;
    private EaterFace _newFace;
    
    public override void _Ready()
    {
        base._Ready();

        _newFace = EnumUtils.GetRandomValueExcluding(SaveManager.ActiveSave.UnlockedFaces.Concat(new EaterFace[1] { EaterFace.Hidden }).ToList());
        SaveManager.ActiveSave.UnlockedFaces.Add(_newFace);
        SaveManager.ActiveSave.Coins -= 1000;
        SaveManager.CommitActiveSave();

        Display.EaterFace = EaterFace.Hidden;
        Display.EaterType = EaterType.Hidden;
        Display.BaseScale = 1f;
        Display.Setup();
        Display.Scale = Vector2.One;

        _purchaseParticles = GetNode<CpuParticles2D>("PurchaseParticles");
        _nameText = GetNode<RichTextLabel>("EaterName");
        _rarityText = GetNode<RichTextLabel>("EaterRarity");
        _rarityBadge = GetNode<Sprite2D>("RarityBadge");
        _nameText.Text = TextUtils.WaveString($"\n???", frequency: 4);
        _rarityText.Text = TextUtils.WaveString($"\n???", frequency: 4);
        _rarityBadge.Modulate = Rarity.Common.GetRarityColor();

        _tapIndicator = HandGuidanceIndicator.CreatePointing(this, Display.GlobalPosition);
        _tapIndicator.ZIndex = 4;
        Display.SelectComponent.ClearActions();
        Display.SelectComponent.Select += HandleSelect;
        Display.SelectComponent.Deselect += HandleDeselect;

        ActionManager.StartBackgroundAction();
        Display.SelectComponent.OverrideSelectCondition(() => true);
    }

    private void HandleSelect()
    {
        _currentTween = TweenUtils.Pop(Display, 3 -  .5f * _tapsToReveal - .75f);
    }

    private void HandleDeselect()
    {
        _currentTween?.Kill();
        if (_tapsToReveal >= 1)
        {
            _currentTween = TweenUtils.Pop(Display, 3 -  .5f * _tapsToReveal);
            _tapsToReveal--;
        }

        if (_tapsToReveal == 0)
        {
            Reveal(_newFace);
        }
    }

    public void Reveal(EaterFace eaterFace)
    {
        var resource = eaterFace.GetEaterResource();
        Display.EaterFace = eaterFace;
        Display.EaterType = EnumUtils.GetRandomValueExcluding(new EaterType[1] { EaterType.Hidden });
        _nameText.Text = TextUtils.WaveString($"\n{resource.EaterName}", frequency: 4);
        _rarityText.Text = TextUtils.WaveString($"\n{resource.EaterRarity}", frequency: 4);
        _rarityBadge.Modulate = resource.EaterRarity.GetRarityColor();
        Display.BaseScale = 2.5f;
        Display.Setup();

        _purchaseParticles.Texture = Display.EaterType.GetFoodType().GetFoodTypeTexture(true);
        _purchaseParticles.Emitting = true;

        Display.SelectComponent.ClearActions();
        Display.EnableSelectComponent();

        ActionManager.FinishBackgroundAction();
    }
}
