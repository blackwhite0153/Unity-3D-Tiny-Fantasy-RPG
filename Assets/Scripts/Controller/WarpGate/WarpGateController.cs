using UnityEngine;

public class WarpGateController : BaseController
{
    private GameObject _player;

    private GameObject _warpGateA;
    private GameObject _warpGateB;

    private BoxCollider _boxCollider;

    protected override void Initialize()
    {
        Setting();
    }

    private void Update()
    {
        // �÷��̾ �Ҵ�� ������ Ž��
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag(Define.PlayerTag);
        }
    }

    private void Setting()
    {
        _warpGateA = GameObject.FindGameObjectWithTag(Define.WarpGateATag);
        _warpGateB = GameObject.FindGameObjectWithTag(Define.WarpGateBTag);

        // ������Ʈ ���� ��������
        _boxCollider = GetComponent<BoxCollider>();

        // BoxCollider ����
        _boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.gameObject.CompareTag(Define.WarpGateATag) && other.CompareTag(Define.PlayerTag))
        {
            _player.transform.position = new Vector3(_warpGateB.transform.position.x + 3.0f, _warpGateB.transform.position.y, _warpGateB.transform.position.z);
        }
        if (this.gameObject.CompareTag(Define.WarpGateBTag) && other.CompareTag(Define.PlayerTag))
        {
            _player.transform.position = new Vector3(_warpGateA.transform.position.x, _warpGateA.transform.position.y, _warpGateA.transform.position.z - 3.0f);
        }
    }
}