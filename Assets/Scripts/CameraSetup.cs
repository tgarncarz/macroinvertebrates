using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetup : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//from http://answers.unity3d.com/questions/618058/mobile-device-screen-sizes.html
		float targetAspect = 16.0f/9.0f;
		float windowAspect = (float)Screen.width/(float)Screen.height;
		float scaleHeight = windowAspect/targetAspect;

		if (scaleHeight < 1.0f) //add letterbox
         {
             Rect rect = Camera.main.rect;
             
             rect.width = 1.0f;
             rect.height = scaleHeight;
             rect.x = 0;
             rect.y = (1.0f - scaleHeight) / 2.0f;
             
             Camera.main.rect = rect;
         }
         else // add pillarbox
         {
             float scaleWidth = 1.0f / scaleHeight;
             
             Rect rect = Camera.main.rect;
             
             rect.width = scaleWidth;
             rect.height = 1.0f;
             rect.x = (1.0f - scaleWidth) / 2.0f;
             rect.y = 0;
             
             Camera.main.rect = rect;
         }
	}
}
