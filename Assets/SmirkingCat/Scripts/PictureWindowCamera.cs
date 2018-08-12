/*
 *
 *  PictureWindowCamera.cs	(c) Ryan Sullivan 2017
 *  url: http://smirkingcat.software/picturewindow
 *
 *  Corrects the camera perspective and rotation
 *  
 */

using SmirkingCat.PictureWindow;
using UnityEngine;

public class PictureWindowCamera : MonoBehaviour
{

    // Whether the camera should use a standard or enhanced perspective
    public bool UseEnhancedPerspective{ get; set; }

    // Whether the PictureWindow camera has already been configured or not
    public bool IsConfigured { get; set; }

    [SerializeField]
    private SmoothMovement _smoothCam;
    public SmoothMovement SmoothCam { get { return _smoothCam; } }

    private PictureWindow PW { get; set; }

    private Camera Cam { get; set; }


    private void Start()
    {

        PW = PictureWindow.Instance;

        Cam = SmoothCam.GetComponentInChildren<Camera>();

    }


    private void LateUpdate()
    {


        if (PW && PW.IsConfigured && PW.WindowController)
        {

            if (!IsConfigured) ConfigureCamera();

            Window window = PW.VirtualWindow;

            // Calculate the projection
            Matrix4x4 genProjection = GeneralizedPerspectiveProjection(window.BottomLeft, window.BottomRight, window.TopLeft,
                Cam.transform.position, Cam.nearClipPlane, Cam.farClipPlane);

            // Update the camera projection matrix
            Cam.projectionMatrix = genProjection;

            // Correct view for POV
            Quaternion correctPerspective = Quaternion.LookRotation(window.WindowNormal);

            // The correct view with additional rotation control.
            // Setting 0 for z here prevents the horizon from twisting, but makes it only work for vertical displays.
            Quaternion enhancedPerspective = Quaternion.Euler(SmoothCam.transform.rotation.eulerAngles.x, SmoothCam.transform.rotation.eulerAngles.y, 0);

            // If using the Vive wand use this to look through the donut
            if (PW.TrackedObject == null)
                enhancedPerspective = enhancedPerspective * Quaternion.Euler(-145, 0, 180);

            Cam.transform.rotation = (UseEnhancedPerspective ? enhancedPerspective : correctPerspective);

        }


    }
    
    
    public void ConfigureCamera()
    {

        // If there's an assigned TrackedObject use that and turn SmoothMovement on by default
        if (PW.TrackedObject)
        {

            SmoothCam.Target = PW.TrackedObject.transform;
            SmoothCam.Enabled = true;

        }
        // Otherwise use the Vive wand
        else
        {

            SmoothCam.Target = PW.WindowController.transform;

            // Use this to look through the donut of the Vive wand
            Cam.transform.localPosition = new Vector3(0, -0.042f, 0);

            // Disable the controller model (it just gets in the way)
            if (PW.WindowController.Model != null)
                PW.WindowController.Model.SetActive(false);

        }

        IsConfigured = true;

    }


    /// <summary>
    /// Calculates the generalized perspective projection 
    /// (i.e. a perspective other than directly in front of and perpendicular to the display)
    /// </summary>
    /// <param name="bottomLeft">Bottom left corner of the display</param>
    /// <param name="bottomRight">Bottom right corner of the display</param>
    /// <param name="topLeft">Top right corner of the display</param>
    /// <param name="camera">Camera position</param>
    /// <param name="near">Near clip plane</param>
    /// <param name="far">Far clip plane</param>
    /// <returns>The new camera matrix</returns>
    private static Matrix4x4 GeneralizedPerspectiveProjection(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 camera, float near, float far)
    {

        Vector3 va, vb, vc;
        Vector3 vr, vu, vn;
       
        // Calculate the orthonormal for the display (the display coordinate system)
        vr = bottomRight - bottomLeft;
        vr.Normalize();
        vu = topLeft - bottomLeft;
        vu.Normalize();
        vn = Vector3.Cross(vr, vu);
        vn.Normalize();

        // Calculate the vector from camera to display corners
        va = bottomLeft - camera;
        vb = bottomRight - camera;
        vc = topLeft - camera;

        // Get the distance; from the camera to the nearest point on the display plane
        float cameraDist = (Vector3.Dot(va, vn));
       
        // Get the variables for the off center projection
        float left = (Vector3.Dot(vr, va) * near) / cameraDist;
        float right  = (Vector3.Dot(vr, vb) * near) / cameraDist;
        float bottom  = (Vector3.Dot(vu, va) * near) / cameraDist;
        float top = (Vector3.Dot(vu, vc) * near) / cameraDist;

        // return
        return PerspectiveOffCenter(left, right, bottom, top, near, far);

    }

    /// <summary>
    /// Calculates the projection matrix to use for the off center perspective.
    /// 
    /// This is lifted directly fron the Unity docs: 
    ///     > https://docs.unity3d.com/ScriptReference/Camera-projectionMatrix.html
    /// </summary>
    /// <param name="left">Left offset for camera's near plane</param>
    /// <param name="right">Right offset for camera's near plane</param>
    /// <param name="bottom">Bottom offset for camera's near plane</param>
    /// <param name="top">Top offset for camera's near plane</param>
    /// <param name="near">Camera's near clipping plane</param>
    /// <param name="far">Camera's far clipping plane</param>
    /// <returns>A new camera matrix from the position of the tracked object</returns>
    private static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {

        float x = 2.0f * near / (right - left);
        float y = 2.0f * near / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -(2.0f * far * near) / (far - near);
        float e = -1.0f;

        Matrix4x4 m = new Matrix4x4();

        m[0, 0] = x;
        m[0, 1] = 0;
        m[0, 2] = a;
        m[0, 3] = 0;
        m[1, 0] = 0;
        m[1, 1] = y;
        m[1, 2] = b;
        m[1, 3] = 0;
        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = c;
        m[2, 3] = d;
        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = e;
        m[3, 3] = 0;

        return m;

    }


}
