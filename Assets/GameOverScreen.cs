using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public Text playerWin;

    public void Setup(int num)
    {
        gameObject.SetActive(true);
        playerWin.text = "Player " + num.ToString() + " WIN";
    }

    public void restartButton()
    {
        SceneManager.LoadScene("Sample Scene");
    }
}
