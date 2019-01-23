using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour
{
	public class Comment
	{
		public string comment;
		public int character;

		public Comment (string c, int ch)
		{
			comment = c;
			character = ch;
		}
	}

	[Header("Objects")]
	public GameObject tutorialMenu;
	public GameObject nextButton;
	public GameObject subMenu;
	public TextMeshProUGUI text;
	public TextMeshProUGUI name;
	public Image image;

	[Header("Sprites")]
	public Sprite character1;
	public Sprite character2;
	public Sprite character3;

	// Local Variables
	private int campaignLevel = 0;
	private int commentIndex = 0;
	private int nameIndex = 0;

	// List
	List<Comment> comments;
	List<Sprite> sprites;
	List<string> names;

    // Start is called before the first frame update
    void Start()
    {
		InitializeSprites ();
		InitializeNames ();
		InitializeComments ();
    }

    // Initialize the comments for the campaign.
	private void InitializeComments ()
	{
		comments = new List<Comment> () {
			new Comment( "Oh! Hello there~", 0 ), // 0
			new Comment( "Hmm? This? This is Go. It's a fantastic game! Would you like to play?", 0), // 1
			new Comment( "Great! We are playing Capture Go. The rules are simple. The first one to surround "+
				"the other players stone or stones win.", 0),// 2
			new Comment( "Let's try playing a game! I will set up the board.", 0), // 3
			new Comment( "Normally we play on an empty board, " +
				"but in this case I will give you an advantage. Black goes first.", 0), // 4
			new Comment( "Game Start!", 0), // 5
			new Comment( "Sorry, but no worries! You can try again!", 0), // 6
			new Comment( "Great job! You got it!", 0), // 7
			new Comment( "Not bad. But let's see how you do without handicap.", 1), // 8
			new Comment( "Oh, this is " + names[1] + ". He is another beginner like you.", 0), // 9
			new Comment( "You two should try playing!", 0), // 10
			new Comment( "Hehe, it won't be as easy as her little test.", 1), // 11
			new Comment( "Game Start!", 1), // 12
			new Comment( "Hehe, I knew I could win.", 1), // 13
			new Comment( "I...I lost...", 1), // 14
			new Comment( "That's okay. You can try again!", 0), // 15
			new Comment( "Heh, fine. I can play you as many times as you want.", 1), // 16
			new Comment( "Wow, you did great!", 0), // 17
			new Comment( "Hmm... I'm impressed.", 2), // 18
			new Comment( names[2] + "?!!!", 1), // 19
			new Comment( "Oh! What a surprise. When did you get here?", 0), // 20
			new Comment( "Just now. I saw that this person had won against both of you.", 2),// 21
			new Comment( "Didn't you just teach the rules today?", 2), // 22
			new Comment( "Yes. I taught the rules myself.", 0), // 23
			new Comment( "It was just luck! I'll win next time.", 1), // 24
			new Comment( "Luck or no. I would like to see for myself.", 2), // 25
			new Comment( "I will give you some starting stones.", 2), // 26
			new Comment( "Shall we begin?", 2), //27
			new Comment( "Game Start!", 2), // 28
			new Comment( "Thank you for the game. Don't worry about the result. We can play again if you'd like.", 2), // 29
			new Comment( "Well I must say, I'm impressed.", 2), // 30
			new Comment( "Oh my goodness! You did it!", 0), // 31
			new Comment( names[2] + " lost?!!", 1), // 32
			new Comment( "I must say, I had some confidence in my skills. But now...", 2), // 33
			new Comment( "I think I will revisit the basics once again. Thank you for this lesson.", 2), // 34
			new Comment( "I must say, I knew you had talent, but to beat Huaiter is most impressive!", 0), // 35
			new Comment( "Yea. I can see why I lost now. I guess it wasn't luck huh...", 1), // 36
			new Comment( "I think you have what it takes to become really strong at this game.", 2), // 37
			new Comment( "If you want to develop your skills further, I recommend climbing the IGo mountain.", 2), // 38
			new Comment( "The IGo mountain!!", 0), // 39
			new Comment( "The IGo mountain?", 1), // 40
			new Comment( "The IGO mountain is a place for the strongest Go players go to train.", 2), // 41
			new Comment( "Even climbing the mountain is tough, but you must pass several test as well to make it to the top.", 2), // 42
			new Comment( names[2] + " must think really highly of you to recommend the IGo mountain.", 0), // 43
			new Comment( "Do you think I could climb the mountain?", 1), // 44
			new Comment( "Maybe some day. For now I think you should practice more.", 2), // 45
			new Comment( "Don't underestimate the mountain. I tried to climb it myself once.", 2), // 46
			new Comment( "I neve made it to the top and was forced to come back down until I developed my skills further.", 2), // 47
			new Comment( "That's okay. I have faith in my #1 pupil.", 0), // 48
			new Comment( "You're so lucky! I hope you come back really strong!!", 1), // 49
			new Comment( "I wouldn't suggest it if I didn't think you could do it.", 2), // 50
			new Comment( "Prepare how you must and begin the climb when you are ready. I have faith you'll do fine.", 2), // 51
			new Comment( "Good luck! I wish you the best~", 0), // 52
			new Comment( "Don't forget about your first rival when you become super strong!", 1), // 53
			new Comment( "Off you go.", 2), // 54
			new Comment( "End of Chapter One." ,2)// 55
		};
	}

	private void InitializeSprites ()
	{
		sprites = new List<Sprite> () {
			character1,
			character2,
			character3
		};
	}

	private void InitializeNames ()
	{
		names = new List<string> () {
			"QiWei",
			"Foshy",
			"Huaiter"
		};
	}

	private void LoadComment ()
	{
		nextButton.SetActive (true);

		if (commentIndex < 0 || commentIndex > comments.Count)
		{
			Debug.Log ("ERROR: Index out of range.");
			nextButton.SetActive (false);
			return;
		}

		text.text = comments[commentIndex].comment;
		image.sprite = sprites[comments[commentIndex].character];
		name.text = names[comments[commentIndex].character];

		if (commentIndex == 4)
		{
			GameObject.Find ("_StoneManager").GetComponent<StoneManagerScript> ().LoadSpecificStoneSet (1);

		} else if (commentIndex == 5)
		{
			StartNewGame (1, 0);
		} else if (commentIndex == 6) {
			commentIndex = 2;
		} else if (commentIndex == 12) 
		{
			StartNewGame(0, 0);
		} else if (commentIndex == 13) {
			commentIndex++;
		}else if (commentIndex == 16) {
			commentIndex = 10;
		} else if (commentIndex == 14)
		{
			commentIndex = 16;
		} else if (commentIndex == 26)
		{
			GameObject.Find ("_StoneManager").GetComponent<StoneManagerScript> ().LoadSpecificStoneSet (2);
		} else if (commentIndex == 28)
		{
			StartNewGame (2, 1);
		} else if (commentIndex == 29)
		{
			commentIndex = 25;
		}

		commentIndex++;

		if (commentIndex == comments.Count)
		{
			nextButton.SetActive (false);
		}
	}

	private void StartNewGame (int stoneSettings, int botLevel)
	{
		nextButton.SetActive (false);
		GameObject roomManager = GameObject.FindGameObjectWithTag ("RoomManager");
		Debug.Log (nameIndex.ToString() + " : " + names.Count);
		Debug.Log (names[nameIndex]);
		roomManager.GetComponent<RoomManager> ().StartGameForCampaign (names[nameIndex], stoneSettings, botLevel);
		commentIndex--;
	}

	public void NextButtonClicked ()
	{
		LoadComment ();
	}

	public void LoadCampaign (int level)
	{
		campaignLevel = level;

		if (campaignLevel == 0)
		{
			image.sprite = sprites [nameIndex];
			name.text = names [nameIndex];;
			LoadComment();
			subMenu.SetActive (false);
		}
	}

	/// <summary>
	/// Called when a game is finished during the tutorial.
	/// </summary>
	/// <param name="won">If set to <c>true</c> won.</param>
	public void PlayedGame (bool won)
	{
		commentIndex++;
		if (won) {
			commentIndex++;
		}

		LoadComment ();
	}

	public string GetNPCName ()
	{
		return names[nameIndex];
	}
}
