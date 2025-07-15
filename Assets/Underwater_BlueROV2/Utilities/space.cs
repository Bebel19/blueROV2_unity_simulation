using UnityEngine;

/// <summary>
/// Binary toggle using the spacebar key.
/// Used as a global flag for switching modes (e.g., enabling/disabling control).
/// </summary>
public class space : MonoBehaviour
{
    public int space_is = 1;

    private void Update()
    {
        // Toggle between 0 and 1 on spacebar press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            space_is = (space_is == 0) ? 1 : 0;
        }
    }
}