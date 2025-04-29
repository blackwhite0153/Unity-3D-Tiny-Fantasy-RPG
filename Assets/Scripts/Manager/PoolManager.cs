using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    // ������Ʈ Ǯ�� �����ϴ� ��ųʸ� (Type ���� ������Ʈ ����Ʈ ����)
    Dictionary<Type, List<GameObject>> _objectPool = new Dictionary<Type, List<GameObject>>();
    // Ǯ������ ������Ʈ���� �����ϱ� ���� �θ� ������Ʈ (Hierarchy���� ���� ����)
    Dictionary<Type, GameObject> _parentObject = new Dictionary<Type, GameObject>();

    // Ư�� Ÿ���� ������Ʈ�� Ǯ���� �������ų�, ������ ���� ����
    public T GetObject<T>(Vector3 spawnPos) where T : BaseController
    {
        Type type = typeof(T);

        // Ư�� Ÿ���� ������Ʈ�� Ǯ�� ������� ����
        if (typeof(EnemyController).IsAssignableFrom(type))
        {
            // �ش� Ÿ���� ������Ʈ�� �̹� Ǯ���Ǿ� �ִ� ���
            if (_objectPool.ContainsKey(type))
            {
                // ��Ȱ��ȭ�� ������Ʈ�� ã�Ƽ� ����
                for (int i = 0; i < _objectPool[type].Count; i++)
                {
                    if (_objectPool[type][i] != null)
                    {
                        // ��Ȱ��ȭ�� ������Ʈ �߰�
                        if (!_objectPool[type][i].activeSelf)
                        {
                            _objectPool[type][i].SetActive(true);               // Ȱ��ȭ
                            _objectPool[type][i].transform.position = spawnPos; // ��ġ ����

                            // ĳ���� �� ��ȯ
                            return _objectPool[type][i].GetComponent<T>();
                        }
                    }
                }

                // ��� ������Ʈ�� ��� ���̶�� ���ο� ������Ʈ ����
                var obj = ObjectManager.Instance.Spawn<T>(spawnPos);

                if (_parentObject[type] == null)
                {
                    _parentObject.Remove(type); // �ߺ� Ű�� �����ϴ� ������ ��� ���� �� �����
                    GameObject gameObject = new GameObject(type.Name);  // Hierarchy ������ ���� �� ������Ʈ ����
                    _parentObject.Add(type, gameObject);     // �θ� ������Ʈ ���
                }

                obj.transform.parent = _parentObject[type].transform;   // ������ �θ� ������Ʈ ����
                _objectPool[type].Add(obj.gameObject);                  // ����Ʈ�� �߰�

                return obj;
            }
            else
            {
                // �ش� Ÿ���� Ǯ���� ������Ʈ�� ó�� ��û�� ���
                if (!_objectPool.ContainsKey(type))
                {
                    GameObject gameObject = new GameObject(type.Name);  // Hierarchy ������ ���� �� ������Ʈ ����
                    _parentObject.Add(type, gameObject);                // �θ� ������Ʈ ���
                }

                // ���ο� ������Ʈ ����
                var obj = ObjectManager.Instance.Spawn<T>(spawnPos);
                obj.transform.parent = _parentObject[type].transform;    // �θ� ������Ʈ ����

                // �� ����Ʈ ���� �� ������Ʈ �߰�
                List<GameObject> newList = new List<GameObject>();
                newList.Add(obj.gameObject);
                _objectPool.Add(type, newList); // ��ųʸ��� �߰�

                return obj;
            }
        }
        // �������� �ʴ� Ÿ���̸� null ��ȯ
        return null;
    }

    // Ư�� Ÿ���� ������Ʈ�� Ǯ���� ������ ������
    public T GetRespawnObject<T>(Vector3 spawnPos) where T : BaseController
    {
        Type type = typeof(T);

        // Ư�� Ÿ���� ������Ʈ�� Ǯ�� ������� ����
        if (typeof(EnemyController).IsAssignableFrom(type))
        {
            // �ش� Ÿ���� ������Ʈ�� �̹� Ǯ���Ǿ� �ִ� ���
            if (_objectPool.ContainsKey(type))
            {
                // ��Ȱ��ȭ�� ������Ʈ�� ã�Ƽ� ����
                for (int i = 0; i < _objectPool[type].Count; i++)
                {
                    if (_objectPool[type][i] != null)
                    {
                        // ��Ȱ��ȭ�� ������Ʈ �߰�
                        if (!_objectPool[type][i].activeSelf)
                        {
                            _objectPool[type][i].SetActive(true);               // Ȱ��ȭ
                            _objectPool[type][i].transform.position = spawnPos; // ��ġ ����

                            // ĳ���� �� ��ȯ
                            return _objectPool[type][i].GetComponent<T>();
                        }
                    }
                }
            }
        }
        // �������� �ʴ� Ÿ���̸� null ��ȯ
        return null;
    }

    protected override void Clear()
    {
        base.Clear();

        _objectPool.Clear();    // ������Ʈ Ǯ ������ ����
        _parentObject.Clear();  // �θ� ������Ʈ ������ ����
    }
}