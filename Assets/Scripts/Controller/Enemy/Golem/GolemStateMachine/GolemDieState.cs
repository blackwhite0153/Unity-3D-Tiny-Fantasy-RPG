using UnityEngine.AI;

// 골렘의 사망 상태를 정의하는 DieState 클래스 (GolemController에 적용)
public class GolemDieState : State<GolemController>
{
    private NavMeshAgent _navMeshAgent;

    // 상태가 처음 초기화될 때 호출 (한 번만 실행)
    public override void OnInitialized()
    {
        // GolemController의 NavMeshAgent 컴포넌트 가져오기
        _navMeshAgent = context?.GetComponent<NavMeshAgent>();
    }

    // 자유 행동 상태에 진입할 때 호출
    public override void OnEnter()
    {
        base.OnEnter();

        context.IsDie = true;        // 사망 플래그 활성화
        context.IsMove = false;      // 이동 플래그 비활성화
        context.IsAttack = false;    // 공격 플래그 비활성화
        context.ComboCount = 0;      // 콤보 카운트 초기화

        // NavMeshAgent를 멈추고 이동 불가 상태로 설정
        if (_navMeshAgent != null)
            _navMeshAgent.isStopped = true;

        // 사망 처리 함수 호출
        context.Die();
    }
}