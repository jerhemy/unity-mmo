using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityMMO.Models
{
	[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)] 
	public struct Entity
	{
		public int id;
        
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=16)]
		public string name;

		public SimpleVector3 loc;
		public float  orientation;

		private int currentWayPointIndex;

        
		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 4)]
		public SimpleVector3[] waypoints;


		public SimpleVector3 getWaypoint(bool getNext) 
		{
			if (getNext)
			{
				if (currentWayPointIndex == waypoints.Length - 1)
				{
					currentWayPointIndex++;
				}
				else
				{
					currentWayPointIndex = 0;
				}
			}

			return waypoints[currentWayPointIndex];
		}
	}
}