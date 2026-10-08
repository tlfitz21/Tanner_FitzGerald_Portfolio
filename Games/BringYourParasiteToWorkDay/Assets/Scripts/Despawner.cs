using UnityEngine;

public class Despawner : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
    }
}
