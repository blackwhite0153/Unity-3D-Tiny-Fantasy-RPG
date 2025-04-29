using UnityEngine;

public class UI_GolemHpBar : UI_Base
{
    // UI 초기화
    protected override void Initialize()
    {
        SetCanvas();
    }

    private void SetCanvas()
    {
        // 현재 GameObject에 Canvas 컴포넌트를 가져오거나 없으면 추가
        Canvas canvas = gameObject.GetOrAddComponent<Canvas>();

        // Canvas가 존재하는 경우 설정 적용
        if (canvas != null)
        {
            // UI를 화면 좌표 기준으로 렌더링
            canvas.renderMode = RenderMode.WorldSpace;
        }

        canvas.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 4.2f, 0.0f);
    }
}