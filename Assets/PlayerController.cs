using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float xBoundary = 14f;
    //serializedfield equivalent to public to Unity, private to other files 

    [Header("Combat Settings")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.35f;

    private float nextFireTime = 0f;



    void Update()
    {
        //1. MOVEMENT LOGIC
        // 1. Read input (-1 to 1 on A/D or Left/Right arrows)
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        //using raw to make it snappier, without it would have smoother acceleration

        // 2. Calculate movement for this frame
        Vector3 direction = new Vector3(horizontalInput, 0f, 0f);
        //horizontalInput is only changing the X coord

        transform.position += direction * moveSpeed * Time.deltaTime;
        //Time.deltaTime: The fraction of a second that elapsed since the last render frame (e.g., at 60 FPS, this is roughly 0.0167 seconds).

        // 3. Keep the player within the visible play area
        //Using Clamp is more efficient but is just checking if pos>max, ret max & if pos<min, ret min 
        float clampedX = Mathf.Clamp(transform.position.x, -xBoundary, xBoundary);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    
    
    // 2.SHOOTING LOGIC
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        // Spawn the laser at firePoint (or player position if firePoint is not set)
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        Quaternion spawnRot = firePoint != null ? firePoint.rotation : transform.rotation;

        Instantiate(laserPrefab, spawnPos, spawnRot);
    }





}