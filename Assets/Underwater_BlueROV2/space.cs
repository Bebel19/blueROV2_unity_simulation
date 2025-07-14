using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class space : MonoBehaviour
{
    // Start is called before the first frame update
    public int space_is = 1;
    void Start()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            if (space_is == 0) space_is = 1;
            else space_is = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            if (space_is == 0) space_is = 1;
            else space_is = 0;
        }
    }
}
