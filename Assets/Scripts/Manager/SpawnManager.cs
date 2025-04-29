using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // ���� ����Ʈ �迭
    private List<Transform> _spawnPoints = new List<Transform>();

    public Transform spawnParent;

    private void Start()
    {
        // �θ� ������Ʈ�� ��� �ڽ��� List�� �߰�
        for (int i = 0; i < spawnParent.childCount; i++)
        {
            _spawnPoints.Add(spawnParent.GetChild(i));
        }

        // ���� ������Ʈ ���ҽ� �ε�
        ObjectManager.Instance.ResourceAllLoad();
        // ������ ��ġ�� �÷��̾� ����
        ObjectManager.Instance.Spawn<PlayerController>(new Vector3(52.0f, -2.5f, -52.0f));
        //ObjectManager.Instance.Spawn<PlayerController>(new Vector3(0.0f, 0.0f, 0.0f));
        // ������ ��ġ�� ���� ����
        ObjectManager.Instance.Spawn<BossController>(new Vector3(-28.0f, 11.0f, 28.0f));
        //ObjectManager.Instance.Spawn<BossController>(new Vector3(0.0f, 0.0f, -20.0f));

        SpawnGolem();
    }

    private void SpawnGolem()
    {
        foreach (Transform point in _spawnPoints)
        {
            PoolManager.Instance.GetObject<GolemController>(point.position);
        }
    }
}