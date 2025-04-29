using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>
{
    // �� ������ ������ �ڷ�ƾ
    private Coroutine _coRespawnDelay;
    // �� ������ ������
    private WaitForSeconds _delay;

    // �÷��̾� ��Ʈ�ѷ� �ν��Ͻ�
    private PlayerController _player;
    // ���� ��Ʈ�ѷ� �ν��Ͻ�
    private BossController _boss;

    // ���ҽ� ���� ����
    private GameObject _dogKnightResource;
    private GameObject _bossGolemResource;
    private List<GameObject> _golemResource = new List<GameObject>();

    // �÷��̾� ��Ʈ�ѷ��� ���� �б� ���� ������Ƽ
    public PlayerController Player { get => _player; }
    // ���� ��Ʈ�ѷ��� ���� �б� ���� ������Ƽ
    public BossController Boss { get => _boss; }

    // �� ��ü���� �����ϴ� HashSet
    public HashSet<GolemController> Golem { get; set; } = new HashSet<GolemController>();

    // �̱��� �ʱ�ȭ (��ӵ� Singleton���� �⺻ �ʱ�ȭ ����)
    protected override void Initialize()
    {
        // Singleton �ʱ�ȭ ȣ��
        base.Initialize();

        _coRespawnDelay = null;
        _delay = new WaitForSeconds(5.0f);
    }

    // ��� ���� ������Ʈ ���ҽ��� �ε��ϴ� �Լ�
    public void ResourceAllLoad()
    {
        // Resources.Load<T>(Path)�� ����Ͽ� ������ �ε�
        _dogKnightResource = Resources.Load<GameObject>(Define.DogPolyartPath);
        _bossGolemResource = Resources.Load<GameObject>(Define.BossGolemPath);

        _golemResource.Add(Resources.Load<GameObject>(Define.HPGolemPath));
        _golemResource.Add(Resources.Load<GameObject>(Define.PBRGolemPath));
        _golemResource.Add(Resources.Load<GameObject>(Define.PolyartGolemPath));
    }

    // ���׸� Ÿ���� ����� Spawn �Լ�
    public T Spawn<T>(Vector3 spawnPos) where T : BaseController
    {
        // ���׸� Ÿ�� T�� �޾Ƽ� Ÿ���� ����
        Type type = typeof(T);

        if (type == typeof(PlayerController))
        {
            // �÷��̾� ĳ���� ����
            GameObject obj = Instantiate(_dogKnightResource, spawnPos, Quaternion.identity);    // �÷��̾� ������Ʈ ����
            PlayerController playerController = obj.GetOrAddComponent<PlayerController>();      // PlayerController ������Ʈ �߰�
            _player = playerController; // _player ������ �Ҵ�

            // ���׸� Ÿ�� T�� ĳ�����Ͽ� ��ȯ
            return playerController as T;
        }
        else if (type == typeof(GolemController))
        {
            // ���� Gelem ������
            int random;
            random = UnityEngine.Random.Range(0, 3);

            // Golem ���� ����
            GameObject obj = Instantiate(_golemResource[random], spawnPos, Quaternion.identity);    // ������Ʈ ����
            GolemController golemController = obj.GetOrAddComponent<GolemController>();             // GolemController ������Ʈ �߰�
            Golem.Add(golemController); // ������ GolemController�� HashSet�� �߰�

            // ���׸� Ÿ�� T�� ĳ�����Ͽ� ��ȯ
            return golemController as T;
        }
        else if (type == typeof(BossController))
        {
            // Boss ���� ����
            GameObject obj = Instantiate(_bossGolemResource, spawnPos, Quaternion.identity);     // ������Ʈ ����
            BossController bossController = obj.GetOrAddComponent<BossController>();        //BossController ������Ʈ �߰�
            _boss = bossController; // _boss ������ �Ҵ�

            // ���׸� Ÿ�� T�� ĳ�����Ͽ� ��ȯ
            return bossController as T;
        }

        // �ش��ϴ� Ÿ���� ������ null ��ȯ
        return null;
    }

    // ������Ʈ�� ��Ȱ��ȭ �ϴ� �Լ�
    public void Despawn<T>(T obj, Vector3 despawnPos, Coroutine coroutine) where T : BaseController
    {
        // ��Ȱ��ȭ �� ���� �ڷ�ƾ ����
        StopCoroutine(coroutine);
        // ������Ʈ Ǯ�� ����� ���� ��Ȱ��ȭ
        obj.gameObject.SetActive(false);

        if (!obj.gameObject.activeSelf)
        {
            if (_coRespawnDelay == null)
            {
                _coRespawnDelay = StartCoroutine(CoRespawnDelay(despawnPos));
            }
        }
    }

    // ������ �����̿� �ڷ�ƾ
    private IEnumerator CoRespawnDelay(Vector3 spawnPos)
    {
        yield return _delay;

        Respawn(spawnPos);
    }

    // ������Ʈ�� Ȱ��ȭ �ϴ� �Լ�
    public void Respawn(Vector3 spawnPos)
    {
        PoolManager.Instance.GetRespawnObject<GolemController>(spawnPos);

        StopCoroutine(_coRespawnDelay);
        _coRespawnDelay = null;
    }

    // ������Ʈ �Ҹ� �Լ�
    public void MonsterDestroy<T>(T obj) where T : BaseController
    {
        // ������Ʈ �Ҹ�
        Destroy(obj.gameObject);
    }

    // ��ü �������� ������Ʈ�� �����ϴ� �Լ�
    protected override void Clear()
    {
        base.Clear();

        Golem.Clear();  // Golem HashSet �ʱ�ȭ (��� GolemController ����)

        _player = null; // �÷��̾� ���� �ʱ�ȭ (�÷��̾� ����)
        _boss = null;   // ���� ���� �ʱ�ȭ (���� ����)

        // Resources�� �ε��� �� �� �޸𸮿��� ������ �ʴ� ���ҽ� ����
        Resources.UnloadUnusedAssets();
    }
}