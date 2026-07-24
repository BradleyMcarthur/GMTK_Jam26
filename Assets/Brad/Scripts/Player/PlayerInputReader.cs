using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; } //Movement input, range -1 to 1 on each axis
    
    public bool DashPressed { get; private set; } //True for exactly one frame when dash is pressed

    private InputAction _moveAction;
    private InputAction _dashAction;

    private void Awake()
    {
        _moveAction = BuildMoveAction();
        _dashAction = new InputAction("Dash", binding: "<Keyboard>/leftShift");
        _dashAction.AddBinding("<Gamepad>/buttonWest");
    }

    private static InputAction BuildMoveAction()
    {
        var action = new InputAction("Move");

        action.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        action.AddBinding("<Gamepad>/leftStick");

        return action;
    }

    private void OnEnable()
    {
        _moveAction.Enable();
        _dashAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _dashAction.Disable();
    }

    private void Update()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();
        DashPressed = _dashAction.WasPressedThisFrame();
    }

    private void OnDestroy()
    {
        _moveAction.Dispose();
        _dashAction.Dispose();
    }
}
