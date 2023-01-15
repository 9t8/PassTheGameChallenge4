using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandVisual : MonoBehaviour
{
    private SpriteRenderer rend;
	public Sprite[] sprites;

	private void Start()
	{
		rend = GetComponent<SpriteRenderer>();
		rend.sprite = sprites[Random.Range(0, sprites.Length)];

		float scaleBuffer = Random.Range(0, .3f);
		transform.localScale += new Vector3(scaleBuffer, scaleBuffer, scaleBuffer);
	}
}
