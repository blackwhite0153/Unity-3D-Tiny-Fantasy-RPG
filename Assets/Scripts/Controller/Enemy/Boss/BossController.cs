using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossController : EnemyController
{
    private Boss _boss;

    private Coroutine _coBossDespawn;
    private WaitForSeconds _despawnTimer;

    private Animator _animator;         // ������ �ִϸ����� ������Ʈ
    private Rigidbody _rigidbody;       // ������ ����(Rigidbody) ������Ʈ
    private FieldOfView _fieldOfView;   // �þ� ������ ����ϴ� FieldOfView ������Ʈ
    private NavMeshAgent _navMeshAgent;
    private CapsuleCollider _capsuleCollider;

    private CapsuleCollider _bossLeftAttackArea;
    private CapsuleCollider _bossRightAttackArea;

    private Vector3 _spawnPos;  // ���� ���� ��ǥ
    private float _attackRange; // ������ ���� ���� (���� : ����Ƽ �Ÿ� ����)

    private bool _isHit;

    private int _playerAtk;

    // ���� �ӽ� (State Machine) ��ü, ������ AI ���¸� ����
    protected StateMachine<BossController> _stateMachine;

    // ���� ������ Ÿ�� (FieldOfView���� ���� ����� ���� ������)
    public Transform Target => _fieldOfView?.NearestTarget;
    // �ִϸ�����
    public Animator Animator => _animator;

    // ������ ���� ������ ��Ÿ���� ������Ƽ
    public Vector3 SpawnPos
    {
        get { return _spawnPos; }
    }

    // ������ �����̴��� ���θ� ��Ÿ���� ������Ƽ
    public bool IsMove
    {
        get { return _animator.GetBool(Define.IsMove); }
        set { _animator.SetBool(Define.IsMove, value); }
    }

    // ������ ���� ������ ���θ� ��Ÿ���� ������Ƽ
    public bool IsAttack
    {
        get { return _animator.GetBool(Define.IsAttack); }
        set { _animator.SetBool(Define.IsAttack, value); }
    }

    // ������ ��� �������� ���θ� ��Ÿ���� ������Ƽ
    public bool IsDie
    {
        get { return _animator.GetBool(Define.IsDie); }
        set { _animator.SetBool(Define.IsDie, value); }
    }

    // ���� ���� ����(�޺�) Ƚ���� �����ϴ� ������Ƽ
    public int ComboCount
    {
        get { return _animator.GetInteger(Define.ComboCount); }
        set { _animator.SetInteger(Define.ComboCount, value); }
    }

    // ������ ü���� Ȯ���ϴ� ������Ƽ
    public int BossHp
    {
        get { return _boss.BossState.BossHp; }
        set { _boss.BossState.BossHp = value; }
    }

    // ������ ���ݷ��� Ȯ���ϴ� ������Ƽ
    public int BossAtk
    {
        get { return _boss.BossState.BossAtk; }
    }

    // ������ ������ Ȯ���ϴ� ������Ƽ
    public int BossDef
    {
        get { return _boss.BossState.BossDef; }
    }

    // ������ ������ �������� Ȯ���ϴ� ������Ƽ (���� ���� ���� Ÿ���� �ִ��� �˻�)
    public bool IsAvailableAttack
    {
        get
        {
            if (!Target) return false;  // Ÿ���� ������ ���� �Ұ���
            float distance = (Target.position - transform.position).sqrMagnitude;   // ���� ��ġ�� Ÿ���� �Ÿ� (����)
            return distance <= (_attackRange * _attackRange);   // ���� ���� ���� ������ true ��ȯ
        }
    }

    // ���� �ִϸ��̼��� �������� Ȯ���ϴ� ������Ƽ
    public bool IsAnimationFinished
    {
        get
        {
            if (!_animator) return false;   // _animator ������ false ��ȯ
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.normalizedTime >= 0.95f;    // �ִϸ��̼��� �����ٸ� true
        }
    }

    protected override void Initialize()
    {
        Setting();
    }

    // ������Ʈ Ȱ��ȭ �� ȣ��
    private void OnEnable()
    {
        // ��Ȱ��ȭ ������ ��쿡�� ����
        if (!gameObject.activeSelf)
        {
            // Ȱ��ȭ�� �� �ɷ�ġ �ʱ�ȭ
            _boss.ResetStats();
        }
    }

    private void FixedUpdate()
    {
        // ���� ������Ʈ���� ���� �ӽ� ������Ʈ ����
        _stateMachine.OnFixedUpdate(Time.fixedDeltaTime);
    }

    private void Update()
    {
        // �� ������ ���� �ӽ� ������Ʈ ����
        _stateMachine.OnUpdate(Time.deltaTime);
    }

    private void Setting()
    {
        _boss = new Boss();

        _despawnTimer = new WaitForSeconds(3.0f);

        // ������Ʈ ���� ��������
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _fieldOfView = GetComponent<FieldOfView>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        // ��� �ڽ� ������Ʈ���� CapsuleCollider ������Ʈ ã��
        CapsuleCollider[] colliders = GetComponentsInChildren<CapsuleCollider>();

        foreach (CapsuleCollider collider in colliders)
        {
            // ã�� CapsuleCollider ������Ʈ �� ������Ʈ �̸��� Hand_L, Hand_R�� ��츸 ���
            if (collider.transform.name == Define.Hand_L_Object) _bossLeftAttackArea = collider;
            if (collider.transform.name == Define.Hand_R_Object) _bossRightAttackArea = collider;
        }

        _rigidbody.mass = 3.0f;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = true;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        _navMeshAgent.speed = _boss.BossState.BossSpeed;

        _capsuleCollider.center = new Vector3(0.0f, 2.11f, 0.2f);
        _capsuleCollider.radius = 1.47f;
        _capsuleCollider.height = 4.21f;
        _capsuleCollider.direction = 1;

        // ���� �ӽ� �ʱ�ȭ �� ���� �߰�
        _stateMachine = new StateMachine<BossController>(this, new BossIdleState());   // �⺻ ���¸� IdleState�� ����
        _stateMachine.AddState(new BossPursuitState());     // ���� ���� �߰�
        _stateMachine.AddState(new BossAttackState());      // ���� ���� �߰�
        _stateMachine.AddState(new BossReturnState());      // ���� ���� �߰�
        _stateMachine.AddState(new BossDieState());         // ��� ���� �߰�

        // Boss Rotation ����
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, 130.0f, 0.0f));

        _spawnPos = transform.position; // ���� ���� ��ǥ ����

        _attackRange = 2.8f;

        IsDie = false;
    }

    // Ÿ�� �������� ȸ��
    public void RotateToTarget()
    {
        // Ÿ���� ������ �������� ����
        if (!Target) return;

        // Ÿ�ٰ��� ���� ���� ���
        Vector3 direction = (Target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0.0f, direction.z));

        // ���� ȸ������ ��ǥ ȸ�� �������� �ε巴�� ����
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
    }

    // Ÿ�ٿ��� �̵�
    public void MoveToTarget()
    {
        // Ÿ���� ������ �������� ����
        if (!Target) return;

        Vector3 targetPos = Target.position;

        _navMeshAgent.SetDestination(targetPos);
    }

    // ���� �������� ����
    public void ReturnMove()
    {
        // ��ǥ ��ġ�� y�� ���� ��ġ�� y�� ���缭 ���� ȸ���� �ϵ��� ����
        Vector3 targetPos = new Vector3(_spawnPos.x, transform.position.y, _spawnPos.z);
        // ������ ���� ��ǥ�� �ٶ󺸵��� ȸ��
        this.transform.LookAt(targetPos);

        // NavMeshAgent�� ����Ͽ� ���� �������� ���ϵ��� ����
        _navMeshAgent.SetDestination(targetPos);
        _animator.SetBool(Define.IsMove, true);
    }

    // ���� ���¿� �´� �ݶ��̴��� Ȱ��ȭ / ��Ȱ��ȭ�ϵ��� ����
    public void SetAttackColliders(bool isLeftActive, bool isRightActive)
    {
        _bossLeftAttackArea.enabled = isLeftActive;
        _bossRightAttackArea.enabled = isRightActive;
    }

    private void Hit()
    {
        _animator.SetTrigger(Define.HitTrigger);
    }

    public void Die()
    {
        if (_coBossDespawn == null)
        {
            _coBossDespawn = StartCoroutine(CoGolemDespawn());
        }
    }

    private IEnumerator CoGolemDespawn()
    {
        _animator.SetTrigger(Define.DieTrigger);

        yield return _despawnTimer;

        ObjectManager.Instance.MonsterDestroy(this);
    }

    private IEnumerator CoResetHitCooldown()
    {
        yield return new WaitForSeconds(0.4f); // 0.4�� �� �ٽ� ���� ����

        _isHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // �̹� �¾����� ó������ ����
        if (_isHit) return;

        // ���� ������ ��쿡�� Trigger �۵�
        if (!IsDie)
        {
            if (other.CompareTag(Define.PlayerAttackAreaTag))
            {
                int damage = 0;

                PlayerController playerController = other.GetComponentInParent<PlayerController>();

                if (playerController != null)
                {
                    _isHit = true;  // ���� ���� �� �÷��� Ȱ��ȭ

                    damage = playerController.PlayerAtk;

                    float defense = BossDef;  // �� ���� ����
                    float damageReductionRate = defense / (defense + 100.0f);   // ���¿� ���� ������ ����
                    float finalDamage = damage * (1.0f - damageReductionRate);

                    BossHp -= Mathf.RoundToInt(finalDamage);

                    if (BossHp > 0) Hit();

                    // ���� Ÿ���� ���� ���¿��ٸ�
                    if (!Target)
                    {
                        // ������ �÷��̾ �ٶ󺸵��� ȸ��
                        this.gameObject.transform.LookAt(playerController.transform.position);
                    }

                    // ���� �ð� �� �ٽ� ���� �����ϵ��� ����
                    StartCoroutine(CoResetHitCooldown());
                }
            }
        }
    }
}