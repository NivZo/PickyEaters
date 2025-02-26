using Godot;

public enum EaterFace
{
    Hidden,
    SmileBasic,
    Vampire,
    CatEyes,
    CuteFang,
    WideOpenSmile,
}

public static class EaterFaceExtensions
{
    public static Texture2D GetEaterFaceTexture(this EaterFace eaterFace)
        => GD.Load<Texture2D>($"Assets/Faces/{eaterFace}.png");

    public static Texture2D GetEaterActiveFaceTexture(this EaterFace eaterFace)
        => GD.Load<Texture2D>($"Assets/Faces/{eaterFace}Active.png"); 
}