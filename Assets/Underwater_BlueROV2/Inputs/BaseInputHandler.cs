using UnityEngine;

/// <summary>
/// Abstract base class for all input handlers.
/// Implementations should provide joystick axis values and trigger states.
/// </summary>
public abstract class BaseInputHandler : MonoBehaviour
{
    /// <summary>
    /// Returns the full control input vector:
    /// [X1, X2, Y1, Y2, G1, G2] where:
    /// X1, Y1 = left joystick (horizontal, vertical)
    /// X2, Y2 = right joystick (horizontal, vertical)
    /// G1 = left trigger (e.g., L2)
    /// G2 = right trigger (e.g., R2)
    /// </summary>
    public abstract float[] GetFullInputVector();
}