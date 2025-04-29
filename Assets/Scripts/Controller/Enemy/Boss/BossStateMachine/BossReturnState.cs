using UnityEngine;
using UnityEngine.AI;

// 보스의 복귀 상태를 정의하는 ReturnState 클래스 (BossController에 적용)
public class BossReturnState : State<BossController>
{
    private NavMeshAgent _navMeshAgent;

    // 상태가 처음 초기화될 때 호출 (한 번만 실행)
    public override void OnInitialized()
    {
        // GolemController의 NavMeshAgent 컴포넌트 가져오기
        _navMeshAgent = context?.GetComponent<NavMeshAgent>();
    }

    // 복귀 상태에 진입할 때 호출
    public override void OnEnter()
    {
        base.OnEnter();

        context.IsMove = true;      // 이동 플래그 활성화
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

        // 오차 범위 계산
        float distanceToSpawn = Vector3.Distance(context.SpawnPos, context.transform.position);

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
        // 이동이 완료되었는지 NavMeshAgent 기준으로 확인
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            // 이동 중지 애니메이션
            context.Animator.SetBool(Define.IsMove, false);

            // 대기 상태 (IdleState)로 전환
            stateMachine.ChangeState<BossIdleState>();
        }

        // 스폰 지점을 바라보도록 회전
        context.RotateToTarget();
    }

    // 물리 업데이트 (고정된 시간 간격으로 실행)
    public override void OnFixedUpdate(float deltaTime)
    {
        base.OnFixedUpdate(deltaTime);

        // 스폰 지점을 향해 이동
        context.ReturnMove();
    }

    // 복귀 상태에서 벗어날 때 호출
    public override void OnExit()
    {
        base.OnExit();

        // 이동 중지
        if (_navMeshAgent != null && _navMeshAgent.enabled)
        {
            _navMeshAgent.ResetPath();
        }

        // 이동 애니메이션 끄기
        context.Animator.SetBool(Define.IsMove, false);
    }
}