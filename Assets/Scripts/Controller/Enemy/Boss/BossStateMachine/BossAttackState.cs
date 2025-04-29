using UnityEngine.AI;

// 보스의 공격 상태를 정의하는 AttackState 클래스 (BossController에 적용)
public class BossAttackState : State<BossController>
{
    private NavMeshAgent _navMeshAgent;

    // 상태가 처음 초기화될 때 호출 (한 번만 실행)
    public override void OnInitialized()
    {
        // BossController의 NavMeshAgent 컴포넌트 가져오기
        _navMeshAgent = context?.GetComponent<NavMeshAgent>();
    }

    // 공격 상태에 진입할 때 호출
    public override void OnEnter()
    {
        base.OnEnter();

        context.IsMove = false;     // 이동 플래그 비활성화
        context.IsAttack = true;    // 공격 플래그 활성화
        context.ComboCount = 0;     // 콤보 카운트 초기화

        // 공격 시작 시 공격 모션 애니메이션 실행
        context.Animator.SetBool(Define.IsAttack, true);

        // 공격 영역 활성화 / 비활성화
        if (context.ComboCount == 0) context.SetAttackColliders(false, true);
        else if (context.ComboCount == 1) context.SetAttackColliders(true, false);

        // NavMeshAgent를 멈추고 이동을 방지
        if (_navMeshAgent != null)
            _navMeshAgent.isStopped = true; // 이동 중지
    }

    // 매 프레임 호출 (게임의 논리 업데이트)
    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        // 타겟을 바라보도록 회전
        context.RotateToTarget();

        // 공격 영역 활성화 / 비활성화
        if (context.ComboCount == 0) context.SetAttackColliders(false, true);
        else if (context.ComboCount == 1) context.SetAttackColliders(true, false);

        // 체력이 0 이하일 경우
        if (context.BossHp <= 0)
        {
            // 사망 상태 (DieState)로 전환
            stateMachine.ChangeState<BossDieState>();
        }
        // 타겟이 없을 경우
        if (!context.Target)
        {
            // 복귀 상태 (ReturnState)로 전환
            stateMachine.ChangeState<BossReturnState>();
        }
        // 공격 모션이 끝났고, 공격 가능한 거리 내에 있지 않으면 추적 상태로 전환
        if (!context.IsAvailableAttack && context.IsAnimationFinished)
        {
            // 추적 상태 (PursuitState)로 전환
            stateMachine.ChangeState<BossPursuitState>();
        }
    }

    // 공격 상태에서 벗어날 때 호출
    public override void OnExit()
    {
        base.OnExit();

        context.IsAttack = false;   // 공격 플래그 비활성화
        context.SetAttackColliders(false, false);   // 공격 영역 전부 비활성화

        // 공격 애니메이션 종료 후 이동 가능하도록 설정
        context.Animator.SetBool(Define.IsAttack, false);

        // NavMeshAgent가 다시 이동 가능하도록 설정
        if (_navMeshAgent != null)
            _navMeshAgent.isStopped = false;    // 이동 재개
    }
}