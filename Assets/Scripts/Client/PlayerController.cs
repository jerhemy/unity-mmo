using System;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.MMO.Client;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityMMO.Manager;
using UnityMMO.Models;

public class PlayerController : MonoBehaviour
{

	
	public GameObject ConnectionManager;
	private UnityConnectionManager _connectionManager;
	private CharacterController controller;
	
	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;

	private bool isRotating = false;
	
	public float mouseSensitivity = 100.0f;
	public float clampAngle = 80.0f;
 
	private float rotY = 0.0f; // rotation around the up/y axis
	private float rotX = 0.0f; // rotation around the right/x axis
	
	void Awake()
	{
		_connectionManager = ConnectionManager.GetComponent<UnityConnectionManager>();
		controller = GetComponent<CharacterController>();
	}

	void Start()
	{
		Vector3 rot = transform.localRotation.eulerAngles;
		rotY = rot.y;
		rotX = rot.x;
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
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = -Input.GetAxis("Mouse Y");
 
		rotY += mouseX * mouseSensitivity * Time.deltaTime;
		rotX += mouseY * mouseSensitivity * Time.deltaTime;
 
		rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
 
		Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
		transform.rotation = localRotation;
		
		isRotating = Input.GetMouseButtonDown(1);
		
		if( Input.GetMouseButtonDown(0) )
		{
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit hit;
         
			if( Physics.Raycast( ray, out hit, 100 ) )
			{
				Debug.Log( hit.transform.gameObject.name );
			}
		}
		
		if (EventSystem.current.currentSelectedGameObject == null)
		{
			if (controller.isGrounded)
			{

				var horizontalMove = 0f;
				var verticalMove = 0f;
				
				if (Input.GetKey(KeyCode.LeftArrow))
				{
					horizontalMove = -1f;
					//transform.position += Vector3.left * speed * Time.deltaTime;
				}
				if (Input.GetKey(KeyCode.RightArrow))
				{
					horizontalMove = 1f;
					//transform.position += Vector3.right * speed * Time.deltaTime;
				}
				if (Input.GetKey(KeyCode.UpArrow))
				{
					verticalMove = 1f;
					//transform.position += Vector3.up * speed * Time.deltaTime;
				}
				if (Input.GetKey(KeyCode.DownArrow))
				{
					verticalMove = -1f;
					//transform.position += Vector3.down * speed * Time.deltaTime;
				}
				
				
				moveDirection = new Vector3(horizontalMove, 0, verticalMove);
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
				_connectionManager.Send(position, MessageType.Movement);
			}
		}
	
	}
}
