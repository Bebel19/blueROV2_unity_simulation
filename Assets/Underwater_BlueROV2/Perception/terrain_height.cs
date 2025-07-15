using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Computes terrain height at the GameObject's position across a grid of 9 terrain tiles.
/// </summary>
public class terrain_height : MonoBehaviour
{
    public Vector3 position;
    public Vector3 calc_pos;
    public Vector3 terrain_size;
    public float Terrain_height;

    public float add_height = 10f;

    public Terrain[] selected_terrain;
    public Terrain terrain;

    void Start()
    {
        // Cache all active terrain tiles in the scene
        selected_terrain = Terrain.activeTerrains;
    }

    void Update()
    {
        position = this.transform.position;

        // Determine which terrain tile to sample based on global X/Z position
        if (position.x < 1000)
        {
            calc_pos.x = position.x;

            if (position.z < 0)
            {
                terrain = selected_terrain[6];
                calc_pos.z = position.z + 1000;
            }
            else if (position.z >= 0 && position.z < 1000)
            {
                terrain = selected_terrain[8];
                calc_pos.z = position.z;
            }
            else
            {
                terrain = selected_terrain[1];
                calc_pos.z = position.z - 1000;
            }
        }
        else if (position.x >= 1000 && position.x < 2000)
        {
            calc_pos.x = position.x - 1000;

            if (position.z < 0)
            {
                terrain = selected_terrain[2];
                calc_pos.z = position.z + 1000;
            }
            else if (position.z >= 0 && position.z < 1000)
            {
                terrain = selected_terrain[4];
                calc_pos.z = position.z;
            }
            else
            {
                terrain = selected_terrain[5];
                calc_pos.z = position.z - 1000;
            }
        }
        else
        {
            calc_pos.x = position.x - 2000;

            if (position.z < 0)
            {
                terrain = selected_terrain[0];
                calc_pos.z = position.z + 1000;
            }
            else if (position.z >= 0 && position.z < 1000)
            {
                terrain = selected_terrain[7];
                calc_pos.z = position.z;
            }
            else
            {
                terrain = selected_terrain[3];
                calc_pos.z = position.z - 1000;
            }
        }

        // Compute terrain height at the adjusted (local) position
        Terrain_height = terrain.terrainData.GetInterpolatedHeight(
            calc_pos.x / terrain.terrainData.size.x,
            calc_pos.z / terrain.terrainData.size.z
        );

        // Optional: apply the height to the GameObject
        // this.transform.position = new Vector3(position.x, Terrain_height + add_height, position.z);
    }
}
