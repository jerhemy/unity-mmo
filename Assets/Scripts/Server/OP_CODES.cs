namespace UnityMMOServer
{
    public enum OpCode
    {
        Login = 0x00,
        Logout = 0x01,
        Target = 0x02,
        Movement = 0x03,
        AttackToggle = 0x04,
        SpellCast = 0x05,
        UseAbility = 0x06,
    }
}