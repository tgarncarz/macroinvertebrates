using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInfo : MonoBehaviour {

	public Sprite image;

	public string name;
	public bool jointedLegs;
	public bool portableCase;
	public bool tailFilaments;
	public bool wingsOrPads;
	public bool analHooks;
	//if bodyShape == 0, worm-like body. if bodyShape == 1, round body shape
	public bool wormBody;
	public int numLegs;
	public int numFilaments;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
