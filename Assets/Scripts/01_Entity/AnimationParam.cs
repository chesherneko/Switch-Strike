using UnityEngine;

public class AnimationParam
{
    //애니메이터 파라미터 해시
    //- string을 이용해서 파라미터를 전달하면 가비지가 생기며, 실수가 많이 일어남
    //  특정 string 값을 애니메이터 별로 Hash값으로 치환하여 애니메이션 파라미터로 사용
    //- static readonly: 상수처럼 다루기 위해 사용하는 키워드
    public static readonly int MOVEMENT = Animator.StringToHash("Movement");
    public static readonly int IS_MOVING = Animator.StringToHash("IsMoving");

    public static readonly int JUMP = Animator.StringToHash("Jump");
    public static readonly int FALL = Animator.StringToHash("Fall");
    public static readonly int CAN_JUMP_OR_FALL = Animator.StringToHash("CanJumpOrFall");

    public static readonly int DASH = Animator.StringToHash("Dash");

    public static readonly int IS_GROUNDED = Animator.StringToHash("IsGrounded");
    public static readonly int IS_WALL_CLING = Animator.StringToHash("IsWallCling");

    public static readonly int IS_ATTACK = Animator.StringToHash("IsAttack");
    
    public static readonly int SKILL = Animator.StringToHash("Skill");
    public static readonly int SKILL_END = Animator.StringToHash("SkillEnd");

    public static readonly int QTE = Animator.StringToHash("QTE");

    public static readonly int ATTACK_1 = Animator.StringToHash("Attack1");
    public static readonly int ATTACK_2 = Animator.StringToHash("Attack2");

    public static readonly int HIT = Animator.StringToHash("Hit");
    public static readonly int DEAD = Animator.StringToHash("Dead");
    public static readonly int SPAWN = Animator.StringToHash("Spawn");

    public static readonly int IS_CHASING = Animator.StringToHash("IsChasing");
}
