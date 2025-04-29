using UnityEngine;

// 유틸리티 메서드를 제공하는 클래스
public class Util
{
    // GameObject에서 특정 타입의 컴포넌트를 가져오거나, 없으면 추가하는 메서드
    public static T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        // GameObject에서 T 타입의 컴포넌트를 가져온다.
        T component = obj.GetComponent<T>();

        // 컴포넌트가 없으면 추가
        if (component == null)
            component = obj.AddComponent<T>();

        // 가져오거나 추가한 컴포넌트를 반환
        return component;
    }
}