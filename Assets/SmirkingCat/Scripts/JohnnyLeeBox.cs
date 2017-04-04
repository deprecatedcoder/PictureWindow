/*
 *
 *  JohnnyLeeBox.cs	(c) Ryan Sullivan 2017
 *  url: http://smirkingcat.software/picturewindow
 *
 *  Creates a box in the style of the infamous "Head Tracking for Desktop VR Displays using the WiiRemote" video:
 *      https://www.youtube.com/watch?v=Jd3-eiid-Uw
 *  
 */

using System;
using System.Collections.Generic;
using UnityEngine;

public class JohnnyLeeBox : MonoBehaviour
{


    [Tooltip("The prefab to use for the walls of the box")]
    public GameObject _wallPrefab;

    [Tooltip("The depth of the box")]
    public float _depth = 2;

    [Tooltip("The thickness of the walls of the box")]
    public float _wallThickness = 0.1f;

    [Tooltip("The prefab to use for the targets")]
    public GameObject _targetPrefab;

    [Tooltip("The number of targets to generate")]
    public float _numberOfTargets = 10;


    public enum Wall { Top, Bottom, Left, Right, Back };

    private Dictionary <Wall, GameObject> _walls = new Dictionary<Wall, GameObject>();

    private float Height
    {
        get { return ((PictureWindow.Instance != null) ? PictureWindow.Instance.Height : 0 ); }
    }

    private float Width
    {
        get { return ((PictureWindow.Instance != null) ? PictureWindow.Instance.Width : 0 ); }
    }


    private void Update()
    {

        if ( _walls.Count == 0 && (Height != 0 && Width != 0) )
        {

            MakeBox();

            MakeTargets();

        }

    }


    private void MakeBox()
    {

        GameObject box = new GameObject("Box");
        box.transform.SetParent(transform);
        box.transform.localPosition = Vector3.zero;
        box.transform.localRotation = Quaternion.identity;

        foreach( Wall wall in Enum.GetValues(typeof(Wall)) )
        {

            _walls.Add( wall, Instantiate( _wallPrefab, box.transform ));
            _walls[wall].transform.localRotation = Quaternion.identity;

            switch (wall)
            {

                case Wall.Top:
                    _walls[wall].name = "Top";
                    _walls[wall].transform.localPosition = new Vector3( 0, ((Height + _wallThickness) / 2), 1 );
                    _walls[wall].transform.localScale = new Vector3( Width, _wallThickness, _depth );
                    break;
                case Wall.Bottom:
                    _walls[wall].name = "Bottom";
                    _walls[wall].transform.localPosition = new Vector3( 0, -((Height + _wallThickness) / 2), 1 );
                    _walls[wall].transform.localScale = new Vector3( Width, _wallThickness, _depth );
                    break;
                case Wall.Left:
                    _walls[wall].name = "Left";
                    _walls[wall].transform.localPosition = new Vector3( -((Width + _wallThickness) / 2), 0, 1 );
                    _walls[wall].transform.localScale = new Vector3( _wallThickness, Height, _depth );
                    break;
                case Wall.Right:
                    _walls[wall].name = "Right";
                    _walls[wall].transform.localPosition = new Vector3( ((Width + _wallThickness) / 2), 0, 1 );
                    _walls[wall].transform.localScale = new Vector3( _wallThickness, Height, _depth );
                    break;
                case Wall.Back:
                    _walls[wall].name = "Back";
                    _walls[wall].transform.localPosition = new Vector3( 0, 0, _depth + (_wallThickness / 2) );
                    _walls[wall].transform.localScale = new Vector3( Width, Height, _wallThickness );
                    break;

            }

        }

    }


    private void MakeTargets()
    {

        GameObject targets = new GameObject("Targets");
        targets.transform.SetParent(transform);
        targets.transform.localPosition = Vector3.zero;
        targets.transform.localRotation = Quaternion.identity;

        for (int i = 0; i < _numberOfTargets; i++)
        {

            // X and Y are within the bounds of the box such that they don't clip the walls
            float x = UnityEngine.Random.Range(-(Width/2-_targetPrefab.transform.lossyScale.x/2), (Width/2-_targetPrefab.transform.lossyScale.x/2));
            float y = UnityEngine.Random.Range(-(Height/2-_targetPrefab.transform.lossyScale.y/2), (Height/2-_targetPrefab.transform.lossyScale.y/2));
            // Z is most of the depth of the box and slightly sticking out forward of it
            float z = UnityEngine.Random.Range(-(_depth/5), (_depth/1.2f));

            GameObject target = Instantiate(_targetPrefab, targets.transform);
            target.transform.localPosition = new Vector3(x, y, z);
            target.transform.localRotation = Quaternion.identity;

        }

    }


    private void OnDestroy()
    {

        _walls.Clear();

    }


}