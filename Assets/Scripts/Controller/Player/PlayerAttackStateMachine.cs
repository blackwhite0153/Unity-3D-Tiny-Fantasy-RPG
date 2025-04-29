using UnityEngine;

public class PlayerAttackStateMachine : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 부모 클래스 (StateMachineBehaviour)의 OnStateEnter을 호출하여 기본 동작 유지
        base.OnStateEnter(animator, stateInfo, layerIndex);

        PlayerController player = animator.GetComponent<PlayerController>();

        if (player != null)
        {
            player.EnableAttackCollider(true);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 부모 클래스 (StateMachineBehaviour)의 OnStateUpdate을 호출하여 기본 동작 유지
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        PlayerController player = animator.GetComponent<PlayerController>();

        // 현재 애니메이션 상태의 진행도가 90% 이상일 때만 실행
        if (stateInfo.normalizedTime > 0.9f)
        {
            player.IsAttack = false;
            player.EnableAttackCollider(false);
        }
    }
}