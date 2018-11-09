using System.Net.Sockets;
using System.Runtime.InteropServices;
using Unity.MMO.Client;
using Org.BouncyCastle.Asn1.X9;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[StructLayout(LayoutKind.Explicit, Size = 12, Pack = 1)]
	public struct Position
	{
		[FieldOffset(0)]
		public float X;
		[FieldOffset(4)]
		public float Y;
		[FieldOffset(8)]
		public float Z;
	}
	
	public GameObject ConnectionManager;
	private ConnectionManager _connectionManager;
	
	void Awake()
	{
		_connectionManager = ConnectionManager.GetComponent<ConnectionManager>();
	}

	private Vector3 oldVal;
	bool CheckForUpdate(Vector3 newVal)
	{
		if (oldVal != newVal)
		{
			oldVal = newVal;
			return true;
		}

		return false;
	}
	
	void Update()
	{
		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

		transform.Rotate(0, x, 0);
		transform.Translate(0, 0, z);
		
		Debug.Log($"X:{transform.position.x} Y:{transform.position.y} Z:{transform.position.y}");
		
		if (CheckForUpdate(transform.position))
		{
			var position = new Position {X = transform.position.x, Y = transform.position.y, Z = transform.position.z};
			Debug.Log($"X:{transform.position.x} Y:{transform.position.y} Z:{transform.position.y}");
			var payload = StructTools.RawSerialize(position);
			_connectionManager.Send(payload, payload.Length);
		}

	}
}
