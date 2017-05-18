using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour {
	//credit to https://www.youtube.com/watch?v=SrCUO46jcxk for much of this script
	public LayerMask touchInputMask;


	private List<GameObject> touchObjectList = new List<GameObject>();
	private GameObject[] oldTouchObjects;
	private GameObject lastObject;
	private RaycastHit2D hit;
	private int TapCount;
	private float MaxDTTime;
	private float NewTime;

	void Start (){
		TapCount = 0;
		MaxDTTime = 0.1f;
	}

	// Update is called once per frame
	void Update () {
		
	//for use in unity editor; won't run on touch device
	if (GameHandler.GH.touchable){
		#if UNITY_EDITOR
			if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)){
				oldTouchObjects = new GameObject[touchObjectList.Count];
				touchObjectList.CopyTo(oldTouchObjects);
				touchObjectList.Clear();

				hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
					

				if (hit.collider != null){
					if (hit.transform.gameObject != lastObject){
						TapCount = 0;
					}
					GameObject recipient = hit.transform.gameObject;
					touchObjectList.Add(recipient);

					if (Input.GetMouseButtonDown(0)){
						recipient.SendMessage("OnTouchDown", hit.point, SendMessageOptions.DontRequireReceiver);
					}

					if (Input.GetMouseButtonUp(0)){
						recipient.SendMessage("OnTouchUp", hit.point, SendMessageOptions.DontRequireReceiver);
						lastObject = recipient;
						TapCount += 1;
					}

					if (Input.GetMouseButton(0)){
						recipient.SendMessage("OnTouchStay", hit.point, SendMessageOptions.DontRequireReceiver);
					}

					if (TapCount == 1){
						NewTime = Time.time + MaxDTTime;
					} else if (TapCount == 2 && Time.time <= NewTime){
						//get the sprite of the bug we're double-tapping
						Sprite zoomedBugSprite = recipient.GetComponent<SpriteRenderer>().sprite;
						GameHandler.GH.ZoomIn(zoomedBugSprite);
						TapCount = 0;
					}
				
				}
				

				//if we slide our finger off the object (but still touching the screen)
				/*foreach (GameObject g in oldTouchObjects){
					if (!touchObjectList.Contains(g)){
						g.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
					}
				}*/
			}
			if (Time.time > NewTime){
				TapCount = 0;
			}
		#endif

			//for use with touch devices
			// double tap courtesy of http://answers.unity3d.com/questions/369230/how-to-detect-double-tap-in-android.html
			if (Input.touchCount > 0){
				oldTouchObjects = new GameObject[touchObjectList.Count];
				touchObjectList.CopyTo(oldTouchObjects);
				touchObjectList.Clear();

				foreach (Touch touch in Input.touches){
					hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);

					if (hit.collider != null){
						if (hit.transform.gameObject != lastObject){
							TapCount = 0;
						}
						GameObject recipient = hit.transform.gameObject;
						touchObjectList.Add(recipient);

						if (touch.phase == TouchPhase.Began){
							recipient.SendMessage("OnTouchDown", hit.point, SendMessageOptions.DontRequireReceiver);
						}

						if (touch.phase == TouchPhase.Ended){
							recipient.SendMessage("OnTouchUp", hit.point, SendMessageOptions.DontRequireReceiver);
							lastObject = recipient;
							TapCount += 1;
						}

						if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved){
							recipient.SendMessage("OnTouchStay", hit.point, SendMessageOptions.DontRequireReceiver);
						}

						if (touch.phase == TouchPhase.Canceled){
							recipient.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
						}

						if (TapCount == 1){
							NewTime = Time.time + MaxDTTime;
						} else if (TapCount == 2 && Time.time <= NewTime){
							//get the sprite of the bug we're double-tapping
							Sprite zoomedBugSprite = recipient.GetComponent<SpriteRenderer>().sprite;
							GameHandler.GH.ZoomIn(zoomedBugSprite);
							TapCount = 0;
						}
					}
					
				}
				if (Time.time > NewTime){
					TapCount = 0;
				}
			}

			//if we slide our finger off the object (but still touching the screen)
			/*foreach (GameObject g in oldTouchObjects){
				if (!touchObjectList.Contains(g)){
					g.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
				}
			}*/
		}
	}
}
