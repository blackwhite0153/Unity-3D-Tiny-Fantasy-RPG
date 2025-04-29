using UnityEngine;

public class GolemStateMachine : StateMachineBehaviour
{
    // 애니메이션 상태가 종료될 때 호출
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 부모 클래스 (StateMachineBehaviour)의 OnStateExit을 호출하여 기본 동작 유지
        base.OnStateExit(animator, stateInfo, layerIndex);

        // 현재 설정된 콤보 카운트 값을 가져옴
        int comboCount = animator.GetInteger(Define.ComboCount);

        // 현재 애니메이션 상태의 진행도가 90% 이상일 때만 실행
        if (stateInfo.normalizedTime > 0.9f)
        {
            // 콤보 카운트가 1 미만이면 1 증가, 1 이상이면 0으로 초기화 (콤보 루프 구현)
            comboCount = comboCount < 1 ? comboCount + 1 : 0;
            // 변경된 콤보 카운트 값을 애니메이터에 적용
            animator.SetInteger(Define.ComboCount, comboCount);
        }
    }
}