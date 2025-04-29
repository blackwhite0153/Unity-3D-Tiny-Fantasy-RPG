using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GolemController : EnemyController
{
    private Golem _golem;

    private Coroutine _coGolemDespawn;
    private WaitForSeconds _despawnTimer;

    private Animator _animator;         // ���� �ִϸ����� ������Ʈ
    private Rigidbody _rigidbody;       // ���� ����(Rigidbody) ������Ʈ
    private FieldOfView _fieldOfView;   // �þ� ������ ����ϴ� FieldOfView ������Ʈ
    private NavMeshAgent _navMeshAgent;
    private CapsuleCollider _capsuleCollider;

    private CapsuleCollider _golemLeftAttackArea;
    private CapsuleCollider _golemRightAttackArea;

    private Vector3 _spawnPos;  // ���� ���� ��ǥ
    private Vector3 _roamingPosition;   // ���� �ൿ �̵� ��ǥ
    private float _roamingDistance;     // ���� �ൿ ���� �Ÿ�

    private float _attackRange; // ���� ���� ���� (���� : ����Ƽ �Ÿ� ����)

    private bool _isHit;

    private int _playerAtk;

    // ���� �ӽ� (State Machine) ��ü, ���� AI ���¸� ����
    protected StateMachine<GolemController> _stateMachine;

    // ���� ������ Ÿ�� (FieldOfView���� ���� ����� ���� ������)
    public Transform Target => _fieldOfView?.NearestTarget;
    // �ִϸ�����
    public Animator Animator => _animator;
    // ���� �̵� ��ǥ ���� �Ÿ� Ȯ���ϴ� ������Ƽ
    public float RoamingDistance => _roamingDistance;

    // ���� �����̴��� ���θ� ��Ÿ���� ������Ƽ
    public bool IsMove
    {
        get { return _animator.GetBool(Define.IsMove); }
        set { _animator.SetBool(Define.IsMove, value); }
    }

    // ���� ���� ������ ���θ� ��Ÿ���� ������Ƽ
    public bool IsAttack
    {
        get { return _animator.GetBool(Define.IsAttack); }
        set { _animator.SetBool(Define.IsAttack, value); }
    }

    // ���� ��� �������� ���θ� ��Ÿ���� ������Ƽ
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

    // ���� ü���� Ȯ���ϴ� ������Ƽ
    public int GolemHp
    {
        get { return _golem.GolemState.GolemHp; }
        set { _golem.GolemState.GolemHp = value; }
    }

    // ���� ���ݷ��� Ȯ���ϴ� ������Ƽ
    public int GolemAtk
    {
        get { return _golem.GolemState.GolemAtk; }
    }

    // ���� ������ Ȯ���ϴ� ������Ƽ
    public int GolemDef
    {
        get { return _golem.GolemState.GolemDef; }
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
        base.Initialize();

        _spawnPos = transform.position; // ���� ���� ��ǥ ����
    }

    // ������Ʈ Ȱ��ȭ �� ȣ��
    private void OnEnable()
    {
        // Ȱ��ȭ�� �� �ɷ�ġ �ʱ�ȭ
        Setting();
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
        if (_golem == null) _golem = new Golem();

        _golem.ResetStats();

        _coGolemDespawn = null;
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
            if (collider.transform.name == Define.Hand_L_Object) _golemLeftAttackArea = collider;
            if (collider.transform.name == Define.Hand_R_Object) _golemRightAttackArea = collider;
        }

        _rigidbody.mass = 2.0f;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = true;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        _navMeshAgent.speed = _golem.GolemState.GolemSpeed;

        _capsuleCollider.center = new Vector3(0.0f, 1.95f, 0.0f);
        _capsuleCollider.radius = 1.2f;
        _capsuleCollider.height = 3.9f;
        _capsuleCollider.direction = 1;

        // ���� �ӽ� �ʱ�ȭ �� ���� �߰�
        _stateMachine = new StateMachine<GolemController>(this, new GolemIdleState());   // �⺻ ���¸� IdleState�� ����
        _stateMachine.AddState(new GolemPursuitState());    // ���� ���� �߰�
        _stateMachine.AddState(new GolemAttackState());     // ���� ���� �߰�
        _stateMachine.AddState(new GolemRoamState());       // ���� �ൿ ���� �߰�
        _stateMachine.AddState(new GolemDieState());        // ��� ���� �߰�

        _attackRange = 2.8f;

        IsDie = false;  // ���� ���·� ����
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

    // �̵� ��ġ �缳��
    public void CoordinateSetting()
    {
        Vector3 newRoamingPos = new Vector3(UnityEngine.Random.Range(_spawnPos.x - 3.5f, _spawnPos.x + 3.5f),
                                            transform.position.y,
                                            UnityEngine.Random.Range(_spawnPos.z - 3.5f, _spawnPos.z + 3.5f));

        _navMeshAgent.SetDestination(newRoamingPos);
    }

    // �� ���¿� �´� �ݶ��̴��� Ȱ��ȭ / ��Ȱ��ȭ�ϵ��� ����
    public void SetAttackColliders(bool isLeftActive, bool isRightActive)
    {
        _golemLeftAttackArea.enabled = isLeftActive;
        _golemRightAttackArea.enabled = isRightActive;
    }

    private void Hit()
    {
        _animator.SetTrigger(Define.HitTrigger);
    }

    public void Die()
    {
        if (_coGolemDespawn == null)
        {
            PlayerController playerController = FindAnyObjectByType<PlayerController>();

            if (playerController != null)
            {
                playerController.GolemKilled();
            }
            _coGolemDespawn = StartCoroutine(CoGolemDespawn());
        }
    }

    private IEnumerator CoGolemDespawn()
    {
        // ��� �ִϸ��̼� ����
        _animator.SetTrigger(Define.DieTrigger);

        yield return _despawnTimer;

        ObjectManager.Instance.Despawn(this, _spawnPos, _coGolemDespawn);
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

                    float defense = GolemDef;  // �� ���� ����
                    float damageReductionRate = defense / (defense + 100.0f);   // ���¿� ���� ������ ����
                    float finalDamage = damage * (1.0f - damageReductionRate);

                    GolemHp -= Mathf.RoundToInt(finalDamage);

                    if (GolemHp > 0) Hit();

                    // ���� Ÿ���� ���� ���¿��ٸ�
                    if (!Target)
                    {
                        // ���� �÷��̾ �ٶ󺸵��� ȸ��
                        this.gameObject.transform.LookAt(playerController.transform.position);
                    }

                    // ���� �ð� �� �ٽ� ���� �����ϵ��� ����
                    StartCoroutine(CoResetHitCooldown());
                }
            }
        }
    }
}