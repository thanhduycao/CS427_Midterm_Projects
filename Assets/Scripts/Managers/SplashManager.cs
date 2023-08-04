using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashManager : MonoBehaviour
{
    [Header("Authentication Manager")]
    [SerializeField] private AuthenticationManager _authManager;

    private void Start()
    {
        _authManager.LoginAnonymously();
    }
}
