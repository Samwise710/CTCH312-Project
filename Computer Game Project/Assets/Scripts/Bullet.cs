using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision objectHit)
    {
        if (objectHit.gameObject.CompareTag("Target"))
        {
            print("hit " + objectHit.gameObject.name);
            CreateBulletImpactEffect(objectHit);
            Destroy(gameObject);
        }
        else if (objectHit.gameObject.CompareTag("Wall"))
        {
            print("hit a wall");
            CreateBulletImpactEffect(objectHit);
            Destroy(gameObject);
        }
        else
        {
            print("hit something other than target or wall");
            CreateBulletImpactEffect(objectHit);
            Destroy(gameObject);
        }
    }

    void CreateBulletImpactEffect(Collision objectHit)
    {
        ContactPoint contact = objectHit.contacts[0];

        if (objectHit.gameObject.CompareTag("Metal"))
        {
            GameObject hole = Instantiate(
                GlobalReferences.Instance.bulletImpactMetalEffect,
                contact.point,
                Quaternion.LookRotation(contact.normal)
                );

            hole.transform.SetParent(objectHit.gameObject.transform);
        }
        else if (objectHit.gameObject.CompareTag("Wood"))
        {
            GameObject hole = Instantiate(
                GlobalReferences.Instance.bulletImpactWoodEffect,
                contact.point,
                Quaternion.LookRotation(contact.normal)
                );

            hole.transform.SetParent(objectHit.gameObject.transform);
        }
        else if (objectHit.gameObject.CompareTag("Sand"))
        {
            GameObject hole = Instantiate(
                GlobalReferences.Instance.bulletImpactSandEffect,
                contact.point,
                Quaternion.LookRotation(contact.normal)
                );

            hole.transform.SetParent(objectHit.gameObject.transform);
        }
        else if (objectHit.gameObject.CompareTag("Flesh"))
        {
            GameObject hole = Instantiate(
                GlobalReferences.Instance.bulletImpactFleshEffect,
                contact.point,
                Quaternion.LookRotation(contact.normal)
                );

            hole.transform.SetParent(objectHit.gameObject.transform);
        }
        else
        {
            GameObject hole = Instantiate(
                GlobalReferences.Instance.bulletImpactStoneEffect,
                contact.point,
                Quaternion.LookRotation(contact.normal)
                );

            hole.transform.SetParent(objectHit.gameObject.transform);
        }

    }
}
