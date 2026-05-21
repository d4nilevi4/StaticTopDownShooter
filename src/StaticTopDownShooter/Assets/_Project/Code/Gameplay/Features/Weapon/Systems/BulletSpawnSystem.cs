namespace Shooter;

public struct BulletSpawnSystem : ISystem
{
    private EventReceiver<GameWT, AttackActionPerformed> _attackEventReceiver;

    public void Init()
    {
        _attackEventReceiver = Game.RegisterEventReceiver<AttackActionPerformed>();
    }

    public void Update()
    {
        All<AmmoCount,
            TransformComponent,
            AttackTargetPosition,
            BulletPrefab,
            WeaponRecoilImpulse,
            ShootAvailable,
            ShootPerSec> filter = default;

        foreach (var attackEvent in _attackEventReceiver)
        {
            var attackEventData = attackEvent.Value;
            
            if (attackEventData.InputOwner.TryUnpack<GameWT>(out var entity) && entity.IsMatch(filter))
            {
                if (entity.Read<AmmoCount>().Value <= 0)
                    continue;

                var aimPosition = entity.Read<AttackTargetPosition>().Value;
                var transform = entity.Mut<TransformComponent>().Value;
                var bulletPrefab = entity.Read<BulletPrefab>().Value;
                var recoilImpulse = entity.Read<WeaponRecoilImpulse>().Value;
                ref var ammoCount = ref entity.Mut<AmmoCount>();

                var direction = (aimPosition - transform.position.ToVector2()).normalized;
                var bulletObj = Object.Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                bulletObj.gameObject.name = $"{bulletPrefab.name}(Owner: {entity.ID})";
                bulletObj.CreateEntity();
                Object.Destroy(bulletObj.gameObject, 10f);

                var bullet = bulletObj.Entity;
                bullet.Add<MoveDirection>().Value = direction;
                bullet.Add<OwnerGID>().Value = entity.GID;

                entity.Add<MoveImpulse>().Value = recoilImpulse * -direction;

                ammoCount.Value--;

                float shootPerSec = entity.Read<ShootPerSec>().Value;

                float cooldown = Mathf.Approximately(shootPerSec, 0) ? 0 : 1 / shootPerSec;
                entity.Add<ShootCooldownTimer>().Value = cooldown;
            }
        }
    }

    public void Destroy()
    {
        Game.DeleteEventReceiver(ref _attackEventReceiver);
    }
}