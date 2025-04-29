using UnityEngine.AI;

// ������ ���� ���¸� �����ϴ� AttackState Ŭ���� (BossController�� ����)
public class BossAttackState : State<BossController>
{
    private NavMeshAgent _navMeshAgent;

    // ���°� ó�� �ʱ�ȭ�� �� ȣ�� (�� ���� ����)
    public override void OnInitialized()
    {
        // BossController�� NavMeshAgent ������Ʈ ��������
        _navMeshAgent = context?.GetComponent<NavMeshAgent>();
    }

    // ���� ���¿� ������ �� ȣ��
    public override void OnEnter()
    {
        base.OnEnter();

        context.IsMove = false;     // �̵� �÷��� ��Ȱ��ȭ
        context.IsAttack = true;    // ���� �÷��� Ȱ��ȭ
        context.ComboCount = 0;     // �޺� ī��Ʈ �ʱ�ȭ

        // ���� ���� �� ���� ��� �ִϸ��̼� ����
        context.Animator.SetBool(Define.IsAttack, true);

        // ���� ���� Ȱ��ȭ / ��Ȱ��ȭ
        if (context.ComboCount == 0) context.SetAttackColliders(false, true);
        else if (context.ComboCount == 1) context.SetAttackColliders(true, false);

        // NavMeshAgent�� ���߰� �̵��� ����
        if (_navMeshAgent != null)
            _navMeshAgent.isStopped = true; // �̵� ����
    }

    // �� ������ ȣ�� (������ �� ������Ʈ)
    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        // Ÿ���� �ٶ󺸵��� ȸ��
        context.RotateToTarget();

        // ���� ���� Ȱ��ȭ / ��Ȱ��ȭ
        if (context.ComboCount == 0) context.SetAttackColliders(false, true);
        else if (context.ComboCount == 1) context.SetAttackColliders(true, false);

        // ü���� 0 ������ ���
        if (context.BossHp <= 0)
        {
            // ��� ���� (DieState)�� ��ȯ
            stateMachine.ChangeState<BossDieState>();
        }
        // Ÿ���� ���� ���
        if (!context.Target)
        {
            // ���� ���� (ReturnState)�� ��ȯ
            stateMachine.ChangeState<BossReturnState>();
        }
        // ���� ����� ������, ���� ������ �Ÿ� ���� ���� ������ ���� ���·� ��ȯ
        if (!context.IsAvailableAttack && context.IsAnimationFinished)
        {
            // ���� ���� (PursuitState)�� ��ȯ
            stateMachine.ChangeState<BossPursuitState>();
        }
    }

    // ���� ���¿��� ��� �� ȣ��
    public override void OnExit()
    {
        base.OnExit();

        context.IsAttack = false;   // ���� �÷��� ��Ȱ��ȭ
        context.SetAttackColliders(false, false);   // ���� ���� ���� ��Ȱ��ȭ

        // ���� �ִϸ��̼� ���� �� �̵� �����ϵ��� ����
        context.Animator.SetBool(Define.IsAttack, false);

        // NavMeshAgent�� �ٽ� �̵� �����ϵ��� ����
        if (_navMeshAgent != null)
            _navMeshAgent.isStopped = false;    // �̵� �簳
    }
}