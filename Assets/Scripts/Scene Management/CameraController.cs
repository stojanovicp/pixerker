using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class CameraController : Singleton<CameraController>
{
    private CinemachineCamera cinemachineCamera;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetPlayerCameraFollow();
    }

    public void SetPlayerCameraFollow() {
        cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();
        cinemachineCamera.Follow = PlayerController.Instance.transform;
    }
}

