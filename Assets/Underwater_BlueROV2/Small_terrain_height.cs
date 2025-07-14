using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Small_terrain_height : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 position;
    public float Terrain_height;


    public Terrain terrain;


	
    void Start()
    {   // position = this.transform.position;     //現在の位置を取得
        terrain = Terrain.activeTerrain;
        position = this.transform.position;
        Terrain_height = terrain.terrainData.GetInterpolatedHeight(position.x / terrain.terrainData.size.x, position.z / terrain.terrainData.size.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        position = this.transform.position;
       
        //  posisionのx,z座標に対応する、terrainの高さ（y座標）を取得
        Terrain_height = terrain.terrainData.GetInterpolatedHeight(position.x / terrain.terrainData.size.x, position.z / terrain.terrainData.size.z);
        //this.transform.position=new Vector3(position.x, Terrain_height+add_height, position.z);
    }
}