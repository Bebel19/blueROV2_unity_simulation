// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using OpenCVForUnity.CoreModule;
// using OpenCVForUnity.ImgprocModule;
// // using UnityEngine.UI;
// using OpenCVForUnity;
// using OpenCVForUnity.UnityUtils;
// using static OpenCVForUnity.CoreModule.CvType;
// // using Rect = OpenCVForUnity.CoreModule.Rect;


// public class Camera_script : MonoBehaviour
// {
//     // [SerializeField]
//     // private Camera ControlCam;

//     [SerializeField]
//     private RenderTexture RT;

//     private Color[] Getpixels()
//     {
//         var currentRT = RenderTexture.active;

//         RenderTexture.active = RT;

//         var texture = new Texture2D(RT.width, RT.height);
//         texture.ReadPixels(new Rect(0, 0, RT.width, RT.height), 0, 0);
//         texture.Apply();

//         var colors = texture.Getpixels();
//         RenderTexture.active = currentRT;

//         return colors;
//         // return texture;
//     }

//     private Texture2D tT2D;

//     Mat img;
//     Mat gray;
//     Mat binary;

//     public int MaxThres = 255;
//     public int minThres = 120;

//     void Start ()
//     {        
        
//     }

    

//     void Update ()
//     {
//         // image generation
//         tT2D = Getpixels();
//         img = new Mat (tT2D.height, tT2D.width, CvType.CV_8UC4);
//         OpenCVForUnity.Utils.Texture2DToMat(tT2D, img, true);

//         // gray scale
//         gray = new Mat(tT2D.height, tT2D.width, CvType.CV_8UC1);
//         Imgproc.cvtColor(img, gray, Imgproc.COLOR_RGBA2GRAY);

//         // binary scale
//         binary = new Mat(tT2D.height, tT2D.width, CvType.CV_8UC1);
//         Imgproc.threshold(gray, binary, minThres, MaxThres, Imgproc.THRESH_BINARY);

//         Texture2D output = new Texture2D(binary.cols(), binary.rows(), TextureFormat.RGBA32, false);
//         OpenCVForUnity.Utils.matToTexture2D(binary, output, true);

//         gameObject.GetComponent<Renderer>().material.mailTexture = output;
//     }

// }