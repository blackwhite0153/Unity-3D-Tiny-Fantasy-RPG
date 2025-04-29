using UnityEngine;

public class GolemHand : MonoBehaviour
{
    private CapsuleCollider _capsuleCollider;

    void Start()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();

        _capsuleCollider.isTrigger = true;
        _capsuleCollider.center = new Vector3(-20.0f, -20.0f, 0.0f);
        _capsuleCollider.radius = 80.0f;
        _capsuleCollider.height = 200.0f;
        _capsuleCollider.direction = 0;
    }
}