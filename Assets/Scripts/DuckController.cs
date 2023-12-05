using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
public class DuckController : MonoBehaviour

{

	private StarterAssetsInputs _input;
	[SerializeField]
	private float yDuckScale;
	private float _normalyScale;
	/*private bool duck = false; // sets duck to false by default*/
	/*private Vector3 originalScale = Vector3.one; // sets the players origional scale to 1.1.1*/
	public bool Grounded = true;
	private bool canStandUp = true;
	public LayerMask HeadobstacleLayer;
	public Transform headCheck;
	
	


	// Start is called before the first frame update
	void Start()
	{
		_input = GetComponent<StarterAssetsInputs>();
		_normalyScale = transform.localScale.y;

		if (yDuckScale == 0.0f)
		{
			yDuckScale = 0.5f;
		}
	}

	// Update is called once per frame
	void Update()
	{
		OnDuck();
		CheckHeadObstacle();
		
	}

	public void OnDuck()
	{
		float newYscale;

		if (_input.duck && Grounded && (transform.localScale.y != yDuckScale))
		{
			newYscale = Mathf.Lerp(transform.localScale.y, yDuckScale, 2 * Time.deltaTime);
			Debug.Log("Ducking");
			/*duck = true;

			// Shrinks the player when ducking
			/*if (duck)
			{
				// Adjust the player's scale to 0.5)
				transform.localScale = new Vector3(transform.localScale.x, yDuckScale, transform.localScale.z);
			}*/

			if (newYscale - yDuckScale < 0.05f)
			{
				newYscale = yDuckScale;
			}
			transform.localScale = new Vector3(transform.localScale.x, yDuckScale, transform.localScale.z);
			
		}
		else if (!_input.duck && Grounded && (transform.localScale.y != _normalyScale) && canStandUp)
		{
			newYscale = Mathf.Lerp(transform.localScale.y, _normalyScale, 2 * Time.deltaTime);

			if(_normalyScale - newYscale < 0.01f)
			{
				newYscale = _normalyScale;
			}

			transform.localScale = new Vector3(transform.localScale.x, newYscale, transform.localScale.z);


			Debug.Log($"Not Ducking (Duck Input: {_input.duck})");


			


			
		}
	}

	private void CheckHeadObstacle()
    {
		if (Physics.Raycast(headCheck.position, Vector3.up, out RaycastHit hit, 1.0f, HeadobstacleLayer))
        {
			canStandUp = false;
			Debug.Log("cant stand up");
        }
        else
        {
			canStandUp = true;
        }
    

	

	}
	
    

}

