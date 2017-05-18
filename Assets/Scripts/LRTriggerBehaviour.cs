using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRTriggerBehaviour : MonoBehaviour {
	public GameObject leftTag, rightTag;

	private GameObject leftIndicator, rightIndicator;
	private float leftAlpha, rightAlpha;
	private Color toChangeLeft, toChangeRight;

	// Use this for initialization
	void Start () {
		leftAlpha = 0.8f;
		rightAlpha = 0.75f;
		
		leftTag.SetActive(false);
		rightTag.SetActive(false);
		leftIndicator = GameObject.Find("LeftTrigger");
		rightIndicator = GameObject.Find("RightTrigger");
		toChangeLeft = leftIndicator.GetComponent<SpriteRenderer>().color;
		toChangeRight = rightIndicator.GetComponent<SpriteRenderer>().color;
	}
	
	// Update is called once per frame
	void Update () {
		leftIndicator.GetComponent<SpriteRenderer>().color = Color.Lerp(leftIndicator.GetComponent<SpriteRenderer>().color, toChangeLeft, Time.deltaTime * 2f);
		rightIndicator.GetComponent<SpriteRenderer>().color = Color.Lerp(rightIndicator.GetComponent<SpriteRenderer>().color, toChangeRight, Time.deltaTime * 2f);
	
		//if there are bugs in the yes column, turn it green, otherwise make
		//it transparent
		if (GameHandler.GH.yesBugs.Count == 0){
			toChangeLeft.a = 0f;
			leftTag.SetActive(false);
		} else {
			toChangeLeft.a = leftAlpha;
			leftTag.SetActive(true);
		}

		//if there are bugs in the no column, turn it red, otherwise make
		//it transparent
		if (GameHandler.GH.noBugs.Count == 0){
			toChangeRight.a = 0f;
			rightTag.SetActive(false);
		} else {
			toChangeRight.a = rightAlpha;
			rightTag.SetActive(true);
		}
	}

	void OnTriggerEnter2D (Collider2D other){
		if (other.gameObject.tag == "Bug"){
			if (this.gameObject.name == "LeftTrigger"){
				GameHandler.GH.yesBugs.Add(other.gameObject);
			} else if (this.gameObject.name == "RightTrigger"){
				GameHandler.GH.noBugs.Add(other.gameObject);
			}
			GameHandler.GH.bugsPlaced++;
		}
	}

	void OnTriggerExit2D (Collider2D other){
		if (other.gameObject.tag == "Bug"){
			if (this.gameObject.name == "LeftTrigger"){
				if (GameHandler.GH.yesBugs.Contains(other.gameObject)){
					GameHandler.GH.yesBugs.Remove(other.gameObject);
				}
			} else if (this.gameObject.name == "RightTrigger"){
				if (GameHandler.GH.noBugs.Contains(other.gameObject)){
					GameHandler.GH.noBugs.Remove(other.gameObject);
				}
			}
			GameHandler.GH.bugsPlaced--;
		}
	}
}
