using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController2 : MonoBehaviour
{
    public ReduceMana mana1,mana2;

    public GameObject player1, player2;

    public static int currentPlayer;

    public void Start()
    {
        currentPlayer = 1;
    }

    public CinemachineVirtualCamera camera;
    public void setCameraBall(Transform transform)
    {
        camera.Follow = transform;
    }

    public void setPlayer1()
    {
        mana1.setCurrentMana(100);
        currentPlayer = 1;
        camera.Follow = player1.transform;
    }

    public void setPlayer2()
    {
        mana2.setCurrentMana(100);
        currentPlayer = 2;
        camera.Follow = player2.transform;
    }
}
