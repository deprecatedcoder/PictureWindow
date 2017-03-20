/*
 *
 *  PictureWindow.cs	(c) Ryan Sullivan 2017
 *  url: http://smirkingcat.software/picturewindow
 *
 *  Allows the user (outside of HMD) to view into the virtual world by turning the display into a window.
 *  
 *  First the user determines which controller to use, then defines the boundaries of the display, then the camera is 
 *  forced to display an off center projection from the perspective of their tracked object.
 *  
 */

using System.Collections.Generic;
using UnityEngine;

public class PictureWindow : MonoBehaviour
{


    // This
    private static PictureWindow _instance;
    public static PictureWindow Instance
    {
        get { return _instance; }
    }


    // The controller assigned to the window
    private PictureWindowControl _windowController;
    public PictureWindowControl WindowController
    {
        get { return _windowController; }
        set { _windowController = value; }
    }

    // The camera assigned to the window
    private GameObject _windowCamera;
    public GameObject WindowCamera
    {
        get { return _windowCamera; }
        set { _windowCamera = value; }
    }


    // Terrible setup to show off the instruction UI
    public static class INSTRUCTION
    {
        public const int None = -1;
        public const int AssignController = 0;
        public const int BottomLeft = 1;
        public const int BottomRight = 2;
        public const int TopLeft = 3;
    };
    private GameObject[] _instructions;
    private GameObject _background, _note;


    public enum Corner { BottomLeft, BottomRight, TopLeft, TopRight };

    private Dictionary <Corner, Vector3> _corners = new Dictionary<Corner, Vector3>();

    public Vector3 BottomLeft
    {
        get { return _corners[Corner.BottomLeft]; }
    }
    public Vector3 BottomRight
    {
        get { return _corners[Corner.BottomRight]; }
    }
    public Vector3 TopLeft
    {
        get { return _corners[Corner.TopLeft]; }
    }
    public Vector3 TopRight
    {
        get { return _corners[Corner.TopRight]; }
    }


    // The normal of the window
    private Vector3 _windowNormal;
    public Vector3 WindowNormal
    {
        get { return _windowNormal; }
        set { _windowNormal = value; }
    }
    

    // Johnny Lee style target filled box used for debugging
    [Tooltip("Whether to show a \"Johnny Lee style\" box filled with targets.\n\n"+
        "Toggled by clicking trackpad.")]
    public bool _useTargetBox;
    public GameObject _targetBoxPrefab;
    private GameObject _targetBox;


    private void Awake()
    {

        _instance = this;

        DontDestroyOnLoad(this.gameObject);

        // Assign all the UI slides
        _background = GameObject.Find("Background");
        _note = GameObject.Find("Note");
        
        _instructions = new GameObject[5];
        _instructions[0] = GameObject.Find("Instruction Assign Controller");
        _instructions[1] = GameObject.Find("Instruction Bottom Left");
        _instructions[2] = GameObject.Find("Instruction Bottom Right");
        _instructions[3] = GameObject.Find("Instruction Top Left");
        _instructions[4] = GameObject.Find("Instruction Cover");

        ShowInstruction(INSTRUCTION.AssignController);

    }


    private void Update()
    {

        // Setup camera if there isn't one and we've established the corners of the window
        if (WindowCamera == null && _corners.ContainsKey(Corner.TopRight))
        {

            _windowController.CreateCamera();

            ShowInstruction(INSTRUCTION.None);

        }

        // Create box filled with targets if necessary.
        if (_useTargetBox && _targetBox == null)
        {

            _targetBox = Instantiate(_targetBoxPrefab, 
                CenterOfVectors(new Vector3[]{ BottomLeft, BottomRight, TopLeft, TopRight }), 
                Quaternion.LookRotation(_windowNormal));
            _targetBox.transform.SetParent(transform);
            _targetBox.SetActive(false);

        }

        if (_targetBox) _targetBox.SetActive(_useTargetBox);

    }


    public Vector3 CenterOfVectors( Vector3[] vectors )
    {
        Vector3 sum = Vector3.zero;
        if( vectors == null || vectors.Length == 0 )
        {
            return sum;
        }
 
        foreach( Vector3 vec in vectors )
        {
            sum += vec;
        }
        return sum/vectors.Length;
    }


    public void ResetWindow()
    {

        _corners.Remove(Corner.BottomLeft);
        _corners.Remove(Corner.BottomRight);
        _corners.Remove(Corner.TopLeft);
        _corners.Remove(Corner.TopRight);

        _windowNormal = Vector3.zero;

        _windowController.DestroyCamera();

        ShowInstruction(INSTRUCTION.BottomLeft);

    }


    public void SetCorner( Vector3 position )
    {

        if (!_corners.ContainsKey(Corner.BottomLeft))
        {

            _corners.Add(Corner.BottomLeft, position);
            ShowInstruction(INSTRUCTION.BottomRight);

        }
        else if (!_corners.ContainsKey(Corner.BottomRight))
        {

            _corners.Add(Corner.BottomRight, position);
            ShowInstruction(INSTRUCTION.TopLeft);

        }
        else if (!_corners.ContainsKey(Corner.TopLeft))
        {

            _corners.Add(Corner.TopLeft, position);
            ShowInstruction(INSTRUCTION.None);

        }

        // Once we've set the necessary three, calculate the fourth
        if (_corners.ContainsKey(Corner.BottomLeft) && _corners.ContainsKey(Corner.BottomRight) && _corners.ContainsKey(Corner.TopLeft))
        {

            position = new Vector3(_corners[Corner.TopLeft].x + (_corners[Corner.BottomRight].x - _corners[Corner.BottomLeft].x),
                _corners[Corner.BottomRight].y + (_corners[Corner.TopLeft].y - _corners[Corner.BottomLeft].y),
                _corners[Corner.BottomRight].z + (_corners[Corner.TopLeft].z - _corners[Corner.BottomLeft].z));

            _corners.Add(Corner.TopRight, position);

        }

    }


    public void ShowInstruction( int instruction )
    {

        // First make sure they are all inactive
        foreach (GameObject inst in _instructions)
        {

            inst.SetActive(false);

        }

        // Also make sure the note is inactive
        _note.SetActive(false);


        // For any valid instruction, activate it
        if (instruction != INSTRUCTION.None)
        {

            _background.SetActive(true);
            _instructions[instruction].SetActive(true);

        }
        else
        {

            _background.SetActive(false);

        }

        // Activate the note for every instruction except picking controller
        if (instruction > INSTRUCTION.AssignController) _note.SetActive(true);

    }


}
