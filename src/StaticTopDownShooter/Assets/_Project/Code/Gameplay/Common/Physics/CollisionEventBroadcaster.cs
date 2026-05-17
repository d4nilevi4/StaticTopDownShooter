namespace Shooter
{
    [RequireComponent(typeof(GameEntityProvider))]
    public class CollisionEventBroadcaster : MonoBehaviour
    {
        private GameEntityProvider _provider;

        private Game.Entity Entity => _provider.Entity;

        private void Awake()
        {
            _provider = GetComponent<GameEntityProvider>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Game.SendEvent(new TriggerEnterEvent
            {
                Source = Entity.GID,
                OtherColliderId = other.GetInstanceID()
            });
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            Game.SendEvent(new TriggerStayEvent
            {
                Source = Entity.GID,
                OtherColliderId = other.GetInstanceID()
            });
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Game.SendEvent(new TriggerExitEvent
            {
                Source = Entity.GID,
                OtherColliderId = other.GetInstanceID()
            });
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Game.SendEvent(new CollisionEnterEvent
            {
                Source = Entity.GID,
                Collision = other
            });
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            Game.SendEvent(new CollisionStayEvent
            {
                Source = Entity.GID,
                Collision = other
            });
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            Game.SendEvent(new CollisionExitEvent
            {
                Source = Entity.GID,
                Collision = other
            });
        }
    }
}