using UnityEngine;
using UnityEngine.AI;

// ���� ���� �ൿ ���¸� �����ϴ� RoamState Ŭ���� (GolemController�� ����)
public class GolemRoamState : State<GolemController>
{
    private NavMeshAgent _navMeshAgent;

    // ���°� ó�� �ʱ�ȭ�� �� ȣ�� (�� ���� ����)
    public override void OnInitialized()
    {
        // GolemController�� NavMeshAgent ������Ʈ ��������
        _navMeshAgent = context?.GetComponent<NavMeshAgent>();
    }

    // ���� �ൿ ���¿� ������ �� ȣ��
    public override void OnEnter()
    {
        base.OnEnter();

        context.IsMove = true;      // �̵� �÷��� Ȱ��ȭ
        context.IsAttack = false;   // ���� �÷��� ��Ȱ��ȭ
        context.ComboCount = 0;     // �޺� ī��Ʈ �ʱ�ȭ

        // ������ ��ǥ ����
        context.CoordinateSetting();

        // �̵� ���� �� �ִϸ��̼� �ѱ�
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
        if (context.Target)
        {
            // ���� ���� (PursuitState)�� ��ȯ
            stateMachine.ChangeState<GolemPursuitState>();
        }
        // �̵��� �Ϸ�Ǿ����� NavMeshAgent �������� Ȯ��
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            // �̵� ���� �ִϸ��̼�
            context.Animator.SetBool(Define.IsMove, false);

            // ��� ���� (IdleState)�� ��ȯ
            stateMachine.ChangeState<GolemIdleState>();
        }
    }

    // ���� �ൿ ���¿��� ��� �� ȣ��
    public override void OnExit()
    {
        base.OnExit();

        // �̵� ����
        if (_navMeshAgent != null && _navMeshAgent.enabled)
        {
            _navMeshAgent.ResetPath();
        }

        // �̵� �ִϸ��̼� ����
        context.Animator.SetBool(Define.IsMove, false);
    }
}