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
        if (IsInternetAvailable()) GlobalVariable.Instance.GameMode = 1;
        else GlobalVariable.Instance.GameMode = 0;
    }

    void Update()
    {
        ShowDialogWarning(GlobalVariable.Instance.GameMode);
    }

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
        if (!IsInternetAvailable())
        {
            return;
        }
        GlobalVariable.Instance.GameMode = 1;
        _authenticationManager.LoginAnonymously();
    }

    public void OnClickPlayOffline()
    {
        GlobalVariable.Instance.GameMode = 0;
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(Constants.Rounds[0], LoadSceneMode.Single);
    }
}
