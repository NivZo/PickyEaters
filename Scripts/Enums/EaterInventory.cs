using Godot;

public enum EaterFace
{
    Hidden,
    SmileBasic,
    Vampire,
    CatEyes,
    CuteFang,
    WideOpenSmile,
    ThreeEye,
    GrumpyCyclops,
    Pigface,
    AngryGrumpy,
    GlassesSmile,
    SquigglyMouth,
    BlueTongue,
    Monkey,
    Dizzy,
    Monocle,
    MustacheAlien,
    ThreeDGlasses,
    Babyface,
}

public static class EaterFaceExtensions
{
    public static Texture2D GetEaterFaceTexture(this EaterFace eaterFace)
        => GD.Load<Texture2D>($"Assets/Faces/{eaterFace}.png");

    public static Texture2D GetEaterActiveFaceTexture(this EaterFace eaterFace)
        => GD.Load<Texture2D>($"Assets/Faces/{eaterFace}Active.png");

    public static EaterResource GetEaterResource(this EaterFace eaterFace)
        => GD.Load<EaterResource>($"CustomResources/Eater/{eaterFace}.tres");
}