using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Pickaxe pickaxe;
    public Pet pet;
    public Player player;

    void Update()
    {
        HandlePickaxeInput();
    }

    void HandlePickaxeInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (player._currentState is Player.PlayerState.HANGING)
                return;
            if (pickaxe.GetCurrentState() is Pickaxe.State.IDLE)
                pickaxe.Charge();
            else if (pickaxe.GetCurrentState() is Pickaxe.State.FIXED or
                Pickaxe.State.FLYING or Pickaxe.State.RETURNING or
                Pickaxe.State.DOLL)
                pet.Fetch();
        }
        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            if (pickaxe.GetCurrentState() is Pickaxe.State.CHARGING)
                pickaxe.ThrowPickaxe();
        }
    }
}
