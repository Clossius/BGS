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

	// Get a specific Stone at the given coordinate
	public Stone GetStone (string coordinate)
	{
		Stone stone = new Stone(null, "", -1);

		for (int i=0; i<stones.Count; i++)
		{
			if(stones[i].coordinate == coordinate){stone = stones [i];}
		}

		if (stone.coordinate == "")
		{
			Debug.Log ("ERROR: No stone found at the given coordinate.");
		}

		return stone;
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

	// Remove a stone that matches the position given.
	public void RemoveMove (string move)
	{
		int index = -1;

		for (int i=0; i<stones.Count; i++)
		{
			if(stones[i].coordinate == move){index = i;}
		}

		if(index == -1){Debug.Log ("ERROR: Move not found."); return;}

		RemoveStoneAt (index);
	}

	// Remove multiple moves that match the coordinates given.
	public void RemoveMultipleMoves (List<string> moves)
	{
		for (int i=0; i<moves.Count; i++)
		{
			for (int g=0; g<stones.Count; g++)
			{
				if (moves[i] == stones[g].coordinate)
				{
					RemoveStoneAt (g);
					g = 0;
				}
			}
		}
	}

	// Remove a stone at a given index
	public void RemoveStoneAt (int index)
	{
		if(stones[index] == null)
		{
			Debug.Log ("ERROR: Nothing found at index.");
			return;
		}
		Debug.Log ("Removing Stone at " + stones[index].coordinate);
		Destroy (stones[index].stone);
		stones.RemoveAt (index);
	}

	// Remove the stones that have 0 liberties that are
	// of the color provided.
	public void RemoveCaptureColor (int color, int boardSize)
	{
		for (int i=0; i<stones.Count;i++)
		{
			if (stones[i].color == color)
			{
				int liberty = this.GetComponent<LibertyManager> ().GetLiberties (stones[i].coordinate, stones, boardSize);

				if(liberty == 0)
				{
					List<string> moves = this.GetComponent<LibertyManager> ().GetStringOfMoves (stones[i].coordinate, stones, boardSize);

					RemoveMultipleMoves (moves);

					i = 0;
				}
			}
		}
	}
}
