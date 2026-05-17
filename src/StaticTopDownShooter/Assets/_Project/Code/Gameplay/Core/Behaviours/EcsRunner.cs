using System.Linq;

namespace Shooter
{
    public sealed class EcsRunner : MonoBehaviour
    {
        public void Start()
        {
            ColliderRegistry.Initialize();

            Game.Create(WorldConfig.Default());

            InputSys.Create();
            UpdateSys.Create();
            PhysicsSys.Create();

            Game.Types().RegisterAll(typeof(EcsRunner).Assembly);
            UnityEventTypes.Register<GameWT>();

            EcsDebug<GameWT>.AddWorld<UpdateST>();

            InputSys
                .Add(new InitializePlayerInputMapInputSys(), 0)
                .Add(new RegisterInputActionEvents(), 1)
                .Add(new SyncTimeResourceSystem(), 10)
                .Add(new IdleTimerTickSystem(), 50)
                .Add(new PeriodicRethinkSystem(), 80)
                .Add(new RethinkBrainSystem(), 90)
                .Add(new UpdateBrainSystem(), 91)
                .Add(new SyncAgentSimulationPositionSystem(), 100)
                .Add(new AgentDesiredVelocityToMoveInputSystem(), 101)
                .Add(new EmitPlayerMoveInputSys(), 500)
                .Add(new EmitPlayerMousePositionSystem(), 501)
                .Add(new HandleBulletCollisionInputSystem(), 1000)
                .Add(new DisposePlayerInputMapInputSys(), short.MaxValue)
                ;

            UpdateSys
                .Add(new UpdateMoveDirectionByMoveInputSystem(), 0)
                .Add(new SpawnAimSystem(), 100)
                .Add(new AimPositionSystem(), 101)
                .Add(new SyncAttackTargetPositionAndAimSystem(), 200)
                .Add(new HandleShootFrequencySystem(), 205)
                .Add(new BulletSpawnSystem(), 210)
                .Add(new DashPhysSystem(), 400)
                .Add(new ApplyDamageSystem(), 1000)
                .Add(new AnimateMovementSystem(), 2000)
                .Add(new UpdateCameraFollowPosition(), 10000)
                .Add(new DestroyViewByDeadEventSystem(), 15000)
                ;

            PhysicsSys
                .Add(new RigidBodyMovePhysSystem(), 100)
                .Add(new ApplyMoveImpulseSystem(), 300)
                .Add(new RotateBulletTowardsMoveDirectionSystem(), 310)
                ;

            Game.Initialize();

            foreach (var entityProvider in FindObjectsByType<AbstractStaticEcsEntityProvider>(FindObjectsSortMode.None))
            {
                entityProvider.CreateEntity();
            }

            var resourceRegistrars = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<IResourcesRegistrar>();

            foreach (IResourcesRegistrar registrar in resourceRegistrars)
            {
                registrar.RegisterResources();
            }

            var componentsRegistrars = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<RootGameComponentsRegistrar>();

            foreach (RootGameComponentsRegistrar componentsRegistrar in componentsRegistrars)
            {
                componentsRegistrar.RegisterComponents();
            }

            InputSys.Initialize();
            UpdateSys.Initialize();
            PhysicsSys.Initialize();
        }

        private void Update()
        {
            InputSys.Update();
            UpdateSys.Update();
            Game.Tick();
        }

        private void FixedUpdate()
        {
            PhysicsSys.Update();
        }

        private void OnDestroy()
        {
            InputSys.Destroy();
            UpdateSys.Destroy();
            PhysicsSys.Destroy();

            Game.Destroy();
        }
    }
}