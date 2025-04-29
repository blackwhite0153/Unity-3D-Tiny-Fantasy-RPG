using UnityEngine;
using UnityEngine.UI;

public class BossHpBar : MonoBehaviour
{
    private BossController _bossController;
    private Slider _hpBar;

    private int MaxHp;
    private int CurrentHp;

    private void Start()
    {
        Setting();
    }

    private void Update()
    {
        HpBarUpdate();
    }

    private void Setting()
    {
        _bossController = GetComponentInParent<BossController>();

        if (_bossController != null)
        {
            MaxHp = _bossController.BossHp;
        }

        _hpBar = GetComponent<Slider>();

        gameObject.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

        _hpBar.maxValue = MaxHp;
        _hpBar.minValue = 0;
    }

    private void HpBarUpdate()
    {
        CurrentHp = _bossController.BossHp;

        _hpBar.value = CurrentHp;
    }
}