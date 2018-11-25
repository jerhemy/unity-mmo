using UnityEngine;
using System.Collections;
 
public class Connection {
 
	public Vector3 start;
	public Vector3 end;
 
 
	public bool open; //walkable or not
	public bool valid = true; //if it respect grid restriction
 
	public Connection(Vector3 _start,Vector3 _end,bool _open)
	{
		start = _start;
		end = _end;
		open = _open;
 
	}
}