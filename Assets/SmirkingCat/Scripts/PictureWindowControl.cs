/*
 *
 *  PictureWindowControl.cs	(c) Ryan Sullivan 2017
 *  url: http://smirkingcat.software/picturewindow
 *
 *  Component used to control the window.
 *  
 */

using SmirkingCat.PictureWindow;
using UnityEngine;


[RequireComponent(typeof(SteamVR_TrackedController))]
public class PictureWindowControl : MonoBehaviour
{

    [Tooltip("The amount of time to hold GRIP in order to reset the window")]
    public float _resetTime = 2;


    // Timer for holding down the button to assign a controller or reset the window
    private float _gripStartTime;

    // Represents the tip of the controller
    private GameObject _tip;

    // This is the distance from the default origin of the Vive wand to it's frontmost tip
    private static Vector3 _tipOffset = new Vector3(0, -0.075f, 0.04f);

	private SteamVR_TrackedController _controller;

    public GameObject Model { get; set; }

    private PictureWindow PW = PictureWindow.Instance;


    private void Start()
    {

        _controller = GetComponent<SteamVR_TrackedController>();

        // Assign controller event listeners
        _controller.Gripped += Controller_Gripped;
        _controller.Ungripped += Controller_Ungripped;
        _controller.MenuButtonClicked += Controller_MenuClicked;
        _controller.PadClicked += Controller_PadClicked;
        _controller.TriggerClicked += Controller_TriggerClicked;

        SteamVR_RenderModel renderModel = GetComponentInChildren<SteamVR_RenderModel>();
        if (renderModel != null) Model = renderModel.gameObject;

        // Create a child object at the tip of the controller to take measurements from
        _tip = new GameObject("Tip");
        _tip.transform.SetParent(transform);
        _tip.transform.localPosition = _tipOffset;

    }


    private void Update()
    {

        // If this is the assigned controller and we're holding the grip
        // update UI or reset the window
        if (PW.WindowController == this)
        {

            if (_gripStartTime != 0)
            {

                // If it's been long enough, reset the window.
                if ((Time.time - _gripStartTime) > _resetTime)
                {

                    _gripStartTime = 0;

                    PW.WindowUI.ResetBar.Value = _gripStartTime;

                    PW.ResetWindow();

                }
                else
                {

                    // Update the UI w/ % of grip time
                    PW.WindowUI.ResetBar.Value = Mathf.Clamp01((Time.time - _gripStartTime) / _resetTime);

                }
            }
            else
                PW.WindowUI.ResetBar.Value = _gripStartTime;
            

        }
    }


    private void OnDestroy()
    {

        if (_controller != null)
        {
            _controller.Gripped -= Controller_Gripped;
            _controller.Ungripped -= Controller_Ungripped;
            _controller.MenuButtonClicked -= Controller_MenuClicked;
            _controller.PadClicked -= Controller_PadClicked;
            _controller.TriggerClicked -= Controller_TriggerClicked;
        }

        Destroy(_tip);

    }


    #region [CameraRig] listeners
    private void Controller_Gripped(object sender, ClickedEventArgs e)
    {

        // Set start time
        if (PW.IsConfigured) _gripStartTime = Time.time;

    }


    private void Controller_Ungripped(object sender, ClickedEventArgs e)
    {

        if (PW.IsConfigured) _gripStartTime = 0;

    }


    private void Controller_MenuClicked(object sender, ClickedEventArgs e)
    {

        if (PW.WindowController == this &&
            PW.IsConfigured)
        {

            // Toggle perspective
            PW.WindowCamera.UseEnhancedPerspective = !PW.WindowCamera.UseEnhancedPerspective;
            PW.WindowUI.ShowSubtitle((PW.WindowCamera.UseEnhancedPerspective ? "Enhanced" : "Standard" ) + " perspective");

        }

    }


    private void Controller_PadClicked(object sender, ClickedEventArgs e)
    {

        if (PW.WindowController == this &&
            PW.IsConfigured)
        {

            // Toggle debug box boolean
            PW.UseTargetBox = !PW.UseTargetBox;

            // If we toggled off, destroy the box
            if (!PW.UseTargetBox) PW.DestroyTargetBox();

        }

    }


    private void Controller_TriggerClicked(object sender, ClickedEventArgs e)
    {

        if (PW.WindowController == null)
            PW.WindowController = this;

        // If all the corners are not assigned yet
        if (!PW.IsConfigured)

            // ... and this is the window controller, then assign one of the corners
            if (PW.WindowController == this)
            {

                PW.SetCorner(_tip.transform.position);

            }

            // ... otherwise it was started with another controller
            // so make sure it's model is turned back on, then make
            // this the window controller and reset the window
            else
            {

                if (PW.WindowController.Model != null) PW.WindowController.Model.SetActive(true);
                PW.WindowController = this;
                PW.ResetWindow();

            }
        else if (PW.WindowController == this)
        {

            PW.WindowCamera.SmoothCam.Enabled = !PW.WindowCamera.SmoothCam.Enabled;
            PW.WindowUI.ShowSubtitle("Smooth movement is " + (PW.WindowCamera.SmoothCam.Enabled ? "enabled" : "disabled" ));

        }

    }
    #endregion


}