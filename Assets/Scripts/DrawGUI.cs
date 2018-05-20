using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGUI : MonoBehaviour {

    public Texture crosshairStickH;
    public Texture crosshairStickV;
    //public Texture bloodScreen;
    float crosshairMinSpread = 3;
    float crosshairSpreadMultiplier = 8;
    float crosshairSpread;

    float deltaTime = 0.0f;

    float bloodscreenAlpha = 0;
    float bloodscreenMax = 1.0f;
    float bloodscreenDec = 4.0f;

    GameObject playerObj;
    PlayerHit playerHit;
    PlayerGun playerGun;
    GameObject canvasHitEffectObj;
    CanvasGroup canvasHitEffectCG;

    // Use this for initialization
    void Start () {
        crosshairSpread = crosshairMinSpread;

        GameObject[] gunObj = GameObject.FindGameObjectsWithTag("PlayerGun");
        if (gunObj.Length == 1)
        {
            playerGun = gunObj[0].GetComponent<PlayerGun>();
        }
        else
        {
            Debug.LogError("player gun count is not one.");
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        playerHit = playerObj.GetComponent<PlayerHit>();

        canvasHitEffectObj = GameObject.Find("Canvas Hit Effect");
        canvasHitEffectCG = canvasHitEffectObj.GetComponent<CanvasGroup>();
    }
	
	// Update is called once per frame
	void Update () {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f; //fps

        crosshairSpread = Mathf.Max(crosshairMinSpread
            , playerGun.CurrentSpread() * crosshairSpreadMultiplier);

        //Screen Hit Effect
        bloodscreenAlpha = Mathf.Max(bloodscreenAlpha - (bloodscreenDec * Time.deltaTime), 0);
        
        canvasHitEffectCG.alpha = bloodscreenAlpha;
        //GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bloodScreen, ScaleMode.StretchToFill,);
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

        


        //FPS counter
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }

    public void HitEffect()
    {
        bloodscreenAlpha = bloodscreenMax;
    }
}
