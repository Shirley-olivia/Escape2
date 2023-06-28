using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI: MonoBehaviour
{

    public enum FSMState
    {
        Wander,     //游荡状态
        Seek,       //搜索状态
        Chase,      //追踪状态
        Attack,     //攻击状态
        Dead,       //死亡状态
        None,
    }

    public FSMState curState;
    private FSMState preState = FSMState.None;

    public float wanderRadis = 10f;
    public float attackRange = 1f;
    public float seekRange = 10f;
    [Space]
    public float wanderSpeed = 1f;
    public float chaseSpeed = 3f;
    public float attackInterval = 0.25f;
    public float attackScope = 10f;
    public int attackDamage = 2;
    [Space]
    public AudioSource footstep;
    public float audioInterval = 1.2f;

    public GameObject SmokeEffect;

    public AudioSource boomAudio;
    public AudioSource attackAudio;

    private EnemySensor enemySensor;
    private EnemyHealth enemyHealth;
    private NavMeshAgent agent;
    private float stopTime;
    private GameObject playerLocked;
    private float attackTimer;
    private Vector3 prePosition;
    private Animator animator;
    private EmotionController emotionController;
    private int currentAttack = 0;

    private float audioTimer = 0f;
    private bool deadDone = false;
    

    private void OnEnable()
    {
        enemySensor = GetComponent<EnemySensor>();
        enemyHealth = GetComponent<EnemyHealth>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        emotionController = GetComponent<EmotionController>();
        agent.enabled = false;
        deadDone = false;
        Born();
    }

    private void Born()
    {
        curState = FSMState.Wander;
        agent.enabled = true;
        agent.ResetPath();
    }

    private void FixedUpdate()
    {
        FSMUpdate();
        AudioUpdate();
        attackTimer += Time.deltaTime;
        audioTimer += Time.deltaTime;
    }

    private bool AgentDone()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    private Vector3 getRandomCirclePoint()
    {
        float ranAngle = (float)Random.Range(0, 2 * Mathf.PI);
        float ranRadis = (float)Random.Range(2, wanderRadis);
        float x = transform.position.x + ranRadis * Mathf.Cos(ranAngle);
        float z = transform.position.z + ranRadis * Mathf.Sin(ranAngle);
        Vector3 tempPos =  new Vector3(x, transform.position.y, z);
        tempPos = NavMesh.SamplePosition(tempPos, out NavMeshHit hit, 4 , 1) ? hit.position : transform.position;
        return tempPos;
    }

    private void setMaxSpeed(float maxSpeed)
    {
        if (agent.desiredVelocity.magnitude > maxSpeed)
        {
            agent.velocity = agent.desiredVelocity.normalized * maxSpeed;
        }
        else
        {
            agent.velocity = agent.desiredVelocity;
        }

        //animator.SetFloat("Blend", agent.velocity.magnitude, 0.15f, Time.deltaTime);
        animator.SetFloat("Blend", agent.velocity.magnitude);
    }

    private void WanderUpdate()
    {
        //发现玩家，->追踪
        playerLocked = enemySensor.getLockedPlayer();
        if (playerLocked != null)
        {
            curState = FSMState.Chase;
            agent.ResetPath();
            return;
        }

        //收到攻击，->搜索
        if (enemyHealth.getDamaged())
        {

            curState = FSMState.Seek;
            agent.ResetPath();
            return;
        }

        GameObject bulletLocked = enemySensor.getLockedBullet();
        if (bulletLocked != null)
        {
            agent.SetDestination(bulletLocked.transform.position);
        }

        //闲逛到随机地点，继续闲逛
        if (AgentDone())
        {
            agent.SetDestination(getRandomCirclePoint());
        }
        setMaxSpeed(wanderSpeed);
        UpdateEmotion();


        //卡住了，向反方向离开
        if (stopTime > 0.5f)
        {
            Vector3 destination = getRandomCirclePoint();
            agent.SetDestination(destination);
        }
        calStopTime();
    }

    private void SeekUpdate()
    {
        //发现玩家，->追踪
        playerLocked = enemySensor.getLockedPlayer();
        if (playerLocked != null)
        {
            curState = FSMState.Chase;
            agent.ResetPath();
            return;
        }

        //受到攻击，向攻击方向搜索
        if (enemyHealth.getDamaged())
        {
            Vector3 seekDirection = enemyHealth.getDamagedDirection();
            Vector3 seekDestination = transform.position + seekDirection * seekRange;
            agent.SetDestination(seekDestination);
            enemyHealth.setDamaged(false);
        }

        setMaxSpeed(chaseSpeed);
        UpdateEmotion();

        //到达最大搜索距离 / 长时间卡住不动，->闲逛
        if (AgentDone() || stopTime > 0.5f)
        {
            curState = FSMState.Wander;
            agent.ResetPath();
            return;
        }


        calStopTime();
    }

    private void ChaseUpdate()
    {
        //发现玩家，实时更新寻路目标
        playerLocked = enemySensor.getLockedPlayer();

        if (playerLocked == null)
        {
            //到达追踪位置（未发现玩家）/卡住，->闲逛
            if (AgentDone() || stopTime > 0.5f)
            {
                curState = FSMState.Wander;
                agent.ResetPath();
            }
            return;
        }
        
        agent.SetDestination(playerLocked.transform.position);

        //玩家出现在攻击范围内，->攻击
        if (Vector3.Distance(transform.position, playerLocked.transform.position) < attackRange)
        {
            curState = FSMState.Attack;
            agent.ResetPath();
            return;
        }
        setMaxSpeed(chaseSpeed);
        UpdateEmotion();

    }

    private void AttackUpdate()
    {
        //丢失玩家，->闲逛
        playerLocked = enemySensor.getLockedPlayer();
        if (playerLocked == null)
        {
            curState = FSMState.Wander;
            agent.ResetPath();
            return;
        }

        //超出攻击范围，->追踪
        if (Vector3.Distance(transform.position, playerLocked.transform.position) > attackRange)
        {
            curState = FSMState.Chase;
            agent.ResetPath();
            return;
        }

        Vector3 direction = playerLocked.transform.position - transform.position;
        float angle = Mathf.Abs(Vector3.Angle(direction, transform.forward));

        if (angle < attackScope/ 2)
        { 
            if (attackTimer >= attackInterval)
            {
                currentAttack++;
                if (currentAttack > 3)
                    currentAttack = 1;
                if (attackTimer > attackInterval * 4)
                {
                    currentAttack = 1;
                }
                //Debug.Log(currentAttack);
                animator.SetTrigger("Attack" + currentAttack);
                attackAudio.Play();
                playerLocked.GetComponent<PlayerController>().ChangeHealth(-attackDamage);
                attackTimer = 0f;
            }
        }
        else
        {
            transform.LookAt(playerLocked.transform);
        }
        //agent.SetDestination(playerLocked.transform.position);
        setMaxSpeed(chaseSpeed);
    }

    private void DeadthUpdate()
    {
        if (!deadDone)
        {
            deadDone = true;
            agent.ResetPath();
            emotionController.setEmotion("dead");
            footstep.Stop();
        }
        //transform.gameObject.SetActive(false);
    }

    private void UpdateEmotion()
    {
        //if (curState != preState)
        //{
            if (curState == FSMState.Wander)
            {
                emotionController.setEmotion("normal");
            }
            else
            {
                emotionController.setEmotion("angry");
            }
        //}
    }


    private void FSMUpdate()
    {
        FSMState tempState = curState;
        switch (curState)
        {
            case FSMState.Wander:
                WanderUpdate();
                break;
            case FSMState.Seek:
                SeekUpdate();
                break;
            case FSMState.Chase:
                ChaseUpdate();
                break;
            case FSMState.Attack:
                AttackUpdate();
                break;
            case FSMState.Dead:
                DeadthUpdate();
                break;
        }
        if (curState != FSMState.Dead && !enemyHealth.isAlive())
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            Collider collider = GetComponent<Collider>();
            collider.enabled = false;
            curState = FSMState.Dead;
        }
        preState = tempState;
    }

    
    private void calStopTime()
    {
        Vector3 diff = prePosition - transform.position;
        //Debug.Log(diff.magnitude);
        if (diff.magnitude < 0.2f)
        {
            stopTime += Time.deltaTime;
        }
        else
        {
            stopTime = 0.0f;
            prePosition = transform.position;
        }
    }

    public void Smoke()
    {
        boomAudio.Play();
        SmokeEffect.SetActive(true);
    }

    private void AudioUpdate()
    {
        if (curState == FSMState.Wander)
        {
            if (audioTimer > audioInterval)
            {
                audioTimer = 0f;
                footstep.time = 0f;
                footstep.Play();
            }
        }
        else if (curState != FSMState.Dead)
        {
            if (audioTimer > audioInterval / 4)
            {
                audioTimer = 0f;
                footstep.time = 0f;
                footstep.Play();
            }
        }
    }
}
