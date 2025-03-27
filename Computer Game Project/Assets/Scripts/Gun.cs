using System.Collections;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GunData gunData;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Transform cameraTransform;

    private float currentAmmo = 0f;
    private float nextTimeToFire = 0f;

    private bool isReloading = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentAmmo = gunData.magazineSize;

        characterController = transform.root.GetComponent<CharacterController>();
        cameraTransform = characterController.GetComponentInChildren<Camera>().transform;
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }

    public void TryReload()
    {
        if (!isReloading && currentAmmo < gunData.magazineSize)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        Debug.Log(gunData.gunName + " is reloading...");

        yield return new WaitForSeconds(gunData.reloadTime);

        currentAmmo = gunData.magazineSize;
        isReloading = false;

        Debug.Log(gunData.gunName + " is reloaded.");
    }

    public void TryShoot()
    {
        if (isReloading)
        {
            Debug.Log(gunData.gunName + " is reloading...");
            return;
        }

        if (currentAmmo <= 0f)
        {
            Debug.Log(gunData.gunName + " has no ammo left. Please reload.");
            return;
        }

        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + (1 / gunData.fireRate);
            HandleShoot();
        }
    }

    private void HandleShoot()
    {
        currentAmmo--;
        Debug.Log(gunData.gunName + " shot! Remaining ammo: " + currentAmmo);
        Shoot();

        // recoil
        // characterController.ApplyRecoil(gunData);

        // muzzleFlash
    }

    public abstract void Shoot();
}
