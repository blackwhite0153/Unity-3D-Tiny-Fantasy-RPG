using UnityEngine.AI;

// ���� ���� ���¸� �����ϴ� GolemPursuitState Ŭ���� (GolemController�� ����)
public class GolemPursuitState : State<GolemController>
{
    private NavMeshAgent _navMeshAgent;

    // ���°� ó�� �ʱ�ȭ�� �� ȣ�� (�� ���� ����)
    public override void OnInitialized()
    {
        // GolemController�� NavMeshAgent ������Ʈ ��������
        _navMeshAgent = context?.GetComponent<NavMeshAgent>();
    }

    // ���� ���¿� ������ �� ȣ��
    public override void OnEnter()
    {
        base.OnEnter();

        context.IsMove = true;      // �̵� �÷��� Ȱ��ȭ
        context.IsAttack = false;   // ���� �÷��� ��Ȱ��ȭ
        context.ComboCount = 0;     // �޺� ī��Ʈ �ʱ�ȭ

        // NavMeshAgent�� �������� �̵� �����ϵ��� ����
        if (_navMeshAgent != null && _navMeshAgent.enabled)
        {
            _navMeshAgent.isStopped = false;  // �̵� ����
            _navMeshAgent.ResetPath();        // ���� ��� �ʱ�ȭ
            _navMeshAgent.SetDestination(context.Target.position);  // Ÿ�� ��ġ�� �̵�
        }

        // �ִϸ��̼� ���� (�̵� �� �ִϸ��̼� Ȱ��ȭ)
        context.Animator.SetBool(Define.IsMove, true);
    }

    // �� ������ ȣ�� (������ �� ������Ʈ)
    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        // ü���� 0 ������ ���
        if (context.GolemHp <= 0)
        {
            // ��� ���� (DieState)�� ��ȯ
            stateMachine.ChangeState<GolemDieState>();
        }
        // Ÿ���� ���� ���
        if (!context.Target)
        {
            // ��� ���� (IdleState)�� ��ȯ
            stateMachine.ChangeState<GolemIdleState>();
        }
        // ������ ������ �������� Ȯ��
        if (context.IsAvailableAttack)
        {
            // ���� ���� (AttackState)�� ��ȯ
            stateMachine.ChangeState<GolemAttackState>();
        }

        // Ÿ���� �ٶ󺸵��� ȸ��
        context.RotateToTarget();
    }

    // ���� ������Ʈ (������ �ð� �������� ����)
    public override void OnFixedUpdate(float deltaTime)
    {
        base.OnFixedUpdate(deltaTime);

        // NavMeshAgent�� Ÿ���� ���� �̵� ��
        if (_navMeshAgent != null && _navMeshAgent.enabled)
        {
            context.MoveToTarget();

            // Ÿ�ٰ��� �Ÿ��� ��������� �̵� ���߱� (���� ���� ����)
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance) _navMeshAgent.isStopped = true;
            else _navMeshAgent.isStopped = false;
        }
    }

    // ���� ���¿��� ��� �� ȣ��
    public override void OnExit()
    {
        base.OnExit();

        // �̵��� ���߱� ���� NavMeshAgent�� ����
        if (_navMeshAgent != null && _navMeshAgent.enabled)
        {
            _navMeshAgent.isStopped = true;
        }

        // �ִϸ��̼� ����
        context.Animator.SetBool(Define.IsMove, false);
    }
}