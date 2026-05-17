using UnityEngine.InputSystem;

namespace Shooter;

public struct RegisterInputActionEvents : ISystem
{
    public void Init()
    {
        PlayerInputActions actions = Game.GetResource<PlayerInputMap>().Value;

        actions.Locomotion.Dash.performed += SendDodgePerformedEvent;

        actions.Combat.Attack.performed += SendAttackPerformedEvent;
    }

    public void Destroy()
    {
        PlayerInputActions actions = Game.GetResource<PlayerInputMap>().Value;

        actions.Locomotion.Dash.performed -= SendDodgePerformedEvent;

        actions.Combat.Attack.performed -= SendAttackPerformedEvent;
    }

    private void SendAttackPerformedEvent(InputAction.CallbackContext _) =>
        Game.SendEvent(new AttackActionPerformed(GetPlayerGID()));

    private void SendDodgePerformedEvent(InputAction.CallbackContext _) =>
        Game.SendEvent(new DodgeActionPerformed(GetPlayerGID()));

    private static EntityGID GetPlayerGID()
    {
        if (Game.Query<All<IsPlayer>>().One(out var entity))
        {
            return entity.GID;
        }

        throw new Exception("No player entity found when trying to send input event.");
    }
}