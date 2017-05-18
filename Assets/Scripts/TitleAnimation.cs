using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleAnimation : MonoBehaviour {
	public GameObject has;
	public GameObject or;
	public GameObject hasnt;
	public GameObject desc;
	public GameObject startButton;
	public float hasSpeed, hasntSpeed, colorSpeed;
    public bool started;
    public Color textStart, textFinish;
    public Color buttonStart, buttonFinish;

	private float lerpValue;
	private float MAX_LERP_VALUE;
	private float tSmooth;
	private Vector2 hasStart, hasntStart;
	private Vector2 hasFinish, hasntFinish;

	// Use this for initialization
	void Start () {
		has = GameObject.Find("TitleHas");
		or = GameObject.Find("TitleOr");
		hasnt = GameObject.Find("TitleHasnt");
		desc = GameObject.Find("DescriptionText");
		startButton = GameObject.Find("Start Game");
		startButton.GetComponent<Image>().color = buttonStart;
		textStart = desc.GetComponent<Text>().color;
		hasFinish = has.transform.position;
		has.transform.position = new Vector2(has.transform.position.x - 1050f,
											 has.transform.position.y);
		hasStart = has.transform.position;
		hasntFinish = hasnt.transform.position;
		hasnt.transform.position = new Vector2(hasnt.transform.position.x + 1350f,
											   hasnt.transform.position.y);
		hasntStart = hasnt.transform.position;
		StartCoroutine(TextAnim());
	}


	IEnumerator TextAnim(){
		started = true;
		float elapsedTime = 0.0f;;
		yield return new WaitForSeconds(0.5f);
		while (elapsedTime < 1.0f){
			has.transform.position = Vector2.Lerp(has.transform.position, 
													     hasFinish,
													     hasSpeed*Time.deltaTime);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(0.3f);
		elapsedTime = 0.0f;
		while (elapsedTime < 1.0f){
			hasnt.transform.position = Vector2.Lerp(hasnt.transform.position,
													   hasntFinish,
													   hasntSpeed*Time.deltaTime);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		yield return new WaitForSeconds(0.2f);
		elapsedTime = 0.0f;
		while (elapsedTime < 1.0f){
			Color currentTextColor = desc.GetComponent<Text>().color;
			Color targetColor = Color.Lerp(currentTextColor, textFinish,
								     colorSpeed*Time.deltaTime);
			desc.GetComponent<Text>().color = targetColor;
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		elapsedTime = 0.0f;
		while (elapsedTime < 1.0f){
			Color currentButtonColor = startButton.GetComponent<Image>().color;
			Color targetColor = Color.Lerp(currentButtonColor, buttonFinish,
								     colorSpeed*Time.deltaTime);
			startButton.GetComponent<Image>().color = targetColor;
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		GameHandler.GH.animFinished = true;
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
