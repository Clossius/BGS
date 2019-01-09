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

	// Create a list of int and randomize what number starts.
	// max must be greater than min. Max is excluded.
	public List<int> RandomIntStartList (int min, int max)
	{
		List<int> nums = new List<int> ();

		if( min >= max )
		{
			Debug.Log ("ERROR: Min greater or equal to max.");
			return null;
		}
			
		int start = Random.Range (min, max);

		for(int i=0; i<100; i++)
		{
			start = Random.Range (min, max);
		}

		for (int i=min;i<max;i++)
		{
			nums.Add (i);
		}

		List<int> newNums = new List<int> ();

		for(int i=0; i<nums.Count;i++)
		{
			if(i==0){newNums.Add(nums[start]);}
			else if (i+start < nums.Count) {newNums.Add (nums[start+i]);}
			else if (i+start >= nums.Count){newNums.Add (nums[start+i-nums.Count]);}
		}

		return newNums;
	}
}
