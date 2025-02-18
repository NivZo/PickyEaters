using Godot;

public enum FaceType
{
    Smile,
    Excited,
}

public static class FaceTypeExtensions
{
    public static Texture2D GetFaceTypeTexture(this FaceType faceType)
    {
        return faceType switch
        {
            FaceType.Smile => GD.Load<Texture2D>("Assets/smile_face.png"),
            FaceType.Excited => GD.Load<Texture2D>("Assets/excited_face.png"),
            _ => throw new System.NotImplementedException(),
        };
    }
}