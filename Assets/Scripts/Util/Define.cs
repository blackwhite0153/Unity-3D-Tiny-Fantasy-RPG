using UnityEngine;

public class Define
{
#region Player Animation Name

    public const string Hit = "Hit";

#endregion

#region Animation Parameters

    public const string IsMove = "IsMove";
    public const string IsRun = "IsRun";
    public const string IsAttack = "IsAttack";
    public const string IsDefend = "IsDefend";
    public const string IsDie = "IsDie";

    public const string Attack1Trigger = "Attack1";
    public const string Attack2Trigger = "Attack2";
    public const string HitTrigger = "Hit";
    public const string DieTrigger = "Die";

    public readonly static int ComboCount = Animator.StringToHash("ComboCount");
    public readonly static int NextCombo = Animator.StringToHash("NextCombo");

#endregion

#region Input

    public const string Horizontal = "Horizontal";
    public const string Vertical = "Vertical";
    public const string LeftShift = "KeyCode.LeftShift";

#endregion

#region Path

    public const string DogPBRPath = "Prefabs/DogKnight/DogPBR";
    public const string DogPolyartPath = "Prefabs/DogKnight/DogPolyart";

    public const string BossGolemPath = "Prefabs/Boss/Boss_Golem";

    public const string HPGolemPath = "Prefabs/Golem/HP_Golem";
    public const string PBRGolemPath = "Prefabs/Golem/PBR_Golem";
    public const string PolyartGolemPath = "Prefabs/Golem/Polyart_Golem";

    public const string BasicSlashBluePath = "Prefabs/Effect/Basic Slash Blue";
    public const string MultipleSlashPath = "Prefabs/Effect/Multiple Slash 2";

#endregion

#region Tag

    public const string Ground = "Ground";

    public const string PlayerTag = "Player";
    public const string PlayerAttackAreaTag = "PlayerAttackArea";
    public const string PlayerAttackSlashOne = "AttackSlash1";
    public const string PlayerAttackSlashTwo = "AttackSlash2";

    public const string BossTag = "Boss";

    public const string GolemTag = "Golem";
    public const string GolemLeftAttackAreaTag = "GolemLeftAttackArea";
    public const string GolemRightAttackAreaTag = "GolemRightAttackArea";

    public const string WarpGateATag = "WarpGateA";
    public const string WarpGateBTag = "WarpGateB";

#endregion

#region Layer

    public const string PlayerLayer = "Player";
    public const string EnemyLayer = "Enemy";
    public const string ObstacleLayer = "Obstacle";

    #endregion

#region Object Name

    public const string Attack_Slash_One_Object = "Attack Slash 1";
    public const string Attack_Slash_Two_Object = "Attack Slash 2";

    public const string Hand_L_Object = "Hand_L";
    public const string Hand_R_Object = "Hand_R";

#endregion
}