using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class CubeScript : MonoBehaviour
{
  [SerializeField] GameObject target1;
  [SerializeField] GameObject target2;
  [SerializeField] GameObject target3;
  [SerializeField] GameObject target4;
  [SerializeField] GameObject target5;
  [SerializeField] GameObject target6;
  [SerializeField] GameObject target7;
  [SerializeField] GameObject target8;
  [SerializeField] GameObject target9;
  [SerializeField] GameObject target0;
  [SerializeField] GameObject target21;
  [SerializeField] GameObject target22;
  [SerializeField] GameObject target23;
  [SerializeField] GameObject target24;
  [SerializeField] GameObject target25;
  [SerializeField] GameObject target26;
  [SerializeField] GameObject target27;
  [SerializeField] GameObject target28;
  [SerializeField] GameObject target29;
  [SerializeField] GameObject target20;
  [SerializeField] GameObject target31;
  [SerializeField] GameObject target32;
  [SerializeField] GameObject target33;
  [SerializeField] GameObject target34;
  [SerializeField] GameObject target35;
  [SerializeField] GameObject target36;
  [SerializeField] GameObject target37;
  [SerializeField] GameObject target38;
  [SerializeField] GameObject target39;
  [SerializeField] GameObject target30;
  private StreamWriter sw;
  void Start()
  {
    // target1 = GameObject.Find("square(1)");
    // target2 = GameObject.Find("square(2)");
    // target3 = GameObject.Find("square(3)");
    // target4 = GameObject.Find("square(4)");
    // target5 = GameObject.Find("square(5)");
    // target6 = GameObject.Find("square(6)");
    // target7 = GameObject.Find("square(7)");
    // target8 = GameObject.Find("square(8)");
    // target9 = GameObject.Find("square(9)");
    // target0 = GameObject.Find("square(10)");
    FileInfo fi;
    fi = new FileInfo("objects.csv");
    sw = fi.AppendText();
    sw.Write(target1.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target1.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target1.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target2.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target2.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target2.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target3.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target3.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target3.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target4.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target4.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target4.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target5.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target5.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target5.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target6.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target6.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target6.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target7.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target7.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target7.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target8.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target8.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target8.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target9.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target9.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target9.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target0.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target0.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target0.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target21.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target21.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target21.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target22.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target22.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target22.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target23.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target23.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target23.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target24.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target24.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target24.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target25.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target25.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target25.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target26.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target26.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target26.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target27.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target27.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target27.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target28.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target28.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target28.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target29.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target29.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target29.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target20.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target20.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target20.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target31.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target31.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target31.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target32.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target32.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target32.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target33.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target33.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target33.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target34.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target34.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target34.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target35.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target35.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target35.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target36.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target36.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target36.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target37.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target37.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target37.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target38.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target38.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target38.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target39.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target39.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target39.transform.position.z.ToString());
    sw.Write("\n");
    sw.Write(target30.transform.position.x.ToString());
    sw.Write(",");
    sw.Write(target30.transform.position.y.ToString());
    sw.Write(",");
    sw.Write(target30.transform.position.z.ToString());
    sw.Write("\n");
    sw.Flush();
    sw.Close();

    Debug.Log(target1.transform.position.x.ToString());
    Debug.Log(target2.transform.position.x.ToString());
    Debug.Log(target3.transform.position.x.ToString());
    Debug.Log(target4.transform.position.x.ToString());
    Debug.Log(target5.transform.position.x.ToString());
    Debug.Log(target6.transform.position.x.ToString());
    Debug.Log(target7.transform.position.x.ToString());
    Debug.Log(target8.transform.position.x.ToString());
    Debug.Log(target9.transform.position.x.ToString());
    Debug.Log(target0.transform.position.x.ToString());

  }
 
  void Update()
  {
    
  }
}
