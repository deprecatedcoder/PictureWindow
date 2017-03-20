/*
 *
 *  PictureWindowCamera.cs	(c) Ryan Sullivan 2017
 *  url: http://smirkingcat.software/picturewindow
 *
 *  Corrects the camera perspective and rotation
 *  
 */

using UnityEngine;

public class PictureWindowCamera : MonoBehaviour
{

    private Camera _cam;


    private void Awake()
    {

        _cam = GetComponent<Camera>();

    }


    private void LateUpdate ()
    {

        if (PictureWindow.Instance.TopRight != null)
        {

            // Calculate the projection
            Matrix4x4 genProjection = GeneralizedPerspectiveProjection(PictureWindow.Instance.BottomLeft, PictureWindow.Instance.BottomRight, PictureWindow.Instance.TopLeft,
                transform.position, _cam.nearClipPlane, _cam.farClipPlane);

            // Update the camera projection matrix
            _cam.projectionMatrix = genProjection;

            // Rotate the camera toward the target
            _cam.transform.rotation = Quaternion.LookRotation(PictureWindow.Instance.WindowNormal);

        }


    }
    
    
    public static Matrix4x4 GeneralizedPerspectiveProjection(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 eye, float near, float far)
    {

        float left, right, bottom, top, eyedistance;
       
        Matrix4x4 projectionM;

        Vector3 va, vb, vc;
        Vector3 vr, vu, vn;
       
        // Calculate the orthonormal for the display (the display coordinate system)
        vr = bottomRight - bottomLeft;
        vr.Normalize();
        vu = topLeft - bottomLeft;
        vu.Normalize();
        vn = Vector3.Cross(vr, vu);
        vn.Normalize();

        // Save this to the PictureWindow
        PictureWindow.Instance.WindowNormal = vn;
        
        // Calculate the vector from eye to display corners
        va = bottomLeft - eye;
        vb = bottomRight - eye;
        vc = topLeft - eye;

        // Get the distance; from the eye to the nearest point on the display plane
        eyedistance = (Vector3.Dot(va, vn));
       
        // Get the variables for the off center projection
        left = (Vector3.Dot(vr, va) * near) / eyedistance;
        right  = (Vector3.Dot(vr, vb) * near) / eyedistance;
        bottom  = (Vector3.Dot(vu, va) * near) / eyedistance;
        top = (Vector3.Dot(vu, vc) * near) / eyedistance;

        // Get the projection matrix
        projectionM = PerspectiveOffCenter(left, right, bottom, top, near, far);
       
        // return
        return Matrix4x4.identity * projectionM;

    }


    private static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {

        float x = 2.0F * near / (right - left);
        float y = 2.0F * near / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = (far + near) / (near - far);
        float d = (2.0F * far * near) / (near - far);
        float e = -1.0F;

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
