using UnityEngine.AI;

// 보스의 대기 상태를 정의하는 IdleState 클래스 (BossController에 적용)
public class BossIdleState : State<BossController>
{
    private NavMeshAgent _navMeshAgent;

    // 상태가 처음 초기화될 때 호출 (한 번만 실행)
    public override void OnInitialized()
    {
        // BossController의 NavMeshAgent 컴포넌트 가져오기
        _navMeshAgent = context?.GetComponent<NavMeshAgent>();
    }

    // 대기 상태 진입 시 실행
    public override void OnEnter()
    {
        base.OnEnter();

        context.IsMove = false;     // 이동 플래그 비활성화
        context.IsAttack = false;   // 공격 플래그 비활성화
        context.ComboCount = 0;     // 콤보 카운트 초기화

        // NavMeshAgent 이동 정지
        if (_navMeshAgent != null && _navMeshAgent.enabled)
        {
            _navMeshAgent.ResetPath();
        }
        // 애니메이션 정지
        context.Animator.SetBool(Define.IsMove, false);
    }

    // 매 프레임 호출 (게임의 논리 업데이트)
    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        // 체력이 0 이하일 경우
        if (context.BossHp <= 0)
        {
            // 사망 상태 (DieState)로 전환
            stateMachine.ChangeState<BossDieState>();
        }
        // 타겟이 있을 경우
        if (context.Target)
        {
            // 추적 상태 (PursuitState)로 전환
            stateMachine.ChangeState<BossPursuitState>();
        }
        // 타겟이 없을 경우
        if (!context.Target)
        {
            // 복귀 상태 (ReturnState)로 전환
            stateMachine.ChangeState<BossReturnState>();
        }
    }
}