using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity;
using OpenCVForUnity.UnityUtils;

/// <summary>
/// Processes camera RenderTexture with OpenCV to compute lateral/angle errors and a visual confidence score.
/// Outputs:
/// - errory_mat: lateral error from estimated path
/// - errorag_mat: angular deviation (in degrees)
/// - confidence: visual tracking confidence
/// </summary>
public class CreateTexture : MonoBehaviour
{
    // Input render texture from Main Camera
    public RenderTexture renderTexture;

    Texture2D tex2D;

    public float pre_conf_det;
    public float now_conf_det;
    public float aft_conf_det;

    float pre_conf;
    float now_conf;
    float aft_conf;

    Mat image, gray, binary, binary2, convert;
    Mat Mask, Mask2;
    List<Point> points;

    int Center_x, Center_y;

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

    // Prediction line heights (fixed horizontal lines)
    int predict_y1, predict_y2, predict_y3, predict_y4, predict_y5;
    int predict_y6, predict_y7, predict_y8, predict_y9, predict_y10;
    int predict_y11, predict_y12, predict_y13, predict_y14, predict_y15;

    float[] pre_conf_buf;
    float[] now_conf_buf;
    float[] aft_conf_buf;

    int done_y1 = 0;
    int done_y2 = 0;

    public float err_y_buf;
    public float err_th_buf;

    int err_flag = 0;

    int boole = 100;

    // Buffers for previous edge detection (unused)
    int y1_beforex1, y1_beforex2;
    int y2_beforex1, y2_beforex2;
    int y3_beforex1, y3_beforex2;

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

    void Start()
    {
        tex2D = CreateTexture2D(renderTexture);

        // Allocate OpenCV Mats
        image = new Mat(tex2D.height, tex2D.width, CvType.CV_8UC4);
        gray = new Mat(tex2D.height, tex2D.width, CvType.CV_8UC4);
        binary = new Mat(tex2D.height, tex2D.width, CvType.CV_8UC4);
        binary2 = new Mat(tex2D.height, tex2D.width, CvType.CV_8UC4);
        convert = new Mat(tex2D.height, tex2D.width, CvType.CV_8UC4);

        // Create mask to limit ROI
        Mask = new Mat(tex2D.height, tex2D.width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
        Imgproc.circle(Mask, new Point(image.width() / 2 + 20, image.height() / 2), mask_radius, new Scalar(255, 255, 255, 255), -1);

        Center_x = image.width() / 2;
        Center_y = image.height() / 2;

        points = new List<Point>();

        confidence = 0.0f;
        errory_mat = 0.0f;
        errorag_mat = 0.0f;
        errory_mat_before = 0.0f;
        errorag_mat_before = 0.0f;

        pre_conf_buf = new float[boole];
        now_conf_buf = new float[boole];
        aft_conf_buf = new float[boole];

        for (int i = 0; i < boole; i++)
        {
            pre_conf_buf[i] = 0.0f;
            now_conf_buf[i] = 0.0f;
            aft_conf_buf[i] = 0.0f;
        }

        // Initialize edge positions to center
        y1LeftX = y1RightX = Center_x;
        y2LeftX = y2RightX = Center_x;
        y3LeftX = y3RightX = Center_x;

        bf_y1LeftX = bf_y1RightX = Center_x;
        bf_y2LeftX = bf_y2RightX = Center_x;
        bf_y3LeftX = bf_y3RightX = Center_x;

        tmp_y1LeftX = tmp_y1RightX = Center_x;
        tmp_y2LeftX = tmp_y2RightX = Center_x;
        tmp_y3LeftX = tmp_y3RightX = Center_x;
    }

    /// <summary>
    /// Creates a Texture2D from a RenderTexture
    /// </summary>
    Texture2D CreateTexture2D(RenderTexture rt)
    {
        Texture2D texture2D = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false, false);
        RenderTexture.active = rt;
        texture2D.ReadPixels(new UnityEngine.Rect(0, 0, rt.width, rt.height), 0, 0);
        texture2D.Apply(false, true);
        RenderTexture.active = null;
        return texture2D;
    }

    void Update()
    {
        // Capture and convert RenderTexture to OpenCV Mat
        tex2D = CreateTexture2D(renderTexture);
        Utils.texture2DToMat(tex2D, image, true);
        Destroy(tex2D);  // Free memory

        estimated_line_x_ave = 0.0f;
        confidence = 0.0f;

        // Define region of interest (ROI) to crop out border artifacts
        OpenCVForUnity.CoreModule.Rect roi = new OpenCVForUnity.CoreModule.Rect(30, 30, this.image.cols()-60, this.image.rows()-60);
        Mat reMat = new Mat(this.image, roi);

        // Grayscale conversion
        Imgproc.cvtColor(reMat, gray, Imgproc.COLOR_RGBA2GRAY);

        // Binary thresholding
        Imgproc.threshold(gray, binary, 10, 255, Imgproc.THRESH_BINARY);

        // Find filled contours from binary image
        List<MatOfPoint> contours = new List<MatOfPoint>();
        List<MatOfPoint> contours2 = new List<MatOfPoint>();
        Imgproc.findContours(binary, contours, new Mat(), Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);
        Imgproc.drawContours(binary, contours, -1, new Scalar(255, 255, 255, 255), -1);
        Core.bitwise_not(binary, binary);  // Invert binary image

        // Find new contours (edges of objects)
        Imgproc.findContours(binary, contours2, new Mat(), Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_NONE);
        gaus = contours2.Count;

        // Parameters for tracking and prediction
        float sum_x_point1 = 0.0f, sum_x_point2 = 0.0f, sum_x_point3 = 0.0f;
        int count_x_point1 = 0, count_x_point2 = 0, count_x_point3 = 0;

        int cef = 2;  // vertical sampling spacing
        int start_n = 30;

        // Precompute prediction lines (fixed Y values)
        predict_y1 = Center_y - cef * (start_n + 4);
        predict_y2 = Center_y - cef * (start_n + 4);
        predict_y3 = Center_y - cef * (start_n + 4);
        predict_y4 = Center_y - cef * (start_n + 4);
        predict_y5 = Center_y - cef * (start_n + 4);
        predict_y6 = Center_y - cef * (start_n + 3);
        predict_y7 = Center_y - cef * (start_n + 3);
        predict_y8 = Center_y - cef * (start_n + 3);
        predict_y9 = Center_y - cef * (start_n + 2);
        predict_y10 = Center_y - cef * (start_n + 2);
        predict_y11 = Center_y - cef * (start_n + 2);
        predict_y12 = Center_y - cef * (start_n + 1);
        predict_y13 = Center_y - cef * (start_n + 1);
        predict_y14 = Center_y - cef * (start_n + 0);
        predict_y15 = Center_y - cef * (start_n + 0);

        int y_point1 = Center_y + 10;
        int y_point2 = Center_y - 10;
        int y_point3 = Center_y;

        done_y1 = Center_y + 60;
        done_y2 = Center_y + 80;

        float Cutoff1 = 1.0f;  // smoothing factor

        // --- Contour analysis ---
        if (gaus > 0)
        {
            for (int i = 0; i < gaus; i++)
            {
                float area = (float)Imgproc.contourArea(contours2[i]);
                if (area > 1000.0f && area < 8500.0f)
                {
                    points = contours2[i].toList();
                    List<int> Y1points_list = new List<int>();
                    List<int> Y2points_list = new List<int>();
                    List<int> Y3points_list = new List<int>();

                    for (int j = 0; j < points.Count; j++)
                    {
                        Point pt = points[j];

                        // Track left/right X for line y1
                        if (pt.y == y_point1 && pt.x > cutting && pt.x < image.cols() - cutting)
                            Y1points_list.Add(30 + (int)pt.x);

                        // Track left/right X for line y2
                        else if (pt.y == y_point2 && pt.x > cutting && pt.x < image.cols() - cutting)
                            Y2points_list.Add(30 + (int)pt.x);

                        // Track left/right X for line y3
                        else if (pt.y == y_point3 && pt.x > cutting && pt.x < image.cols() - cutting)
                            Y3points_list.Add(30 + (int)pt.x);

                        // Prediction bands 1–8
                        else if (pt.y == predict_y1 && pt.x > cutting && pt.x < image.cols() - cutting) count_x_point1++;
                        else if (pt.y == predict_y2 && pt.x > cutting && pt.x < image.cols() - cutting) count_x_point2++;
                        else if (pt.y == predict_y3 && pt.x > cutting && pt.x < image.cols() - cutting) count_x_point3++;
                    }

                    // Backup when no valid left/right pair is detected
                    tmp_y1LeftX = (Y1points_list.Count == 2) ? Y1points_list[0] : bf_y1LeftX;
                    tmp_y1RightX = (Y1points_list.Count == 2) ? Y1points_list[1] : bf_y1RightX;

                    tmp_y2LeftX = (Y2points_list.Count == 2) ? Y2points_list[0] : bf_y2LeftX;
                    tmp_y2RightX = (Y2points_list.Count == 2) ? Y2points_list[1] : bf_y2RightX;

                    tmp_y3LeftX = (Y3points_list.Count == 2) ? Y3points_list[0] : bf_y3LeftX;
                    tmp_y3RightX = (Y3points_list.Count == 2) ? Y3points_list[1] : bf_y3RightX;
                }
            }

            // Estimate confidence based on number of detected prediction points
            pre_conf = (count_x_point1 + count_x_point2 + count_x_point3) * 3.0f;
            pre_conf /= 2.0f;

            // Exponential smoothing of left/right points
            y1LeftX = bf_y1LeftX + (tmp_y1LeftX - bf_y1LeftX) * Time.deltaTime * 2.0f * Mathf.PI * Cutoff1;
            y1RightX = bf_y1RightX + (tmp_y1RightX - bf_y1RightX) * Time.deltaTime * 2.0f * Mathf.PI * Cutoff1;
            y2LeftX = bf_y2LeftX + (tmp_y2LeftX - bf_y2LeftX) * Time.deltaTime * 2.0f * Mathf.PI * Cutoff1;
            y2RightX = bf_y2RightX + (tmp_y2RightX - bf_y2RightX) * Time.deltaTime * 2.0f * Mathf.PI * Cutoff1;
            y3LeftX = bf_y3LeftX + (tmp_y3LeftX - bf_y3LeftX) * Time.deltaTime * 2.0f * Mathf.PI * Cutoff1;
            y3RightX = bf_y3RightX + (tmp_y3RightX - bf_y3RightX) * Time.deltaTime * 2.0f * Mathf.PI * Cutoff1;

            // Reduce confidence if left/right are too close (detection noise)
            if (Mathf.Abs(y1LeftX - y1RightX) < 8.0f) pre_conf -= 3.0f;
            if (Mathf.Abs(y2LeftX - y2RightX) < 8.0f) pre_conf -= 3.0f;
            if (Mathf.Abs(y3LeftX - y3RightX) < 8.0f) pre_conf -= 2.0f;

            // Save previous frame values
            bf_y1LeftX = y1LeftX; bf_y1RightX = y1RightX;
            bf_y2LeftX = y2LeftX; bf_y2RightX = y2RightX;
            bf_y3LeftX = y3LeftX; bf_y3RightX = y3RightX;

            // Clamp confidence if in unreliable range
            if (pre_conf > 30.0f) pre_conf = 0.0f;
            else if (pre_conf > 15.0f && pre_conf <= 30.0f)
            {
                pre_conf = 30.0f - 1.1f * pre_conf;
                if (pre_conf < 0.0f) pre_conf = 0.0f;
            }

            // Confidence smoothing (buffered over time)
            for (int k = 0; k < boole - 1; k++)
                pre_conf_buf[k] = pre_conf_buf[k + 1];
            pre_conf_buf[boole - 1] = pre_conf;

            float sum_pre_buf = 0.0f;
            for (int k = 0; k < boole; k++)
                sum_pre_buf += pre_conf_buf[k];

            pre_conf_det = sum_pre_buf / boole;
            confidence = pre_conf_det;

            // Estimated path centerlines from detected left/right
            if (first_timing > 0)
            {
                esti_befo_line_x_1 = estimated_line_x_1;
                esti_befo_line_x_2 = estimated_line_x_2;
                esti_befo_line_x_3 = estimated_line_x_3;
            }

            estimated_line_x_1 = ((y1LeftX + y1RightX) / 2.0f + (y2LeftX + y2RightX) / 2.0f) / 2.0f;
            estimated_line_x_2 = ((y1LeftX + y1RightX) / 2.0f + (y3LeftX + y3RightX) / 2.0f) / 2.0f;
            estimated_line_x_3 = ((y2LeftX + y2RightX) / 2.0f + (y3LeftX + y3RightX) / 2.0f) / 2.0f;

            if (first_timing == 0)
            {
                esti_befo_line_x_1 = estimated_line_x_1;
                esti_befo_line_x_2 = estimated_line_x_2;
                esti_befo_line_x_3 = estimated_line_x_3;
                first_timing = 1;
            }

            // Limit sudden jumps
            if (Mathf.Abs(estimated_line_x_1 - esti_befo_line_x_1) > diff)
                estimated_line_x_1 = esti_befo_line_x_1 + Mathf.Sign(estimated_line_x_1 - esti_befo_line_x_1) * diff;

            if (Mathf.Abs(estimated_line_x_2 - esti_befo_line_x_2) > diff)
                estimated_line_x_2 = esti_befo_line_x_2 + Mathf.Sign(estimated_line_x_2 - esti_befo_line_x_2) * diff;

            if (Mathf.Abs(estimated_line_x_3 - esti_befo_line_x_3) > diff)
                estimated_line_x_3 = esti_befo_line_x_3 + Mathf.Sign(estimated_line_x_3 - esti_befo_line_x_3) * diff;

            // Fallback to center if invalid
            if (float.IsInfinity(estimated_line_x_1)) estimated_line_x_1 = Center_x;
            if (float.IsInfinity(estimated_line_x_2)) estimated_line_x_2 = Center_x;
            if (float.IsInfinity(estimated_line_x_3)) estimated_line_x_3 = Center_x;

            // Final lateral error = avg estimated line - center
            esti_befo_line_x_ave = estimated_line_x_ave;
            estimated_line_x_ave = (estimated_line_x_1 + estimated_line_x_2 + estimated_line_x_3) / 3.0f;

            float tangent = -(y1LeftX + y1RightX) / 2.0f + (y2LeftX + y2RightX) / 2.0f;

            err_y_buf = estimated_line_x_ave - Center_x;
            err_th_buf = Mathf.Atan2(tangent, 20.0f) * Mathf.Rad2Deg;

            errory_mat = err_y_buf;
            errorag_mat = err_th_buf;
        }
        else
        {
            // No valid contours found: reset values
            errory_mat = 0.0f;
            errorag_mat = 0.0f;
        }
    }
}
