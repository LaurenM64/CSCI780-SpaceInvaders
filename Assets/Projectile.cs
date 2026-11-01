using UnityEngine;

public class Projectile : MonoBehaviour
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
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // When it hits an alien later on:
        if (other.CompareTag("Alien"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}