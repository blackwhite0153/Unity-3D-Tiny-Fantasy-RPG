using UnityEngine;

public class Sword : BaseWeapon
{
    private CapsuleCollider _capsuleCollider;

    protected override void Initialize()
    {
        Setting();
    }

    private void Setting()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();

        _capsuleCollider.isTrigger = true;
        _capsuleCollider.center = new Vector3(0.0f, -0.65f, 0.0f);
        _capsuleCollider.radius = 0.2f;
        _capsuleCollider.height = 1.4f;
        _capsuleCollider.direction = 1;    // Y-Axis
    }
}