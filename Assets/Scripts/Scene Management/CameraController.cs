using UnityEngine;
using Unity.Cinemachine;

public class CameraController : Singleton<CameraController>
{
    private CinemachineCamera cinemachineCamera;
    public void SetPlayerCameraFollow()
    {
        cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
        if (cinemachineCamera != null)
        {
            cinemachineCamera.Target.TrackingTarget = PlayerController.Instance.transform;
        }
        else
        {
            Debug.LogWarning("Unable to find Cinemachine Camera");
        }
    }
}

