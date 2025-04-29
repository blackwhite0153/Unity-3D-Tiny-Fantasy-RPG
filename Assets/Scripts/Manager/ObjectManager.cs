using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>
{
    // 적 리스폰 딜레이 코루틴
    private Coroutine _coRespawnDelay;
    // 적 리스폰 딜레이
    private WaitForSeconds _delay;

    // 플레이어 컨트롤러 인스턴스
    private PlayerController _player;
    // 보스 컨트롤러 인스턴스
    private BossController _boss;

    // 리소스 참조 변수
    private GameObject _dogKnightResource;
    private GameObject _bossGolemResource;
    private List<GameObject> _golemResource = new List<GameObject>();

    // 플레이어 컨트롤러에 대한 읽기 전용 프로퍼티
    public PlayerController Player { get => _player; }
    // 보스 컨트롤러에 대한 읽기 전용 프로퍼티
    public BossController Boss { get => _boss; }

    // 적 객체들을 관리하는 HashSet
    public HashSet<GolemController> Golem { get; set; } = new HashSet<GolemController>();

    // 싱글톤 초기화 (상속된 Singleton에서 기본 초기화 수행)
    protected override void Initialize()
    {
        // Singleton 초기화 호출
        base.Initialize();

        _coRespawnDelay = null;
        _delay = new WaitForSeconds(5.0f);
    }

    // 모든 게임 오브젝트 리소스를 로드하는 함수
    public void ResourceAllLoad()
    {
        // Resources.Load<T>(Path)를 사용하여 프리팹 로드
        _dogKnightResource = Resources.Load<GameObject>(Define.DogPolyartPath);
        _bossGolemResource = Resources.Load<GameObject>(Define.BossGolemPath);

        _golemResource.Add(Resources.Load<GameObject>(Define.HPGolemPath));
        _golemResource.Add(Resources.Load<GameObject>(Define.PBRGolemPath));
        _golemResource.Add(Resources.Load<GameObject>(Define.PolyartGolemPath));
    }

    // 제네릭 타입을 사용한 Spawn 함수
    public T Spawn<T>(Vector3 spawnPos) where T : BaseController
    {
        // 제네릭 타입 T를 받아서 타입을 결정
        Type type = typeof(T);

        if (type == typeof(PlayerController))
        {
            // 플레이어 캐릭터 생성
            GameObject obj = Instantiate(_dogKnightResource, spawnPos, Quaternion.identity);    // 플레이어 오브젝트 생성
            PlayerController playerController = obj.GetOrAddComponent<PlayerController>();      // PlayerController 컴포넌트 추가
            _player = playerController; // _player 변수에 할당

            // 제네릭 타입 T로 캐스팅하여 반환
            return playerController as T;
        }
        else if (type == typeof(GolemController))
        {
            // 랜덤 Gelem 프리팹
            int random;
            random = UnityEngine.Random.Range(0, 3);

            // Golem 유닛 생성
            GameObject obj = Instantiate(_golemResource[random], spawnPos, Quaternion.identity);    // 오브젝트 생성
            GolemController golemController = obj.GetOrAddComponent<GolemController>();             // GolemController 컴포넌트 추가
            Golem.Add(golemController); // 생성된 GolemController를 HashSet에 추가

            // 제네릭 타입 T로 캐스팅하여 반환
            return golemController as T;
        }
        else if (type == typeof(BossController))
        {
            // Boss 유닛 생성
            GameObject obj = Instantiate(_bossGolemResource, spawnPos, Quaternion.identity);     // 오브젝트 생성
            BossController bossController = obj.GetOrAddComponent<BossController>();        //BossController 컴포넌트 추가
            _boss = bossController; // _boss 변수에 할당

            // 제네릭 타입 T로 캐스팅하여 반환
            return bossController as T;
        }

        // 해당하는 타입이 없으면 null 반환
        return null;
    }

    // 오브젝트를 비활성화 하는 함수
    public void Despawn<T>(T obj, Vector3 despawnPos, Coroutine coroutine) where T : BaseController
    {
        // 비활성화 시 사용된 코루틴 해제
        StopCoroutine(coroutine);
        // 오브젝트 풀링 방식을 위한 비활성화
        obj.gameObject.SetActive(false);

        if (!obj.gameObject.activeSelf)
        {
            if (_coRespawnDelay == null)
            {
                _coRespawnDelay = StartCoroutine(CoRespawnDelay(despawnPos));
            }
        }
    }

    // 리스폰 딜레이용 코루틴
    private IEnumerator CoRespawnDelay(Vector3 spawnPos)
    {
        yield return _delay;

        Respawn(spawnPos);
    }

    // 오브젝트를 활성화 하는 함수
    public void Respawn(Vector3 spawnPos)
    {
        PoolManager.Instance.GetRespawnObject<GolemController>(spawnPos);

        StopCoroutine(_coRespawnDelay);
        _coRespawnDelay = null;
    }

    // 오브젝트 소멸 함수
    public void MonsterDestroy<T>(T obj) where T : BaseController
    {
        // 오브젝트 소멸
        Destroy(obj.gameObject);
    }

    // 객체 관리에서 오브젝트를 정리하는 함수
    protected override void Clear()
    {
        base.Clear();

        Golem.Clear();  // Golem HashSet 초기화 (모든 GolemController 제거)

        _player = null; // 플레이어 변수 초기화 (플레이어 삭제)
        _boss = null;   // 보스 변수 초기화 (보스 삭제)

        // Resources로 로드한 것 중 메모리에서 사용되지 않는 리소스 해제
        Resources.UnloadUnusedAssets();
    }
}