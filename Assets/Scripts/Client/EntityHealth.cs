using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
	private int health;

	public void SetHP(int value)
	{
		health = value;
	}

	public void Damage(int value)
	{
		health -= value;
	}

	public void Regen(int value)
	{
		health += value;
	}
}
