using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    void Start()
    {
        SetDefaultScreenResolution();
    }

    // Extracted function to set the default screen resolution
    private void SetDefaultScreenResolution()
    {
        Screen.SetResolution(1080, 720, false);
    }
}
