using UnityEngine;

// 무기의 기본 설정을 정의하는 추상 클래스
public abstract class BaseWeapon : MonoBehaviour
{
    // MonoBehaviour의 Awake()를 오버라이드하여 초기화 함수 호출
    private void Awake()
    {
        Initialize();
    }

    // 각 무기 스크립트에서 반드시 구현해야 하는 초기화 함수
    protected abstract void Initialize();
}