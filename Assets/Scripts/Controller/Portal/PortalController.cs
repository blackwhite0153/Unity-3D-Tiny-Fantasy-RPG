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
        // 보스가 할당될 때까지 탐색
        if (_bossController == null)
        {
            GameObject bossObj = GameObject.FindGameObjectWithTag(Define.BossTag);

            if (bossObj != null)
            {
                _bossController = bossObj.GetComponent<BossController>();
            }
        }

        // 할당된 경우에만 체력 체크
        if (_bossController != null)
        {
            BossHpUpdate();
        }
    }

    private void Setting()
    {
        // 컴포넌트 참조 가져오기
        _boxCollider = GetComponent<BoxCollider>();

        // BoxCollider 설정
        _boxCollider.isTrigger = true;
        _boxCollider.center = new Vector3(0.0f, 3.0f, 0.0f);
        _boxCollider.size = new Vector3(4.0f, 4.0f, 1.0f);

        _boxCollider.enabled = false;
    }

    // 보스 체력 업데이트
    private void BossHpUpdate()
    {
        _bossHp = _bossController.BossHp;

        if (_bossHp <= 0)
        {
            ClearPortalOpen();
        }
    }

    // 보스 체력 소진 시 클리어 포탈 오픈
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