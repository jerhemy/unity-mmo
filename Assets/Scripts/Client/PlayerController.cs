using System;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Unity.MMO.Client;
using Org.BouncyCastle.Asn1.X9;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityMMO.Manager;

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
		isRotating = Input.GetMouseButtonDown(1);
		
		if (EventSystem.current.currentSelectedGameObject == null)
		{
			if (controller.isGrounded) {
				moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
				moveDirection = transform.TransformDirection(moveDirection);
				moveDirection *= speed;
				if (Input.GetButton("Jump"))
					moveDirection.y = jumpSpeed;
             
			}
			
			moveDirection.y -= gravity * Time.deltaTime;
			controller.Move(moveDirection * Time.deltaTime);

			if (Input.GetMouseButtonDown(1))
			{
				float mouseInput = Input.GetAxis("Mouse X");
				Vector3 lookhere = new Vector3(0, mouseInput, 0);
				transform.Rotate(lookhere);
			}

//			var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
//			var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
//
//			transform.Rotate(0, x, 0);
//			transform.Translate(0, 0, z);

			if (CheckForUpdate(transform.position))
			{
				//Debug.Log($"X:{transform.position.x} Y:{transform.position.y} Z:{transform.position.z}");
				
				var position = new Position(controller.transform.position);
				byte[] obj = StructTools.RawSerialize(position);
				byte[] type = BitConverter.GetBytes((short) MessageType.Movement);

				byte[] payload = type.Concat(obj).ToArray();
				_connectionManager.Send(payload, payload.Length);
			}
		}
	
	}
}
