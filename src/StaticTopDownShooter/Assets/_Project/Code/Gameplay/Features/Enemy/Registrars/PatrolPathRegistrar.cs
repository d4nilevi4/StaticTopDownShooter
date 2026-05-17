namespace Shooter
{
    public sealed class PatrolPathRegistrar : GameEntityComponentsRegistrar
    {
        public Transform[] Points;

        public override void RegisterComponents(World<GameWT>.Entity entity)
        {
            entity.Set(new PatrolPoints { Value = Points ?? Array.Empty<Transform>() });
            entity.Set(new PatrolPointIndex { Value = 0 });
        }

        private void OnDrawGizmos()
        {
            if (Points == null || Points.Length == 0) return;
            Gizmos.color = Color.cyan;
            for (int i = 0; i < Points.Length; i++)
            {
                if (Points[i] == null) continue;
                Gizmos.DrawSphere(Points[i].position, 0.2f);
                int next = (i + 1) % Points.Length;
                if (Points[next] != null)
                    Gizmos.DrawLine(Points[i].position, Points[next].position);
            }
        }
    }
}
