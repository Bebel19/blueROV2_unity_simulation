using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity;
using OpenCVForUnity.UnityUtils;
// namespace OpenCVForUnityExample
// {
   
// }
public class CreateTexture : MonoBehaviour
{
    
    //このScriptはMainCameraにアタッチしてください

    public RenderTexture renderTexture;             //mainCameraにつけるRendertexture(アタッチしてね)
    Texture2D tex2D;
    

    // public Texture2D output_tex;
    public double tmp;

    public float pre_conf_det;
    public float now_conf_det;
    public float aft_conf_det;

    float pre_conf;
    float now_conf;
    float aft_conf;
    Mat image;
    Mat gray;
    Mat binary;
    Mat binary2;

    Mat convert;

    Mat Mask;
    Mat Mask2;

    List<Point> points;

    int Center_x;
    int Center_y;

    public float errory_mat;
    public float errorag_mat;

    public float errory_mat_before;
    public float errorag_mat_before;
    public float confidence;

    public int MaxThres = 255;
    public int minThres = 45;
    public int mask_radius = 180;
    public int gaus = 5;

    public float estimated_line_x_1 = 0.0f;
    public float estimated_line_x_2 = 0.0f;
    public float estimated_line_x_3 = 0.0f;
    public float estimated_line_x_ave = 0.0f;

    float esti_befo_line_x_1 = 0.0f;
    float esti_befo_line_x_2 = 0.0f;
    float esti_befo_line_x_3 = 0.0f;
    float esti_befo_line_x_ave = 0.0f;

    public int cutting = 50;

    float diff = 50.0f;

    int first_timing = 0;

    int predict_y1 = 0;
    int predict_y2 = 0;
    int predict_y3 = 0;
    int predict_y4 = 0;
    int predict_y5 = 0;
    int predict_y6 = 0;
    int predict_y7 = 0;
    int predict_y8 = 0;
    int predict_y9  = 0;
    int predict_y10 = 0;
    int predict_y11 = 0;
    int predict_y12 = 0;
    int predict_y13 = 0;
    int predict_y14 = 0;
    int predict_y15 = 0;

    float[] pre_conf_buf;
    float[] now_conf_buf;
    float[] aft_conf_buf;

    int done_y1 = 0;
    int done_y2 = 0;

    public float err_y_buf;
    public float err_th_buf;

    int err_flag = 0;

    int boole = 100;

    int y1_beforex1 = 0;
    int y1_beforex2 = 0;
    int y2_beforex1 = 0;
    int y2_beforex2 = 0;
    int y3_beforex1 = 0;
    int y3_beforex2 = 0;

    public int y1_beforeL = 9000;
    public int y1_beforeR = 9000;
    public int y2_beforeL = 9000;
    public int y2_beforeR = 9000;
    public int y3_beforeL = 9000;
    public int y3_beforeR = 9000;

    public int y1LeftBuffer = 9000;
    public int y1RightBuffer = 9000;
    public int y2LeftBuffer = 9000;
    public int y2RightBuffer = 9000;
    public int y3LeftBuffer = 9000;
    public int y3RightBuffer = 9000;

    public float y1LeftX = 0.0f;
    public float y1RightX = 0.0f;
    public float y2LeftX = 0.0f;
    public float y2RightX = 0.0f;
    public float y3LeftX = 0.0f;
    public float y3RightX = 0.0f;

    public float bf_y1LeftX = 0.0f;
    public float bf_y1RightX = 0.0f;
    public float bf_y2LeftX = 0.0f;
    public float bf_y2RightX = 0.0f;
    public float bf_y3LeftX = 0.0f;
    public float bf_y3RightX = 0.0f;

    public float tmp_y1LeftX = 0.0f;
    public float tmp_y1RightX = 0.0f;
    public float tmp_y2LeftX = 0.0f;
    public float tmp_y2RightX = 0.0f;
    public float tmp_y3LeftX = 0.0f;
    public float tmp_y3RightX = 0.0f;


    void Start ()
    {
        tex2D = CreateTexture2D(renderTexture);
        this.image = new Mat(tex2D.height, tex2D.width, CvType.CV_8UC4);
        this.Mask = new Mat(tex2D.height, tex2D.width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
        Imgproc.circle(this.Mask, new Point(this.image.width() / 2 + 20, this.image.height() / 2), mask_radius, new Scalar(255, 255, 255, 255), -1);
        this.convert = new Mat(tex2D.height, tex2D.width, CvType.CV_8UC4);
        this.gray = new Mat(tex2D.height, tex2D.width, CvType.CV_8UC4);
        this.binary = new Mat(tex2D.height, tex2D.width, CvType.CV_8UC4);
        this.binary2 = new Mat(tex2D.height, tex2D.width, CvType.CV_8UC4);
        // this.output_tex = new Texture2D(image.cols(), image.rows(), TextureFormat.RGBA32, false);
        // this.output_tex = new Texture2D(binary.cols(), binary.rows(), TextureFormat.RGBA32, false);
        Center_x = this.image.width() / 2;
        Center_y = this.image.height() / 2;
        this.points = new  List<Point>();
        confidence = 0.0f;
        errory_mat = 0.0f;
        errorag_mat = 0.0f;
        errory_mat_before = 0.0f;
        errorag_mat_before = 0.0f;
        pre_conf_buf = new float [boole];
        now_conf_buf = new float [boole];
        aft_conf_buf = new float [boole];

        for (int i=0;i<boole;i++){
            pre_conf_buf[i] = 0.0f;
            now_conf_buf[i] = 0.0f;
            aft_conf_buf[i] = 0.0f;
        }
        y1LeftX = Center_x;
        y1RightX = Center_x;
        y2LeftX = Center_x;
        y2RightX = Center_x;
        y3LeftX = Center_x;
        y3RightX = Center_x;
        bf_y1LeftX = Center_x;
        bf_y1RightX = Center_x;
        bf_y2LeftX = Center_x;
        bf_y2RightX = Center_x;
        bf_y3LeftX = Center_x;
        bf_y3RightX = Center_x;
        tmp_y1LeftX = Center_x;
        tmp_y1RightX = Center_x;
        tmp_y2LeftX = Center_x;
        tmp_y2RightX = Center_x;
        tmp_y3LeftX = Center_x;
        tmp_y3RightX = Center_x;
    }

    void Update()
    {

        
        tex2D = CreateTexture2D(renderTexture);
        OpenCVForUnity.UnityUtils.Utils.texture2DToMat(tex2D, this.image, true);
        Destroy(tex2D);
        estimated_line_x_ave = 0.0f;
        confidence = 0.0f;

        // Imgproc.GaussianBlur(this.binary, this.binary, new Size(gaus, gaus), 0);
        
        // OpenCVForUnity.CoreModule.Rect roi = new OpenCVForUnity.CoreModule.Rect(30, 30, this.binary.cols()-60, this.binary.rows()-60);
        OpenCVForUnity.CoreModule.Rect roi = new OpenCVForUnity.CoreModule.Rect(30, 30, this.image.cols()-60, this.image.rows()-60);
        
        Mat reMat = new Mat(this.image, roi);
        // this.image = new Mat(reMat);
        // gray scale
        Imgproc.cvtColor(reMat, this.gray, Imgproc.COLOR_RGBA2GRAY);
        // // binary scale
        Imgproc.threshold(this.gray, this.binary, 10, 255, Imgproc.THRESH_BINARY);
        // Imgproc.resize(this.binary, this.binary, new Size(this.binary.cols()/resize_int, this.binary.rows()/resize_int));
        // Imgproc.resize(this.binary, this.binary, new Size(this.binary.cols() *resize_int, this.binary.rows() * resize_int));

        List<MatOfPoint> contours = new List<MatOfPoint>();
        List<MatOfPoint> contours2 = new List<MatOfPoint>();
        // Core.bitwise_not(this.binary, this.binary);
        Imgproc.findContours(this.binary, contours, new Mat(), Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);
        Imgproc.drawContours(this.binary, contours, -1, new Scalar(255, 255, 255, 255), -1);
        // OpenCVForUnity.CoreModule.Rect roi = new OpenCVForUnity.CoreModule.Rect(30, 30, this.binary.cols()-60, this.binary.rows()-60);
        // Mat reMat = new Mat(this.binary, roi);
        // Imgproc.drawContours(this.binary, contours, -1, new Scalar(255, 255, 255, 255), 2);
        Core.bitwise_not(this.binary, this.binary);
        Imgproc.findContours(this.binary, contours2, new Mat(), Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_NONE);
        gaus = contours2.Count;
        Scalar color = new Scalar(255, 255, 255, 255);
        float sum_x_point1 = 0.0f;
        float sum_x_point2 = 0.0f;
        float sum_x_point3 = 0.0f;
        int count_x_point1 = 0;
        int count_x_point2 = 0;
        int count_x_point3 = 0;

        int cef = 2;
        int start_n = 30;

        predict_y1 = Center_y - cef  * (start_n + 4);//(start_n + 14);
        predict_y2 = Center_y - cef  * (start_n + 4);//(start_n + 13);
        predict_y3 = Center_y - cef  * (start_n + 4);//(start_n + 12);
        predict_y4 = Center_y - cef  * (start_n + 4);//(start_n + 11);
        predict_y5 = Center_y - cef  * (start_n + 4);//(start_n + 10);
        predict_y6 = Center_y - cef  * (start_n + 3);//(start_n + 9);
        predict_y7 = Center_y - cef  * (start_n + 3);//(start_n + 8);
        predict_y8 = Center_y - cef  * (start_n + 3);//(start_n + 7);
        predict_y9 = Center_y - cef  * (start_n + 2);//(start_n + 6);
        predict_y10 = Center_y - cef * (start_n + 2);//(start_n + 5);
        predict_y11 = Center_y - cef * (start_n + 2);//(start_n + 4);
        predict_y12 = Center_y - cef * (start_n + 1);//(start_n + 3);
        predict_y13 = Center_y - cef * (start_n + 1);//(start_n + 2);
        predict_y14 = Center_y - cef * (start_n + 0);//(start_n + 1);
        predict_y15 = Center_y - cef * (start_n + 0);//start_n;
        float sum_x_predict1 = 0.0f;
        float sum_x_predict2 = 0.0f;
        float sum_x_predict3 = 0.0f;
        float sum_x_predict4 = 0.0f;
        float sum_x_predict5 = 0.0f;
        float sum_x_predict6 = 0.0f;
        float sum_x_predict7 = 0.0f;
        float sum_x_predict8 = 0.0f;
        int count_x_predict1 = 0;
        int count_x_predict2 = 0;
        int count_x_predict3 = 0;
        int count_x_predict4 = 0;
        int count_x_predict5 = 0;
        int count_x_predict6 = 0;
        int count_x_predict7 = 0;
        int count_x_predict8 = 0;
        int count_x_predict9 = 0;
        int count_x_predict10 = 0;
        int count_x_predict11 = 0;
        int count_x_predict12 = 0;
        int count_x_predict13 = 0;
        int count_x_predict14 = 0;
        int count_x_predict15 = 0;
        
        int y_point1 = Center_y + 10;
        int y_point2 = Center_y - 10;
        int y_point3 = Center_y;
        
        done_y1 = Center_y + 60;
        done_y2 = Center_y + 80;
        float sum_x_done1 = 0.0f;
        float sum_x_done2 = 0.0f;
        int count_x_done1 = 0;
        int count_x_done2 = 0;

        float Cutoff1 = 1.0f;

        // int color_R = 10;
        if (gaus > 0){
            for (int i = 0; i < gaus; i++)
            {
                tmp = Imgproc.contourArea(contours2[i]);
                // Debug.Log(tmp);
                if (tmp > 1000.0 && tmp < 8500.0){
                    // Imgproc.drawContours(reMat, contours2, i, new Scalar(200, 200, 200, 255), 1);
                    this.points = contours2[i].toList();
                    int y1LB = 1000;
                    int y2LB = 1000;
                    int y3LB = 1000;
                    List<int> Y1points_list = new List<int>();
                    List<int> Y2points_list = new List<int>();
                    List<int> Y3points_list = new List<int>();

                    // Debug.Log(this.points.Count);
                    for (int j = 0; j < this.points.Count; j++){
                        if (this.points[j].y == y_point1){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                int NOW_point = 30 + (int)this.points[j].x;
                                Y1points_list.Add(NOW_point);
                                // if (y1LB >= NOW_point){
                                //     y1LeftBuffer = NOW_point;
                                //     y1LB = NOW_point;
                                // }else if (y1LB < NOW_point - 5){
                                //     y1RightBuffer = NOW_point;
                                // }

                                // if (y1_beforeL == 9000){
                                //     y1_beforeL = y1LeftBuffer;
                                // }

                                // if (y1_beforeR == 9000){
                                //     y1_beforeR = y1RightBuffer;
                                // }

                                // int y1L_diff = Mathf.Abs(y1_beforeL - NOW_point);
                                

                                // int y1R_diff = Mathf.Abs(y1_beforeR - NOW_point);
                                

                                
                                // if (y1L_diff < 10){
                                //     tmp_y1LeftX = (float)y1LeftBuffer;
                                //     y1_beforeL = y1LeftBuffer;
                                //     Imgproc.circle(image, new Point((int)tmp_y1LeftX, 30 + y_point1), 3, new Scalar(255, 0, 0, 255), -1);
                                // }else{
                                //     tmp_y1LeftX = (float)y1_beforeL;
                                //     y1_beforeL = 9000;
                                //     Imgproc.circle(image, new Point((int)tmp_y1LeftX, 30 + y_point1), 3, new Scalar(255, 0, 0, 255), -1);
                                // }

                                // if (y1R_diff < 10){
                                //     tmp_y1RightX = (float)y1RightBuffer;
                                //     y1_beforeR = y1RightBuffer;
                                //     Imgproc.circle(image, new Point((int)tmp_y1RightX, 30 + y_point1), 3, new Scalar(0, 255, 0, 255), -1);
                                // }else{
                                //     tmp_y1RightX = (float)y1_beforeR;
                                //     y1_beforeR = 9000;
                                //     Imgproc.circle(image, new Point((int)tmp_y1RightX, 30 + y_point1), 3, new Scalar(0, 255, 0, 255), -1);
                                // }
                            }


                        }else if (this.points[j].y == y_point2){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                int NOW_point = 30 + (int)this.points[j].x;
                                Y2points_list.Add(NOW_point);
                                // if (y2LB >= NOW_point){
                                //     y2LeftBuffer = NOW_point;
                                //     y2LB = NOW_point;
                                // }else if (y2LB < NOW_point - 5){
                                //     y2RightBuffer = NOW_point;
                                // }

                                // if (y2_beforeL == 1000){
                                //     y2_beforeL = y2LeftBuffer;
                                // }

                                // if (y2_beforeR == 1000){
                                //     y2_beforeR = y2RightBuffer;
                                // }

                                // int y2L_diff = Mathf.Abs(y2_beforeL - NOW_point);
                                // int y2R_diff = Mathf.Abs(y2_beforeR - NOW_point);
                                

                                // if (y2L_diff < 10){
                                //     tmp_y2LeftX = (float)y2LeftBuffer;
                                //     y2_beforeL = y2LeftBuffer;
                                //     Imgproc.circle(image, new Point((int)tmp_y2LeftX, 30 + y_point2), 3, new Scalar(255, 0, 255, 255), -1);
                                // }else{
                                //     tmp_y2LeftX = (float)y2_beforeL;
                                //     Imgproc.circle(image, new Point((int)tmp_y2LeftX, 30 + y_point2), 3, new Scalar(255, 0, 255, 255), -1);
                                // }

                                // if (y2R_diff < 10){
                                //     tmp_y2RightX = (float)y2RightBuffer;
                                //     y2_beforeR = y2RightBuffer;
                                //     Imgproc.circle(image, new Point((int)tmp_y2RightX, 30 + y_point2), 3, new Scalar(0, 255, 255, 255), -1);
                                // }else{
                                //     tmp_y2RightX = (float)y2_beforeR;
                                //     Imgproc.circle(image, new Point((int)tmp_y2RightX, 30 + y_point2), 3, new Scalar(0, 255, 255, 255), -1);
                                // }
                            }
                        }else if (this.points[j].y == y_point3){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                int NOW_point = 30 + (int)this.points[j].x;
                                Y3points_list.Add(NOW_point);
                                // if (y3LB >= NOW_point){
                                //     y3LeftBuffer = NOW_point;
                                //     y3LB = NOW_point;
                                // }else if (y3LB < NOW_point - 5){
                                //     y3RightBuffer = NOW_point;
                                // }

                                // if (y3_beforeL == 1000){
                                //     y3_beforeL = y3LeftBuffer;
                                // }

                                // if (y3_beforeR == 1000){
                                //     y3_beforeR = y3RightBuffer;
                                // }

                                // int y3L_diff = Mathf.Abs(y3_beforeL - NOW_point);
                                // int y3R_diff = Mathf.Abs(y3_beforeR - NOW_point);
                                

                                // if (y3L_diff < 10){
                                //     tmp_y3LeftX = (float)y3LeftBuffer;
                                //     y3_beforeL = y3LeftBuffer;
                                //     // Imgproc.circle(image, new Point((int)y3LeftX, 30 + y_point3), 3, new Scalar(255, 170, 255, 255), -1);
                                // }else{
                                //     tmp_y3LeftX = (float)y3_beforeL;
                                //     Imgproc.circle(image, new Point((int)tmp_y3LeftX, 30 + y_point3), 3, new Scalar(255, 170, 255, 255), -1);
                                // }

                                // if (y3R_diff < 10){
                                //     tmp_y3RightX = (float)y3RightBuffer;
                                //     y3_beforeR = y3RightBuffer;
                                //     Imgproc.circle(image, new Point((int)tmp_y3RightX, 30 + y_point3), 3, new Scalar(170, 255, 255, 255), -1);
                                // }else{
                                //     tmp_y3RightX = (float)y3_beforeR;
                                //     Imgproc.circle(image, new Point((int)tmp_y3RightX, 30 + y_point3), 3, new Scalar(170, 255, 255, 255), -1);
                                // }
                            }
                        }else if (this.points[j].y == predict_y1){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                sum_x_predict1 += (float)this.points[j].x;
                                count_x_predict1 += 1;
                            }
                        }else if (this.points[j].y == predict_y2){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                sum_x_predict2 += (float)this.points[j].x;
                                count_x_predict2 += 1;
                            }
                        }else if (this.points[j].y == predict_y3){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                sum_x_predict3 += (float)this.points[j].x;
                                count_x_predict3 += 1;
                            }
                        }else if (this.points[j].y == predict_y4){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                sum_x_predict4 += (float)this.points[j].x;
                                count_x_predict4 += 1;
                            }
                        }else if (this.points[j].y == predict_y5){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                sum_x_predict5 += (float)this.points[j].x;
                                count_x_predict5 += 1;
                            }
                        }else if (this.points[j].y == predict_y6){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                sum_x_predict6 += (float)this.points[j].x;
                                count_x_predict6 += 1;
                            }
                        }else if (this.points[j].y == predict_y7){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                sum_x_predict7 += (float)this.points[j].x;
                                count_x_predict7 += 1;
                            }
                        }else if (this.points[j].y == predict_y8){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                sum_x_predict8 += (float)this.points[j].x;
                                count_x_predict8 += 1;
                            }
                        }else if (this.points[j].y == predict_y9){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                // sum_x_predict9 += (float)this.points[j].x;
                                count_x_predict9 += 1;
                            }
                        }else if (this.points[j].y == predict_y10){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                // sum_x_predict10 += (float)this.points[j].x;
                                count_x_predict10 += 1;
                            }
                        }else if (this.points[j].y == predict_y11){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                // sum_x_predict11 += (float)this.points[j].x;
                                count_x_predict11 += 1;
                            }
                        }else if (this.points[j].y == predict_y12){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                // sum_x_predict12 += (float)this.points[j].x;
                                count_x_predict12 += 1;
                            }
                        }else if (this.points[j].y == predict_y13){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                // sum_x_predict13 += (float)this.points[j].x;
                                count_x_predict13 += 1;
                            }
                        }else if (this.points[j].y == predict_y14){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                // sum_x_predict14 += (float)this.points[j].x;
                                count_x_predict14 += 1;
                            }
                        }else if (this.points[j].y == predict_y15){
                            if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                                // sum_x_predict15 += (float)this.points[j].x;
                                count_x_predict15 += 1;
                            }
                        }
                        // }else if (this.points[j].y == done_y1){
                        //     if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                        //         sum_x_done1 += (float)this.points[j].x;
                        //         count_x_done1 += 1;
                        //     }
                        // }else if (this.points[j].y == done_y2){
                        //     if (this.points[j].x > cutting && this.points[j].x < this.image.cols() - cutting){
                        //         sum_x_done2 += (float)this.points[j].x;
                        //         count_x_done2 += 1;
                        //     }
                        // }
                    }


                    // Debug.Log("Y1 points num");
                    // Debug.Log(Y1points_list.Count);

                    // Debug.Log("Y2 points num");
                    // Debug.Log(Y2points_list.Count);

                    // Debug.Log("Y3 points num");
                    // Debug.Log(Y3points_list.Count);

                    if (Y1points_list.Count == 2){
                        tmp_y1LeftX = Y1points_list[0];
                        tmp_y1RightX= Y1points_list[1];
                    }else{
                        tmp_y1LeftX = bf_y1LeftX;
                        tmp_y1RightX= bf_y1RightX;
                    }
                    if (Y2points_list.Count == 2){
                        tmp_y2LeftX = Y2points_list[0];
                        tmp_y2RightX= Y2points_list[1];
                    }else{
                        tmp_y2LeftX = bf_y2LeftX;
                        tmp_y2RightX= bf_y2RightX;
                    }
                    if (Y3points_list.Count == 2){
                        tmp_y3LeftX = Y3points_list[0];
                        tmp_y3RightX= Y3points_list[1];
                    }else{
                        tmp_y3LeftX = bf_y3LeftX;
                        tmp_y3RightX= bf_y3RightX;
                    }

                    // if (float.IsInfinity(sum_x_point1)){
                    //     sum_x_point1 = 0.0f;
                    // }
                    // if (float.IsInfinity(sum_x_point2)){
                    //     sum_x_point2 = 0.0f;
                    // }
                    // if (float.IsInfinity(sum_x_point3)){
                    //     sum_x_point3 = 0.0f;
                    // }
                }
            }
            pre_conf = ((float)(count_x_predict1 + count_x_predict2 + count_x_predict3 + count_x_predict4 + 
                                count_x_predict5 + count_x_predict6 + count_x_predict7 + count_x_predict8 + 
                                count_x_predict9 + count_x_predict10 + count_x_predict11 + count_x_predict12 +
                                count_x_predict13 + count_x_predict14 + count_x_predict15)) * 3.0f;
            // now_conf = ((float)(count_x_point1 + count_x_point2 + count_x_point3));
            // aft_conf = ((float)(count_x_done1 + count_x_done2));

            pre_conf = pre_conf / 2.0f;
            // now_conf = now_conf * 7.0f / 6.0f;
            // aft_conf = aft_conf * 2.0f / 4.0f;
            //              total:15

            
            
            y1LeftX  = bf_y1LeftX  + (tmp_y1LeftX  - bf_y1LeftX) * Time.deltaTime * 2.0f * Mathf.PI * Cutoff1;
            y1RightX = bf_y1RightX + (tmp_y1RightX - bf_y1RightX) * Time.deltaTime * 2.0f * Mathf.PI * Cutoff1;
            y2LeftX  = bf_y2LeftX  + (tmp_y2LeftX  - bf_y2LeftX) * Time.deltaTime * 2.0f * Mathf.PI * Cutoff1;
            y2RightX = bf_y2RightX + (tmp_y2RightX - bf_y2RightX) * Time.deltaTime * 2.0f * Mathf.PI * Cutoff1;
            y3LeftX  = bf_y3LeftX  + (tmp_y3LeftX  - bf_y3LeftX) * Time.deltaTime * 2.0f * Mathf.PI * Cutoff1;
            y3RightX = bf_y3RightX + (tmp_y3RightX - bf_y3RightX) * Time.deltaTime * 2.0f * Mathf.PI * Cutoff1;

            Imgproc.circle(image, new Point((int)y1LeftX, 30 + y_point1), 3, new Scalar(255, 20, 0, 255), -1);
            Imgproc.circle(image, new Point((int)y1RightX, 30 + y_point1), 3, new Scalar(255, 200, 0, 255), -1);
            Imgproc.circle(image, new Point((int)y2LeftX, 30 + y_point2), 3, new Scalar(255, 20, 50, 255), -1);
            Imgproc.circle(image, new Point((int)y2RightX, 30 + y_point2), 3, new Scalar(255, 200, 50, 255), -1);
            Imgproc.circle(image, new Point((int)y3LeftX, 30 + y_point3), 3, new Scalar(255, 20, 100, 255), -1);
            Imgproc.circle(image, new Point((int)y3RightX, 30 + y_point3), 3, new Scalar(255, 200, 100, 255), -1);

            if (Mathf.Abs(y1LeftX - y1RightX) < 8.0f){
                pre_conf -= 3.0f;
            }
            if (Mathf.Abs(y2LeftX - y2RightX) < 8.0f){
                pre_conf -= 3.0f;
            }
            if (Mathf.Abs(y3LeftX - y3RightX) < 8.0f){
                pre_conf -= 2.0f;
            }           

            bf_y1LeftX  = y1LeftX;
            bf_y1RightX = y1RightX;
            bf_y2LeftX  = y2LeftX;
            bf_y2RightX = y2RightX;
            bf_y3LeftX  = y3LeftX;
            bf_y3RightX = y3RightX;





            // if (pre_conf > 12.0f){
            //     pre_conf = 0.0f;
            // }else if (pre_conf > 6.0f && pre_conf <= 12.0f){
            //     pre_conf = 12.0f - pre_conf;
            // }
            // if (now_conf > 14.0f){
            //     now_conf = 0.0f;
            // }else if (now_conf > 7.0f && now_conf <= 14.0f){
            //     now_conf = 14.0f - now_conf;
            // }
            // if (aft_conf > 4.0f){
            //     aft_conf = 0.0f;
            // }else if (aft_conf > 2.0f && aft_conf <= 4.0f){
            //     aft_conf = 4.0f - aft_conf;
            // }
            if (pre_conf > 30.0f){
                pre_conf = 0.0f;
            }else if (pre_conf > 15.0f && pre_conf <= 30.0f){
                pre_conf = 30.0f - 1.1f * pre_conf;
                if (pre_conf < 0.0f) pre_conf = 0.0f;
            }
            // if (now_conf > 14.0f){
            //     now_conf = 0.0f;
            // }else if (now_conf > 7.0f && now_conf <= 14.0f){
            //     now_conf = 14.0f - now_conf;
            // }
            // if (aft_conf > 4.0f){
            //     aft_conf = 0.0f;
            // }else if (aft_conf > 2.0f && aft_conf <= 4.0f){
            //     aft_conf = 4.0f - aft_conf;
            // }

            for (int k = 0; k < boole - 1; k++){
                pre_conf_buf[k] = pre_conf_buf[k+1];
                // now_conf_buf[k] = now_conf_buf[k+1];
                // aft_conf_buf[k] = aft_conf_buf[k+1];
            }

            pre_conf_buf[boole - 1] = pre_conf;
            // now_conf_buf[boole - 1] = now_conf;
            // aft_conf_buf[boole - 1] = aft_conf;

            float sum_pre_buf = 0.0f;
            // float sum_now_buf = 0.0f;
            // float sum_aft_buf = 0.0f;

            for (int k = 0; k < boole; k++){
                sum_pre_buf += pre_conf_buf[k];
                // sum_now_buf += now_conf_buf[k];
                // sum_aft_buf += aft_conf_buf[k];
            }
            pre_conf_det = sum_pre_buf / (float)boole;
            // now_conf_det = sum_now_buf / (float)boole;
            // aft_conf_det = sum_aft_buf / (float)boole;
                        
            

            if(pre_conf_det < 0.0f) pre_conf_det = 0.0f;
            confidence = pre_conf_det; // + now_conf_det + aft_conf_det;

            if(count_x_point1 == 0){
                sum_x_point1 = Center_x;
                count_x_point1 = 1;
            }
            if(count_x_point2 == 0){
                sum_x_point2 = Center_x;
                count_x_point2 = 1;
            }
            if(count_x_point3 == 0){
                sum_x_point3 = Center_x;
                count_x_point3 = 1;
            }



            if (y1LeftX > 800.0f)y1LeftX = Center_x;
            if (y1RightX > 800.0f)y1RightX = Center_x;
            if (y2LeftX > 800.0f)y2LeftX = Center_x;
            if (y2RightX > 800.0f)y2RightX = Center_x;
            if (y3LeftX > 800.0f)y3LeftX = Center_x;
            if (y3RightX > 800.0f)y3RightX = Center_x;

            if (first_timing > 0){
                esti_befo_line_x_1 = estimated_line_x_1;
                esti_befo_line_x_2 = estimated_line_x_2;
                esti_befo_line_x_3 = estimated_line_x_3;

                // estimated_line_x_1 = ((float)(sum_x_point1/count_x_point1) + (float)(sum_x_point2/count_x_point2)) / 2.0f;
                // estimated_line_x_2 = ((float)(sum_x_point2/count_x_point2) + (float)(sum_x_point3/count_x_point3)) / 2.0f;
                // estimated_line_x_3 = ((float)(sum_x_point3/count_x_point3) + (float)(sum_x_point1/count_x_point1)) / 2.0f;
                estimated_line_x_1 = ((y1LeftX + y1RightX) / 2.0f + (y2LeftX + y2RightX) / 2.0f) / 2.0f;
                estimated_line_x_2 = ((y1LeftX + y1RightX) / 2.0f + (y3LeftX + y3RightX) / 2.0f) / 2.0f;
                estimated_line_x_3 = ((y2LeftX + y2RightX) / 2.0f + (y3LeftX + y3RightX) / 2.0f) / 2.0f;

                

            }else{
                // estimated_line_x_1 = ((float)(sum_x_point1/count_x_point1) + (float)(sum_x_point2/count_x_point2)) / 2.0f;
                // estimated_line_x_2 = ((float)(sum_x_point2/count_x_point2) + (float)(sum_x_point3/count_x_point3)) / 2.0f;
                // estimated_line_x_3 = ((float)(sum_x_point3/count_x_point3) + (float)(sum_x_point1/count_x_point1)) / 2.0f;
                estimated_line_x_1 = ((y1LeftX + y1RightX) / 2.0f + (y2LeftX + y2RightX) / 2.0f) / 2.0f;
                estimated_line_x_2 = ((y1LeftX + y1RightX) / 2.0f + (y3LeftX + y3RightX) / 2.0f) / 2.0f;
                estimated_line_x_3 = ((y2LeftX + y2RightX) / 2.0f + (y3LeftX + y3RightX) / 2.0f) / 2.0f;
                
                esti_befo_line_x_1 = estimated_line_x_1;
                esti_befo_line_x_2 = estimated_line_x_2;
                esti_befo_line_x_3 = estimated_line_x_3;
                first_timing = 1;
            }
            if (Mathf.Abs(estimated_line_x_1 - esti_befo_line_x_1) > diff){
                estimated_line_x_1 = esti_befo_line_x_1 + Mathf.Sign(estimated_line_x_1 - esti_befo_line_x_1) * diff;
            }
            if (Mathf.Abs(estimated_line_x_2 - esti_befo_line_x_2) > diff){
                estimated_line_x_2 = esti_befo_line_x_2 + Mathf.Sign(estimated_line_x_2 - esti_befo_line_x_2) * diff;
            }
            if (Mathf.Abs(estimated_line_x_3 - esti_befo_line_x_3) > diff){
                estimated_line_x_3 = esti_befo_line_x_3 + Mathf.Sign(estimated_line_x_3 - esti_befo_line_x_3) * diff;
            }

            if (float.IsInfinity(estimated_line_x_1)){
                estimated_line_x_1 = Center_x;
            }
            if (float.IsInfinity(estimated_line_x_2)){
                estimated_line_x_2 = Center_x;
            }
            if (float.IsInfinity(estimated_line_x_3)){
                estimated_line_x_3 = Center_x;
            }

            esti_befo_line_x_ave = estimated_line_x_ave;
            estimated_line_x_ave = (estimated_line_x_1 + estimated_line_x_2 + estimated_line_x_3) / 3.0f;

            
            

            
            // float tangent = ((float)(sum_x_point1/count_x_point1) - (float)(sum_x_point2/count_x_point2));
            float tangent = (-(float)((y1LeftX + y1RightX) / 2.0f) + (float)((y2LeftX + y2RightX) / 2.0f));
            
            if (err_flag > 0){
                err_y_buf = (float)estimated_line_x_ave - (float)Center_x;
                err_th_buf = Mathf.Atan2(tangent, 20.0f) * Mathf.Rad2Deg;

                errory_mat_before = errory_mat;
                errorag_mat_before = errorag_mat;
                
            }else{
                errory_mat_before = errory_mat;
                errorag_mat_before = errorag_mat;

                err_y_buf = (float)estimated_line_x_ave - (float)Center_x;
                err_th_buf = Mathf.Atan2(tangent, 20.0f) * Mathf.Rad2Deg;
                err_flag = 1;
            }
            float err_Y_diff = err_y_buf - errory_mat_before;
            float err_Angle_diff = err_th_buf - errorag_mat_before;

            if (Mathf.Abs(err_Y_diff) > 0.5f){
                errory_mat = errory_mat_before + 0.5f * Mathf.Sign(err_Y_diff);
            }else{
                errory_mat = err_y_buf;
            }

            if (Mathf.Abs(err_Angle_diff) > 1.0f){
                errorag_mat = errorag_mat_before + 1.0f * Mathf.Sign(err_Angle_diff);
            }else{
                errorag_mat = err_th_buf;
            }
            errory_mat = err_y_buf;
            errorag_mat = err_th_buf;


        }else{
            errory_mat = 0.0f;
            errorag_mat = 0.0f;
        }
        // Destroy(this.output_tex);
        
        // Imgproc.circle(image, new Point(estimated_line_x_1, Center_y), 3, new Scalar(0, 255, 255), -1);
        // Imgproc.circle(image, new Point(estimated_line_x_2, Center_y), 3, new Scalar(0, 255, 255), -1);
        
        // this.output_tex = new Texture2D(this.image.cols(), this.image.rows(), TextureFormat.RGBA32, false);
        // this.output_tex = new Texture2D(this.image.cols(), this.image.rows(), TextureFormat.RGBA32, false);

        // OpenCVForUnity.UnityUtils.Utils.matToTexture2D(this.image, this.output_tex, true);
        // GetComponent<RawImage>().texture = this.output_tex;
        

    }


    Texture2D CreateTexture2D(RenderTexture rt)
    {
        //Texture2Dを作成
        Texture2D texture2D = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false, false);

        //subCameraにRenderTextureを入れる
        // mainCamera.targetTexture = rt;

        //手動でカメラをレンダリングします
        // mainCamera.Render();


        RenderTexture.active = rt;
        texture2D.ReadPixels(new UnityEngine.Rect(0, 0, rt.width, rt.height), 0, 0);
        texture2D.Apply(false, true);


        //元に戻す別のカメラを用意してそれをRenderTexter用にすれば下のコードはいらないです。
        // mainCamera.targetTexture = null;
        RenderTexture.active = null;

        return texture2D;
    }


}