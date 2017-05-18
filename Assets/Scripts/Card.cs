using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {

	public Color defaultColor, touchedColor;
	public Sprite image;
	public Vector2 targetPos;

	private SpriteRenderer rend;
	private Vector2 originalPos;
	private GameObject leftTarget, rightTarget;

	void Start(){
		rend = gameObject.GetComponent<SpriteRenderer>();
		originalPos = transform.position;

		leftTarget = GameObject.Find("LeftTarget");
		rightTarget = GameObject.Find("RightTarget");
	}

	void Update (){
		transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * 8f);
	}

	void OnTriggerEnter2D (Collider2D other){
		//player swipes the card into the left or right bound (for yes/no answer to question)
		if (other.gameObject.name == "LeftBound"){
			//if player swiped left and that was the correct answer
			if (GameHandler.GH.answerChosen == GameHandler.GH.correctAnswer){
				GameHandler.GH.score += 1;
			} else {
				GameHandler.GH.score -= 1;
			}

		} else if (other.gameObject.name == "RightBound"){
			//if player swiped right and that was the correct answer
			if (GameHandler.GH.answerChosen == GameHandler.GH.correctAnswer){
				GameHandler.GH.score += 1;
			} else {
				GameHandler.GH.score -= 1;
			}
			//GameHandler.GH.NextCard();
		}
		
	}


	void OnTouchDown(){
		rend.color = touchedColor;
	}

	void OnTouchUp(){
		rend.color = Color.white;
		//targetPos = originalPos;
		//rend.color = defaultColor;
	}

	void OnTouchStay(Vector2 point){
		targetPos = new Vector2(point.x, point.y);
	}

	void OnTouchExit(){
	}



}
