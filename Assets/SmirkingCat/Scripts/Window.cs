/*
 *
 *  Window.cs	(c) Ryan Sullivan 2018
 *  url: http://smirkingcat.software/picturewindow
 *
 *  
 */

using UnityEngine;

namespace SmirkingCat.PictureWindow
{

    public class Window : MonoBehaviour
    {

        [SerializeField]
        private GameObject _display;
        public GameObject Display { get { return _display; } set { _display = value; } }

        public Vector3 TopLeft { get; set; }
        public Vector3 BottomLeft { get; set; }
        public Vector3 BottomRight { get; set; }
        public Vector3 TopRight { get; set; }

        public float Width
        {
            get { return (BottomRight - BottomLeft).magnitude; }
        }

        public float Height
        {
            get { return (TopLeft - BottomLeft).magnitude; }
        }

        public Vector3 Center
        {
            get { return CenterOfVectors(new Vector3[] { TopLeft, BottomLeft, BottomRight, TopRight }); }
        }

        public Vector3 WindowNormal
        {
            get { return Vector3.Cross((BottomRight - BottomLeft).normalized, (TopLeft - BottomLeft).normalized).normalized; }
        }


        // Update is called once per frame
        public virtual void Update()
        {

            if (PictureWindow.Instance.IsConfigured && _display.transform.position == Vector3.zero)
            {

                _display.transform.position = Center;
                _display.transform.localScale = new Vector3(Width, Height, 0);
                _display.transform.rotation = Quaternion.LookRotation(WindowNormal);

            }


        }


        /// <summary>
        /// Utility method to get the center of any number of positions
        /// </summary>
        public static Vector3 CenterOfVectors(Vector3[] vectors)
        {

            Vector3 sum = Vector3.zero;

            if (vectors == null || vectors.Length == 0)
            {
                return sum;
            }

            foreach (Vector3 vec in vectors)
            {
                sum += vec;
            }

            return sum / vectors.Length;

        }


        /// <summary>
        /// Resets the window corners
        /// </summary>
        public void ResetWindow()
        {

            TopLeft = Vector3.zero;
            BottomLeft = Vector3.zero;
            BottomRight = Vector3.zero;
            TopRight = Vector3.zero;

            _display.transform.position = Vector3.zero;
            _display.transform.localScale = Vector3.zero;

        }


    }


}