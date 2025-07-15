using UnityEngine;

/// <summary>
/// Continuously reads the Unity terrain elevation at the GameObject’s X/Z position
/// and stores the height in Terrain_height. Used for reference altitude.
/// </summary>
public class Small_terrain_height : MonoBehaviour
{
    public Vector3 position;
    public float Terrain_height;

    public Terrain terrain;

    private void Start()
    {
        // Get reference to active terrain in the scene
        terrain = Terrain.activeTerrain;

        // Get initial GameObject position
        position = transform.position;

        // Retrieve terrain height at this X/Z position
        Terrain_height = terrain.terrainData.GetInterpolatedHeight(
            position.x / terrain.terrainData.size.x,
            position.z / terrain.terrainData.size.z
        );
    }

    private void FixedUpdate()
    {
        // Update position every physics frame
        position = transform.position;

        // Retrieve terrain height at current X/Z position
        Terrain_height = terrain.terrainData.GetInterpolatedHeight(
            position.x / terrain.terrainData.size.x,
            position.z / terrain.terrainData.size.z
        );
    }
}