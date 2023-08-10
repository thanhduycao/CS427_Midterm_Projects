using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MovementController
{
    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }

    private void Start()
    {
        if (IsOwner)
            PlayerCameraFollow.Instance.FollowPlayer(m_PlayerTransform);
    }
}
