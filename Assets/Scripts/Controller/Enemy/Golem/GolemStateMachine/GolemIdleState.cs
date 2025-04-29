using UnityEngine;
using UnityEngine.AI;

// 골렘의 대기 상태를 정의하는 IdleState 클래스 (GolemController에 적용)
public class GolemIdleState : State<GolemController>
{
    private NavMeshAgent _navMeshAgent;

    // 자유 행동 상태 전환 딜레이 타이머
    private float _timer;
    private float _waitTimer;

    // 상태가 처음 초기화될 때 호출 (한 번만 실행)
    public override void OnInitialized()
    {
        // GolemController의 NavMeshAgent 컴포넌트 가져오기
        _navMeshAgent = context?.GetComponent<NavMeshAgent>();
    }

    // 대기 상태 진입 시 실행
    public override void OnEnter()
    {
        base.OnEnter();

        context.IsMove = false;     // 이동 플래그 비활성화
        context.IsAttack = false;   // 공격 플래그 비활성화
        context.ComboCount = 0;     // 콤보 카운트 초기화

        // 타이머 초기화
        _timer = 0.0f;
        _waitTimer = Random.Range(2.0f, 5.0f);

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
        if (context.GolemHp <= 0)
        {
            // 사망 상태 (DieState)로 전환
            stateMachine.ChangeState<GolemDieState>();
        }
        // 타겟이 있을 경우
        if (context.Target)
        {
            // 추적 상태 (PursuitState)로 전환
            stateMachine.ChangeState<GolemPursuitState>();
        }
        // 타겟이 없을 경우
        if (!context.Target)
        {
            // Time.deltaTime을 사용한 딜레이
            _timer += Time.deltaTime;

            if (_timer > _waitTimer)
            {
                // 자유 행동 상태 (RoamState)로 전환
                stateMachine.ChangeState<GolemRoamState>();
            }
        }
    }
}