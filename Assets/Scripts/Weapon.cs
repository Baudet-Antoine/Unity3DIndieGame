using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon/Weapon")]
public class Weapon : Item
{
    public bool isMelee;
    public bool isSlowing;
    public bool isBurning;
    public bool isPoisoning;
    public bool isVamping;
    public int EffectAmount;
    public int range;
    public int Damage;
    public float fireRate;
    public float projectileSpeed;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float nextFireTime;

    public virtual void Attack(GameObject currentTarget)
    {
        if (currentTarget != null)
        {
            if(!projectileSpawnPoint)
            {
                projectileSpawnPoint = PlayerController.Instance.WeaponModel.transform.Find("BulletSpawnPoint");
            }
            if (!isMelee)
            {
                if (projectilePrefab != null)
                {
                    GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                    
                    BulletMovement bulletMovement = projectile.GetComponent<BulletMovement>();
                    bulletMovement.Weapon = this;
                    if (bulletMovement != null && currentTarget != null)
                    {
                        bulletMovement.Initialize(currentTarget.transform,projectileSpeed);
                    }

                    nextFireTime = Time.time + fireRate; 
                }
            }
            else if (isMelee)
            {
            }
        }
    }
        
}
