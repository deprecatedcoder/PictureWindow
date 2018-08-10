/*
 *
 *  JohnnyLeeBox.cs	(c) Ryan Sullivan 2017
 *  url: http://smirkingcat.software/picturewindow
 *
 *  Creates a box in the style of the infamous "Head Tracking for Desktop VR Displays using the WiiRemote" video:
 *      https://www.youtube.com/watch?v=Jd3-eiid-Uw
 *  
 */

using SmirkingCat.PictureWindow;
using UnityEngine;

public class JohnnyLeeBox : MonoBehaviour
{

    [Tooltip("The box to use as the volume")]
    public GameObject _box;

    [Tooltip("The prefab that will be spawned in the box volume")]
    public GameObject _targetPrefab;

    private float Height
    {
        get { return PictureWindow.Instance.VirtualWindow.Height; }
    }

    private float Width
    {
        get { return PictureWindow.Instance.VirtualWindow.Width; }
    }

    private float Depth
    {
        get { return (Height > Width ? Height : Width); }
    }

    private Vector3 Dimensions
    {
        get { return new Vector3(Width, Height, 4 * Depth); }
    }

    private GameObject _targets;


    private void Update()
    {

        if ( Dimensions != Vector3.zero && _targets == null)
        {

            _box.transform.localScale = Dimensions;

            MakeTargets();

        }

    }


    private void MakeTargets()
    {

        // Set to ~100 per m^2 of display size... I think?
        int numTargets = Mathf.RoundToInt(100 * Width * Height);

        _targets = new GameObject("Targets");
        _targets.transform.SetParent(transform);
        _targets.transform.localPosition = Vector3.zero;
        _targets.transform.localRotation = Quaternion.identity;

        for (int i = 0; i < numTargets; i++)
        {

            // X and Y are within the bounds of the box such that they don't clip the walls
            float x = Random.Range(-(Width/2-_targetPrefab.transform.lossyScale.x/2), (Width/2-_targetPrefab.transform.lossyScale.x/2));
            float y = Random.Range(-(Height/2-_targetPrefab.transform.lossyScale.y/2), (Height/2-_targetPrefab.transform.lossyScale.y/2));
            float z = Random.Range(-Depth, Depth);

            GameObject target = Instantiate(_targetPrefab, _targets.transform);
            target.transform.localPosition = new Vector3(x, y, z);
            target.transform.localRotation = Quaternion.identity;
            target.transform.localScale *= Random.Range(0.1f, 1f);

        }

    }


}