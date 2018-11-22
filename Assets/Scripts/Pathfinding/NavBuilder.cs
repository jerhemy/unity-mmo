using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class NavBuilder : MonoBehaviour
{
	public Collider baseCollider;
	public float raycastFromHeight;
	public List<Vector3>[,] dataStruct;
	public Transform marker;
 
	void Start()
	{
		// figure out size, roughly done for example :)
		Vector3 baseSize = baseCollider.bounds.extents * 2;
 
		// instantiate the 2d array
		dataStruct = new List<Vector3>[(int)baseSize.x,(int)baseSize.z];
		// instantiate the lists in 2d array
		for (int i = 0; i < dataStruct.GetLength(0); i++)
		{
			for (int j = 0; j < dataStruct.GetLength(1); j++)
			{
				dataStruct[i, j] = new List<Vector3>();
			}
		}
 
		//do stuff
		for(int i= 0; i<(int)baseSize.x; i++)
		{
			for(int j=0;j<(int)baseSize.z;j++)
			{
				RaycastHit[] hits;
				Ray ray = new Ray(new Vector3(i - (baseSize.x / 2), raycastFromHeight, j - (baseSize.z / 2)), Vector3.down);
				hits = Physics.RaycastAll(ray);
				for(int k=0; k<hits.Length; k++)
				{
					dataStruct[i,j].Add(hits[k].point);
					Instantiate(marker, hits[k].point, Quaternion.identity);
				}
			}
		}
	}
}