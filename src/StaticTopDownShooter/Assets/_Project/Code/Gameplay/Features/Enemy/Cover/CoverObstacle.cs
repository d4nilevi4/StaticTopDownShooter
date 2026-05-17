namespace Shooter
{
    public sealed class CoverObstacle : MonoBehaviour
    {
        public Transform[] Points;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.25f);

            if (Points == null) return;
            Gizmos.color = Color.green;
            foreach (var p in Points)
            {
                if (p == null) continue;
                Gizmos.DrawWireSphere(p.position, 0.2f);
                Gizmos.DrawLine(transform.position, p.position);
            }
        }
    }
}
