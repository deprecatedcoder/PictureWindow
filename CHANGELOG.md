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
