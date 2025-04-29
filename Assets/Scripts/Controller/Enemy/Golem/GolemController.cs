using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GolemController : EnemyController
{
    private Golem _golem;

    private Coroutine _coGolemDespawn;
    private WaitForSeconds _despawnTimer;

    private Animator _animator;         // 골렘의 애니메이터 컴포넌트
    private Rigidbody _rigidbody;       // 골렘의 물리(Rigidbody) 컴포넌트
    private FieldOfView _fieldOfView;   // 시야 감지를 담당하는 FieldOfView 컴포넌트
    private NavMeshAgent _navMeshAgent;
    private CapsuleCollider _capsuleCollider;

    private CapsuleCollider _golemLeftAttackArea;
    private CapsuleCollider _golemRightAttackArea;

    private Vector3 _spawnPos;  // 최초 스폰 좌표
    private Vector3 _roamingPosition;   // 자유 행동 이동 좌표
    private float _roamingDistance;     // 자유 행동 남은 거리

    private float _attackRange; // 골렘의 공격 범위 (단위 : 유니티 거리 단위)

    private bool _isHit;

    private int _playerAtk;

    // 상태 머신 (State Machine) 객체, 골렘의 AI 상태를 관리
    protected StateMachine<GolemController> _stateMachine;

    // 현재 공격할 타겟 (FieldOfView에서 가장 가까운 적을 가져옴)
    public Transform Target => _fieldOfView?.NearestTarget;
    // 애니메이터
    public Animator Animator => _animator;
    // 자유 이동 좌표 남은 거리 확인하는 프로퍼티
    public float RoamingDistance => _roamingDistance;

    // 골렘이 움직이는지 여부를 나타내는 프로퍼티
    public bool IsMove
    {
        get { return _animator.GetBool(Define.IsMove); }
        set { _animator.SetBool(Define.IsMove, value); }
    }

    // 골렘이 공격 중인지 여부를 나타내는 프로퍼티
    public bool IsAttack
    {
        get { return _animator.GetBool(Define.IsAttack); }
        set { _animator.SetBool(Define.IsAttack, value); }
    }

    // 골렘이 사망 상태인지 여부를 나타내는 프로퍼티
    public bool IsDie
    {
        get { return _animator.GetBool(Define.IsDie); }
        set { _animator.SetBool(Define.IsDie, value); }
    }

    // 현재 연속 공격(콤보) 횟수를 관리하는 프로퍼티
    public int ComboCount
    {
        get { return _animator.GetInteger(Define.ComboCount); }
        set { _animator.SetInteger(Define.ComboCount, value); }
    }

    // 골렘의 체력을 확인하는 프로퍼티
    public int GolemHp
    {
        get { return _golem.GolemState.GolemHp; }
        set { _golem.GolemState.GolemHp = value; }
    }

    // 골렘의 공격력을 확인하는 프로퍼티
    public int GolemAtk
    {
        get { return _golem.GolemState.GolemAtk; }
    }

    // 골렘의 방어력을 확인하는 프로퍼티
    public int GolemDef
    {
        get { return _golem.GolemState.GolemDef; }
    }

    // 공격이 가능한 상태인지 확인하는 프로퍼티 (공격 범위 내에 타겟이 있는지 검사)
    public bool IsAvailableAttack
    {
        get
        {
            if (!Target) return false;  // 타겟이 없으면 공격 불가능
            float distance = (Target.position - transform.position).sqrMagnitude;   // 현재 위치와 타겟의 거리 (제곱)
            return distance <= (_attackRange * _attackRange);   // 공격 범위 내에 있으면 true 반환
        }
    }

    // 현재 애니메이션이 끝났는지 확인하는 프로퍼티
    public bool IsAnimationFinished
    {
        get
        {
            if (!_animator) return false;   // _animator 없으면 false 반환
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.normalizedTime >= 0.95f;    // 애니메이션이 끝났다면 true
        }
    }

    protected override void Initialize()
    {
        base.Initialize();

        _spawnPos = transform.position; // 최초 스폰 좌표 저장
    }

    // 오브젝트 활성화 시 호출
    private void OnEnable()
    {
        // 활성화될 때 능력치 초기화
        Setting();
    }

    private void FixedUpdate()
    {
        // 물리 업데이트에서 상태 머신 업데이트 수행
        _stateMachine.OnFixedUpdate(Time.fixedDeltaTime);
    }

    private void Update()
    {
        // 매 프레임 상태 머신 업데이트 수행
        _stateMachine.OnUpdate(Time.deltaTime);
    }

    private void Setting()
    {
        if (_golem == null) _golem = new Golem();

        _golem.ResetStats();

        _coGolemDespawn = null;
        _despawnTimer = new WaitForSeconds(3.0f);

        // 컴포넌트 참조 가져오기
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _fieldOfView = GetComponent<FieldOfView>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        // 모든 자식 오브젝트에서 CapsuleCollider 컴포넌트 찾기
        CapsuleCollider[] colliders = GetComponentsInChildren<CapsuleCollider>();

        foreach (CapsuleCollider collider in colliders)
        {
            // 찾은 CapsuleCollider 컴포넌트 중 오브젝트 이름이 Hand_L, Hand_R인 경우만 사용
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

        // 상태 머신 초기화 및 상태 추가
        _stateMachine = new StateMachine<GolemController>(this, new GolemIdleState());   // 기본 상태를 IdleState로 설정
        _stateMachine.AddState(new GolemPursuitState());    // 추적 상태 추가
        _stateMachine.AddState(new GolemAttackState());     // 공격 상태 추가
        _stateMachine.AddState(new GolemRoamState());       // 자유 행동 상태 추가
        _stateMachine.AddState(new GolemDieState());        // 사망 상태 추가

        _attackRange = 2.8f;

        IsDie = false;  // 생존 상태로 설정
    }

    // 타겟 방향으로 회전
    public void RotateToTarget()
    {
        // 타겟이 없으면 실행하지 않음
        if (!Target) return;

        // 타겟과의 방향 벡터 계산
        Vector3 direction = (Target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0.0f, direction.z));

        // 현재 회전에서 목표 회전 방향으로 부드럽게 보간
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
    }

    // 타겟에게 이동
    public void MoveToTarget()
    {
        // 타겟이 없으면 실행하지 않음
        if (!Target) return;

        Vector3 targetPos = Target.position;

        _navMeshAgent.SetDestination(targetPos);
    }

    // 이동 위치 재설정
    public void CoordinateSetting()
    {
        Vector3 newRoamingPos = new Vector3(UnityEngine.Random.Range(_spawnPos.x - 3.5f, _spawnPos.x + 3.5f),
                                            transform.position.y,
                                            UnityEngine.Random.Range(_spawnPos.z - 3.5f, _spawnPos.z + 3.5f));

        _navMeshAgent.SetDestination(newRoamingPos);
    }

    // 골렘 상태에 맞는 콜라이더를 활성화 / 비활성화하도록 설정
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
        // 사망 애니메이션 실행
        _animator.SetTrigger(Define.DieTrigger);

        yield return _despawnTimer;

        ObjectManager.Instance.Despawn(this, _spawnPos, _coGolemDespawn);
    }

    private IEnumerator CoResetHitCooldown()
    {
        yield return new WaitForSeconds(0.4f); // 0.4초 후 다시 감지 가능

        _isHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 이미 맞았으면 처리하지 않음
        if (_isHit) return;

        // 생존 상태일 경우에만 Trigger 작동
        if (!IsDie)
        {
            if (other.CompareTag(Define.PlayerAttackAreaTag))
            {
                int damage = 0;

                PlayerController playerController = other.GetComponentInParent<PlayerController>();

                if (playerController != null)
                {
                    _isHit = true;  // 공격 감지 시 플래그 활성화

                    damage = playerController.PlayerAtk;

                    float defense = GolemDef;  // 골렘 방어력 변수
                    float damageReductionRate = defense / (defense + 100.0f);   // 방어력에 따른 데미지 감소
                    float finalDamage = damage * (1.0f - damageReductionRate);

                    GolemHp -= Mathf.RoundToInt(finalDamage);

                    if (GolemHp > 0) Hit();

                    // 만약 타겟이 없는 상태였다면
                    if (!Target)
                    {
                        // 골렘이 플레이어를 바라보도록 회전
                        this.gameObject.transform.LookAt(playerController.transform.position);
                    }

                    // 일정 시간 후 다시 공격 가능하도록 설정
                    StartCoroutine(CoResetHitCooldown());
                }
            }
        }
    }
}