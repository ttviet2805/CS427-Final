using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    // Number of cartridges in a pistol
    public int maxAmmoInMag = 1000;
    public int maxAmmoInStorage = 0;
    public float shootCooldown = 0.75f;
    private float switchCooldown = 0.5f;
    public float shootRange = 250f;

    public int currentAmmoInMag;
    public int currentAmmoInStorage;
    public int damager = 20;
    public bool canShoot = true;
    public bool canSwitch = true;
    private float shootTimer;

    public Transform cartridgeEjectionPoint;
    public GameObject cartridgePrefab;
    public float cartridgeEjectionForce = 5f;

    public ParticleSystem impactEffect;

    public Animator gun;
    public ParticleSystem muzzleFlash;
    public GameObject muzzleFlashLight;
    public AudioSource shoot;

    void Start() {
        currentAmmoInMag = maxAmmoInMag;
        currentAmmoInStorage = maxAmmoInStorage;
        canSwitch = true;
        muzzleFlashLight.SetActive(false);
    }

    void Update() {
        currentAmmoInMag = Mathf.Clamp(currentAmmoInMag, 0, maxAmmoInMag);
        currentAmmoInStorage = Mathf.Clamp(currentAmmoInStorage, 0, maxAmmoInStorage);

        if (Input.GetButtonDown("Fire1") && canShoot) {
            switchCooldown = shootCooldown;
            Shoot();
        }
        if (shootTimer > 0f)
            shootTimer -= Time.deltaTime;
    }

    void Shoot() {
        if (currentAmmoInMag > 0 && shootTimer <= 0f) {
            canSwitch = false;
            shoot.Play();
            muzzleFlash.Play();
            muzzleFlashLight.SetActive(true);
            gun.SetBool("shoot", true);

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootRange)) {
                if (hit.collider.CompareTag("Enemy")) {
                    Debug.Log("HEHE");
                    EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                        enemyHealth.TakeDamage(damager);
                }
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            GameObject cartridge = Instantiate(cartridgePrefab, cartridgeEjectionPoint.position, cartridgeEjectionPoint.rotation);
            Rigidbody cartridgeRigidbody = cartridge.GetComponent<Rigidbody>();

            cartridgeRigidbody.AddForce(cartridgeEjectionPoint.right * cartridgeEjectionForce, ForceMode.Impulse);

            StartCoroutine(endAnimations());
            StartCoroutine(endLight());
            StartCoroutine(canswitchshoot());

            switchCooldown -= Time.deltaTime;
            currentAmmoInMag--;
            shootTimer = shootCooldown;
        }
    }

    IEnumerator endAnimations() {
        yield return new WaitForSeconds(.1f);
        gun.SetBool("shoot", false);
    }

    IEnumerator endLight() {
        yield return new WaitForSeconds(.1f);
        muzzleFlashLight.SetActive(false);
    }

    IEnumerator canswitchshoot() {
        yield return new WaitForSeconds(shootCooldown);
        canSwitch = true;
    }

}