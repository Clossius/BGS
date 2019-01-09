using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class DrawLineScript : MonoBehaviour {

    private LineRenderer lineRenderer;
    private float counter;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private float lineWidth = 0.025f;

	private int boardSize = 3; // Number of lines on the goban
    private int maxBoardSize = 31;
    public float distance = 0.25f; // Distance between lines.

    public GameObject lineDrawer;
    private List<GameObject> lines;

    public GameObject starPoint;
    public GameObject goban;

	public GameObject coordinateText;
	private List<GameObject> coordinates;

	public GameObject cam;

	public int GetMaxBoardSize ()
	{
		return maxBoardSize;
	}

	// Initialize everything and set up the goban.
	public void MakeGoban ()
	{
		lines = new List<GameObject>();
		coordinates = new List<GameObject> ();

		boardSize = GameObject.Find ("_GameSettings").GetComponent<GameSettings> ().game.boardSize;

		DrawLines();
		ResizeGoban();
		MoveCamera();
		this.GetComponent<CoordinateManager> ().CreatePoints (distance, boardSize);
		CreateCoordinates ();

	}

	// Clear the list of lines
	public void DestroyLines ()
	{
		for (int i=0; i<lines.Count; i++)
		{
			Destroy (lines[i]);
		}

		lines.Clear ();

		for (int i=0;i<coordinates.Count;i++)
		{
			Destroy (coordinates [i]);
		}

		coordinates.Clear ();

		this.GetComponent<CoordinateManager> ().DestroyCoordinates ();
	}

	// Create the letters and numbers on the goban
	void CreateCoordinates ()
	{
		float startPoint = distance * ((boardSize - 1) / 2);

		// Make letters on sides.
		for(int i=0;i<boardSize;i++)
		{
			GameObject coor = Instantiate (coordinateText, new Vector3 (0.0f, 0.0f, 0.0f), Quaternion.Euler(90.0f, 0.0f, 0.0f)) as GameObject;
			coor.transform.SetParent (this.transform, false);


			float posX = (startPoint*(-1)) + (i*(distance));
			float posZ = (startPoint*(-1)) - (distance * 0.75f);
			float posY = -10f;

			float width = distance * 0.75f;
			float height = distance * 0.75f;

			string text = this.GetComponent<CoordinateManager> ().GetLetter (i);

			coor.transform.localPosition = new Vector3 (posX, posY, posZ);
			TextMeshPro tmp = coor.GetComponent<TextMeshPro> ();
			tmp.text = text;
			RectTransform rc = coor.GetComponent<RectTransform> ();
			rc.localScale = new Vector3 (width, height, 0.25f);

			coordinates.Add (coor);

			coor = Instantiate (coordinateText, new Vector3 (0.0f, 0.0f, 0.0f), Quaternion.Euler(90.0f, 0.0f, 0.0f)) as GameObject;
			coor.transform.SetParent (this.transform, false);

			posZ = posZ * (-1);

			coor.transform.localPosition = new Vector3 (posX, posY, posZ);
			tmp = coor.GetComponent<TextMeshPro> ();
			tmp.text = text;
			rc = coor.GetComponent<RectTransform> ();
			rc.localScale = new Vector3 (width, height, 0.25f);

			coordinates.Add (coor);
		}

		// Make numbers on sides.
		for (int i = 0; i < boardSize; i++) {
			GameObject coor = Instantiate (coordinateText, new Vector3 (0.0f, 0.0f, 0.0f), Quaternion.Euler(90.0f, 0.0f, 0.0f)) as GameObject;
			coor.transform.SetParent (this.transform, false);


			float posZ = (startPoint*(-1)) + (i*(distance));
			float posX = (startPoint*(-1)) - (distance * 0.75f);
			float posY = -10f;

			float width = distance * 0.75f;
			float height = distance * 0.75f;

			string text = (i+1).ToString();

			coor.transform.localPosition = new Vector3 (posX, posY, posZ);
			TextMeshPro tmp = coor.GetComponent<TextMeshPro> ();
			tmp.text = text;
			RectTransform rc = coor.GetComponent<RectTransform> ();
			rc.localScale = new Vector3 (width, height, 0.25f);

			coordinates.Add (coor);

			posX = posX * (-1);

			coor = Instantiate (coordinateText, new Vector3 (0.0f, 0.0f, 0.0f), Quaternion.Euler(90.0f, 0.0f, 0.0f)) as GameObject;
			coor.transform.SetParent (this.transform, false);

			coor.transform.localPosition = new Vector3 (posX, posY, posZ);
			tmp = coor.GetComponent<TextMeshPro> ();
			tmp.text = text;
			rc = coor.GetComponent<RectTransform> ();
			rc.localScale = new Vector3 (width, height, 0.25f);

			coordinates.Add (coor);


		}


	}

    // Set Camera position
    void SetCameraPos (Vector3 pos)
    {
		cam.transform.position = pos;
    }

    // Set the camera position relative to the board size
    void MoveCamera ()
    {
        Vector3 pos = new Vector3(0.0f, 4f, 0.0f); // Default position.
        SetCameraPos(pos);

        int lines = (boardSize - 1) / 2;
		// Adjust camera distance depending on goban size.
        float dis = lines / 2.2f;
		if (lines < 7) {dis = lines/2;}
		if (lines < 6) {dis = lines/1.75f;}
		if (lines == 1) {dis = lines;}
        pos = new Vector3(0.0f, dis, 0.0f);
        SetCameraPos(pos);
    }

    // Changes the scale of the goban game object
    // to the given scale.
    void SetGobanScale (Vector3 size)
    {
        goban.transform.localScale = size;
    }

    // Change the goban size relative to
    // the boardSize.
    void ResizeGoban ()
    {
        Vector3 size = new Vector3(5f, 1f, 5f); // Default size.

        int lines = (boardSize - 1) / 2;
        float scale = lines * 0.56f;
        size = new Vector3(scale, 1f, scale);

        SetGobanScale (size);
    }

    // Create a star point at a given position.
    void CreateStarPoint (Vector3 pos)
    {
        GameObject point = Instantiate(starPoint) as GameObject;
        point.transform.position = pos;

		lines.Add (point);
    }

    // Draw the star points
    void DrawStarPoints ()
    {
        float y = 0.0001f; // Set height of starpoint in space.

        // Tengen
        if (boardSize > 5)
        {
            Vector3 pos = new Vector3(0.0f, y, 0.0f);
            CreateStarPoint(pos);
        }

        // Corner star points
        if (boardSize > 7)
        {
            int linesPerSide = (boardSize - 1) / 2;
            float point = distance * (linesPerSide - 3); // Corner star point positions

            // draw 3-3 star points if
            // the board size is below a certain size.
            if (boardSize < 13)
            {
                point = distance * (linesPerSide - 2);
            }
            // top right corner.
            Vector3 pos = new Vector3(point, y, point);
            CreateStarPoint(pos);

            // bottom right corner
            pos = new Vector3(point, y, (point*(-1)));
            CreateStarPoint(pos);

            // top left corner
            pos = new Vector3((point * (-1)), y, point);
            CreateStarPoint(pos);

            // bottom left corner.
            pos = new Vector3((point * (-1)), y, (point * (-1)));
            CreateStarPoint(pos);
            
            // Side star points
            if (boardSize > 13)
            {
                // top side
                pos = new Vector3(0.0f, y, point);
                CreateStarPoint(pos);

                // bottom side
                pos = new Vector3(0.0f, y, (point * (-1)));
                CreateStarPoint(pos);

                // right side
                pos = new Vector3(point, y, 0.0f);
                CreateStarPoint(pos);

                // left side
                pos = new Vector3((point * (-1)), y, 0.0f);
                CreateStarPoint(pos);
            }
        }
    }
	
    // Draw a line between the points given.
	void DrawLine (Vector3 start, Vector3 end)
    {
        GameObject lineGO = Instantiate(lineDrawer) as GameObject;
        lineRenderer = lineGO.GetComponent<LineRenderer>();
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        lines.Add(lineGO);
    }

    // Draw lines on the goban
    void DrawLines ()
    {
        if (boardSize < 3) { boardSize = 3; }
        else if (boardSize > maxBoardSize) { boardSize = maxBoardSize; }
        int linesToDraw = (boardSize - 1) / 2; // Automatically makes int odd.

        List<Vector3> points = new List<Vector3>();

        // Put the points to draw on in a list
        // then draw the lines.
        for (int i=0; i<=linesToDraw;i++)
        {
            // Create the points to draw between.
            for (int g = 0; g <= linesToDraw; g++)
            {
                float pointX = distance * i;
                float pointZ = distance * g;

                Vector3 start;
                Vector3 end;

                if (i < linesToDraw)
                {
                    // side lines

                    // positive z
                    start = new Vector3(((distance * linesToDraw) * (-1)), 0.0f, pointZ);
                    end = new Vector3((distance * linesToDraw), 0.0f, pointZ);
                    points.Add(start);
                    points.Add(end);

                    // neagative z
                    start = new Vector3(((distance * linesToDraw) * (-1)), 0.0f, (pointZ*(-1)));
                    end = new Vector3((distance * linesToDraw), 0.0f, (pointZ*(-1)));
                    points.Add(start);
                    points.Add(end);
                }

                if (g < linesToDraw)
                {
                    // verticle lines

                    // positive x
                    start = new Vector3(pointX, 0.0f, ((distance * linesToDraw) * (-1)));
                    end = new Vector3(pointX, 0.0f, (distance * linesToDraw));
                    points.Add(start);
                    points.Add(end);

                    // negative x
                    pointX = pointX * (-1);
                    start = new Vector3(pointX, 0.0f, ((distance * linesToDraw) * (-1)));
                    end = new Vector3(pointX, 0.0f, (distance * linesToDraw));
                    points.Add(start);
                    points.Add(end);
                }

            }
            
        }

        // Draw the lines
        for (int i=0; i<points.Count;i++)
        {
            DrawLine(points[i], points[i+1]);
            i++;
        }

        // Draw the star points after the lines are drawn.
        DrawStarPoints();
    }
}
