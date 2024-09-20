using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Destroy the bullet upon collision with any object
        Destroy(gameObject);
    }
}