using UnityEngine;
using System.Collections;
using TMPro;
using System;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{

    public bool IsDead { get; private set; } = false;
    public bool isBurning = false;
    public bool isPoisoning = false;
    public bool isSlowing = false;
    public int Attack = 15;
    public float maxHealth = 100;
    public float currentHealth;
    private float accumulatedDamage = 0f; // Dégâts accumulés
    private float lastDamageDisplayTime = 0f;
    public int[] DropsItemsID;
    public int DropRate;
    public int textureSampleRate = 4;
    public int PoisonAmount = 0;
    public int BurnAmount = 0;
    public int SlowAmount = 0;

    public float speed = 3f;
    public float BaseSpeed = 3f;
    public float attackInterval = 1f;
    public float stopDistance = 5f;
    public float AggroRange = 35f;
    public float distanceToPlayer;
    public float BurnTime = 0.0f;
    public float PoisonTime = 0.0f;
    public float SlowTime = 0.0f;

    public Collider triggerCollider;
    public Collider physicsCollider;

    public GameObject DamageText;
    public GameObject explosionEffect;
    public GameObject Model;
    public GameObject XpPrefab;
    public GameObject CoinPrefab;
    public GameObject Item;
    public GameObject IceStunPrefab;
    public GameObject target;

    private Renderer enemyRenderer;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    
    private Color[] enemyColors;
    
    private bool isAttacking = false;
    private Coroutine attackCoroutine;

    public Animator animator;
    
    public Action OnProjectileDestroyed;

    void Start()
    {
        currentHealth = maxHealth;
        enemyRenderer = Model.GetComponent<Renderer>();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>(); // Récupère le NavMeshAgent
        if (enemyRenderer != null)
        {
            enemyColors = GetUsedColorsFromTextureByUV(enemyRenderer);
        }
    }

    void Update()
    {
        MoveTowardsPlayer();

        if(BurnTime > 0.0f && isBurning == false)
        {
            isBurning = true;
            StartCoroutine(Burn(BurnAmount));
        }   
        if(PoisonTime > 0.0f && isPoisoning == false)
        {
            isPoisoning = true;
            StartCoroutine(Poison(PoisonAmount));
        }
        if(SlowTime > 0.0f && isSlowing == false)
        {
            isSlowing = true;
            StartCoroutine(Slow(SlowAmount));
        }
    }

    private IEnumerator Slow(int Amount)
    {
        ApplySlow(Amount);
        float duration = (SlowAmount == 100) ? SlowTime/2 : SlowTime;
        GameObject Ice = Instantiate(IceStunPrefab, this.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(duration);
        SlowTime = 0;
        SpeedReset();
        Destroy(Ice);
        isSlowing = false;
    }

    private IEnumerator Burn(int Amount)
    {
        while (BurnTime > 0.0f)
        {
            TakeDamage(Amount, Color.red);
            BurnTime -= 0.3f;
            yield return new WaitForSeconds(0.3f);
        }
        isBurning = false;
    }
    private IEnumerator Poison(int Amount)
    {
        while (PoisonTime > 0.0f)
        {
            TakeDamage(Amount, Color.magenta);
            PoisonTime -= 0.3f;
            yield return new WaitForSeconds(0.3f);
        }
        isPoisoning = false;
    }

    
    GameObject FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {   
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        if(closestPlayer != null && closestDistance <= AggroRange)
        {
            return closestPlayer;
        }
        else
        {
            return null;
        }
    }

    void MoveTowardsPlayer()
    {

        target = FindClosestPlayer();

        if (target != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);
            // Si le joueur est à portée d'attaque
            if (distanceToPlayer <= stopDistance)
            {
                animator.SetBool("isRunning", false);
                navMeshAgent.isStopped = true;
                // Commencer l'attaque si ce n'est pas déjà fait
                if (!isAttacking)
                {
                    StartAttack();
                }

                // Rotation de l'ennemi pour faire face au joueur
                Vector3 direction = (target.transform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.Slerp(Model.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
                Model.transform.rotation = lookRotation;
            }
            else // Si le joueur est trop loin
            {
                animator.SetBool("isRunning", true);
                // Configurer la destination du NavMeshAgent
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(target.transform.position);
                navMeshAgent.speed = speed;

                // Arrêter l'attaque si le joueur est trop éloigné
                if (isAttacking)
                {
                    StopAttack();
                }

                // Rotation de l'ennemi pour faire face au joueur tout en se déplaçant
                Vector3 direction = (target.transform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.Slerp(Model.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
                Model.transform.rotation = lookRotation;
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
            navMeshAgent.isStopped = true;
        }
    }


    void StartAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }
            attackCoroutine = StartCoroutine(AttackPlayer());
        }
    }

    public void TakeDamage(float damage, Color color)
    {
        // Ajouter les dégâts à la somme accumulée
        accumulatedDamage += damage;

        // Vérifier si suffisamment de temps s'est écoulé pour afficher les dégâts
        if (Time.time - lastDamageDisplayTime >= 0.1f)
        {
            DisplayDamage(accumulatedDamage, color);
            lastDamageDisplayTime = Time.time; // Mettre à jour le temps de la dernière mise à jour
            accumulatedDamage = 0f; // Réinitialiser les dégâts accumulés après l'affichage
        }

        currentHealth -= damage; // Appliquer les dégâts à la santé actuelle
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void DisplayDamage(float damage,Color color)
    {
        float objectHeight = 0f;
        Collider objectCollider = GetComponent<Collider>();
        if (objectCollider != null)
        {
            objectHeight = objectCollider.bounds.size.y;
        }

        Vector3 damageVector = new Vector3(transform.position.x, transform.position.y + objectHeight, transform.position.z);
        var damageText = Instantiate(DamageText, damageVector, Quaternion.identity);
        damageText.GetComponent<TextMeshPro>().text = "-" + Mathf.FloorToInt(damage).ToString();
        damageText.GetComponent<TextMeshPro>().color = color;
    
        Destroy(damageText, 1f);
    }

    public void ApplySlow(float amount)
    {
        speed = speed * (1 - (amount/100));
    }

    public void SpeedReset()
    {
        speed = BaseSpeed;
    }

    public void DieNoDrop()
    {
        GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();

        if (ps != null && enemyColors.Length > 0)
        {
            ParticleSystem.MainModule mainModule = ps.main;
            mainModule.startColor = enemyColors[UnityEngine.Random.Range(0, enemyColors.Length)];
        }

        if (EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.EnemyDied();
        }

        Destroy(effect, 3f);
        Destroy(gameObject);
    }

    public void Die()
    {
        IsDead = true;
        GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();

        if (ps != null && enemyColors.Length > 0)
        {
            ParticleSystem.MainModule mainModule = ps.main;
            mainModule.startColor = enemyColors[UnityEngine.Random.Range(0, enemyColors.Length)];
        }

        DropXP(UnityEngine.Random.Range(2, 5));
        DropCoin(UnityEngine.Random.Range(2,5));

        if (UnityEngine.Random.Range(0, DropRate) == DropRate - 1)
        {
            DropItem(UnityEngine.Random.Range(0, GameManager.Instance.ItemList.Count));
        }

        if (EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.EnemyDied();
        }

        Destroy(effect, 3f);
        Destroy(gameObject);
    }

    void DropItem(int ID)
    {
        Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f));
        
        GameObject item = Instantiate(GameManager.Instance.GetItemByID(ID).ModelGround, transform.position + randomOffset, Quaternion.identity);

        Rigidbody rb = item.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 randomForce = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0.5f, 1.5f), UnityEngine.Random.Range(-1f, 1f)) * 5f;
            rb.AddForce(randomForce, ForceMode.Impulse);

            rb.useGravity = true;

            rb.drag = 2f; 
            rb.angularDrag = 0.5f;
        }

        Collider mainCollider = item.GetComponent<Collider>();
        if (mainCollider != null)
        {
            mainCollider.isTrigger = false; 
        }

        SphereCollider triggerCollider = item.AddComponent<SphereCollider>(); 
        triggerCollider.isTrigger = true;
        triggerCollider.radius = 0.5f; 

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Item"), true);
    }

    void DropXP(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f));

            GameObject xpObject = Instantiate(XpPrefab, transform.position + randomOffset, Quaternion.identity);

            Rigidbody rb = xpObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 randomForce = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0.5f, 1.5f), UnityEngine.Random.Range(-1f, 1f)) * 5f;
                rb.AddForce(randomForce, ForceMode.Impulse);

                rb.useGravity = true;

                rb.drag = 2f;
                rb.angularDrag = 0.5f;
            }

            Collider mainCollider = xpObject.GetComponent<Collider>();
            if (mainCollider != null)
            {
                mainCollider.isTrigger = false;
            }

            SphereCollider triggerCollider = xpObject.AddComponent<SphereCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.radius = 0.5f;

            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("XP"), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Item"), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("XP"), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Item"), true);
        }
    }

    void DropCoin(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f));

            GameObject coinObject = Instantiate(CoinPrefab, transform.position + randomOffset, Quaternion.identity);

            Rigidbody rb = coinObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 randomForce = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0.5f, 1.5f), UnityEngine.Random.Range(-1f, 1f)) * 5f;
                rb.AddForce(randomForce, ForceMode.Impulse);

                rb.useGravity = true;

                rb.drag = 2f;
                rb.angularDrag = 0.5f;
            }

            Collider mainCollider = coinObject.GetComponent<Collider>();
            if (mainCollider != null)
            {
                mainCollider.isTrigger = false;
            }

            SphereCollider triggerCollider = coinObject.AddComponent<SphereCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.radius = 0.5f;

            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Coin"), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Item"), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Coin"), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Item"), true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Weapon weapon = other.GetComponent<BulletMovement>().Weapon;
            OnProjectileDestroyed?.Invoke();
            Destroy(other.gameObject);
            TakeDamage(PlayerController.Instance.Attack + weapon.Damage, Color.white);

            var states = new Dictionary<string, bool>
            {
                { "Burning", weapon.isBurning },
                { "Vamping", weapon.isVamping },
                { "Poisoning", weapon.isPoisoning },
                { "Slowing", weapon.isSlowing }
            };

            foreach (var state in states)
            {
                if (state.Value)
                {
                    switch (state.Key)
                    {
                        case "Burning":
                            BurnTime = Mathf.Max(BurnTime, 2.0f);
                            BurnAmount = weapon.EffectAmount;
                            break;
                        case "Vamping":
                            PlayerController.Instance.HealPlayer((PlayerController.Instance.Attack + weapon.Damage)/5);
                            break;
                        case "Poisoning":
                            PoisonTime = Mathf.Max(PoisonTime, 2.0f);
                            PoisonAmount = weapon.EffectAmount;
                            break;
                        case "Slowing":
                            SlowTime = Mathf.Max(SlowTime, 2.0f);
                            SlowAmount = weapon.EffectAmount;
                            break;
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isAttacking)
            {
                StopAttack();
            }
        }
    }

    private IEnumerator AttackPlayer()
    {
        while (isAttacking)
        {
            PlayerController.Instance.TakeDamage(Attack, Color.white);
            yield return new WaitForSeconds(attackInterval);
        }
    }

    void StopAttack()
    {
        if (isAttacking)
        {
            isAttacking = false;
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }
        }
    }

    private Color[] GetUsedColorsFromTextureByUV(Renderer renderer)
    {
        Mesh mesh = null;
        MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
        SkinnedMeshRenderer skinnedMeshRenderer = renderer.GetComponent<SkinnedMeshRenderer>();

        if (meshFilter != null)
        {
            mesh = meshFilter.mesh;
        }
        else if (skinnedMeshRenderer != null)
        {
            mesh = skinnedMeshRenderer.sharedMesh;
        }

        if (mesh == null)
        {
            return null;
        }

        Vector2[] uvs = mesh.uv;

        Texture2D texture = (Texture2D)renderer.material.mainTexture;

        if (texture == null || !texture.isReadable)
        {
            return null;
        }

        List<Color> colors = new List<Color>();
        int texWidth = texture.width;
        int texHeight = texture.height;

        foreach (Vector2 uv in uvs)
        {
            int x = Mathf.FloorToInt(uv.x * texWidth);
            int y = Mathf.FloorToInt(uv.y * texHeight);
            Color color = texture.GetPixel(x, y);
            colors.Add(color);
        }

        return colors.ToArray();
    }
}
