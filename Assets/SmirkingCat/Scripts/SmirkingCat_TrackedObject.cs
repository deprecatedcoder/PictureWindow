// Modified version of SteamVR_TrackedObject using the pose updating from Hand

//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: For controlling in-game objects with tracked devices.
//
//=============================================================================

using UnityEngine;
using Valve.VR;

public class SmirkingCat_TrackedObject : MonoBehaviour
{
	public enum EIndex
	{
		None = -1,
		Hmd = (int)OpenVR.k_unTrackedDeviceIndex_Hmd,
		Device1,
		Device2,
		Device3,
		Device4,
		Device5,
		Device6,
		Device7,
		Device8,
		Device9,
		Device10,
		Device11,
		Device12,
		Device13,
		Device14,
		Device15
	}

	public EIndex index;
	public Transform origin; // if not set, relative to parent
    public bool isValid = false;

    void FixedUpdate()
    {
        UpdateHandPoses();
    }

    void UpdateHandPoses()
    {
        SteamVR vr = SteamVR.instance;
        if ( vr != null )
        {
            var pose = new Valve.VR.TrackedDevicePose_t();
            var gamePose = new Valve.VR.TrackedDevicePose_t();
            var err = vr.compositor.GetLastPoseForTrackedDeviceIndex( (uint)index, ref pose, ref gamePose );
            if ( err == Valve.VR.EVRCompositorError.None )
            {
                isValid = gamePose.bPoseIsValid;

                var t = new SteamVR_Utils.RigidTransform( gamePose.mDeviceToAbsoluteTracking );
                transform.localPosition = t.pos;
                transform.localRotation = t.rot;
            }
        }
    }

	public void SetDeviceIndex(int index)
	{
		if (System.Enum.IsDefined(typeof(EIndex), index))
			this.index = (EIndex)index;
	}
}

