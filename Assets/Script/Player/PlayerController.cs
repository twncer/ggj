using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Pickaxe pickaxe;
    
    void Update()
    {
        HandlePickaxeInput();
    }

    void HandlePickaxeInput()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            pickaxe.Charge();
            print(System.DateTime.Now);
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            pickaxe.ThrowPickaxe();
            print(System.DateTime.Now);
        }
    }
}
