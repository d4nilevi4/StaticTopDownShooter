namespace Shooter
{
    public sealed class EnemyAggressionGizmo : MonoBehaviour
    {
        public float Range = 8f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Range);
        }
    }
}
