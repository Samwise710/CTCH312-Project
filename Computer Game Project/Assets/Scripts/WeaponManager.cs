using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using static Weapon;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; set; }

    public List<GameObject> weaponSlots;

    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalGlock18Ammo = 0;
    public int totalAK47Ammo = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void Start()
    {
        activeWeaponSlot = weaponSlots[0];
    }

    public void Update()
    {
        foreach (GameObject weaponSlot in weaponSlots)
        {
            if (weaponSlot == activeWeaponSlot)
            {
                weaponSlot.SetActive(true);
            }
            else
            {
                weaponSlot.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) // "1" key
        {
            SwapActiveWeaponSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) // "2" key
        {
            SwapActiveWeaponSlot(1);
        }
    }

    public void PickUpWeapon(GameObject pickedUpWeapon)
    {
        EquipWeaponIntoActiveSlot(pickedUpWeapon);
    }

    // Puts weapon into active weapon slot
    private void EquipWeaponIntoActiveSlot(GameObject pickedUpWeapon)
    {
        DropCurrentWeapon(pickedUpWeapon);

        pickedUpWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        Weapon weapon = pickedUpWeapon.GetComponent<Weapon>();

        // disables box collider, used for detecting hover, because it blocks bullets sometimes
        weapon.GetComponent<BoxCollider>().enabled = false;

        pickedUpWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        pickedUpWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);

        weapon.isActiveWeapon = true;
        weapon.animator.enabled = true; // enable animations on pick up
    }

    internal void PickUpAmmo(AmmoCrate ammo)
    {
        switch (activeWeaponSlot.GetComponentInChildren<Weapon>().currentWeaponModel)
        {
            case Weapon.WeaponModel.Glock18:
                totalGlock18Ammo = ammo.Glock18AmmoCapacity;
                break;

            case Weapon.WeaponModel.AK47:
                totalAK47Ammo = ammo.AK47AmmoCapacity;
                break;
        }
    }

    private void DropCurrentWeapon(GameObject pickedUpWeapon)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;

            // re-enables box collider, used for detecting hover
            weaponToDrop.GetComponent<BoxCollider>().enabled = true;

            weaponToDrop.GetComponent<Weapon>().isActiveWeapon = false;
            weaponToDrop.GetComponent<Weapon>().animator.enabled = false; // turn off animations when dropped

            weaponToDrop.transform.SetParent(pickedUpWeapon.transform.parent);
            weaponToDrop.transform.localPosition = pickedUpWeapon.transform.localPosition;
            weaponToDrop.transform.localRotation = pickedUpWeapon.transform.localRotation;
        }
    }

    public void SwapActiveWeaponSlot(int slotNumber)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActiveWeapon = false;
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            newWeapon.isActiveWeapon = true;
        }
    }

    internal void DecreaseTotalAmmo(int bulletsToDecrease, Weapon.WeaponModel currentWeaponModel)
    {
        switch (currentWeaponModel)
        {
            case Weapon.WeaponModel.Glock18:
                totalGlock18Ammo -= bulletsToDecrease; 
                break;
            case Weapon.WeaponModel.AK47:
                totalAK47Ammo -= bulletsToDecrease;
                break;
        }
    }

    public int CheckAmmoRemaining(Weapon.WeaponModel currentWeaponModel)
    {
        switch (currentWeaponModel)
        {
            case Weapon.WeaponModel.Glock18:
                return totalGlock18Ammo;

            case Weapon.WeaponModel.AK47:
                return totalAK47Ammo;

            default:
                return 0;
        }
    }
}
