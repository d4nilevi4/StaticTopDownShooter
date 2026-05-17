namespace Shooter
{
    [CreateAssetMenu(fileName = nameof(CameraFollowSettings),
        menuName = nameof(Shooter) + "/" + nameof(CameraFollowSettings))]
    [Serializable]
    public sealed class CameraFollowSettings : ScriptableObject, IResource
    {
        [Header("Aim Look-Ahead")]
        [Range(0f, 1f)] public float AimWeight = 0.35f;
        [Min(0f)] public float MaxLookOffset = 3f;

        [Header("Smoothing")]
        [Min(0f)] public float FollowSpeed = 8f;
    }
}
