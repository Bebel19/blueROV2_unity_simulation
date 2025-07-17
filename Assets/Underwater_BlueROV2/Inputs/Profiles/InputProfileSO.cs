using UnityEngine;

/// <summary>
/// ScriptableObject defining the normalization values for a specific input device.
/// </summary>
[CreateAssetMenu(fileName = "InputProfile", menuName = "BlueROV/Input Profile", order = 1)]
public class InputProfileSO : ScriptableObject
{
    [Header("Normalization Parameters")]
    public float Xmax = 1f;
    public float Ymax = 1f;
    public float G1 = 1f;
    public float G2 = 1f;
    public float Gmax = 1f;

    public float Alpha => (G1 * G2) / (Gmax * Gmax);
}