using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "YellowStaff", menuName = "Inventory/Weapon/YellowStaff")]

public class YellowStaff : Weapon
{
    public override void Attack(GameObject currentTarget)
    {
        if (currentTarget != null)
        {
            if(!projectileSpawnPoint)
            {
                projectileSpawnPoint = PlayerController.Instance.WeaponModel.transform;
            }
            if (projectilePrefab != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                
                BulletMovement bulletMovement = projectile.GetComponent<BulletMovement>();
                if (bulletMovement != null && currentTarget != null)
                {
                    bulletMovement.Initialize(currentTarget.transform,projectileSpeed);
                    List<GameObject> ennemies = FindEnemyChain();

                    currentTarget.GetComponent<EnemyController>().OnProjectileDestroyed = () =>
                    {
                        ExecuteChainDamage(ennemies);
                    };

                    nextFireTime = Time.time + fireRate; 
                }
            }
        }
    }

    private void ExecuteChainDamage(List<GameObject> ennemies)
    {
        for (int i = 0; i < ennemies.Count - 1; i++)
        {
            Vector3 startPosition = ennemies[i].transform.position;
            Vector3 endPosition = ennemies[i + 1].transform.position;
            Vector3 midPoint = (startPosition + endPosition) / 2;
            float distance = Vector3.Distance(startPosition, endPosition);
            Quaternion rotation = Quaternion.LookRotation(endPosition - startPosition);

            GameObject yellowRectangle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            yellowRectangle.transform.position = midPoint;
            yellowRectangle.transform.rotation = rotation;
            yellowRectangle.transform.localScale = new Vector3(0.5f, 0.5f, distance);
            yellowRectangle.GetComponent<Renderer>().material.color = Color.yellow;
            Destroy(yellowRectangle, 0.3f);

            int totalDamage = Damage + PlayerController.Instance.Attack;

            if (i > 0 && ennemies[i])
            {
                ennemies[i].GetComponent<EnemyController>().TakeDamage(
                    totalDamage - (totalDamage * (float)Mathf.Pow(0.2f, i)),
                    Color.yellow
                );
            }
        }
    }


    List<GameObject> FindEnemyChain()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> closestEnemies = new List<GameObject>();
        float maxDistance = 10.0f; 
        Vector3 currentPosition = projectileSpawnPoint.position;

        for (int i = 0; i < 5; i++)
        {
            GameObject closestEnemy = null;
            float closestDistance = Mathf.Infinity;

            foreach (GameObject enemy in enemies)
            {
                if (!closestEnemies.Contains(enemy))
                {
                    float distance = Vector3.Distance(currentPosition, enemy.transform.position);
                    if (distance < closestDistance && distance <= maxDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemy;
                    }
                }
            }

            if (closestEnemy != null)
            {
                closestEnemies.Add(closestEnemy);
                currentPosition = closestEnemy.transform.position;
            }
            else
            {
                break;
            }
        }

        return closestEnemies;
    }
    

}
