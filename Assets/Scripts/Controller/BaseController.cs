using UnityEngine;

// ��Ʈ�ѷ��� �⺻ ������ �����ϴ� �߻� Ŭ����
public abstract class BaseController : MonoBehaviour
{
    // MonoBehaviour�� Awake()�� �������̵��Ͽ� �ʱ�ȭ �Լ� ȣ��
    private void Awake()
    {
        Initialize();
    }

    // �� ��Ʈ�ѷ����� �ݵ�� �����ؾ� �ϴ� �ʱ�ȭ �Լ�
    protected abstract void Initialize();
}