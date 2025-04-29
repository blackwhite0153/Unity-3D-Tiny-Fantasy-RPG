using UnityEngine;
using UnityEngine.UI;

public class GolemHpBar : MonoBehaviour
{
    private GolemController _golemController;
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
        _golemController = GetComponentInParent<GolemController>();

        if (_golemController != null)
        {
            MaxHp = _golemController.GolemHp;
        }

        _hpBar = GetComponent<Slider>();

        gameObject.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

        _hpBar.maxValue = MaxHp;
        _hpBar.minValue = 0;
    }

    private void HpBarUpdate()
    {
        CurrentHp = _golemController.GolemHp;

        _hpBar.value = CurrentHp;
    }
}