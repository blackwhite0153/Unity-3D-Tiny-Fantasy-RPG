
public class GolemState
{
    private int _defaultHp;
    private int _defaultAtk;
    private int _defaultDef;
    private float _defaultSpeed;

    public int GolemHp { get; set; }
    public int GolemAtk { get; set; }
    public int GolemDef { get; set; }
    public float GolemSpeed { get; set; }

    // 기본값을 설정하는 생성자 추가
    public GolemState()
    {
        // 초기값 저장
        _defaultHp = 50;
        _defaultAtk = 10;
        _defaultDef = 5;
        _defaultSpeed = 2.5f;

        // 능력치 초기화
        ResetStats();
    }

    // 능력치를 기본값으로 초기화
    public void ResetStats()
    {
        GolemHp = _defaultHp;
        GolemAtk = _defaultAtk;
        GolemDef = _defaultDef;
        GolemSpeed = _defaultSpeed;
    }
}

public class Golem
{
    public GolemState GolemState { get; private set; }

    public Golem()
    {
        GolemState = new GolemState();
    }

    // 능력치를 원래 값으로 초기화
    public void ResetStats()
    {
        GolemState.ResetStats();
    }
}