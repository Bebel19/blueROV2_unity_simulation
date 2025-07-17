using UnityEngine;

/// <summary>
/// Applies the matrix mapping using normalization from an InputProfileSO.
/// </summary>
public class MappingMatrix : MonoBehaviour
{
    [Header("Mapping Matrix M (6x6)")] public float[,] M = new float[6, 6];

    [Header("Sensitivity Vector k")] public float[] k = new float[6];

    [Header("Input Profile")] public InputProfileSO inputProfile;

    void Awake()
    {
        if (inputProfile == null)
        {
            Debug.LogWarning("MappingMatrix: No InputProfile assigned. Defaulting to 1.");
            inputProfile = ScriptableObject.CreateInstance<InputProfileSO>();
        }

        float Xmax = inputProfile.Xmax;
        float Ymax = inputProfile.Ymax;
        float alpha = inputProfile.Alpha;

        //Simple one on one mapping with an eye matrix
        /* M = new float[6, 6]
         {
             { 1f, 0f, 0f, 0f, 0f, 0f }, // vx ← X1
             { 0f, 1f, 0f, 0f, 0f, 0f }, // vy ← X2
             { 0f, 0f, 1f, 0f, 0f, 0f }, // vz ← Y1
             { 0f, 0f, 0f, 1f, 0f, 0f }, // wx ← Y2
             { 0f, 0f, 0f, 0f, 1f, 0f }, // wy ← G1
             { 0f, 0f, 0f, 0f, 0f, 1f }  // wz ← G2
         };*/
        
        // One on one mapping with translations on the right hand and rotations on the left hand
        /* M = new float[6, 6]
         {
             { 1f, 0f, 0f, 0f, 0f, 0f }, // vx ← X1
             { 0f, 1f, 0f, 0f, 0f, 0f }, // vy ← X2
             { 0f, 0f, 0f, 0f, 1f, 0f }, // vz ← Y1
             { 0f, 0f, 0f, 0f, 0f, 1f }, // wx ← Y2
             { 0f, 0f, 0f, 1f, 0f, 0f }, // wy ← G1
             { 0f, 0f, 1f, 0f, 0f, 0f }  // wz ← G2
         };*/

        M = new float[6, 6]
        {
            { 0f, 0f, (1 - alpha) / (2 * Ymax), (1 - alpha) / (2 * Ymax), 0f, 0f },
            { (1 - alpha) / (2 * Xmax), (1 - alpha) / (2 * Xmax), 0f, 0f, 0f, 0f },
            { 0f, 0f, alpha / (2 * Ymax), alpha / (2 * Ymax), 0f, 0f },
            { alpha / (2 * Xmax), alpha / (2 * Xmax), 0f, 0f, 0f, 0f },
            { -(1 - alpha) / (2 * Xmax), (1 - alpha)  / (2 * Xmax), 0f, 0f, 0f, 0f },
            { 0f, 0f, (1 - alpha) / (2 * Ymax), -(1 - alpha) / (2 * Ymax), 0f, 0f }
        };

        k[0] = 1f; // vx sensitivity
        k[1] = 1f; // vy sensitivity
        k[2] = 1f; // vz sensitivity
        k[3] = 0.1f; // wx sensitivity
        k[4] = 0.1f; // wy sensitivity
        k[5] = 0.1f; // wz sensitivity
        

    }

    public float[] GetMappedCommand(float[] J)
    {
        float G1 = J[4];
        float G2 = J[5];
        float alpha = (inputProfile.Gmax > 0f) ? (G1 * G2) / (inputProfile.Gmax * inputProfile.Gmax) : 0f;

        float Xmax = inputProfile.Xmax;
        float Ymax = inputProfile.Ymax;

        // Log current alpha
        Debug.Log($"[MappingMatrix] alpha = {alpha:F3} (G1 = {G1:F2}, G2 = {G2:F2})");

        // Rebuild the matrix M dynamically
        
        // Log current Mapping matrix
        string mString = "[MappingMatrix] M =\\n";
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                mString += $"{M[i, j]:F3}\t";                
            }
            mString += "\\n";
        }
        Debug.Log(mString);

            
        //Simple one on one mapping with an eye matrix
        /* M = new float[6, 6]
         {
             { 1f, 0f, 0f, 0f, 0f, 0f }, // vx ← X1
             { 0f, 1f, 0f, 0f, 0f, 0f }, // vy ← X2
             { 0f, 0f, 1f, 0f, 0f, 0f }, // vz ← Y1
             { 0f, 0f, 0f, 1f, 0f, 0f }, // wx ← Y2
             { 0f, 0f, 0f, 0f, 1f, 0f }, // wy ← G1
             { 0f, 0f, 0f, 0f, 0f, 1f }  // wz ← G2
         };*/
        
        // One on one mapping with translations on the right hand and rotations on the left hand
        /* M = new float[6, 6]
         {
             { 1f, 0f, 0f, 0f, 0f, 0f }, // vx ← X1
             { 0f, 1f, 0f, 0f, 0f, 0f }, // vy ← X2
             { 0f, 0f, 0f, 0f, 1f, 0f }, // vz ← Y1
             { 0f, 0f, 0f, 0f, 0f, 1f }, // wx ← Y2
             { 0f, 0f, 0f, 1f, 0f, 0f }, // wy ← G1
             { 0f, 0f, 1f, 0f, 0f, 0f }  // wz ← G2
         };*/
        
        M = new float[6, 6]
        {
            { 0f, 0f, (1 - alpha) / (2 * Ymax), (1 - alpha) / (2 * Ymax), 0f, 0f },
            { (1 - alpha) / (2 * Xmax), (1 - alpha) / (2 * Xmax), 0f, 0f, 0f, 0f },
            { 0f, 0f, -alpha / (2 * Ymax), -alpha / (2 * Ymax), 0f, 0f },
            { alpha / (2 * Xmax), alpha / (2 * Xmax), 0f, 0f, 0f, 0f },
            { -(1 - alpha) / (2 * Xmax), (1 - alpha)  / (2 * Xmax), 0f, 0f, 0f, 0f },
            { 0f, 0f, (1 - alpha) / (2 * Ymax), -(1 - alpha) / (2 * Ymax), 0f, 0f }
        };
        

        // Apply mapping: U = k ⊙ (M · J)
        float[] U = new float[6];
        for (int i = 0; i < 6; i++)
        {
            float sum = 0f;
            for (int j = 0; j < 6; j++)
                sum += M[i, j] * J[j];
            U[i] = k[i] * sum;
        }
        //Logging current U vector
        Debug.Log($"[MappingMatrix] U = [{U[0]:F3}, {U[1]:F3}, {U[2]:F3}, {U[3]:F3}, {U[4]:F3}, {U[5]:F3}]");

        return U;
    }
}