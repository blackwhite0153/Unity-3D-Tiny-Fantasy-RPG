using UnityEngine;

public class PortalController : BaseController
{
    private BossController _bossController;

    private BoxCollider _boxCollider;

    private int _bossHp;

    protected override void Initialize() { }

    private void Start()
    {
        Setting();
    }

    private void Update()
    {
        // ������ �Ҵ�� ������ Ž��
        if (_bossController == null)
        {
            GameObject bossObj = GameObject.FindGameObjectWithTag(Define.BossTag);

            if (bossObj != null)
            {
                _bossController = bossObj.GetComponent<BossController>();
            }
        }

        // �Ҵ�� ��쿡�� ü�� üũ
        if (_bossController != null)
        {
            BossHpUpdate();
        }
    }

    private void Setting()
    {
        // ������Ʈ ���� ��������
        _boxCollider = GetComponent<BoxCollider>();

        // BoxCollider ����
        _boxCollider.isTrigger = true;
        _boxCollider.center = new Vector3(0.0f, 3.0f, 0.0f);
        _boxCollider.size = new Vector3(4.0f, 4.0f, 1.0f);

        _boxCollider.enabled = false;
    }

    // ���� ü�� ������Ʈ
    private void BossHpUpdate()
    {
        _bossHp = _bossController.BossHp;

        if (_bossHp <= 0)
        {
            ClearPortalOpen();
        }
    }

    // ���� ü�� ���� �� Ŭ���� ��Ż ����
    private void ClearPortalOpen()
    {
        _boxCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Define.PlayerTag))
        {
            GameManager.Instance.IsGameClear = true;
        }
    }
}