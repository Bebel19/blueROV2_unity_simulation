using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrain_height : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 position;
    public Vector3 calc_pos;
    public Vector3 terrain_size;
    public float Terrain_height;

    public float add_height = 10;

    public Terrain[] selected_terrain;
    public Terrain terrain;


	
    void Start()
    {   
        // position = this.transform.position;     //現在の位置を取得
        selected_terrain = Terrain.activeTerrains;

    }

    // Update is called once per frame
    void Update()
    {
        position = this.transform.position;
        if (position.x < 1000){
            calc_pos.x = position.x;
            if (position.z < 0){
                terrain = selected_terrain[6];
                calc_pos.z = position.z + 1000;
            }
            else if (position.z >= 0 && position.z < 1000){
                terrain = selected_terrain[8];
                calc_pos.z = position.z;
            }
            else{
                terrain = selected_terrain[1];
                calc_pos.z = position.z - 1000;
            }
        }
        else if (position.x >= 1000 && position.x < 2000){
            calc_pos.x = position.x - 1000;
            if (position.z < 0){
                terrain = selected_terrain[2];
                calc_pos.z = position.z + 1000;
            }
            else if (position.z >= 0 && position.z < 1000){
                terrain = selected_terrain[4];
                calc_pos.z = position.z;
            }
            else{
                terrain = selected_terrain[5];
                calc_pos.z = position.z - 1000;
            }
        }
        else{
            calc_pos.x = position.x - 2000;
            if (position.z < 0){
                terrain = selected_terrain[0];
                calc_pos.z = position.z + 1000;
            }
            else if (position.z >= 0 && position.z < 1000){
                terrain = selected_terrain[7];
                calc_pos.z = position.z;
            }
            else{
                terrain = selected_terrain[3];
                calc_pos.z = position.z - 1000;
            }
        }
        //  posisionのx,z座標に対応する、terrainの高さ（y座標）を取得
        Terrain_height = terrain.terrainData.GetInterpolatedHeight(calc_pos.x / terrain.terrainData.size.x, calc_pos.z / terrain.terrainData.size.z);
        //this.transform.position=new Vector3(position.x, Terrain_height+add_height, position.z);
    }
}