using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _textWarning;
    [SerializeField] private AuthenticationManager _authenticationManager;

    void Start()
    {
        _textWarning.text = "";
        //if (IsInternetAvailable()) GlobalVariable.Instance.gameMode = 1;
        //else GlobalVariable.Instance.gameMode = 0;
    }

    //void Update()
    //{
    //    ShowDialogWarning(GlobalVariable.Instance.gameMode);
    //}

    private void ShowDialogWarning(int gameMode)
    {
        _textWarning.text = gameMode != 0 ? "" : "No internet connection available. You can play offline.";
    }

    private bool IsInternetAvailable()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }
        return true;
    }

    public void OnClickPlayMutliplayer()
    {
        _authenticationManager.LoginAnonymously();
    }

    public void OnClickPlayOffline()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(Constants.Rounds[0], LoadSceneMode.Single);
    }
}