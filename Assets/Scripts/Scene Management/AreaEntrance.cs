using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class AreaEntrance : MonoBehaviour
{
    [SerializeField] private string transitionName;

    private void Start()
    {
        if (SceneManagement.Instance == null)
            return;

        if (transitionName == SceneManagement.Instance.SceneTransitionName)
        {
            if (PlayerController.Instance != null)
                PlayerController.Instance.transform.position = transform.position;

            StartCoroutine(EnsureCameraFollow());
        }
        UIFade.Instance.FadeToClear();
    }

    private IEnumerator EnsureCameraFollow()
    {
        // Give one frame to let singletons/scene objects initialize
        yield return null;

        // Short timeout loop to wait for PlayerController/CameraController or a Cinemachine vcam to exist
        float timeout = 1.0f;
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            // If CameraController singleton exists, prefer it
            if (CameraController.Instance != null)
            {
                CameraController.Instance.SetPlayerCameraFollow();
                yield break;
            }

            // Try direct vcam fallback
            var vcam = FindFirstObjectByType<CinemachineCamera>();
            if (vcam != null && PlayerController.Instance != null)
            {
                vcam.Follow = PlayerController.Instance.transform;
                vcam.Target.TrackingTarget = PlayerController.Instance.transform;
                vcam.Target.LookAtTarget = PlayerController.Instance.transform;
                vcam.LookAt = PlayerController.Instance.transform;
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.LogWarning($"CameraController not found and no Cinemachine vcam found to set follow for entrance '{name}'.");
    }
}
