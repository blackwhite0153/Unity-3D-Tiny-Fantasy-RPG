using UnityEngine;

public class GolemStateMachine : StateMachineBehaviour
{
    // �ִϸ��̼� ���°� ����� �� ȣ��
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // �θ� Ŭ���� (StateMachineBehaviour)�� OnStateExit�� ȣ���Ͽ� �⺻ ���� ����
        base.OnStateExit(animator, stateInfo, layerIndex);

        // ���� ������ �޺� ī��Ʈ ���� ������
        int comboCount = animator.GetInteger(Define.ComboCount);

        // ���� �ִϸ��̼� ������ ���൵�� 90% �̻��� ���� ����
        if (stateInfo.normalizedTime > 0.9f)
        {
            // �޺� ī��Ʈ�� 1 �̸��̸� 1 ����, 1 �̻��̸� 0���� �ʱ�ȭ (�޺� ���� ����)
            comboCount = comboCount < 1 ? comboCount + 1 : 0;
            // ����� �޺� ī��Ʈ ���� �ִϸ����Ϳ� ����
            animator.SetInteger(Define.ComboCount, comboCount);
        }
    }
}