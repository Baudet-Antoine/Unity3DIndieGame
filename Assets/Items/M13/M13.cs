using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "M13", menuName = "Inventory/Weapon/M13")]
public class M13 : Weapon
{

    public int burstCount = 3;   
    public float burstInterval = 0.1f;

    public override void Attack(GameObject currentTarget)
    {
        if (currentTarget != null)
        {
            if (!projectileSpawnPoint)
            {
                projectileSpawnPoint = PlayerController.Instance.WeaponModel.transform;
            }
            

            if (!isMelee)
            {
                if (projectilePrefab != null)
                {
                    PlayerController.Instance.StartCoroutine(FireBurst(currentTarget));
                    nextFireTime = Time.time + fireRate; 
                }
            }
        }
    }

    private IEnumerator FireBurst(GameObject currentTarget)
    {
        for (int i = 0; i < burstCount; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

            BulletMovement bulletMovement = projectile.GetComponent<BulletMovement>();
            if (bulletMovement != null && currentTarget != null)
            {
                bulletMovement.Initialize(currentTarget.transform, projectileSpeed);
            }

            yield return new WaitForSeconds(burstInterval);
        }
    }
}
