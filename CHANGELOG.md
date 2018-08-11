# Changelog

# 1.0.0

> Initial release of PictureWindow

* **PictureWindow**
 * Allows the user (outside of HMD) to view into the virtual world by turning the display into a window.

   First the user determines which controller to use, then defines the boundaries of the display, then the camera is forced to display an off center projection from the perspective of their tracked object.
* **PictureWindowCamera**
 * Corrects the camera perspective and rotation
* **PictureWindowControl**
 * Component used to control the window.
* **SmirkingCat_TrackedObject**
 * Modified version of SteamVR_TrackedObject using the pose updating from Hand

# 1.1.0

> First significant update

- Updated Unity version used to 2018.1.1f1

- Moved the Unity sample VRSampleScenes out of the third party directory to allow easier ignoring of everything else in there

- Added material for debug box

- Added texture for window reset progress indicator

- Updated prefabs

- Created SmirkingCat.PictureWindow namespace to be able to use easier

* **PictureWindow.cs**:
 - Moved all the UI stuff into new PictureWindowUI
 - Cleaned up target box usage and management
 - Disabled cursor visibility
 - Added Initialization IEnumerator to wait for both controllers

* **PictureWindowUI.cs**:
 - Controls showing instructions for initial configuration, OSD for reseting the window and subtitles as applicable

* **PictureWindowCamera.cs**:
 - Updated matrix calculation to fix bugs
 - Added ConfigureCamera() to handle differences between using a Tracker and a Vive wand
 - Added optional enhancedPerspective that allows rotating the camera as well

* **SmoothMovement.cs**:
 - Added this utility component for smoothing out the motion of the camera

* **PictureWindowControl.cs**:
 - Simplified and added additional button handling

* **RadialProgressBar.cs**:
 - Added this utility component for displaying a radial progress indicator

* **Window.cs**:
 - Base class for a window, containing it's dimensions and other info
 - Contains utility methods for reseting the window and getting it's center

* **RealWindow.cs**:
 - Contains info specific to the real window

* **VirtualWindow.cs**:
 - Contains info specific to the virtual window
 - Updates the corner positions if moved

* **JohnnyLeeBox.cs**:
 - Basically entirely rewrote to use a prefab that has a semi-transparent grid from in or outside
 - Target count is now determined by display size

* **FlipNormals.cs**:
 - Utility script to flip the normals to cover the inside of the JohnnyLeeBox

* **SmirkingCat_TrackedObject.cs**:
 - Removed as was no longer necessary
