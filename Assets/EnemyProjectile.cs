using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 25f;
    [SerializeField] private float lifetime = 3f;

    void Start()
    {
        // Automatically destroy after 'lifetime' seconds to prevent memory leaks
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move forward along its local forward direction
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    
    
}