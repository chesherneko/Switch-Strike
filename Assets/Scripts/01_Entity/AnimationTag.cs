using UnityEngine;

public class AnimationTag
{
    //애니메이터 상태 Tag
    //- 애니메이터의 상태 마다 지정할 수 있는 string tag
    //- 컴포넌트에서는 해당 정보를 읽어서 현재 애니메이션 상태를 추측 가능
    public const string ANIM_TAG_IDLE = "Idle";

    public const string ANIM_TAG_MOVEMENT = "Movement";

    public const string ANIM_TAG_JUMP = "Jump";
    public const string ANIM_TAG_DASH = "Dash";
    public const string ANIM_TAG_WALL_CLING = "WallCling";

    public const string ANIM_TAG_ATTACK = "Attack";
    public const string ANIM_TAG_ATTACK_END = "AttackEnd";
    public const string ANIM_TAG_SKILL = "Skill";
    public const string ANIM_TAG_SKILL_END = "SkillEnd";
    public const string ANIM_TAG_QTE = "QTE";

    public const string ANIM_TAG_HIT = "Hit";
    public const string ANIM_TAG_DEAD = "Dead";
    public const string ANIM_TAG_SPAWN = "Spawn";

    public const string ANIM_TAG_PATROL = "Patrol";
    public const string ANIM_TAG_CHASE = "Chase";
}
