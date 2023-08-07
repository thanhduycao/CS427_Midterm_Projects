using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashManager : MonoBehaviour
{
    [Header("Authentication Manager")]
    [SerializeField] private AuthenticationManager _authManager;
    [SerializeField] private TMPro.TMP_Text _loadingText;

    private void Start()
    {
        // check internet connection
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            _loadingText.text = "No internet connection";
            return;
        }
        _authManager.LoginAnonymously();
    }
}
