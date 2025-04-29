using UnityEngine;
using UnityEngine.AI;

// ���� ��� ���¸� �����ϴ� IdleState Ŭ���� (GolemController�� ����)
public class GolemIdleState : State<GolemController>
{
    private NavMeshAgent _navMeshAgent;

    // ���� �ൿ ���� ��ȯ ������ Ÿ�̸�
    private float _timer;
    private float _waitTimer;

    // ���°� ó�� �ʱ�ȭ�� �� ȣ�� (�� ���� ����)
    public override void OnInitialized()
    {
        // GolemController�� NavMeshAgent ������Ʈ ��������
        _navMeshAgent = context?.GetComponent<NavMeshAgent>();
    }

    // ��� ���� ���� �� ����
    public override void OnEnter()
    {
        base.OnEnter();

        context.IsMove = false;     // �̵� �÷��� ��Ȱ��ȭ
        context.IsAttack = false;   // ���� �÷��� ��Ȱ��ȭ
        context.ComboCount = 0;     // �޺� ī��Ʈ �ʱ�ȭ

        // Ÿ�̸� �ʱ�ȭ
        _timer = 0.0f;
        _waitTimer = Random.Range(2.0f, 5.0f);

        // NavMeshAgent �̵� ����
        if (_navMeshAgent != null && _navMeshAgent.enabled)
        {
            _navMeshAgent.ResetPath();
        }

        // �ִϸ��̼� ����
        context.Animator.SetBool(Define.IsMove, false);
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
        // Ÿ���� ���� ���
        if (!context.Target)
        {
            // Time.deltaTime�� ����� ������
            _timer += Time.deltaTime;

            if (_timer > _waitTimer)
            {
                // ���� �ൿ ���� (RoamState)�� ��ȯ
                stateMachine.ChangeState<GolemRoamState>();
            }
        }
    }
}