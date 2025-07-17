using UnityEngine;

/// <summary>
/// Converts a user command vector U into a physical force/moment vector tau,
/// using a customizable gain vector, and writes it to the Thruster's desired_tau field.
/// </summary>
public class MappingController : MonoBehaviour
{
    [Header("References")]
    public MappingMatrix mappingMatrix;           // Maps joystick input vector J to command vector U
    public BaseInputHandler inputHandler;         // Supplies the joystick input vector J
    public Thruster thruster;                     // Destination object that holds desired_tau[]

    [Header("Gain vector for tau (6 DOF)")]
    public float[] k_tau = new float[6];          // One gain per DOF: [Fx, Fy, Fz, Mx, My, Mz]

    void Start()
    {
        // Default to unit gain if not overridden
        k_tau = new float[6] {
            85f, 85f, 120f,   // Fx, Fy, Fz (translation)
            26f, 14f, 22f    // Mx, My, Mz (rotation)
        };
    }

    void FixedUpdate()
    {
        // Safety check
        if (inputHandler == null || mappingMatrix == null || thruster == null || thruster.CO == null)
        {
            Debug.LogWarning("MappingController: One or more references are missing.");
            return;
        }

        // Get input vector J = [X1, X2, Y1, Y2, G1, G2]
        float[] J = inputHandler.GetFullInputVector();

        // Compute command vector U = [vx, vy, vz, wx, wy, wz]
        float[] U = mappingMatrix.GetMappedCommand(J);

        // Apply gain to compute tau = k_tau ⊙ U
        for (int i = 0; i < 6; i++)
        {
            thruster.CO.desired_tau[i] = k_tau[i] * U[i];
        }
    }
}