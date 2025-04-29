using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    // 오브젝트 풀을 저장하는 딕셔너리 (Type 별로 오브젝트 리스트 관리)
    Dictionary<Type, List<GameObject>> _objectPool = new Dictionary<Type, List<GameObject>>();
    // 풀링도니 오브젝트들을 정리하기 위한 부모 오브젝트 (Hierarchy에서 정리 목적)
    Dictionary<Type, GameObject> _parentObject = new Dictionary<Type, GameObject>();

    // 특정 타입의 오브젝트를 풀에서 가져오거나, 없으면 새로 생성
    public T GetObject<T>(Vector3 spawnPos) where T : BaseController
    {
        Type type = typeof(T);

        // 특정 타입의 오브젝트만 풀링 대상으로 설정
        if (typeof(EnemyController).IsAssignableFrom(type))
        {
            // 해당 타입의 오브젝트가 이미 풀링되어 있는 경우
            if (_objectPool.ContainsKey(type))
            {
                // 비활성화된 오브젝트를 찾아서 재사용
                for (int i = 0; i < _objectPool[type].Count; i++)
                {
                    if (_objectPool[type][i] != null)
                    {
                        // 비활성화된 오브젝트 발견
                        if (!_objectPool[type][i].activeSelf)
                        {
                            _objectPool[type][i].SetActive(true);               // 활성화
                            _objectPool[type][i].transform.position = spawnPos; // 위치 설정

                            // 캐스팅 후 반환
                            return _objectPool[type][i].GetComponent<T>();
                        }
                    }
                }

                // 모든 오브젝트가 사용 중이라면 새로운 오브젝트 생성
                var obj = ObjectManager.Instance.Spawn<T>(spawnPos);

                if (_parentObject[type] == null)
                {
                    _parentObject.Remove(type); // 중복 키가 존재하는 상태일 경우 제거 후 재생성
                    GameObject gameObject = new GameObject(type.Name);  // Hierarchy 정리를 위한 빈 오브젝트 생성
                    _parentObject.Add(type, gameObject);     // 부모 오브젝트 등록
                }

                obj.transform.parent = _parentObject[type].transform;   // 정리용 부모 오브젝트 설정
                _objectPool[type].Add(obj.gameObject);                  // 리스트에 추가

                return obj;
            }
            else
            {
                // 해당 타입의 풀링된 오브젝트가 처음 요청될 경우
                if (!_objectPool.ContainsKey(type))
                {
                    GameObject gameObject = new GameObject(type.Name);  // Hierarchy 정리를 위한 빈 오브젝트 생성
                    _parentObject.Add(type, gameObject);                // 부모 오브젝트 등록
                }

                // 새로운 오브젝트 생성
                var obj = ObjectManager.Instance.Spawn<T>(spawnPos);
                obj.transform.parent = _parentObject[type].transform;    // 부모 오브젝트 설정

                // 새 리스트 생성 후 오브젝트 추가
                List<GameObject> newList = new List<GameObject>();
                newList.Add(obj.gameObject);
                _objectPool.Add(type, newList); // 딕셔너리에 추가

                return obj;
            }
        }
        // 지원되지 않는 타입이면 null 반환
        return null;
    }

    // 특정 타입의 오브젝트를 풀에서 가져와 리스폰
    public T GetRespawnObject<T>(Vector3 spawnPos) where T : BaseController
    {
        Type type = typeof(T);

        // 특정 타입의 오브젝트만 풀링 대상으로 설정
        if (typeof(EnemyController).IsAssignableFrom(type))
        {
            // 해당 타입의 오브젝트가 이미 풀링되어 있는 경우
            if (_objectPool.ContainsKey(type))
            {
                // 비활성화된 오브젝트를 찾아서 재사용
                for (int i = 0; i < _objectPool[type].Count; i++)
                {
                    if (_objectPool[type][i] != null)
                    {
                        // 비활성화된 오브젝트 발견
                        if (!_objectPool[type][i].activeSelf)
                        {
                            _objectPool[type][i].SetActive(true);               // 활성화
                            _objectPool[type][i].transform.position = spawnPos; // 위치 설정

                            // 캐스팅 후 반환
                            return _objectPool[type][i].GetComponent<T>();
                        }
                    }
                }
            }
        }
        // 지원되지 않는 타입이면 null 반환
        return null;
    }

    protected override void Clear()
    {
        base.Clear();

        _objectPool.Clear();    // 오브젝트 풀 데이터 삭제
        _parentObject.Clear();  // 부모 오브젝트 데이터 삭제
    }
}