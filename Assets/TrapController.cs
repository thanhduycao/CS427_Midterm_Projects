using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    public string Sname;
    public int damage = 20;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (gameObject.tag == "Dead Trap")
            {
                collision.gameObject.GetComponent<ReduceHealth>().takeAllDamage();
            } else if (gameObject.tag == "Normal Trap")
            {
                collision.gameObject.GetComponent<ReduceHealth>().takeDamage(damage);
            }
        }
    }
}
