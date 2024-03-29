using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedShip : Ship
{
	Rigidbody2D rigidbody;

	[SerializeField] GameObject ArrivedShipPrefab;
	[Header("Movement settings")]
	[SerializeField] float targetSpeed;
	[SerializeField] float slowdownDistance;
 	[SerializeField] [Range(0,1)] float forwardAcceleration, sideDrag;
	Transform target;
	[SerializeField] float rotationSmoothness;
	[Header("Point settings")]
	[SerializeField] float sinkCargoAmount;
	[SerializeField] float arriveCargoAmount;
	[SerializeField] Poppup poppupPrefab;
	bool arrived = false;
	void Start()
	{
		rigidbody = GetComponent<Rigidbody2D>();
	}
	public override void SetTarget(Transform newTarget){
		target = newTarget;
	}
	public override void Destroyed(){
		if(arrived){return;}//Should not be necessary but prevents last second destruction
		GameManager.instance.ChangeCargo(sinkCargoAmount);
		Instantiate(poppupPrefab, transform.position, Quaternion.identity).SetPoints((int)sinkCargoAmount);
	}

	void Update()
	{
		if(arrived){return;}
		Vector2 targetDelta = target.position - transform.position;
		if(targetDelta.magnitude < 1f){
			GameManager.instance.ChangeCargo(arriveCargoAmount);
			Instantiate(ArrivedShipPrefab, transform.position, transform.rotation);
			Instantiate(poppupPrefab, transform.position, Quaternion.identity).SetPoints((int)arriveCargoAmount);
			Destroy(gameObject);
			return;
		}


		float forwardComponent = Vector2.Dot(rigidbody.velocity, transform.up);
		float tangentComponent = Vector2.Dot(rigidbody.velocity, transform.right);
		forwardComponent = Mathf.Lerp(forwardComponent, targetSpeed* Mathf.Min(targetDelta.magnitude/slowdownDistance,1), 1 - Mathf.Pow(1-forwardAcceleration,Time.deltaTime));
		tangentComponent = Mathf.Lerp(tangentComponent, 0, 1 - Mathf.Pow(1-sideDrag,Time.deltaTime));
		rigidbody.velocity = forwardComponent*transform.up + tangentComponent*transform.right;

		float targetAngle = Mathf.Atan2(targetDelta.y,targetDelta.x)*Mathf.Rad2Deg - 90f;
		float curAngle = transform.eulerAngles.z;
		curAngle = Mathf.LerpAngle(curAngle, targetAngle, (1 - Mathf.Pow(rotationSmoothness,Time.deltaTime)));
		transform.rotation = Quaternion.Euler(0,0,curAngle);
	}

	/*
		float targetAngle = Mathf.Atan2(rigidbody.velocity.y,rigidbody.velocity.x)*Mathf.Rad2Deg - 90f;
		float curAngle = transform.eulerAngles.z;
		curAngle = Mathf.LerpAngle(curAngle, targetAngle, rigidbody.velocity.magnitude* (1 - Mathf.Pow(rotationSmoothness,Time.deltaTime)));
		transform.rotation = Quaternion.Euler(0,0,curAngle);
	*/
}
