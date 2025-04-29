using UnityEngine;

public class PlayerAttackStateMachine : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // �θ� Ŭ���� (StateMachineBehaviour)�� OnStateEnter�� ȣ���Ͽ� �⺻ ���� ����
        base.OnStateEnter(animator, stateInfo, layerIndex);

        PlayerController player = animator.GetComponent<PlayerController>();

        if (player != null)
        {
            player.EnableAttackCollider(true);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // �θ� Ŭ���� (StateMachineBehaviour)�� OnStateUpdate�� ȣ���Ͽ� �⺻ ���� ����
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        PlayerController player = animator.GetComponent<PlayerController>();

        // ���� �ִϸ��̼� ������ ���൵�� 90% �̻��� ���� ����
        if (stateInfo.normalizedTime > 0.9f)
        {
            player.IsAttack = false;
            player.EnableAttackCollider(false);
        }
    }
}