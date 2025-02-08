using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PlayerController : MonoBehaviour, IDataPersistence
{
    public int BaseAttack;
    public int Attack;
    public int BaseHealingRate = 5;
    public int XP = 0;
    public int XPNeeded;
    public int Balance = 0;
    public int Level;
    public int maxHealth = 100;
    public int BasemaxHealth = 100;
    public int currentHealth;

    
    public float bulletSpeed = 10f;
    public float rotationSpeed = 5f;  
    public float healingInterval = 2f;
    public float MovementSpeed = 4.0f;
    public float dashCooldown = 2f;    
    public float lastDashTime = -Mathf.Infinity;
    public float nextFireTime = 0f;
    public float blinkRange = 10f;  // Distance maximale du blink
    public float shrinkScale = 0.5f;  // Échelle de rétrécissement
    public float growDuration = 0.5f;  // Durée de l'agrandissement
    public float AttackSpeedMultiplier = 1f;

    public Animator animator;

    public bool isMoving = false; 
    public bool isAttacking = false;
    public bool isMovingTowardsEnemy = false;
    public bool onCombat; 

    public GameObject Model;
    public GameObject WeaponModel;
    public GameObject bulletPrefab;   
    public GameObject DamageText;
    public GameObject MouvementArrow;
    public GameObject AttackArrow;
    public GameObject DyingScreen;
    private GameObject currentArrow;
    private GameObject currentTarget;  
    public GameObject HandPosition;

    public Transform bulletSpawnPoint;
    public Vector3 targetPosition;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    public XPBarManager XPBarManager;
    public static PlayerController Instance;
    private Coroutine healingCoroutine;
    
    public Weapon CurrentWeapon; 

    public MeshFilter weaponMeshFilter; 
    public MeshRenderer weaponMeshRenderer;
    public RaidTimer Timer;

    
    public void LoadData(GameData data)
    {
        this.Balance = data.Balance;
    }

    public void SaveData(GameData data)
    {
        data.Balance = this.Balance;
    }
    void Awake()
    {
        currentHealth = maxHealth;
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();  // Récupère le NavMeshAgent
        XPNeeded = 50;
        EquipWeapon(0);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Item"), true);
        lastDashTime = Time.time - dashCooldown;
        HandPosition = GameObject.FindWithTag("Hand");
    }

    public void Shoot()
    {
        if (currentTarget != null)
        {
            
            // Effectuer le tir réel ici
            CurrentWeapon.Attack(currentTarget);

            // Mettre à jour le temps pour la prochaine attaque
            nextFireTime = Time.time + (CurrentWeapon.fireRate * AttackSpeedMultiplier);
        }
    }

    void AutoShoot()
    {
        if (currentTarget != null)
        {
          
            animator.SetBool("IsWalking",false);  
            navMeshAgent.isStopped = true;

            if (nextFireTime > Time.time)
            {
                return; 
            }
            

            EnemyController enemyController = currentTarget.GetComponent<EnemyController>();
            if (enemyController == null || enemyController.IsDead)
            {
                Debug.Log("Target is dead");
                currentTarget = null; 
                return; 
            }

            Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
            directionToTarget.y = 0; // Keep Y-axis unchanged

            // Rotate the model toward the target
            Model.transform.rotation = Quaternion.Slerp(Model.transform.rotation, Quaternion.LookRotation(directionToTarget), rotationSpeed * Time.deltaTime);

            // If the model is almost aligned with the target
            if (Quaternion.Angle(Model.transform.rotation, Quaternion.LookRotation(directionToTarget)) < 1f)
            {
                // Trigger the attack animation
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("AttackAnimation"))
                {
                    animator.SetTrigger("AttackTrigger");
                }
            }
        }
        else
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(gameObject.transform.position);
            isAttacking = false;
            isMoving = false;
            isMovingTowardsEnemy = false;
        }
    }



    public void EquipWeapon(int Index)
    {
        if (Inventory.Instance.FastInventory[Index] != null)
        {
            CurrentWeapon = Inventory.Instance.FastInventory[Index];
            UpdateStats();
            if(WeaponModel)
            {
                Destroy(WeaponModel);
            }
            
            WeaponModel = Instantiate(Inventory.Instance.FastInventory[Index].Model,HandPosition.transform);

        }
    }


    public void LevelUp()
    {
        LevelUpManager.Instance.LevelUp();
        XP = 0;
        XPNeeded = XPNeeded * 2;
        Level++;
    }

    public void UpdateStats()
    {    
        if (LevelUpManager.Instance != null)
        {
            maxHealth = BasemaxHealth + LevelUpManager.Instance.maxHealthUpgrade;  
            Attack = BaseAttack + CurrentWeapon.Damage + LevelUpManager.Instance.DamageUpgrade;
            navMeshAgent.speed = MovementSpeed * LevelUpManager.Instance.MovementSpeedUpgrade;
            AttackSpeedMultiplier = Mathf.Max(LevelUpManager.Instance.AttackSpeedUpgrade, 0.1f);
            animator.SetFloat("AnimationSpeed", 1/AttackSpeedMultiplier);
        }
        else
        {
            maxHealth = BasemaxHealth;  
            Attack = BaseAttack + CurrentWeapon.Damage;
            navMeshAgent.speed = MovementSpeed;
        }
    }

    public void ResetStats()
    {
        maxHealth = BasemaxHealth;
        Attack = BaseAttack + CurrentWeapon.Damage;
        navMeshAgent.speed = MovementSpeed;
        XPNeeded = 50;
    }

    void Update()
    {
         if (onCombat)
        {
            if (healingCoroutine != null)
            {
                StopCoroutine(healingCoroutine);
                healingCoroutine = null;
            }
        }
        else
        {
            if (healingCoroutine == null)
            {
                healingCoroutine = StartCoroutine(RegenerateHealth());
            }
        }



        HandleInput();

        if(isMoving && targetPosition != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            navMeshAgent.isStopped = false;
            RotatePlayerModel();

            if(distanceToTarget <= 0.25f)
            {
                navMeshAgent.isStopped = true;
                isMoving = false;
                animator.SetBool("IsWalking",false);
            }
        }

        if(isMovingTowardsEnemy && targetPosition != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            targetPosition = currentTarget.transform.position;
            UpdateTarget();
            
            if(distanceToTarget <= CurrentWeapon.range)
            {
                isAttacking = true;
                isMovingTowardsEnemy = false;
                navMeshAgent.isStopped = true;
            }
            else
            {
                navMeshAgent.isStopped = false;
                RotatePlayerModel();
            }
        }

        if(isAttacking)
        {
            AutoShoot();
        }
    }

    void UpdateTarget()
    {
        navMeshAgent.SetDestination(targetPosition);
    }

    void HandleInput()
    {

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > lastDashTime + dashCooldown)
        {
            isAttacking = false;
            Blink();
        }

        if(Input.GetMouseButtonDown(0))
        {
            isAttacking = false;
            if (EventSystem.current.IsPointerOverGameObject() || hudController.Instance.CurrentHUD.Count > 0)
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float distance;

            if(groundPlane.Raycast(ray,out distance))
            {
                isMoving = true;
                isMovingTowardsEnemy = false;
                targetPosition = ray.GetPoint(distance);
                animator.SetBool("IsWalking",true);
                
                if (currentArrow != null)
                {
                    Destroy(currentArrow);
                }

                currentArrow = Instantiate(MouvementArrow, targetPosition, Quaternion.identity);
                Destroy(currentArrow, 1f);

                UpdateTarget();
            }

        }
        else if(Input.GetMouseButtonDown(1))
        {
            isAttacking = false;
            if (EventSystem.current.IsPointerOverGameObject() || hudController.Instance.CurrentHUD.Count > 0)
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float distance;

            if(groundPlane.Raycast(ray,out distance))
            {
                currentTarget = FindClosestEnemyNearPosition(ray.GetPoint(distance)).transform.GetChild(0).gameObject;
                if(currentTarget)
                {
                    isMoving = false;
                    isMovingTowardsEnemy = true;
                    targetPosition = currentTarget.transform.position;
                    animator.SetBool("IsWalking",true);                

                    if (currentArrow != null)
                    {
                        Destroy(currentArrow);
                    }

                    currentArrow = Instantiate(AttackArrow, new Vector3(targetPosition.x, 1, targetPosition.z), Quaternion.identity);
                    Destroy(currentArrow, 1f);

                    UpdateTarget();
                }
            }
        }
    }

    private void RotatePlayerModel()
    {
        if(Vector3.Distance(targetPosition,transform.position) > 1f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Model.transform.rotation = Quaternion.Slerp(Model.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void Blink()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        float distance;

        if (playerPlane.Raycast(ray, out distance))
        {
            Vector3 mousePosition = ray.GetPoint(distance);
            
            float distanceToMouse = Vector3.Distance(transform.position, mousePosition);

            if (distanceToMouse <= blinkRange)
            {
                targetPosition = mousePosition;
            }
            else
            {
                Vector3 direction = (mousePosition - transform.position).normalized;
                targetPosition = transform.position + direction * blinkRange;
            }

            // Démarrer l'effet de blink avec animation
            StartCoroutine(BlinkEffect(targetPosition));
            lastDashTime = Time.time;
        }
    }


    private IEnumerator BlinkEffect(Vector3 targetPosition)
    {
        // Rétrécir le joueur
        Vector3 originalScale = Model.transform.localScale;
        Model.transform.localScale = originalScale * shrinkScale;

        // Attendre un court instant
        yield return new WaitForSeconds(0.1f);

        // Téléporter le joueur à la nouvelle position
        transform.position = targetPosition;

        // Agrandir le joueur à sa taille originale
        float elapsedTime = 0f;
        while (elapsedTime < growDuration)
        {
            Model.transform.localScale = Vector3.Lerp(transform.localScale, originalScale, elapsedTime / growDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Attendre la prochaine frame
        }
        
        // Assurer que la taille finale est bien l'originale
        Model.transform.localScale = originalScale;
    }


    public void TakeDamage(int damage, Color color)
    {
        float objectHeight = GetComponent<Collider>().bounds.size.y;
        Vector3 DamageVector = new Vector3(transform.position.x, transform.position.y + objectHeight, transform.position.z);
        var damageText = Instantiate(DamageText, DamageVector, Quaternion.identity);
        damageText.GetComponent<TextMeshPro>().text = "-" + damage.ToString();
        damageText.GetComponent<TextMeshPro>().color = color;
        Destroy(damageText, 1f);

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        DyingScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    private IEnumerator RegenerateHealth()
    {
        while (!onCombat)
        {
            HealPlayer();
            yield return new WaitForSeconds(healingInterval);
        }
    }

    public void HealPlayer()
    {
        if (currentHealth < maxHealth)
        {
            float objectHeight = GetComponent<Collider>().bounds.size.y;
            Vector3 DamageVector = new Vector3(transform.position.x, transform.position.y + objectHeight + 1, transform.position.z);
            var damageText = Instantiate(DamageText, DamageVector, Quaternion.identity);
            damageText.GetComponent<TextMeshPro>().color = new Color(0f,0.5f,0f);
            damageText.GetComponent<TextMeshPro>().text = "+" + BaseHealingRate.ToString();
            Destroy(damageText, 1f);

            currentHealth += BaseHealingRate;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
    }

    public void HealPlayer(int HealAmount)
    {
        if (currentHealth < maxHealth)
        {
            float objectHeight = GetComponent<Collider>().bounds.size.y;
            Vector3 DamageVector = new Vector3(transform.position.x, transform.position.y + objectHeight + 1, transform.position.z);
            var damageText = Instantiate(DamageText, DamageVector, Quaternion.identity);
            damageText.GetComponent<TextMeshPro>().color = new Color(0f,1.0f,0f);
            damageText.GetComponent<TextMeshPro>().text = "+" + HealAmount.ToString();
            Destroy(damageText, 1f);

            currentHealth += HealAmount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
    }

    public void LeaveBase()
    {
        for (int i = 0; i < Mathf.Min(SpellBook.instance.spells.Count, 4); i++)
        {
            SpellBook.instance.spells[i].isFirstAttack = true;
        }
        onCombat = true;
        Timer.StartTimer();
        Timer.gameObject.SetActive(true);
    }

    public void EnterBase()
    {
        ResetStats();
        Timer.StopTimer();
        Timer.gameObject.SetActive(false);
        animator.SetBool("IsWalking",false);
        onCombat = false;
        isMoving = false;
        isMovingTowardsEnemy = false;

    }

    public GameObject FindClosestEnemyNearPosition(Vector3 position)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(position, enemy.transform.position);
            if (distanceToEnemy <= 15f)
            {
                if (closestEnemy == null || distanceToEnemy < Vector3.Distance(position, closestEnemy.transform.position))
                {
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }
}