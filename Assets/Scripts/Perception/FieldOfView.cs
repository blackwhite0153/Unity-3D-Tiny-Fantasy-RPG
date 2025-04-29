using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ViewCast 결과를 저장하는 구조체
public struct ViewCastInfo
{
    public Vector3 point;      // 충돌한 지점 (또는 최대 거리 지점)
    public float angle;        // 시야에서 이 점까지의 각도
    public float destination;  // 충돌한 거리
    public bool isHit;         // 충돌 여부

    public ViewCastInfo(Vector3 _point, float _angle, float _destination, bool _isHit)
    {
        point = _point;
        angle = _angle;
        destination = _destination;
        isHit = _isHit;
    }
}

// 시야 경계(edge)를 나타내는 구조체
public struct Edge
{
    public Vector3 PointA;     // 경계 시작점
    public Vector3 PointB;     // 경계 끝점

    public Edge(Vector3 _pointA, Vector3 _pointB)
    {
        PointA = _pointA;
        PointB = _pointB;
    }
}

// 시야를 계산하고 그리는 클래스
public class FieldOfView : MonoBehaviour
{
    private Mesh _viewMesh;             // 시야를 표현할 메쉬
    private Transform _nearestTarget;   // 가장 가까운 타겟

    private List<Transform> _visibleTargets = new List<Transform>();    // 시야에 보이는 타겟 리스트

    private float _distanceToTarget;    // 가장 가까운 타겟까지 거리

    public MeshFilter ViewMeshFilter;   // 메쉬를 적용할 메쉬 필터 컴포넌트

    private Coroutine _coFindTargetWithDelay;   // 타겟 탐색 코루틴
    private WaitForSeconds _findTargetDelay;    // 타겟 탐색 간격

    public LayerMask TargetMask;    // 타겟이 속한 레이어
    public LayerMask ObstacleMask;  // 장애물이 속한 레이어

    [Header("Sight Settings")]
    public float SightRadius = 10.0f;   // 시야 반경
    [Range(0, 360)]
    public float SightAngle = 90.0f;    // 시야 각도 (0~360)

    public float MeshResolution;    // 메쉬의 해상도 (샘플링 간격)

    public int EdgeResolveIterations;   // 에지 보간 반복 횟수
    public float EdgeDstThreshold;      // 에지 거리 임계값

    public Transform NearestTarget => _nearestTarget;   // 가장 가까운 타겟을 반환하는 프로퍼티

    // 오브젝트가 활성화될 때 호출 (초기 설정)
    private void OnEnable()
    {
        Setting();
    }

    // 매 프레임마다 시야를 그림
    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    // 초기 설정 함수
    private void Setting()
    {
        Vector3 meshFilterPos = new Vector3(0.0f, 0.01f, 0.0f);  // 메쉬 위치 약간 띄우기

        _viewMesh = new Mesh();
        _viewMesh.name = "View Mesh";

        _nearestTarget = null;

        ViewMeshFilter.mesh = _viewMesh;
        ViewMeshFilter.transform.position = transform.position + meshFilterPos;

        TargetMask = LayerMask.GetMask(Define.PlayerLayer);
        ObstacleMask = LayerMask.GetMask(Define.ObstacleLayer);

        if (_coFindTargetWithDelay != null) StopCoroutine(_coFindTargetWithDelay);

        _coFindTargetWithDelay = null;
        _findTargetDelay = new WaitForSeconds(0.2f);

        // 0.2초 간격으로 타겟 탐색 코루틴 시작
        if (_coFindTargetWithDelay == null)
        {
            _coFindTargetWithDelay = StartCoroutine(CoFindTargetWithDelay());
        }
    }

    // 타겟을 주기적으로 탐색하는 코루틴
    private IEnumerator CoFindTargetWithDelay()
    {
        while (true)
        {
            yield return _findTargetDelay;  // 0.2초 대기

            FindVisibleTargets();   // 시야 내 타겟 찾기
        }
    }

    // 시야 내에서 보이는 타겟을 찾는 함수
    private void FindVisibleTargets()
    {
        _distanceToTarget = 0;
        _nearestTarget = null;
        _visibleTargets.Clear();

        // 시야 반경 안에 있는 모든 타겟 찾기
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, SightRadius, TargetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            // 타겟 방향 벡터 계산
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // 타겟이 시야각 안에 있는지 확인
            if (Vector3.Angle(transform.forward, dirToTarget) < SightAngle / 2)
            {
                // 타겟과의 거리 계산
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                // 타겟과 장애물 사이에 가로막힌 것이 없는지 검사
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, ObstacleMask))
                {
                    // 시야 내 타겟 목록에 추가
                    _visibleTargets.Add(target);

                    // 가장 가까운 타겟 저장
                    if (_nearestTarget == null || _distanceToTarget > dstToTarget)
                    {
                        _nearestTarget = target;
                    }
                    _distanceToTarget = dstToTarget;
                }
            }
        }
    }

    // 주어진 각도에 대해 방향 벡터를 구하는 함수
    public Vector3 DirFromAngle(float angleInDegree, bool angleIsGlobal)
    {
        // 글로벌 각도가 아니라면
        if (!angleIsGlobal)
        {
            angleInDegree += transform.eulerAngles.y;
        }

        // 주어진 각도에 대한 방향 벡터 계산
        Vector3 angleDir = new Vector3(Mathf.Sin(angleInDegree * Mathf.Deg2Rad), 0.0f, Mathf.Cos(angleInDegree * Mathf.Deg2Rad));

        return angleDir;
    }

    // 특정 각도로 Raycast하여 시야 정보를 얻는 함수
    private ViewCastInfo ViewCast(float globalAngle)
    {
        RaycastHit hit;

        Vector3 direction = DirFromAngle(globalAngle, true);

        if (Physics.Raycast(transform.position, direction, out hit, SightRadius, ObstacleMask))
        {
            // 장애물에 맞은 경우
            return new ViewCastInfo(hit.point, globalAngle, hit.distance, true);
        }
        else
        {
            // 맞은 것이 없는 경우
            return new ViewCastInfo(transform.position + direction * SightRadius, globalAngle, SightRadius, false);
        }
    }

    // 두 ViewCast 사이의 경계를 보간하여 찾는 함수
    private Edge FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;

        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < EdgeResolveIterations; i++)
        {
            float angle = minAngle + (maxAngle - minAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);
            bool edgeDstThresholdExceed = Mathf.Abs(minViewCast.destination - newViewCast.destination) > EdgeDstThreshold;

            if (newViewCast.isHit == minViewCast.isHit && !edgeDstThresholdExceed)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new Edge(minPoint, maxPoint);
    }

    // 시야를 메쉬로 그리는 함수
    private void DrawFieldOfView()
    {
        // 시야를 얼마나 세밀하게 나눌지 결정
        int stepCount = Mathf.RoundToInt(SightAngle * MeshResolution);
        float stepAngleSize = SightAngle / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo prevViewCast = new ViewCastInfo();

        // 각도를 일정 간격으로 샘플링하여 ViewCast를 수행
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - SightAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            // 정점 보간 (Edge 찾기)
            if (i != 0)
            {
                bool edgeDstThresholdExceed = Mathf.Abs(prevViewCast.destination - newViewCast.destination) > EdgeDstThreshold;

                // 둘 중 한 raycast가 장애물을 만나지 않았거나, 두 raycast가 서로 다른 장애물에 hit 된 것이라면 (edgeDstThresholdExceed 여부로 계산)
                if (prevViewCast.isHit != newViewCast.isHit || (prevViewCast.isHit && newViewCast.isHit && edgeDstThresholdExceed))
                {
                    Edge e = FindEdge(prevViewCast, newViewCast);

                    // zero가 아닌 정점을 추가
                    if (e.PointA != Vector3.zero)
                    {
                        viewPoints.Add(e.PointA);
                    }
                    if (e.PointB != Vector3.zero)
                    {
                        viewPoints.Add(e.PointB);
                    }
                }
            }
            viewPoints.Add(newViewCast.point);
            prevViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;     // 얻어낸 정점을 이어줄 중심 정점이 하나 더 필요해서 + 1
        int[] triangles = new int[(vertexCount - 2) * 3];   // _viewMesh의 triangles 멤버에 넣어줄 배열
                                                            // 삼각형의 개수는 vertexCount - 2이고 삼각형의 꼭짓점은 3개이므로 모든 index를 할당해야 하기 때문

        Vector3[] vertices = new Vector3[vertexCount];      // _viewMesh의 vertices 멤버에 넣어줄 배열
        vertices[0] = Vector3.zero; // Mesh는 로컬 좌표가 기준이기 때문에 중심 좌표가 (0, 0, 0)

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        // 메쉬에 정점과 삼각형 정보 반영
        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }
}