using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour {

	private static GameHandler _GH;

	public static GameHandler GH {
		get { return _GH; }
	}

	public int numCardsInDeck;
	public int numYesCards, numNoCards;
	public int score;
	public int bugsOut, bugsPlaced;
	public GameObject card;
	public GameObject xmark, checkmark;
	public GameObject mainMenuUI;
	public GameObject OKButton, ProceedButton, FinishButton;
	public List<Sprite> protoImages = new List<Sprite>();
	public List<GameObject> yesBugs = new List<GameObject>();
	public List<GameObject> noBugs = new List<GameObject>();
	public List<Sprite> yesImages = new List<Sprite>();
	public bool gameStarted;
	public bool correctAnswer;
	public bool answerChosen;
	public bool touchable;
	public bool animFinished;

	private GameObject currentCard;
	private GameObject petriDish;
	private GameObject leftTag, rightTag;
	private GameObject qTextObject;
	private List<GameObject> bugInitZones = new List<GameObject>();
	private GameObject zoomCard;
	private GameObject xButton;
	private Vector2 zoomCardDest;
	private Vector2 zoomCardOrg;
	private int currentCardNum;
	private int currentQuestion;
	private float lerpValue, tSmooth;
	private float MAX_LERP_VALUE;
	private Vector2 insOrigin;
	private List<Sprite> imagesInDeck = new List<Sprite>();
	private List<string[]> questions = new List<string[]>();
	private List<Transform> bugLoc = new List<Transform>();
	private Text questionTextRef;
	private string questionText;

	void Awake(){
		if (_GH != null && _GH != this){
			Destroy(this.gameObject);
		} else {
			_GH = this;
		}
	}

	// Use this for initialization
	void Start () {
		//set up playspace
		zoomCard = GameObject.Find("ZoomCard");
		zoomCardDest = GameObject.Find("ZoomCardDestination").transform.position;
		zoomCardOrg = zoomCard.transform.position;
		xButton = GameObject.Find("XButton");
		xButton.SetActive(false);
		petriDish = GameObject.Find("PetriDish");
		petriDish.SetActive(false);
		for (int i = 0; i < 3; i++){
			bugInitZones.Add(GameObject.Find("BugInitZone" + i.ToString()));
		}

		OKButton.SetActive(false);
		ProceedButton.SetActive(false);
		FinishButton.SetActive(false);
		leftTag = GameObject.Find("LeftTag");
		rightTag = GameObject.Find("RightTag");
		qTextObject = GameObject.Find("QuestionText");
		touchable = true;

		lerpValue = 0f;
		MAX_LERP_VALUE = 2f;
		bugsPlaced = 0;
		
		insOrigin = new Vector2(0f,0f);
		numYesCards = 0;
		numNoCards = 0;
		questionTextRef = qTextObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		lerpValue += Time.deltaTime;
		if (lerpValue == MAX_LERP_VALUE){
			lerpValue = 0f;
		}

		float t = map(lerpValue, 0, MAX_LERP_VALUE, 0, 1);
		tSmooth = t = t*t * (3f - 2f*t);

		if (gameStarted){
			if (bugsPlaced == bugsOut){
				OKButton.SetActive(true);
			} else {
				OKButton.SetActive(false);
			}
		}
		/*if (gameStarted && currentCardNum == (numCardsInDeck)){
			foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card")){
				Destroy(card);
			}
			questionTextRef.text = questions[currentQuestion][1];
			ShuffleDeck();
			currentCardNum = -1;
		}*/
	}

	//do everything we need to do to get the game started
	public void StartGame () {
		if (animFinished){	
			mainMenuUI.SetActive(false);
			petriDish.SetActive(true);

			gameStarted = true;
			currentCardNum = 0;
			currentQuestion = -1;
			ShuffleQuestions();
			ShuffleDeck();
			InitBugs();
			//NextCard();
			questionTextRef.text = questions[0][1];
			//questionTextRef.text = questions[currentQuestion][1];
		}
	}


	//load up collection of all macros, show on-screen
	/*public void ShowCollection(){
		for (int i = 0; i < 2; i++){
			for (int j = 0; j < 10; j++){

			}
		}
	}*/

	public void InitBugs() {
		for (int i = 0; i < 3; i++){
			string path = "Prefabs/Bugs/" + (currentCardNum + i).ToString();
			//WWW reader = new WWW(path);
			//while (!reader.isDone){}
			//filePath = Application.persistentDataPath
			currentCard = (GameObject) Instantiate(Resources.Load(path),
						   bugInitZones[i].transform.position, Quaternion.identity);
			currentCard.GetComponent<Card>().targetPos = bugInitZones[i].transform.position;
			//currentCard = (GameObject) Instantiate(card, bugInitZone.transform.position, bugInitZone.transform.rotation);
			//currentCard.GetComponent<SpriteRenderer>().sprite = imagesInDeck[currentCardNum + i];//protoImages[currentCardNum + i];
			currentCard.GetComponent<SpriteRenderer>().sortingOrder = i+1;
			currentCard.transform.position = new Vector3(currentCard.transform.position.x, 
														 currentCard.transform.position.y, 100);
		}
		bugsOut = 3;
		currentCardNum += 3;
	}

	public void NextQuestion(){
		OKButton.SetActive(true);
		ProceedButton.SetActive(false);
		currentQuestion++;
		questionTextRef.text = questions[currentQuestion][1];
		for (int i = 0; i < yesBugs.Count; i++){
			yesBugs[i].GetComponent<Card>().targetPos = bugInitZones[i].transform.position;
			Destroy(yesBugs[i].transform.GetChild(0).gameObject);
		}
		for (int i = 0; i < noBugs.Count; i++){
			noBugs[i].GetComponent<Card>().targetPos = bugInitZones[i + yesBugs.Count].transform.position;
			Destroy(noBugs[i].transform.GetChild(0).gameObject);
			//Destroy(noBugs[i]);
		}
	}

	//reads in questions from text file
	//from http://answers.unity3d.com/questions/279750/loading-data-from-a-txt-file-c.html
	private bool LoadQuestions(string fileName) {
		print(fileName);
		int qNum = 0;
		// Create a new StreamReader, tell it which file to read and what encoding the file
		// was saved as
		TextAsset questionTxt = (TextAsset)Resources.Load(fileName, typeof(TextAsset));
		print(questionTxt.text);
		string[] linesFromFile = questionTxt.text.Split('\n');
		foreach (string line in linesFromFile){
			if (line != null){
				string[] toAdd = new string[2];
		 	toAdd[0] = qNum.ToString();
		 	toAdd[1] = line;
		    questions.Add(toAdd);
			}
			qNum += 1;
		}
     /*StreamReader theReader = new StreamReader(fileName, Encoding.Default);
     // Immediately clean up the reader after this block of code is done.
     // You generally use the "using" statement for potentially memory-intensive objects
     // instead of relying on garbage collection.
     // (Do not confuse this with the using directive for namespace at the 
     // beginning of a class!)
     using (theReader)
     {
         // While there's lines left in the text file, do this:
         do
         {
             line = theReader.ReadLine();
                 
             if (line != null)
             {
                 // Do whatever you need to do with the text line, it's a string now
                 // In this example, I split it into arguments based on comma
                 // deliniators, then send that array to DoStuff()

             	//stick the question number and the question itself in a small string
             	//array, then put it into the questions list. This way we can keep track
             	//of which questions are asking what
             	string[] toAdd = new string[2];
             	toAdd[0] = qNum.ToString();
             	toAdd[1] = line;
                questions.Add(toAdd);
             }
             qNum += 1;
         }*/
         //while (line != null);
         // Done reading, close the reader and return true to broadcast success    
         //theReader.Close();
         return true;
     }

	public void ShuffleQuestions(){
		string[] toAdd = new string[2];
		LoadQuestions("Questions/QuestionText");
		currentQuestion += 1;
	}

	public void ShuffleDeck(){
		//advance current question
		currentQuestion += 1;
		//if we're just starting the game, shuffle all the cards
		if (numYesCards == 0 && numNoCards == 0){
			imagesInDeck = Resources.LoadAll("CardSprites/prototype bugs", typeof(Sprite)).Cast<Sprite>().ToList();
			numCardsInDeck = imagesInDeck.Count;
		//if not, only shuffle the yes cards
		} else {
			numCardsInDeck = numYesCards;
			imagesInDeck = yesImages;
		}
	}

	/*public void NextCard (){
		currentCardNum += 1;
		currentCard = (GameObject) Instantiate(card, insOrigin, transform.rotation);
		currentCard.GetComponent<SpriteRenderer>().sprite = imagesInDeck[currentCardNum];
		correctAnswer = CheckCard();
	}*/

	//see what the correct answer to the current card is
	public bool CheckCard (GameObject card){
		CardInfo cardInfo = card.GetComponent<CardInfo>();
		bool cardAnswer;
		int qNum = int.Parse(questions[currentQuestion][0]);

		if (qNum == 0){
			if (cardInfo.jointedLegs) cardAnswer = true;
			else cardAnswer = false;
		} else if (qNum == 1){
			if (cardInfo.numLegs == 6) cardAnswer = true;
			else cardAnswer = false;
		} else if (qNum == 2){
			if (cardInfo.portableCase) cardAnswer = true;
			else cardAnswer = false;
		} else if (qNum == 3){
			if (cardInfo.wingsOrPads) cardAnswer = true;
			else cardAnswer = false;
		} else if (qNum == 4){
			if (cardInfo.tailFilaments) cardAnswer = true;
			else cardAnswer = false;
		} else if (qNum == 5){
			if (cardInfo.analHooks) cardAnswer = true;
			else cardAnswer = false;
		} else {
			if (cardInfo.wormBody) cardAnswer = true;
			else cardAnswer = false;
		}
		/*} else if (qNum == 7){
			if (cardInfo.terminalProlegs) cardAnswer = true;
			else cardAnswer = false;
		} else if (qNum == 8){
			if (cardInfo.tailType == 0) cardAnswer = true;
			else cardAnswer = false;
		} else if (qNum == 9){
			if (cardInfo.tailType == 1) cardAnswer = true;
			else cardAnswer = false;
		} else if (qNum == 10){
			if (cardInfo.analHooks) cardAnswer = true;
			else cardAnswer = false;
		} else {
			if (cardInfo.bodyShape == 1) cardAnswer = true;
			else cardAnswer = false;
		} */

		print (card.name + ": " + cardAnswer.ToString());
		return cardAnswer;
	}

	public void CheckBugs(){
		//show checkmarks or exes for bug placements
		print("these are the yes bugs");
		for (int i = 0; i < yesBugs.Count; i++){
			print(yesBugs[i].name);
		}
		print("this has been the yes bugs");

		print("these are the no bugs");
		for (int i = 0; i < noBugs.Count; i++){
			print(noBugs[i].name);
		}
		print("this has been the no bugs");

		float correctBugs = 0; 
		for (int i = 0; i < yesBugs.Count; i++){
			Vector2 checkPosition = new Vector2(yesBugs[i].transform.position.x + 0.45f, yesBugs[i].transform.position.y + 0.35f);
			if (yesBugs[i].transform.childCount > 0){
				if (yesBugs[i].transform.GetChild(0).gameObject.name == "x(Clone)"){
					Destroy(yesBugs[i].transform.GetChild(0).gameObject);
				} else if (yesBugs[i].transform.GetChild(0).gameObject.name == "check(Clone)"){
					Destroy(yesBugs[i].transform.GetChild(0).gameObject);
				}
			}
			if (CheckCard(yesBugs[i]) == false){
				GameObject xmarkObject = (GameObject)Instantiate(xmark, checkPosition, yesBugs[i].transform.rotation);
				xmarkObject.transform.parent = yesBugs[i].transform;
			} else {
				GameObject checkmarkObject = (GameObject) Instantiate(checkmark, checkPosition, yesBugs[i].transform.rotation);
				checkmarkObject.transform.parent = yesBugs[i].transform;
				correctBugs++;
			}
		}

		for (int i = 0; i < noBugs.Count; i++){
			Vector2 checkPosition = new Vector2(noBugs[i].transform.position.x + 0.45f, noBugs[i].transform.position.y + 0.35f);
			if (noBugs[i].transform.childCount > 0){
				if (noBugs[i].transform.GetChild(0).gameObject.name == "x(Clone)"){
					Destroy(noBugs[i].transform.GetChild(0).gameObject);
				} else if (noBugs[i].transform.GetChild(0).gameObject.name == "check(Clone)"){
					Destroy(noBugs[i].transform.GetChild(0).gameObject);
				}
			}
			if (CheckCard(noBugs[i]) == true){
				GameObject xmarkObject = (GameObject)Instantiate(xmark, checkPosition, noBugs[i].transform.rotation);
				xmarkObject.transform.parent = noBugs[i].transform;
			} else {
				GameObject checkmarkObject = (GameObject) Instantiate(checkmark, checkPosition, noBugs[i].transform.rotation);
				checkmarkObject.transform.parent = noBugs[i].transform;
				correctBugs++;
			}
		}
		print("correctBugs: " + correctBugs.ToString());

		if (correctBugs == bugsPlaced){
			OKButton.SetActive(false);
			ProceedButton.SetActive(true);
		} 
	}

	public void ZoomIn(Sprite zoomedBugSprite){
		touchable = false;
		xButton.SetActive(true);
		qTextObject.SetActive(false);
		leftTag.SetActive(false);
		rightTag.SetActive(false);

		//move zoomcard up to the center of the screen
		//zoomCard.transform.position = Vector2.Lerp(originalPosition, zoomCardDest, tSmooth);
		zoomCard.transform.position = zoomCardDest;
		//change the image to that of the selected bug
		GameObject bugObject = zoomCard.transform.GetChild(0).gameObject;
		bugObject.GetComponent<SpriteRenderer>().sprite = zoomedBugSprite;
	}

	public void ZoomOut(){
		zoomCard.transform.position = zoomCardOrg;
		xButton.SetActive(false);
		qTextObject.SetActive(true);
		leftTag.SetActive(true);
		rightTag.SetActive(true);
		touchable = true;
	}

	//good old map function from processing
    public static float map (float OldValue, float OldMin, float OldMax, float NewMin, float NewMax)
    {
        
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
        
        return(NewValue);
    }
}
