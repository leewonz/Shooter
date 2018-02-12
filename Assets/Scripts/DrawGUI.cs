using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGUI : MonoBehaviour {

    public Texture crosshairStickH;
    public Texture crosshairStickV;
    float crosshairMinSpread = 4;
    float crosshairSpread;

    // Use this for initialization
    void Start () {
        crosshairSpread = crosshairMinSpread;


    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        // Crosshair - Top
        GUI.DrawTexture(
            new Rect(
                new Vector2(
                    (Screen.width / 2) + (float)(-crosshairStickV.width / 2)
                  , (Screen.height / 2) - Mathf.Floor(crosshairSpread))
              , new Vector2(crosshairStickV.width, - (crosshairStickV.height)))
            , crosshairStickV, ScaleMode.StretchToFill);

        // Crosshair - Bottom
        GUI.DrawTexture(
            new Rect(
                new Vector2(
                    (Screen.width / 2) + (float)(-crosshairStickV.width / 2)
                  , (Screen.height / 2) + Mathf.Floor(crosshairSpread))
              , new Vector2(crosshairStickV.width, + (crosshairStickV.height)))
            , crosshairStickV, ScaleMode.StretchToFill);

        // Crosshair - Left
        GUI.DrawTexture(
            new Rect(
                new Vector2(
                    (Screen.width / 2) - Mathf.Floor(crosshairSpread)
                  , (Screen.height / 2) - (float)(crosshairStickH.height / 2))
              , new Vector2( - (crosshairStickH.width), crosshairStickH.height))
            , crosshairStickH, ScaleMode.StretchToFill);

        // Crosshair - Right
        GUI.DrawTexture(
            new Rect(
                new Vector2(
                    (Screen.width / 2) + Mathf.Floor(crosshairSpread)
                  , (Screen.height / 2) - (float)(crosshairStickH.height / 2))
              , new Vector2( + (crosshairStickH.width), crosshairStickH.height))
            , crosshairStickH, ScaleMode.StretchToFill);
        //crosshairStick
    }
}
