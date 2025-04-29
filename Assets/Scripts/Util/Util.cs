using UnityEngine;

// ��ƿ��Ƽ �޼��带 �����ϴ� Ŭ����
public class Util
{
    // GameObject���� Ư�� Ÿ���� ������Ʈ�� �������ų�, ������ �߰��ϴ� �޼���
    public static T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        // GameObject���� T Ÿ���� ������Ʈ�� �����´�.
        T component = obj.GetComponent<T>();

        // ������Ʈ�� ������ �߰�
        if (component == null)
            component = obj.AddComponent<T>();

        // �������ų� �߰��� ������Ʈ�� ��ȯ
        return component;
    }
}