using System;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Unity.MMO.Client;
using Org.BouncyCastle.Asn1.X9;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
	[StructLayout(LayoutKind.Explicit, Size = 12, Pack = 1)]
	public struct Position
	{
		[FieldOffset(0)] private float x;
		[FieldOffset(4)] private float y;
		[FieldOffset(8)] private float z;
		
		[FieldOffset(12)] private float rotX;
		[FieldOffset(16)] private float rotY;
		[FieldOffset(20)] private float rotZ;
		
		public Position(Vector3 position)
		{
			x = position.x;
			y = position.y;
			z = position.z;
			
			rotX = 0;
			rotY = 0;
			rotZ = 0;
		}
		
		public Position(Vector3 position, Quaternion rotation)
		{
			x = position.x;
			y = position.y;
			z = position.z;

			rotX = rotation.x;
			rotY = rotation.y;
			rotZ = rotation.z;
		}

		public override string ToString()
		{
			return $"X:{x} Y:{y} Z:{z}";
		}
	}
	
	public GameObject ConnectionManager;
	private UnityConnectionManager _connectionManager;
	private CharacterController controller;
	
	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;

	private bool isRotating = false;
	
	void Awake()
	{
		_connectionManager = ConnectionManager.GetComponent<UnityConnectionManager>();
		controller = GetComponent<CharacterController>();
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
		if (EventSystem.current.currentSelectedGameObject == null)
		{
			var h = Input.GetAxis("Horizontal");
			var v = Input.GetAxis("Vertical");

			if (!Input.GetMouseButton(1)) // NEW
				transform.Rotate(0, h * 3.0f, 0); // Turn left/right

			// Only allow user control when on ground
			if (controller.isGrounded)
			{
				if (Input.GetMouseButton(1)) // NEW
					moveDirection = new Vector3(h, 0, v); // Strafe
				else
					moveDirection = Vector3.forward * v; // Move forward/backward

				moveDirection = transform.TransformDirection(moveDirection);
				moveDirection *= 6.0f;
				if (Input.GetButton("Jump"))
					moveDirection.y = 8.0f;
			}

			moveDirection.y -= 20.0f * Time.deltaTime; // Apply gravity
			controller.Move(moveDirection * Time.deltaTime);
			
			if (CheckForUpdate(transform.position))
			{
				Debug.Log($"X:{transform.position.x} Y:{transform.position.y} Z:{transform.position.z}");
			
				var position = new Position(controller.transform.position);
				byte[] obj = StructTools.RawSerialize(position);
				byte[] type = BitConverter.GetBytes((short) MessageType.Movement);

				byte[] payload = type.Concat(obj).ToArray();
				_connectionManager.Send(payload, payload.Length);
			}
		}
	
	}
}
