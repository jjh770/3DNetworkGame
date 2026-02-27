public enum BearState
{
    Idle,       // 멈춰서 대기
    Patrol,     // 순찰 중
    Wait,       // 침입자 감지 (추격 전 경고)
    Chase,      // 추격 중
    Attack,     // 공격 중
    Hit,        // 피격 (에어본/경직 등)
    Death,      // 사망
}
