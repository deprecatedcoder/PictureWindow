/*
 *
 *  SmoothMovement.cs	(c) Ryan Sullivan 2018
 *  url: http://smirkingcat.software/
 *
 *  Smoothly matches the position and rotation of this object to that of a target object
 * 
 */

using UnityEngine;


public class SmoothMovement : MonoBehaviour {


    public Transform Target { get; set; }

    public bool Enabled { get; set; }

    [Range(0.0f, 90.0f)]
    public float posLerpRate = 45.0f;

    [Range(1.0f, 90.0f)]
    public float rotLerpRate = 10.0f;


    private void FixedUpdate()
    {
        if (!Target) Target = transform;

        if (Enabled)
        {

            transform.localPosition = Vector3.Lerp(transform.localPosition, Target.localPosition, Mathf.Clamp01(posLerpRate * Time.fixedDeltaTime));
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Target.localRotation, Mathf.Clamp01(rotLerpRate * Time.fixedDeltaTime));

        }
        else
        {

            transform.localPosition = Target.localPosition;
            transform.localRotation = Target.localRotation;

        }

    }

}