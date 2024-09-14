using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    // Maginzine maximum ammo capacity
    public int maxAmmoInMag = 15;
    // Maximum ammo capacity in the storage
    public int maxAmmoInStorage = 0;
    public float shootCooldown = 0.75f;
    public float reloadCooldown = 0.5f;
    private float switchCooldown = 0.5f;
    public float shootRange = 250f;

    public int currentAmmoInMag;       // Current ammo in the magazine
    public int currentAmmoInStorage;   // Current ammo in the storage
    public int damager;   // Current ammo in the storage
    public bool canShoot = true;       // Flag to check if shooting is allowed
    public bool canSwitch = true;       // Flag to check if shooting is allowed
    private bool isReloading = false;   // Flag to check if reloading is in progress
    private float shootTimer;           // Timer for shoot cooldown

    // Ejection point of the cartridge
    public Transform cartridgeEjectionPoint;
    // Prefab of the cartridge
    public GameObject cartridgePrefab;
    // Force applied to the cartridge
    public float cartridgeEjectionForce = 5f;

    // Particle effect for impact
    public ParticleSystem impactEffect;

    public Animator gun;
    public ParticleSystem muzzleFlash;
    public GameObject muzzleFlashLight;
    public AudioSource shoot;

    void Start()
    {
        currentAmmoInMag = maxAmmoInMag;
        currentAmmoInStorage = maxAmmoInStorage;
        canSwitch = true;
        muzzleFlashLight.SetActive(false);
    }

    void Update()
    {
        // Update current ammo counts
        currentAmmoInMag = Mathf.Clamp(currentAmmoInMag, 0, maxAmmoInMag);
        currentAmmoInStorage = Mathf.Clamp(currentAmmoInStorage, 0, maxAmmoInStorage);

        // Check for shoot input
        if (Input.GetButtonDown("Fire1") && canShoot && !isReloading)
        {
            switchCooldown = shootCooldown;
            Shoot();
        }

        // Update the shoot timer
        if (shootTimer > 0f)
        {
            shootTimer -= Time.deltaTime;
        }
    }

    void Shoot()
    {
        // Check if there is ammo in the magazine
        if (currentAmmoInMag > 0 && shootTimer <= 0f)
        {
            canSwitch = false;
            shoot.Play();
            muzzleFlash.Play();
            muzzleFlashLight.SetActive(true);
            gun.SetBool("shoot", true);

            // Perform the shoot action
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootRange))
            {
                // Instantiate impact effect at the hit point
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            // Instantiate the empty cartridge
            GameObject cartridge = Instantiate(cartridgePrefab, cartridgeEjectionPoint.position, cartridgeEjectionPoint.rotation);
            Rigidbody cartridgeRigidbody = cartridge.GetComponent<Rigidbody>();

            // Apply force to eject the cartridge
            cartridgeRigidbody.AddForce(cartridgeEjectionPoint.right * cartridgeEjectionForce, ForceMode.Impulse);

            StartCoroutine(endAnimations());
            StartCoroutine(endLight());
            StartCoroutine(canswitchshoot());

            switchCooldown -= Time.deltaTime;

            // Reduce ammo count
            currentAmmoInMag--;

            // Start the shoot cooldown
            shootTimer = shootCooldown;
        }
    }

    IEnumerator endAnimations()
    {
        yield return new WaitForSeconds(.1f);
        gun.SetBool("shoot", false);
        //gun.SetBool("reload", false);
    }

    IEnumerator endLight()
    {
        yield return new WaitForSeconds(.1f);
        muzzleFlashLight.SetActive(false);
    }

    IEnumerator canswitchshoot()
    {
        yield return new WaitForSeconds(shootCooldown);
        canSwitch = true;
    }

}