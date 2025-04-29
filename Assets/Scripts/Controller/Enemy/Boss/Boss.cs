
public class BossState
{
    private int _defaultHp;
    private int _defaultAtk;
    private int _defaultDef;
    private float _defaultSpeed;

    public int BossHp { get; set; }
    public int BossAtk { get; set; }
    public int BossDef { get; set; }
    public float BossSpeed { get; set; }

    // 기본값을 설정하는 생성자 추가
    public BossState()
    {
        // 초기값 저장
        _defaultHp = 500;
        _defaultAtk = 50;
        _defaultDef = 30;
        _defaultSpeed = 3.0f;

        // 능력치 초기화
        ResetStats();
    }

    // 능력치를 기본값으로 초기화
    public void ResetStats()
    {
        BossHp = _defaultHp;
        BossAtk = _defaultAtk;
        BossDef = _defaultDef;
        BossSpeed = _defaultSpeed;
    }
}


public class Boss
{
    public BossState BossState { get; private set; }

    public Boss()
    {
        BossState = new BossState();
    }

    // 능력치를 원래 값으로 초기화
    public void ResetStats()
    {
        BossState.ResetStats();
    }
}