namespace Shooter;

[Serializable]
public struct TimeData : IResource
{
    public float Time;
    public float DeltaTime;
    public float FixedDeltaTime;
}