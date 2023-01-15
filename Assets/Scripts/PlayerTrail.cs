using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
    public GameObject trail;

    public float startTimer;
    private float timer;

	private void Update()
	{
		if(timer <= 0){ 
		
			Instantiate(trail, transform.position, Quaternion.identity);
			timer = startTimer;
		} else{ 
			timer -= Time.deltaTime;
		}
	}
}
