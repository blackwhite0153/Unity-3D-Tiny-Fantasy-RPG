using UnityEngine;

// ���׸� �̱��� Ŭ������ �����ϴ� Ŭ����
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // �̱��� �ν��Ͻ��� �����ϴ� ���� ����
    protected static T _instance = null;

    // �̱��� �ν��Ͻ��� ��ȯ�ϴ� ������Ƽ
    public static T Instance
    {
        get
        {
            // �ν��Ͻ��� ������ ����
            if (_instance == null)
            {
                // Managers ��� �̸��� ���� ������Ʈ ã��
                GameObject manager = GameObject.Find("Managers");

                // ������ ���Ӱ� �����ϰ� ���� ����Ǿ �������� �ʵ��� ����
                if (manager == null)
                {
                    manager = new GameObject("Managers");
                    DontDestroyOnLoad(manager);
                }

                // ���� ������ T Ÿ���� ������Ʈ�� ã�Ƽ� �Ҵ�
                _instance = FindAnyObjectByType<T>();

                // �׷��� ������ �� GameObject�� �����Ͽ� T Ÿ�� ������Ʈ�� �߰�
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    T component = obj.AddComponent<T>();

                    // ���� ���� ������Ʈ�� "Managers"�� �ڽ����� ����
                    obj.transform.parent = manager.transform;

                    // �ν��Ͻ� �Ҵ�
                    _instance = component;
                }
            }
            return _instance;
        }
    }

    // MonoBehaviour�� Awake()�� �������̵��Ͽ� �ʱ�ȭ �Լ� ȣ��
    private void Awake()
    {
        Initialize();
    }

    // �̱��� �ʱ�ȭ �Լ� (��� ����)
    protected virtual void Initialize()
    {

    }

    // �� ���� �� ���� �۾��� �����ϴ� ���� �Լ� (����Ͽ� ������ ����)
    protected virtual void Clear()
    {

    }
}