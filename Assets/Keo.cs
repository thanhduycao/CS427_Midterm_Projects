using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keo : MonoBehaviour
{
    LineRenderer lr;

    Vector3 startPos;
    Vector3 endPos;
    Vector3 pointoffset;

    public GameObject ball;

    public Transform player1, player2;
    public Transform temp;
    public float speed;
    private void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lr.enabled = true;
            lr.positionCount = 2;
            startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 1);
            lr.SetPosition(0, startPos);
        }
        if (Input.GetMouseButton(0))
        {
            pointoffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - startPos + new Vector3(0, 0, 1);
            lr.SetPosition(1, startPos + pointoffset);
        }
        if (Input.GetMouseButtonUp(0))
        {
            lr.enabled = false;
            temp = player2.transform;
            if (CameraController2.currentPlayer == 1) temp = player1.transform;
            GameObject ballFired=Instantiate(ball, temp.position, Quaternion.identity);
            Rigidbody2D rb = ballFired.GetComponent<Rigidbody2D>();
            rb.AddForce((-pointoffset) * speed);
            FindObjectOfType<CameraController2>().setCameraBall(ballFired.transform);
            MusicManager.findMusic("Ball Shoot");
        }
    }
}
