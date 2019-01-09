using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomScript : MonoBehaviour {

	// Generate a random int between min and max.
	// min is included, max is excluded.
	public int RandInt (int min, int max)
	{
		int num;

		for(int i=0; i<100; i++)
		{
			num = Random.Range(min, max);
		}

		num = Random.Range(min, max);
			
		return num;
	}

	// Generate a random float between min and max.
	// min is included, max is excluded.
	public float RandFloat (float min, float max)
	{
		float num;

		for (int i=0; i<100;i++)
		{
			num = Random.Range (min, max);
		}

		num = Random.Range(min, max);

		return num;
	}
}
