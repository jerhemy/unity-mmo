
using System;

public class Classes {

    public static char[] GetClassIDName(UInt32 classId, byte level = 0)
    {
        return new char[1];
    }
    
    public static char[] GetPlayerClassName(UInt32 player_class_value, byte level = 0)
    {
        return new char[1];
    }

    public static UInt32 GetPlayerClassValue(byte classId)
    {
        return 1;
    }
    
    public static UInt32 GetPlayerClassBit(byte classId)
    {
        return 1;
    }

    public static byte GetClassIDFromPlayerClassValue(UInt32 player_class_value)
    {
        return 0x01;
    }
    
    public static byte GetClassIDFromPlayerClassBit(UInt32 player_class_bit)
    {
        return 0x01;
    }

    public static bool IsFighterClass(byte classId)
    {
        return false;
    }
    
    public static bool IsSpellFighterClass(byte classId)
    {
        return false;
    }
    
    public static bool IsNonSpellFighterClass(byte classId)
    {
        return false;
    }
    
    public static bool IsCasterClass(byte classId)
    {
        return false;
    }
    
    public static bool IsINTCasterClass(byte classId)
    {
        return false;
    }

    public static bool IsWISCasterClass(byte classId)
    {
        return false;
    }

    public static bool IsPlateClass(byte class_id)
    {
        return false;
    }

    public static bool IsChainClass(byte class_id)
    {
        return false;
    }

    public static bool IsLeatherClass(byte class_id)
    {
        return false;
    }

    public static bool IsClothClass(byte class_id)
    {
        return false;
    }

    public static byte ClassArmorType(byte class_id)
    {
        return 0x01;
    }
}
