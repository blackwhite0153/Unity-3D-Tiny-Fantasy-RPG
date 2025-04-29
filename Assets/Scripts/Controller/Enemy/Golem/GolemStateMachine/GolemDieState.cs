using UnityEngine.AI;

// ���� ��� ���¸� �����ϴ� DieState Ŭ���� (GolemController�� ����)
public class GolemDieState : State<GolemController>
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

        context.IsDie = true;        // ��� �÷��� Ȱ��ȭ
        context.IsMove = false;      // �̵� �÷��� ��Ȱ��ȭ
        context.IsAttack = false;    // ���� �÷��� ��Ȱ��ȭ
        context.ComboCount = 0;      // �޺� ī��Ʈ �ʱ�ȭ

        // NavMeshAgent�� ���߰� �̵� �Ұ� ���·� ����
        if (_navMeshAgent != null)
            _navMeshAgent.isStopped = true;

        // ��� ó�� �Լ� ȣ��
        context.Die();
    }
}