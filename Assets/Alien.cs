using UnityEngine;

public class Alien : MonoBehaviour
{
    [Header("Inscribed")]

    [SerializeField] private GameObject bulletPrefab;
    public AlienFleet fleet;
    
    //ensuring these are not static so that the bulletSpawnRate can be different for each alien, and so that the lastShot can be different for each alien
    [SerializeField] private float bulletSpawnRate = .002f;
    [SerializeField] private float lastShot = 0;
    [SerializeField] private float shotCooldown = .1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 position = transform.position;
        
        if(UnityEngine.Random.value < bulletSpawnRate && Time.time - lastShot > shotCooldown){
            GameObject bullet = Instantiate<GameObject>(bulletPrefab);
            bullet.transform.position = position;
            lastShot = Time.time;
        }
    }

    void OnTriggerEnter(Collider other)
{
    fleet.aliens.Remove(gameObject); // (With our redundancy fix applied!)
    Destroy(gameObject);
    Destroy(other.gameObject);
}
}