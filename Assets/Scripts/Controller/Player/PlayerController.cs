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

    private Vector3 _moveDirection;    // �÷��̾ �ٶ󺸴� ����

    private int _attackCombo = 0;          // ���� �޺� ����
    private float _attackCooldown = 0.4f;  // ���� �Է� �Ұ� �ð�
    private float _attackTimer = 0.0f;     // ���� ��� �ð�
    private float _comboDuration = 0.8f;   // �޺� ���� �ð�
    private float _comboTimer = 0.0f;      // �޺� �ð� Ÿ�̸�

    private float _slashDelay = 2.1f;

    private float _cameraRotationSpeed;
    private float _moveSpeed;
    private float _runSpeed;

    private bool _isAttack;     // ���� ����
    private bool _isDefend;     // ��� ����
    private bool _isHit;        // �ǰ� ����
    private bool _isDie;        // ��� ����

    private int _golemAtk;
    private int _bossAtk;

    // �÷��̾��� ������ Ȯ���ϴ� ������Ƽ
    public int PlayerLevel
    {
        get { return _player.PlayerState.PlayerLevel; }
        set { _player.PlayerState.PlayerLevel = value; }
    }

    // �÷��̾��� �ִ� ü���� Ȯ���ϴ� ������Ƽ
    public int PlayerMaxHp
    {
        get { return _player.PlayerState.PlayerMaxHp; }
        set { _player.PlayerState.PlayerMaxHp = value; }
    }

    // �÷��̾��� ���� ü���� Ȯ���ϴ� ������Ƽ
    public int PlayerCurrentHp
    {
        get { return _player.PlayerState.PlayerCurrentHp; }
        set { _player.PlayerState.PlayerCurrentHp = value; }
    }

    // �÷��̾��� �ִ� ����ġ�� Ȯ���ϴ� ������Ƽ
    public int PlayerMaxExp
    {
        get { return _player.PlayerState.PlayerMaxExp; }
        set { _player.PlayerState.PlayerMaxExp = value; }
    }

    // �÷��̾��� ���� ����ġ�� Ȯ���ϴ� ������Ƽ
    public int PlayerCurrentExp
    {
        get { return _player.PlayerState.PlayerCurrentExp; }
        set { _player.PlayerState.PlayerCurrentExp = value; }
    }

    // �÷��̾��� ���ݷ��� �޾ƿ��� ������Ƽ
    public int PlayerAtk
    {
        get { return _player.PlayerState.PlayerAtk; }
    }

    // �÷��̾��� ���ݷ��� �޾ƿ��� ������Ƽ
    public int PlayerDef
    {
        get { return _player.PlayerState.PlayerDef; }
    }

    // �÷��̾��� ���� ���θ� �����ϴ� ������Ƽ
    public bool IsAttack
    {
        set { _isAttack = value; }
    }

    // �÷��̾��� ��� ���θ� ��ȯ�ϴ� ������Ƽ
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
            // ã�� ParticleSystem ������Ʈ �� ������Ʈ �̸��� Attack Slash 1, Attack Slash 2�� ��츸 ���
            if (particale.transform.name == Define.Attack_Slash_One_Object) _basicSlashBlue = particale.gameObject;
            if (particale.transform.name == Define.Attack_Slash_Two_Object) _multipleSlash = particale.gameObject;
        }

        _basicSlashBlue.SetActive(false);
        _multipleSlash.SetActive(false);

        _warpGateA = GameObject.FindGameObjectWithTag(Define.WarpGateATag);
        _warpGateB = GameObject.FindGameObjectWithTag(Define.WarpGateBTag);

        if (playerAttackAreaObject != null) _playerAttackArea = playerAttackAreaObject.GetComponent<CapsuleCollider>();

        // ���� ���� �� ���� ���� ��Ȱ��ȭ
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

        // Player Rotation ����
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, -50.0f, 0.0f));

        _cameraRotationSpeed = 20.0f;
        _moveSpeed = 3.0f;
        _runSpeed = 5.0f;

        _isAttack = false;
        _isDefend = false;
        _isHit = false;
        _isDie = false;
    }

    // �̵� ó�� (ī�޶� �ٶ󺸴� ������ �������� �̵�)
    private void Move()
    {
        if (Input.GetButton(Define.Horizontal) || Input.GetButton(Define.Vertical))
        {
            float horizontal = Input.GetAxisRaw(Define.Horizontal);
            float vertical = Input.GetAxisRaw(Define.Vertical);

            Vector3 movement = new Vector3(horizontal, vertical, 0.0f);

            // ī�޶� ���� ����, ���� ���⿡ ���� ������ ����
            Vector3 lookForward = new Vector3(_camera.transform.forward.x, 0.0f, _camera.transform.forward.z).normalized;
            Vector3 lookRight = new Vector3(_camera.transform.right.x, 0.0f, _camera.transform.right.z).normalized;

            // ���� ���꿡 ���� ī�޶� ���� ���� ���� * Y�� �̵� �Է� + ī�޶� ���� ���� ���� * X�� �̵� �Է��� ���Ͽ� ������ ����
            _moveDirection = lookForward * movement.y + lookRight * movement.x;

            if (_moveDirection != Vector3.zero)
            {
                // �̵� ����� ������ �÷��̾��� ����� ����ϴ� PlayerModel�� ������ �����̴� �������� ȸ����Ű�� ���� �ٶ󺸴� ������ ���Ѵ�.
                Quaternion viewRotation = Quaternion.LookRotation(_moveDirection.normalized);
                // Lerp�� ����� �ε巯�� ���� ��ȯ
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

    // ���� ó��
    private void Attack()
    {
        if (_isAttack) return;

        if (_attackTimer > 0.0f)
        {
            _attackTimer -= Time.deltaTime;
            return; // 0.4�� ���� �Է��� ����
        }

        if (_comboTimer > 0.0f)
        {
            _comboTimer -= Time.deltaTime;
        }
        else
        {
            _attackCombo = 0; // �޺� �ʱ�ȭ
        }

        if (Input.GetMouseButtonDown(0) && _attackTimer <= 0.0f)
        {
            if (_attackCombo == 0) // ù ��° ����
            {
                _attackCombo = 1;
                _animator.SetTrigger(Define.Attack1Trigger);

                _basicSlashBlue.SetActive(true);
                Invoke("EnableAttackOneEffect", _slashDelay);
            }
            else if (_attackCombo == 1 && _comboTimer > 0.0f) // �� ��° ����
            {
                _attackCombo = 0;
                _animator.SetTrigger(Define.Attack2Trigger);

                _multipleSlash.SetActive(true);
                Invoke("EnableAttackTwoEffect", _slashDelay);
            }
            _isAttack = true;
            _attackTimer = _attackCooldown; // ���� �� 0.4�ʰ� �Է� ����
            _comboTimer = _comboDuration;   // �޺� ���� �ð� �ʱ�ȭ
        }
    }

    // ���� ����Ʈ ��Ȱ��ȭ
    private void EnableAttackOneEffect()
    {
        if (_basicSlashBlue.activeSelf) _basicSlashBlue.SetActive(false);
    }

    private void EnableAttackTwoEffect()
    {
        if (_multipleSlash.activeSelf) _multipleSlash.SetActive(false);
    }

    // �ִϸ��̼� �̺�Ʈ���� ȣ���� �Լ� (����)
    public void EnableAttackCollider(bool value)
    {
        _playerAttackArea.enabled = value;
    }

    // ��� ó��
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

        yield return new WaitForSeconds(0.5f); // 0.5�� �� �ٽ� ���� ����

        _isHit = IsAnimationFinished(Define.Hit);

        StopCoroutine(CoResetHitCooldown());
    }

    private void Die()
    {
        _isDie = true;
        _animator.SetTrigger(Define.DieTrigger);
    }

    // �ɷ�ġ ����
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

    // ���� �ִϸ��̼��� �������� Ȯ�� (�ִϸ��̼� �̸�)
    public bool IsAnimationFinished(string animation)
    {
        AnimatorStateInfo stateInfo;

        if (!_animator) return false;   // _animator ������ false ��ȯ

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(animation))
        {
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            return stateInfo.normalizedTime >= 0.95f;    // �ִϸ��̼��� �����ٸ� true
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ��� or �ǰ� ���¸� ����
        if (_isDie || _isHit) return;

        // �� Ȥ�� ������ ���� �±��� ��츸 ó��
        if (other.CompareTag(Define.GolemLeftAttackAreaTag) || other.CompareTag(Define.GolemRightAttackAreaTag))
        {
            int damage = 0;

            // ������ Ȯ��
            GolemController golemController = other.GetComponentInParent<GolemController>();

            if (golemController != null)
            {
                damage = golemController.GolemAtk;
            }
            else
            {
                // �������� Ȯ��
                BossController bossController = other.GetComponentInParent<BossController>();

                if (bossController != null)
                {
                    damage = bossController.BossAtk;
                }
            }

            // �������� ���� ��� ó��
            if (damage > 0)
            {
                if (!_isDefend) _isHit = true;

                float defense = PlayerDef;  // �÷��̾� ���� ����
                float damageReductionRate = defense / (defense + 100.0f);   // ���¿� ���� ������ ����
                float finalDamage = damage * (1.0f - damageReductionRate);

                if (_isDefend)
                {
                    finalDamage *= 0.7f; // �߰��� 30% ����
                }

                PlayerCurrentHp -= Mathf.RoundToInt(finalDamage);

                // �ǰ� �� ��� �ִϸ��̼�
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