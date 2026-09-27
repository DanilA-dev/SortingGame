namespace D_Dev.VATAnimationSystem
{
    public interface IVertexAnimatorInfo
    {
        VertexAnimationClipInfo CurrentClip { get; }
        float NormalizedTime { get; }
    }
}
