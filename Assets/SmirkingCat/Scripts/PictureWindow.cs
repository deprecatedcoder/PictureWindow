/*
 *
 *  PictureWindow.cs	(c) Ryan Sullivan 2017
 *  url: http://smirkingcat.software/picturewindow
 *
 *  Allows the user (outside of HMD) to view into the virtual world by turning the display into a window.
 *  
 *  First the user defines the boundaries of the display, then the camera is forced to 
 *  display an off center projection from the perspective of their tracked object.
 *  
 */

using System.Collections;
using UnityEngine;


namespace SmirkingCat.PictureWindow
{

    public class PictureWindow : MonoBehaviour {

        public static PictureWindow Instance { get; private set; }

        [SerializeField]
        private PictureWindowUI _windowUI;
        public PictureWindowUI WindowUI { get { return _windowUI; } }

        [SerializeField]
        private RealWindow _realWindow;
        public RealWindow RealWindow { get { return _realWindow; } }

        [SerializeField]
        private VirtualWindow _virtualWindow;
        public VirtualWindow VirtualWindow { get { return _virtualWindow; } }

        [SerializeField]
        private PictureWindowCamera _windowCamera;
        public PictureWindowCamera WindowCamera { get { return _windowCamera; } }

        [SerializeField]
        [Tooltip("The tracked object to use." + "\n" +
            "Leave empty to use one of the controllers.")]
        private Transform _trackedObject;
        public Transform TrackedObject { get { return _trackedObject; } }

        public PictureWindowControl Left { get; set; }
        public PictureWindowControl Right { get; set; }
        public PictureWindowControl WindowController { get; set; }

        // Johnny Lee style target filled box used for debugging
        public GameObject _targetBoxPrefab;
        private GameObject _targetBox;
        public bool UseTargetBox { get; set; }

        // Whether the window has already been setup or not
        public bool IsConfigured { get; set; }


        private void Awake()
        {

            // There can only be one PictureWindow
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);

        }


        private IEnumerator Start()
        {

            // Set Cursor to not be visible
            Cursor.visible = false;

            yield return StartCoroutine(Initialize());

        }


        /// <summary>
        /// Initializes controllers
        /// </summary>
        private IEnumerator Initialize()
        {

            // Grab the SteamVR_companionAppManager so we can identify our devices
            SteamVR_ControllerManager controllerManager = FindObjectOfType<SteamVR_ControllerManager>();

            // We don't want to do anything with the hands until we have both controllers
            while (controllerManager.left.GetComponent<SteamVR_TrackedObject>().index == SteamVR_TrackedObject.EIndex.None ||
                controllerManager.right.GetComponent<SteamVR_TrackedObject>().index == SteamVR_TrackedObject.EIndex.None)
                yield return null;

            // Add the PictureWindowControl components to the controllers
            Left = controllerManager.left.AddComponent<PictureWindowControl>();
            Right = controllerManager.right.AddComponent<PictureWindowControl>();

        }


        private void Update()
        {

            // Create box filled with targets if necessary
            if (UseTargetBox && _targetBox == null)
            {

                _targetBox = Instantiate(_targetBoxPrefab, VirtualWindow.Center, Quaternion.LookRotation(VirtualWindow.WindowNormal), VirtualWindow.transform);
                _targetBox.SetActive(false);

            }

            if (_targetBox) _targetBox.SetActive(UseTargetBox);

        }


        /// <summary>
        /// Destroys the target box and resets the use flag
        /// </summary>
        public void DestroyTargetBox()
        {

            UseTargetBox = false;
            Destroy(_targetBox);

        }


        /// <summary>
        /// Resets the PictureWindow
        /// </summary>
        public void ResetWindow()
        {

            WindowController = null;

            DestroyTargetBox();

            RealWindow.ResetWindow();
            VirtualWindow.ResetWindow();

            WindowCamera.SmoothCam.Target = null;
            WindowCamera.IsConfigured = false;

            WindowUI.ShowInstruction(PictureWindowUI.INSTRUCTION.TopLeft);

            IsConfigured = false;

        }


        /// <summary>
        /// Steps through setting the necessary positions to determine the location of the display.
        /// 
        /// This makes the assumption none of the corners will ever be exactly Vector3.zero.
        /// </summary>
        /// <param name="position"></param>
        public void SetCorner( Vector3 position )
        {

            bool calculateLast = false;

            if (RealWindow.TopLeft == Vector3.zero)
            {

                RealWindow.TopLeft = position;
                WindowUI.ShowInstruction(PictureWindowUI.INSTRUCTION.BottomLeft);

            }
            else if (RealWindow.BottomLeft == Vector3.zero)
            {

                RealWindow.BottomLeft = position;
                WindowUI.ShowInstruction(PictureWindowUI.INSTRUCTION.BottomRight);

            }
            else if (RealWindow.BottomRight == Vector3.zero)
            {

                RealWindow.BottomRight = position;
                WindowUI.ShowInstruction(PictureWindowUI.INSTRUCTION.None);

                // Set the flag that the required corners are set
                calculateLast = true;
            }

            // Once we've set the necessary three, calculate the fourth
            if (calculateLast)
            {

                RealWindow.TopRight = new Vector3(RealWindow.TopLeft.x + (RealWindow.BottomRight.x - RealWindow.BottomLeft.x),
                    RealWindow.BottomRight.y + (RealWindow.TopLeft.y - RealWindow.BottomLeft.y),
                    RealWindow.BottomRight.z + (RealWindow.TopLeft.z - RealWindow.BottomLeft.z));

                // Duplicate everything over to the VirtualWindow
                VirtualWindow.TopLeft = RealWindow.TopLeft;
                VirtualWindow.BottomLeft = RealWindow.BottomLeft;
                VirtualWindow.BottomRight = RealWindow.BottomRight;
                VirtualWindow.TopRight = RealWindow.TopRight;

                // Set the config flag
                IsConfigured = true;

                // Turn the instructions off
                WindowUI.ShowInstruction(PictureWindowUI.INSTRUCTION.None);

            }

        }

    }


}