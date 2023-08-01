using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deadball : MonoBehaviour
{
    public string Sname;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player1")
        {
            if (CameraController2.currentPlayer == 2)
            {
                Destroy(gameObject);
                FindObjectOfType<CameraController2>().setPlayer1();
                //gameObject.GetComponent<ReduceMana>().setCurrentMana(100);
                collision.gameObject.GetComponent<ReduceHealth>().takeDamage(20);
                MusicManager.findMusic(Sname);
            }
        }
        if (collision.transform.tag == "Player2")
        {
            if (CameraController2.currentPlayer == 1)
            {
                Destroy(gameObject);
                FindObjectOfType<CameraController2>().setPlayer2();

                collision.gameObject.GetComponent<ReduceHealth>().takeDamage(20);
                MusicManager.findMusic(Sname);
            }
        }
        if (collision.transform.tag == "Obstacles")
        {
            if (CameraController2.currentPlayer == 1) FindObjectOfType<CameraController2>().setPlayer2();
            else
                if (CameraController2.currentPlayer == 2) FindObjectOfType<CameraController2>().setPlayer1();
            Destroy(gameObject);
        }
    }
    
}
