using UnityEngine.AI;

// 골렘의 추적 상태를 정의하는 GolemPursuitState 클래스 (GolemController에 적용)
public class GolemPursuitState : State<GolemController>
{
    private NavMeshAgent _navMeshAgent;

    // 상태가 처음 초기화될 때 호출 (한 번만 실행)
    public override void OnInitialized()
    {
        // GolemController의 NavMeshAgent 컴포넌트 가져오기
        _navMeshAgent = context?.GetComponent<NavMeshAgent>();
    }

    // 추적 상태에 진입할 때 호출
    public override void OnEnter()
    {
        base.OnEnter();

        context.IsMove = true;      // 이동 플래그 활성화
        context.IsAttack = false;   // 공격 플래그 비활성화
        context.ComboCount = 0;     // 콤보 카운트 초기화

        // NavMeshAgent가 목적지로 이동 시작하도록 설정
        if (_navMeshAgent != null && _navMeshAgent.enabled)
        {
            _navMeshAgent.isStopped = false;  // 이동 시작
            _navMeshAgent.ResetPath();        // 기존 경로 초기화
            _navMeshAgent.SetDestination(context.Target.position);  // 타겟 위치로 이동
        }

        // 애니메이션 시작 (이동 중 애니메이션 활성화)
        context.Animator.SetBool(Define.IsMove, true);
    }

    // 매 프레임 호출 (게임의 논리 업데이트)
    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        // 체력이 0 이하일 경우
        if (context.GolemHp <= 0)
        {
            // 사망 상태 (DieState)로 전환
            stateMachine.ChangeState<GolemDieState>();
        }
        // 타겟이 없을 경우
        if (!context.Target)
        {
            // 대기 상태 (IdleState)로 전환
            stateMachine.ChangeState<GolemIdleState>();
        }
        // 공격이 가능한 상태인지 확인
        if (context.IsAvailableAttack)
        {
            // 공격 상태 (AttackState)로 전환
            stateMachine.ChangeState<GolemAttackState>();
        }

        // 타겟을 바라보도록 회전
        context.RotateToTarget();
    }

    // 물리 업데이트 (고정된 시간 간격으로 실행)
    public override void OnFixedUpdate(float deltaTime)
    {
        base.OnFixedUpdate(deltaTime);

        // NavMeshAgent가 타겟을 향해 이동 중
        if (_navMeshAgent != null && _navMeshAgent.enabled)
        {
            context.MoveToTarget();

            // 타겟과의 거리가 가까워지면 이동 멈추기 (추적 상태 종료)
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance) _navMeshAgent.isStopped = true;
            else _navMeshAgent.isStopped = false;
        }
    }

    // 추적 상태에서 벗어날 때 호출
    public override void OnExit()
    {
        base.OnExit();

        // 이동을 멈추기 위해 NavMeshAgent를 멈춤
        if (_navMeshAgent != null && _navMeshAgent.enabled)
        {
            _navMeshAgent.isStopped = true;
        }

        // 애니메이션 정지
        context.Animator.SetBool(Define.IsMove, false);
    }
}