using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Weapon Fire Stats
    public bool isFiring, readyToFire;
    bool allowReset = true;
    public float firingDelay = 2f;

    // Burst fire
    public int bulletsPerBurst = 3;
    public int remainingBulletsInBurst;

    // Spread
    public float bulletSpread;

    // Bullet info
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletLifeTime = 3f;

    public GameObject muzzleFlashEffect;
    private Animator animator;

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
    }

    // Update is called once per frame
    void Update()
    {
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

        if (readyToFire && isFiring)
        {
            remainingBulletsInBurst = bulletsPerBurst;
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        muzzleFlashEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");

        SoundManager.Instance.shootingSoundGlock18.Play();

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
