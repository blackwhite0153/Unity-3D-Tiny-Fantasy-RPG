using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ViewCast ����� �����ϴ� ����ü
public struct ViewCastInfo
{
    public Vector3 point;      // �浹�� ���� (�Ǵ� �ִ� �Ÿ� ����)
    public float angle;        // �þ߿��� �� �������� ����
    public float destination;  // �浹�� �Ÿ�
    public bool isHit;         // �浹 ����

    public ViewCastInfo(Vector3 _point, float _angle, float _destination, bool _isHit)
    {
        point = _point;
        angle = _angle;
        destination = _destination;
        isHit = _isHit;
    }
}

// �þ� ���(edge)�� ��Ÿ���� ����ü
public struct Edge
{
    public Vector3 PointA;     // ��� ������
    public Vector3 PointB;     // ��� ����

    public Edge(Vector3 _pointA, Vector3 _pointB)
    {
        PointA = _pointA;
        PointB = _pointB;
    }
}

// �þ߸� ����ϰ� �׸��� Ŭ����
public class FieldOfView : MonoBehaviour
{
    private Mesh _viewMesh;             // �þ߸� ǥ���� �޽�
    private Transform _nearestTarget;   // ���� ����� Ÿ��

    private List<Transform> _visibleTargets = new List<Transform>();    // �þ߿� ���̴� Ÿ�� ����Ʈ

    private float _distanceToTarget;    // ���� ����� Ÿ�ٱ��� �Ÿ�

    public MeshFilter ViewMeshFilter;   // �޽��� ������ �޽� ���� ������Ʈ

    private Coroutine _coFindTargetWithDelay;   // Ÿ�� Ž�� �ڷ�ƾ
    private WaitForSeconds _findTargetDelay;    // Ÿ�� Ž�� ����

    public LayerMask TargetMask;    // Ÿ���� ���� ���̾�
    public LayerMask ObstacleMask;  // ��ֹ��� ���� ���̾�

    [Header("Sight Settings")]
    public float SightRadius = 10.0f;   // �þ� �ݰ�
    [Range(0, 360)]
    public float SightAngle = 90.0f;    // �þ� ���� (0~360)

    public float MeshResolution;    // �޽��� �ػ� (���ø� ����)

    public int EdgeResolveIterations;   // ���� ���� �ݺ� Ƚ��
    public float EdgeDstThreshold;      // ���� �Ÿ� �Ӱ谪

    public Transform NearestTarget => _nearestTarget;   // ���� ����� Ÿ���� ��ȯ�ϴ� ������Ƽ

    // ������Ʈ�� Ȱ��ȭ�� �� ȣ�� (�ʱ� ����)
    private void OnEnable()
    {
        Setting();
    }

    // �� �����Ӹ��� �þ߸� �׸�
    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    // �ʱ� ���� �Լ�
    private void Setting()
    {
        Vector3 meshFilterPos = new Vector3(0.0f, 0.01f, 0.0f);  // �޽� ��ġ �ణ ����

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

        // 0.2�� �������� Ÿ�� Ž�� �ڷ�ƾ ����
        if (_coFindTargetWithDelay == null)
        {
            _coFindTargetWithDelay = StartCoroutine(CoFindTargetWithDelay());
        }
    }

    // Ÿ���� �ֱ������� Ž���ϴ� �ڷ�ƾ
    private IEnumerator CoFindTargetWithDelay()
    {
        while (true)
        {
            yield return _findTargetDelay;  // 0.2�� ���

            FindVisibleTargets();   // �þ� �� Ÿ�� ã��
        }
    }

    // �þ� ������ ���̴� Ÿ���� ã�� �Լ�
    private void FindVisibleTargets()
    {
        _distanceToTarget = 0;
        _nearestTarget = null;
        _visibleTargets.Clear();

        // �þ� �ݰ� �ȿ� �ִ� ��� Ÿ�� ã��
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, SightRadius, TargetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            // Ÿ�� ���� ���� ���
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // Ÿ���� �þ߰� �ȿ� �ִ��� Ȯ��
            if (Vector3.Angle(transform.forward, dirToTarget) < SightAngle / 2)
            {
                // Ÿ�ٰ��� �Ÿ� ���
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                // Ÿ�ٰ� ��ֹ� ���̿� ���θ��� ���� ������ �˻�
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, ObstacleMask))
                {
                    // �þ� �� Ÿ�� ��Ͽ� �߰�
                    _visibleTargets.Add(target);

                    // ���� ����� Ÿ�� ����
                    if (_nearestTarget == null || _distanceToTarget > dstToTarget)
                    {
                        _nearestTarget = target;
                    }
                    _distanceToTarget = dstToTarget;
                }
            }
        }
    }

    // �־��� ������ ���� ���� ���͸� ���ϴ� �Լ�
    public Vector3 DirFromAngle(float angleInDegree, bool angleIsGlobal)
    {
        // �۷ι� ������ �ƴ϶��
        if (!angleIsGlobal)
        {
            angleInDegree += transform.eulerAngles.y;
        }

        // �־��� ������ ���� ���� ���� ���
        Vector3 angleDir = new Vector3(Mathf.Sin(angleInDegree * Mathf.Deg2Rad), 0.0f, Mathf.Cos(angleInDegree * Mathf.Deg2Rad));

        return angleDir;
    }

    // Ư�� ������ Raycast�Ͽ� �þ� ������ ��� �Լ�
    private ViewCastInfo ViewCast(float globalAngle)
    {
        RaycastHit hit;

        Vector3 direction = DirFromAngle(globalAngle, true);

        if (Physics.Raycast(transform.position, direction, out hit, SightRadius, ObstacleMask))
        {
            // ��ֹ��� ���� ���
            return new ViewCastInfo(hit.point, globalAngle, hit.distance, true);
        }
        else
        {
            // ���� ���� ���� ���
            return new ViewCastInfo(transform.position + direction * SightRadius, globalAngle, SightRadius, false);
        }
    }

    // �� ViewCast ������ ��踦 �����Ͽ� ã�� �Լ�
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

    // �þ߸� �޽��� �׸��� �Լ�
    private void DrawFieldOfView()
    {
        // �þ߸� �󸶳� �����ϰ� ������ ����
        int stepCount = Mathf.RoundToInt(SightAngle * MeshResolution);
        float stepAngleSize = SightAngle / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo prevViewCast = new ViewCastInfo();

        // ������ ���� �������� ���ø��Ͽ� ViewCast�� ����
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - SightAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            // ���� ���� (Edge ã��)
            if (i != 0)
            {
                bool edgeDstThresholdExceed = Mathf.Abs(prevViewCast.destination - newViewCast.destination) > EdgeDstThreshold;

                // �� �� �� raycast�� ��ֹ��� ������ �ʾҰų�, �� raycast�� ���� �ٸ� ��ֹ��� hit �� ���̶�� (edgeDstThresholdExceed ���η� ���)
                if (prevViewCast.isHit != newViewCast.isHit || (prevViewCast.isHit && newViewCast.isHit && edgeDstThresholdExceed))
                {
                    Edge e = FindEdge(prevViewCast, newViewCast);

                    // zero�� �ƴ� ������ �߰�
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

        int vertexCount = viewPoints.Count + 1;     // �� ������ �̾��� �߽� ������ �ϳ� �� �ʿ��ؼ� + 1
        int[] triangles = new int[(vertexCount - 2) * 3];   // _viewMesh�� triangles ����� �־��� �迭
                                                            // �ﰢ���� ������ vertexCount - 2�̰� �ﰢ���� �������� 3���̹Ƿ� ��� index�� �Ҵ��ؾ� �ϱ� ����

        Vector3[] vertices = new Vector3[vertexCount];      // _viewMesh�� vertices ����� �־��� �迭
        vertices[0] = Vector3.zero; // Mesh�� ���� ��ǥ�� �����̱� ������ �߽� ��ǥ�� (0, 0, 0)

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

        // �޽��� ������ �ﰢ�� ���� �ݿ�
        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }
}