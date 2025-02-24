using System.Linq;
using Godot;

public partial class EaterDisplay : Node2D
{
    [Export] public EaterType EaterType { get; set; }
    [Export] public EaterFace EaterFace { get; set; }
    
    public Sprite2D Body;
    public Sprite2D Face;


    public override void _Ready()
    {
        base._Ready();

        Setup();
    }

    public void Setup()
    {
        Face = GetNode<Sprite2D>("Face");
        Face.Texture = EaterFace.GetEaterFaceTexture();
        Body = GetNode<Sprite2D>("Body");
        Body.Texture = EaterType.GetEaterTypeBodyTexture();
    }

    public void Activate()
    {
        Face.Texture = EaterFace.GetEaterActiveFaceTexture();
    }

    public void Deactivate()
    {
        Face.Texture = EaterFace.GetEaterFaceTexture();
    }
}
