using System;
using System.Collections;
using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;

    // Weapon Fire Stats
    public bool isFiring, readyToFire;
    bool allowReset = true;
    public float firingDelay = 2f;

    [Header("Burst")] // Burst fire
    public int bulletsPerBurst = 3;
    public int remainingBulletsInBurst;

    [Header("Spread")] // Spread
    public float bulletSpread;
    public float hipfireSpread;
    public float adsSpread;

    [Header("Bullet")] // Bullet info
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletLifeTime = 3f;

    // Weapon Effects
    public GameObject muzzleFlashEffect;
    internal Animator animator;

    [Header("Reload")] // Reload Stats
    public float reloadTime;
    public int magazineCapacity, bulletsRemaining, bulletsMissing;
    public bool isReloading;

    [Header("Other")] // Where to put weapon in camera view
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    bool isADS;

    // Track what weapon we are using
    public enum WeaponModel
    {
        Glock18,
        AK47
    }

    public WeaponModel currentWeaponModel;

    public enum SelectFireMode
    {
        SemiAuto,
        Burst,
        FullAuto
    }

    public SelectFireMode currentSelectFireMode;

    private void Awake()
    {
        readyToFire = true;
        remainingBulletsInBurst = bulletsPerBurst;
        animator = GetComponent<Animator>();

        bulletsRemaining = magazineCapacity;
        bulletsMissing = 0;

        bulletSpread = hipfireSpread;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActiveWeapon)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("WeaponRender"); // display onto of everything else
            }

            if (Input.GetMouseButtonDown(1))
            {
                EnterADS();
            }

            if (Input.GetMouseButtonUp(1))
            {
                ExitADS();
            }

            GetComponent<Outline>().enabled = false; // double check that outline is disabled

            if (bulletsRemaining == 0 && isFiring && !isReloading)
            {
                // SoundManager.Instance.dryFireSoundGlock18.Play(); // old sound setup
                SoundManager.Instance.PlayDryFireSound(currentWeaponModel);

            }

            if (currentSelectFireMode == SelectFireMode.FullAuto)
            {
                // Holding down left mouse button
                isFiring = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentSelectFireMode == SelectFireMode.SemiAuto || currentSelectFireMode == SelectFireMode.Burst)
            {
                // Clicking left mouse button once
                isFiring = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletsRemaining < magazineCapacity && !isReloading && 
                WeaponManager.Instance.CheckAmmoRemaining(currentWeaponModel) > 0)
            {
                Reload();
            }

            // Automatically reload when magazine is empty
            if (readyToFire && !isFiring && !isReloading && bulletsRemaining <= 0)
            {
                // Reload();
            }

            if (readyToFire && isFiring && bulletsRemaining > 0)
            {
                remainingBulletsInBurst = bulletsPerBurst;
                FireWeapon();
            }
        }
        else // not active weapon
        {
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Default"); // stops weapon from showing through walls
            }
        }
    }

    private void EnterADS()
    {
        animator.SetTrigger("enterADS");
        isADS = true;
        UIManager.Instance.crosshair.SetActive(false);
        bulletSpread = adsSpread;
    }

    private void ExitADS()
    {
        animator.SetTrigger("exitADS");
        isADS = false;
        UIManager.Instance.crosshair.SetActive(true);
        bulletSpread = hipfireSpread;
    }
    
    private void FireWeapon()
    {
        if (!isReloading) // only fire if weapon isn't currently reloading
        {
            bulletsRemaining--;
            bulletsMissing++;

            muzzleFlashEffect.GetComponent<ParticleSystem>().Play();

            if (isADS)
            {
                animator.SetTrigger("RECOIL_ADS");
            }
            else
            {
                animator.SetTrigger("RECOIL");
            }

            // SoundManager.Instance.shootingSoundGlock18.Play(); // old sound setup
            SoundManager.Instance.PlayFiringSound(currentWeaponModel);

            readyToFire = false;

            Vector3 shootingDirection = CalculateDirectionAndSpeed().normalized;

            // Create the bullet
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

            // Aim bullet in shooting direction
            bullet.transform.forward = shootingDirection;

            // Shoot the bullet
            bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

            // Destroy the bullet after some amount of time
            StartCoroutine(DestroyBullet(bullet, bulletLifeTime));

            // Check if we are finished firing
            if (allowReset)
            {
                Invoke("ResetShot", firingDelay);
                allowReset = false;
            }

            // Burst mode
            if (currentSelectFireMode == SelectFireMode.Burst && remainingBulletsInBurst > 1)
            {
                remainingBulletsInBurst -= 1;
                Invoke("FireWeapon", firingDelay);
            }
        }
    }

    private void Reload()
    {
        // SoundManager.Instance.reloadingSoundGlock18.Play(); // old sound setup
        SoundManager.Instance.PlayReloadSound(currentWeaponModel);

        animator.SetTrigger("RELOAD");

        isReloading = true;
        Invoke("ReloadComplete", reloadTime);
    }

    private void ReloadComplete()
    {
        if (WeaponManager.Instance.CheckAmmoRemaining(currentWeaponModel) >= magazineCapacity)
        {
            bulletsRemaining = magazineCapacity;
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsMissing, currentWeaponModel);
        }
        else // less than one magazine of ammo remaining
        {
            // Weapon missing more ammo than we have remaining
            if (bulletsMissing > WeaponManager.Instance.CheckAmmoRemaining(currentWeaponModel))
            {
                bulletsRemaining += WeaponManager.Instance.CheckAmmoRemaining(currentWeaponModel);
                WeaponManager.Instance.DecreaseTotalAmmo(WeaponManager.Instance.CheckAmmoRemaining(currentWeaponModel), currentWeaponModel);
            }
            else // Weapon missing less or equal ammo than we have remaining
            {
                bulletsRemaining = magazineCapacity;
                WeaponManager.Instance.DecreaseTotalAmmo(bulletsMissing, currentWeaponModel);
            }
        }

        // Set missing ammo in case you don't fully reload magazine because not enough reserve ammo
        bulletsMissing = magazineCapacity - bulletsRemaining;
        isReloading = false;
    }

    private void ResetShot()
    {
        readyToFire = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpeed()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            // Hit an object
            targetPoint = hit.point;
        }
        else
        {
            // Doesn't hit an object, fly off in air
            targetPoint = ray.GetPoint(50);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-bulletSpread, bulletSpread);
        float y = UnityEngine.Random.Range(-bulletSpread, bulletSpread);
        float z = UnityEngine.Random.Range(-bulletSpread, bulletSpread);

        // Return shooting direction with bullet spread
        return direction + new Vector3(x, y, z);
    }

    private IEnumerator DestroyBullet(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
