using UnityEngine;

public class UI_GolemHpBar : UI_Base
{
    // UI �ʱ�ȭ
    protected override void Initialize()
    {
        SetCanvas();
    }

    private void SetCanvas()
    {
        // ���� GameObject�� Canvas ������Ʈ�� �������ų� ������ �߰�
        Canvas canvas = gameObject.GetOrAddComponent<Canvas>();

        // Canvas�� �����ϴ� ��� ���� ����
        if (canvas != null)
        {
            // UI�� ȭ�� ��ǥ �������� ������
            canvas.renderMode = RenderMode.WorldSpace;
        }

        canvas.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 4.2f, 0.0f);
    }
}