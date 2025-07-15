using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Moves a target object along a spline and logs its position to a CSV file.
/// Useful for generating reference trajectories.
/// </summary>
public class Spline_Follow : MonoBehaviour
{
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private Transform _followTarget;

    public string Participant_NAME;
    public Vector3 World_pos;

    private StreamWriter csv;
    [SerializeField, Range(0, 1)] private float _percentage;

    private void Start()
    {
        _percentage = 0.0f;

        // Create or overwrite CSV file using Shift_JIS encoding
        csv = new StreamWriter(@"SaveData.csv", false, Encoding.GetEncoding("Shift_JIS"));

        // Write CSV header
        string[] header = { "x", "y", "z" };
        string headerLine = string.Join(",", header);
        csv.WriteLine(headerLine);
    }

    private void Update()
    {
        // Abort if spline or target is not assigned
        if (_splineContainer == null || _followTarget == null)
            return;

        // Evaluate spline at current percentage
        _followTarget.position = _splineContainer.EvaluatePosition(_percentage);
        World_pos = _followTarget.position;

        // Write position to CSV
        string[] row = { World_pos.x.ToString(), World_pos.y.ToString(), World_pos.z.ToString() };
        string csvLine = string.Join(",", row);
        csv.WriteLine(csvLine);

        // End simulation if spline traversal is complete
        if (_percentage >= 1.0f)
        {
            csv.Close();
            EndGame();
        }

        _percentage += 0.0001f;
    }

    private void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}