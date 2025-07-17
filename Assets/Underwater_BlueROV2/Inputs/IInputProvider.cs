using UnityEngine;

namespace Underwater_BlueROV2
{

    /// <summary>
    /// Interface to abstract input sources for the underwater ROV.
    /// </summary>
    public interface IInputProvider
    {
        /// <summary>Gets the movement input vector (e.g., forward/backward, lateral).</summary>
        Vector3 GetInputs();

        /// <summary>Gets the rotation input (e.g., yaw).</summary>
        float GetAngle();
    }
}