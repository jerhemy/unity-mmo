using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
	// Strength effects Melee Damage
	public byte STR;
	
	// Dexterity effects Ranged Damage
	public byte DEX;
	
	// Agility effects To Hit, Dodge, Block
	public byte AGI;
	
	// Constitution effects HP and Stamina Regen and Maximums
	public byte CON;
	
	// Intelligence effects Max Mana, Mana Regen, Skill Progression, Pure Caster Spell Damage
	public byte INT;
	
	// Wisdom effects Max Mana, Mana Regen, Priest Caster Spell/Heal Effectiveness
	public byte WIS;
	
	// Charisma effects Merchant purchase/sell rate, Crowd Control Effectiveness
	public byte CHR;
	
}
