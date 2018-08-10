/*
 *
 *  VirtualWindow.cs	(c) Ryan Sullivan 2018
 *  url: http://smirkingcat.software/picturewindow
 *
 *  
 */

using UnityEngine;

namespace SmirkingCat.PictureWindow
{

    public class VirtualWindow : Window
    {

        public override void Update()
        {

            base.Update();

            if (Display.transform.position != Vector3.zero)
            {

                MeshFilter rend = Display.GetComponent<MeshFilter>();

                BottomLeft = Display.transform.TransformPoint(rend.mesh.vertices[0]);
                TopRight = Display.transform.TransformPoint(rend.mesh.vertices[1]);
                BottomRight = Display.transform.TransformPoint(rend.mesh.vertices[2]);
                TopLeft = Display.transform.TransformPoint(rend.mesh.vertices[3]);

            }

        }

    }

}