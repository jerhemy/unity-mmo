using UnityEngine;
using System.Collections;
 
public class Node {
 
	public Vector3 worldPosition;
	public Connection[] connections;
	public bool open;
 
 
	public Node(Vector3 _worldPos,bool _open)
	{
		worldPosition = _worldPos;
		open = _open;
		connections = new Connection[4];
	}
}