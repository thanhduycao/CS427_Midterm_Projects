using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameOverScreen GameOverScreen;
    public GameObject player1, player2;

    public void Update()
    {
        if (player1.GetComponent<ReduceHealth>().currentHealth == 0) GameOverScreen.Setup(2);
        if (player2.GetComponent<ReduceHealth>().currentHealth == 0) GameOverScreen.Setup(1);
    }

}
