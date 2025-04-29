using System.Collections;
using UnityEngine;

public class PlayerController : BaseController
{
    private Player _player;

    private Animator _animator;
    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;

    private CapsuleCollider _playerAttackArea;
    private GameObject _camera;

    private GameObject _basicSlashBlue;
    private GameObject _multipleSlash;

    private GameObject _warpGateA;
    private GameObject _warpGateB;

    private GameObject _exitGate;

    private Vector3 _moveDirection;    // 플레이어가 바라보는 방향

    private int _attackCombo = 0;          // 현재 콤보 상태
    private float _attackCooldown = 0.4f;  // 공격 입력 불가 시간
    private float _attackTimer = 0.0f;     // 공격 대기 시간
    private float _comboDuration = 0.8f;   // 콤보 유지 시간
    private float _comboTimer = 0.0f;      // 콤보 시간 타이머

    private float _slashDelay = 2.1f;

    private float _cameraRotationSpeed;
    private float _moveSpeed;
    private float _runSpeed;

    private bool _isAttack;     // 공격 여부
    private bool _isDefend;     // 방어 여부
    private bool _isHit;        // 피격 여부
    private bool _isDie;        // 사망 여부

    private int _golemAtk;
    private int _bossAtk;

    // 플레이어의 레벨을 확인하는 프로퍼티
    public int PlayerLevel
    {
        get { return _player.PlayerState.PlayerLevel; }
        set { _player.PlayerState.PlayerLevel = value; }
    }

    // 플레이어의 최대 체력을 확인하는 프로퍼티
    public int PlayerMaxHp
    {
        get { return _player.PlayerState.PlayerMaxHp; }
        set { _player.PlayerState.PlayerMaxHp = value; }
    }

    // 플레이어의 현재 체력을 확인하는 프로퍼티
    public int PlayerCurrentHp
    {
        get { return _player.PlayerState.PlayerCurrentHp; }
        set { _player.PlayerState.PlayerCurrentHp = value; }
    }

    // 플레이어의 최대 경험치를 확인하는 프로퍼티
    public int PlayerMaxExp
    {
        get { return _player.PlayerState.PlayerMaxExp; }
        set { _player.PlayerState.PlayerMaxExp = value; }
    }

    // 플레이어의 현재 경험치를 확인하는 프로퍼티
    public int PlayerCurrentExp
    {
        get { return _player.PlayerState.PlayerCurrentExp; }
        set { _player.PlayerState.PlayerCurrentExp = value; }
    }

    // 플레이어의 공격력을 받아오는 프로퍼티
    public int PlayerAtk
    {
        get { return _player.PlayerState.PlayerAtk; }
    }

    // 플레이어의 공격력을 받아오는 프로퍼티
    public int PlayerDef
    {
        get { return _player.PlayerState.PlayerDef; }
    }

    // 플레이어의 공격 여부를 설정하는 프로퍼티
    public bool IsAttack
    {
        set { _isAttack = value; }
    }

    // 플레이어의 사망 여부를 반환하는 프로퍼티
    public bool IsDie => _isDie;

    protected override void Initialize() { }

    private void OnEnable()
    {
        Setting();
    }

    private void FixedUpdate()
    {
        if (!_isDie)
        {
            if (!_isAttack && !_isDefend && !_isHit) Move();
        }
    }

    private void Update()
    {
        if (!_isDie)
        {
            if (!_isHit)
            {
                Attack();
                Defend();
            }
        }
    }

    private void Setting()
    {
        _player = new Player();

        GameObject playerAttackAreaObject = GameObject.FindGameObjectWithTag(Define.PlayerAttackAreaTag);

        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();

        foreach (var particale in particles)
        {
            // 찾은 ParticleSystem 컴포넌트 중 오브젝트 이름이 Attack Slash 1, Attack Slash 2인 경우만 사용
            if (particale.transform.name == Define.Attack_Slash_One_Object) _basicSlashBlue = particale.gameObject;
            if (particale.transform.name == Define.Attack_Slash_Two_Object) _multipleSlash = particale.gameObject;
        }

        _basicSlashBlue.SetActive(false);
        _multipleSlash.SetActive(false);

        _warpGateA = GameObject.FindGameObjectWithTag(Define.WarpGateATag);
        _warpGateB = GameObject.FindGameObjectWithTag(Define.WarpGateBTag);

        if (playerAttackAreaObject != null) _playerAttackArea = playerAttackAreaObject.GetComponent<CapsuleCollider>();

        // 최초 실행 시 공격 영역 비활성화
        if (_playerAttackArea != null) _playerAttackArea.enabled = false;

        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        _camera = GameObject.Find("GamePlaying/Camera").transform.Find("Main Camera").gameObject;

        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        _capsuleCollider.center = new Vector3(0.0f, 0.65f, 0.2f);
        _capsuleCollider.radius = 0.4f;
        _capsuleCollider.height = 1.3f;
        _capsuleCollider.direction = 1; // Y-Axis

        // Player Rotation 세팅
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, -50.0f, 0.0f));

        _cameraRotationSpeed = 20.0f;
        _moveSpeed = 3.0f;
        _runSpeed = 5.0f;

        _isAttack = false;
        _isDefend = false;
        _isHit = false;
        _isDie = false;
    }

    // 이동 처리 (카메라가 바라보는 방향을 기준으로 이동)
    private void Move()
    {
        if (Input.GetButton(Define.Horizontal) || Input.GetButton(Define.Vertical))
        {
            float horizontal = Input.GetAxisRaw(Define.Horizontal);
            float vertical = Input.GetAxisRaw(Define.Vertical);

            Vector3 movement = new Vector3(horizontal, vertical, 0.0f);

            // 카메라 암의 정면, 측면 방향에 대한 방향을 저장
            Vector3 lookForward = new Vector3(_camera.transform.forward.x, 0.0f, _camera.transform.forward.z).normalized;
            Vector3 lookRight = new Vector3(_camera.transform.right.x, 0.0f, _camera.transform.right.z).normalized;

            // 벡터 연산에 의해 카메라 암의 정면 방향 * Y축 이동 입력 + 카메라 암의 측면 방향 * X축 이동 입력을 더하여 방향을 결정
            _moveDirection = lookForward * movement.y + lookRight * movement.x;

            if (_moveDirection != Vector3.zero)
            {
                // 이동 방향과 별개로 플레이어의 모습을 담당하는 PlayerModel의 방향을 움직이는 방향으로 회전시키기 위해 바라보는 방향을 구한다.
                Quaternion viewRotation = Quaternion.LookRotation(_moveDirection.normalized);
                // Lerp를 사용한 부드러운 방향 전환
                transform.rotation = Quaternion.Lerp(transform.rotation, viewRotation, _cameraRotationSpeed * Time.deltaTime);
            }

            if (!Input.GetKey(KeyCode.LeftShift))
            {
                _animator.SetBool(Define.IsMove, true);
                _animator.SetBool(Define.IsRun, false);

                gameObject.transform.position += (_moveDirection).normalized * _moveSpeed * Time.deltaTime;
            }
            else
            {
                _animator.SetBool(Define.IsMove, false);
                _animator.SetBool(Define.IsRun, true);

                gameObject.transform.position += (_moveDirection).normalized * _runSpeed * Time.deltaTime;
            }
        }
        else
        {
            _animator.SetBool(Define.IsMove, false);
            _animator.SetBool(Define.IsRun, false);
        }
    }

    // 공격 처리
    private void Attack()
    {
        if (_isAttack) return;

        if (_attackTimer > 0.0f)
        {
            _attackTimer -= Time.deltaTime;
            return; // 0.4초 동안 입력을 막음
        }

        if (_comboTimer > 0.0f)
        {
            _comboTimer -= Time.deltaTime;
        }
        else
        {
            _attackCombo = 0; // 콤보 초기화
        }

        if (Input.GetMouseButtonDown(0) && _attackTimer <= 0.0f)
        {
            if (_attackCombo == 0) // 첫 번째 공격
            {
                _attackCombo = 1;
                _animator.SetTrigger(Define.Attack1Trigger);

                _basicSlashBlue.SetActive(true);
                Invoke("EnableAttackOneEffect", _slashDelay);
            }
            else if (_attackCombo == 1 && _comboTimer > 0.0f) // 두 번째 공격
            {
                _attackCombo = 0;
                _animator.SetTrigger(Define.Attack2Trigger);

                _multipleSlash.SetActive(true);
                Invoke("EnableAttackTwoEffect", _slashDelay);
            }
            _isAttack = true;
            _attackTimer = _attackCooldown; // 공격 후 0.4초간 입력 차단
            _comboTimer = _comboDuration;   // 콤보 유지 시간 초기화
        }
    }

    // 공격 이펙트 비활성화
    private void EnableAttackOneEffect()
    {
        if (_basicSlashBlue.activeSelf) _basicSlashBlue.SetActive(false);
    }

    private void EnableAttackTwoEffect()
    {
        if (_multipleSlash.activeSelf) _multipleSlash.SetActive(false);
    }

    // 애니메이션 이벤트에서 호출할 함수 (공격)
    public void EnableAttackCollider(bool value)
    {
        _playerAttackArea.enabled = value;
    }

    // 방어 처리
    private void Defend()
    {
        _isDefend = Input.GetMouseButton(1);

        if (_isDefend)
        {
            if (_animator.GetBool(Define.IsMove)) _animator.SetBool(Define.IsMove, false);
            if (_animator.GetBool(Define.IsRun)) _animator.SetBool(Define.IsRun, false);
        }
        _animator.SetBool(Define.IsDefend, _isDefend);
    }

    private IEnumerator CoResetHitCooldown()
    {
        if (_isAttack)
        {
            _isAttack = false;
            EnableAttackCollider(_isAttack);
        }

        _animator.SetTrigger(Define.HitTrigger);

        yield return new WaitForSeconds(0.5f); // 0.5초 후 다시 감지 가능

        _isHit = IsAnimationFinished(Define.Hit);

        StopCoroutine(CoResetHitCooldown());
    }

    private void Die()
    {
        _isDie = true;
        _animator.SetTrigger(Define.DieTrigger);
    }

    // 능력치 증가
    public void GolemKilled()
    {
        _player.PlayerState.PlayerCurrentExp += 20;

        if (_player.PlayerState.PlayerCurrentExp >= _player.PlayerState.PlayerMaxExp)
        {
            int extraExp = _player.PlayerState.PlayerCurrentExp - _player.PlayerState.PlayerMaxExp;

            _player.PlayerState.PlayerLevel++;

            _player.PlayerState.PlayerMaxHp += 15;
            _player.PlayerState.PlayerCurrentHp = _player.PlayerState.PlayerMaxHp;

            if (extraExp > 0) _player.PlayerState.PlayerCurrentExp = extraExp;
            else _player.PlayerState.PlayerCurrentExp = 0;
            _player.PlayerState.PlayerMaxExp += 50;

            _player.PlayerState.PlayerAtk += 5;
            _player.PlayerState.PlayerDef += 3;
        }
    }

    // 현재 애니메이션이 끝났는지 확인 (애니메이션 이름)
    public bool IsAnimationFinished(string animation)
    {
        AnimatorStateInfo stateInfo;

        if (!_animator) return false;   // _animator 없으면 false 반환

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(animation))
        {
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            return stateInfo.normalizedTime >= 0.95f;    // 애니메이션이 끝났다면 true
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 사망 or 피격 상태면 무시
        if (_isDie || _isHit) return;

        // 골렘 혹은 보스의 공격 태그일 경우만 처리
        if (other.CompareTag(Define.GolemLeftAttackAreaTag) || other.CompareTag(Define.GolemRightAttackAreaTag))
        {
            int damage = 0;

            // 골렘인지 확인
            GolemController golemController = other.GetComponentInParent<GolemController>();

            if (golemController != null)
            {
                damage = golemController.GolemAtk;
            }
            else
            {
                // 보스인지 확인
                BossController bossController = other.GetComponentInParent<BossController>();

                if (bossController != null)
                {
                    damage = bossController.BossAtk;
                }
            }

            // 데미지를 받은 경우 처리
            if (damage > 0)
            {
                if (!_isDefend) _isHit = true;

                float defense = PlayerDef;  // 플레이어 방어력 변수
                float damageReductionRate = defense / (defense + 100.0f);   // 방어력에 따른 데미지 감소
                float finalDamage = damage * (1.0f - damageReductionRate);

                if (_isDefend)
                {
                    finalDamage *= 0.7f; // 추가로 30% 감소
                }

                PlayerCurrentHp -= Mathf.RoundToInt(finalDamage);

                // 피격 및 사망 애니메이션
                if (PlayerCurrentHp > 0)
                {
                    if (!_isDefend)
                        StartCoroutine(CoResetHitCooldown());
                }
                else
                {
                    Die();
                }
            }
        }
    }
}