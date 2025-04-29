using UnityEngine;

// Ȯ�� �޼��带 �����ϴ� ���� Ŭ����
public static class Extionsion
{
    // GameObject�� Ư�� Ÿ���� ������Ʈ�� �������ų�, ������ �߰��ϴ� Ȯ�� �޼���
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
    {
        // GameObject���� T Ÿ���� ������Ʈ�� �����´�.
        T component = obj.GetComponent<T>();

        // ������Ʈ�� ������ �߰�
        if (component == null)
            component = obj.AddComponent<T>();

        // �������ų� �߰��� ������Ʈ ��ȯ
        return component;
    }
}