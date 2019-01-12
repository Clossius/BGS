using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Stone {
	public GameObject stone;
	public string coordinate;
	public int color;

	public Stone (GameObject _stone, string _coordinate, int _color)
	{
		stone = _stone;
		coordinate = _coordinate;
		color = _color;
	}
}

public class StoneManagerScript : MonoBehaviour {


    public GameObject stone;

    public float y = 0.025f; // height of all the stones.

	List<Stone> stones;

	// Initalize stone manager settings.
	public void InitializeStones ()
	{
		if(stones != null){ DestroyStones (); }

		stones = new List<Stone> ();
	}

	public void LoadListOfStones (List<string> moves, List<int> colors)
	{
		InitializeStones ();

		for (int i=0; i< moves.Count; i++)
		{
			CreateStone (moves[i], colors[i]);
		}
	}

	public void DestroyStones ()
	{
		for ( int i=0; i<stones.Count; i++ )
		{
			Destroy (stones[i].stone);
		}

		stones.Clear ();
	}

	// Create stone at given coordinate
	public void CreateStone (string coor, int color)
    {
		Vector3 newPos = GameObject.Find ("_GobanManager").GetComponent<CoordinateManager> ().CoordinateToVector3 (coor);

        GameObject newMove = Instantiate(stone) as GameObject;
		newMove.transform.position = newPos;
		SetStoneColor (newMove, color);

		Stone newStone = new Stone (newMove, coor, color);

		stones.Add (newStone);
    }
		
	// Set color of the stone.
	private void SetStoneColor (GameObject stone, int color)
	{
		stone.GetComponent<Renderer> ().material.color = Color.black;
		if (color == 1) {
			stone.GetComponent<Renderer> ().material.color = Color.white;
		} else if (color == 2) {
			stone.GetComponent<Renderer> ().material.color = Color.blue;
		} else if (color == 3) {
			stone.GetComponent<Renderer> ().material.color = Color.red;
		} else if (color == 4) {
			stone.GetComponent<Renderer> ().material.color = Color.green;
		} else if (color == 5) {
			stone.GetComponent<Renderer> ().material.color = Color.magenta;
		}
	}

	// Get the list of stones
	public List<Stone> GetStones ()
	{
		return stones;
	}

	// Get the string coordinate positions of all the stones.
	public List<string> GetStringCoordinates ()
	{
		List<string> coors = new List<string> ();

		for (int i=0; i<stones.Count; i++)
		{
			coors.Add (stones[i].coordinate);
		}

		return coors;
	}

	// Get a list of int colors of the moves that have been played.
	public List<int> GetIntColors ()
	{
		List<int> colors = new List<int> ();

		for (int i=0; i<stones.Count; i++)
		{
			colors.Add (stones[i].color);
		}

		return colors;
	}



}
