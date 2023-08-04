using Cinemachine;
using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    public static PlayerCameraFollow Instance { get; private set; }
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        }
    }
    
    public void FollowPlayer(Transform transform)
    {
        if (cinemachineVirtualCamera == null) return;
        cinemachineVirtualCamera.Follow = transform;
    }
}
