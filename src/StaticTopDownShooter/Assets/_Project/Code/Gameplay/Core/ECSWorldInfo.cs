namespace Shooter;

[StaticEcsEditorName("Game")]
public struct GameWT : IWorldType {}
public abstract class Game : World<GameWT> {}


public struct InputST : ISystemsType {}
public struct UpdateST : ISystemsType {}
public struct PhysicsST : ISystemsType {}

public abstract class InputSys : Game.Systems<InputST> {}
public abstract class UpdateSys : Game.Systems<UpdateST> {}
public abstract class PhysicsSys : Game.Systems<PhysicsST> {}


public struct CharacterET : IEntityType { public byte Id() => 1; }
public struct BulletET : IEntityType { public byte Id() => 2; }