using UnityEngine;

public class Weakness : MonoBehaviour
{
    [SerializeField] private GameObject m_Parent;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_Parent.GetComponent<EnemyController>().OnDestroyObject();
        }
    }
}
