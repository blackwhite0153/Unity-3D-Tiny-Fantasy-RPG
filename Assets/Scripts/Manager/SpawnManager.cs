using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // 스폰 포인트 배열
    private List<Transform> _spawnPoints = new List<Transform>();

    public Transform spawnParent;

    private void Start()
    {
        // 부모 오브젝트의 모든 자식을 List에 추가
        for (int i = 0; i < spawnParent.childCount; i++)
        {
            _spawnPoints.Add(spawnParent.GetChild(i));
        }

        // 게임 오브젝트 리소스 로드
        ObjectManager.Instance.ResourceAllLoad();
        // 지정한 위치에 플레이어 생성
        ObjectManager.Instance.Spawn<PlayerController>(new Vector3(52.0f, -2.5f, -52.0f));
        //ObjectManager.Instance.Spawn<PlayerController>(new Vector3(0.0f, 0.0f, 0.0f));
        // 지정한 위치에 보스 생성
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