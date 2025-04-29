using UnityEngine;

// ������ �⺻ ������ �����ϴ� �߻� Ŭ����
public abstract class BaseWeapon : MonoBehaviour
{
    // MonoBehaviour�� Awake()�� �������̵��Ͽ� �ʱ�ȭ �Լ� ȣ��
    private void Awake()
    {
        Initialize();
    }

    // �� ���� ��ũ��Ʈ���� �ݵ�� �����ؾ� �ϴ� �ʱ�ȭ �Լ�
    protected abstract void Initialize();
}