using UnityEngine;

public class Pistol : Gun
{
    public override void Update()
    {
        base.Update();

        if (Input.GetButtonDown("Fire1"))
        {
            TryShoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            TryReload();
        }
    }

    public override void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, gunData.shootingRange, gunData.targetLayerMask))
        {
            Debug.Log(gunData.gunName + " hit " + hit.collider.name);
        }
    }
}
