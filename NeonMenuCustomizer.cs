#nullable disable
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using ImageMagick;

[assembly: MelonInfo(typeof(NeonMenuCustomizer), "NeonMenuCustomizer", "1.0.0", "PlixtzlBit")]

public class NeonMenuCustomizer : MelonMod
{
    private GameObject EndPortraitObject;
    private Image EndPortraitImage;
    private GameObject PlayerAnchor;
	private GameObject ShakeHolder;
	private GameObject PlayerPortrait;
    private GameObject ResultsPanel;
    private GameObject AnimatedPortrait;
    private GameObject FailurePanel;
    private GameObject MedalCheck;
    private GameObject EndBG;
    private GameObject EndHideFlames;

    private List<List<Sprite>> ImageLoads = new List<List<Sprite>>{new List<Sprite>(), new List<Sprite>(), new List<Sprite>(), new List<Sprite>(), new List<Sprite>()};
    private List<int> ImageFPS = new List<int>{0,0,0,0,0};
    private List<int> ImageFrame = new List<int>{0,0,0,0,0};
    private List<float> ImageTimer = new List<float>{0f,0f,0f,0f,0f};
    private List<string> ImagePath = new List<string>{"","","","",""};
    private List<string> LastImagePath = new List<string>{"","","","",""};

    private Vector3 ImFinalPos = new Vector3(-12.13f, 21.58f, 19.23f);
    private Vector3 ImHidePos = new Vector3(-12.74f, 17.83f, 19.23f);
    private Vector3 DefaultScale = new Vector3(0.4f, 0.75f, 1);
    private Vector3 FailurePos = new Vector3(1520, 850, 0);

    private bool MoveEndScreen;
    private bool MoveFailure;
	private bool ReplacePP;
	private bool ReplaceAP;
    private bool EndPortrait;
    private bool Slide;
    private bool ShowImage;
    private float SlideTime;
    private bool ReplaceEndBG;
    private bool ReplaceMedalBG;
    private bool InitAll = true;
    private bool EAPortrait;
    private bool MedalEAPortrait;

    private Vector3 ButtonsPos = new Vector3(638f, -317.5959f, 0f);
    private Vector3 MainPos = new Vector3(461f, 52.1022f, 0f);
    private Vector3 LeaderboardPos = new Vector3(64.7999f, -36.9998f, 0f);

    private MelonPreferences_Category ImagesCategory;
    private MelonPreferences_Category MenuLayoutCategory;
    private MelonPreferences_Category EndAnimationCategory;

    private MelonPreferences_Entry<string> PrefPPath;
    private MelonPreferences_Entry<bool> PrefReplacePP;
    private MelonPreferences_Entry<string> PrefEndBGPath;
    private MelonPreferences_Entry<bool> PrefReplaceEndBG;
    private MelonPreferences_Entry<string> PrefMedalBGPath;
    private MelonPreferences_Entry<bool> PrefReplaceMedalBG;
    private MelonPreferences_Entry<string> PrefEPPath;
    private MelonPreferences_Entry<bool> PrefEndPortrait;
    private MelonPreferences_Entry<string> PrefAPPath;
    private MelonPreferences_Entry<bool> PrefReplaceAP;

    private MelonPreferences_Entry<bool> PrefSlide;
    private MelonPreferences_Entry<bool> PrefMoveEndScreen;
    private MelonPreferences_Entry<bool> PrefMoveFailure;

    private MelonPreferences_Entry<Vector3> PrefFinalPos;
    private MelonPreferences_Entry<Vector3> PrefHidePos;
    private MelonPreferences_Entry<Vector3> PrefButtonsPos;
    private MelonPreferences_Entry<Vector3> PrefMainPos;
    private MelonPreferences_Entry<Vector3> PrefLeaderboardPos;
    private MelonPreferences_Entry<Vector3> PrefFailurePos;

    private MelonPreferences_Entry<bool> PrefEAPortrait;
    private MelonPreferences_Entry<bool> PrefMedalEAPortrait;

    public override void OnInitializeMelon()
    {
		InitAll = true;
		
        ImagesCategory = MelonPreferences.CreateCategory("NeonMenuCustomizer/Images");
        MenuLayoutCategory = MelonPreferences.CreateCategory("NeonMenuCustomizer/MenuLayout");
        EndAnimationCategory = MelonPreferences.CreateCategory("NeonMenuCustomizer/LevelEndAnimation");

		PrefPPath = ImagesCategory.CreateEntry("PPPath", "C:/PlayerPortrait.png", "Player Portrait Path", "Path to PNG/GIF for the player portrait (Cannot have quotes!)");
		PrefReplacePP = ImagesCategory.CreateEntry("ReplacePP", false, "Replace Player Portrait", "Replace player portrait with custom image");
        
		PrefEndBGPath = ImagesCategory.CreateEntry("EndBGPath", "C:/EndBG.png", "End Background Path", "Path to PNG/GIF for the background of the level win animation. Works best with 1920x1080 resolution (Cannot have quotes!)");
        PrefReplaceEndBG = ImagesCategory.CreateEntry("ReplaceEndBG", false, "Replace End Background", "Enable new end animation background");

        PrefMedalBGPath = ImagesCategory.CreateEntry("MedalBGPath", "C:/MedalBG.png", "Medal Background Path", "Path to PNG/GIF for medal background. Works best with 1920x1080 resolution (Cannot have quotes!)");
        PrefReplaceMedalBG = ImagesCategory.CreateEntry("ReplaceMedalBG", false, "Replace Medal Background", "Enable new medal background");

        PrefEPPath = ImagesCategory.CreateEntry("EPPath", "C:/EndPortrait.png", "End Portrait Path", "Path to PNG/GIF to replace end portrait. Works best with  (Cannot have quotes!)");
        PrefEndPortrait = ImagesCategory.CreateEntry("EndPortrait", false, "End Portrait", "Enable end portrait");
		
		PrefAPPath = ImagesCategory.CreateEntry("APPath", "C:/AnimationPortrait.png", "Animation Portrait Path", "Path to PNG/GIF for the end animation image (Cannot have quotes!)");
		PrefReplaceAP = ImagesCategory.CreateEntry("ReplaceAP", false, "Replace Animation Portrait", "Replace end animation portrait");

        PrefSlide = EndAnimationCategory.CreateEntry("SlideAnimation", true, "Slide Animation", "Enable slide animation for end portrait");
        PrefFinalPos = EndAnimationCategory.CreateEntry("FinalPosition", ImFinalPos, "Final Position", "Final position of end portrait");
        PrefHidePos = EndAnimationCategory.CreateEntry("HiddenPosition", ImHidePos, "Hidden Position", "Start position of slide animation");

        PrefMoveEndScreen = MenuLayoutCategory.CreateEntry("MoveEndScreen", false, "Move End Screen", "Move end screen panels");
        PrefMoveFailure = MenuLayoutCategory.CreateEntry("MoveFailure", true, "Move Failure", "Move failure text");
        PrefButtonsPos = MenuLayoutCategory.CreateEntry("ButtonsPosition", ButtonsPos, "Buttons Position", "Position of results buttons");
        PrefMainPos = MenuLayoutCategory.CreateEntry("InfoPosition", MainPos, "Level Information Position", "Position of results info panel");
        PrefLeaderboardPos = MenuLayoutCategory.CreateEntry("LeaderboardPosition", LeaderboardPos, "Leaderboard Position", "Position of results leaderboard");
        PrefFailurePos = MenuLayoutCategory.CreateEntry("FailurePosition", FailurePos, "Failure Position", "Position of failure text");

        PrefEAPortrait = EndAnimationCategory.CreateEntry("EAPortrait", true, "End Animation Portrait", "Toggle animation portrait");
        PrefMedalEAPortrait = EndAnimationCategory.CreateEntry("MedalEAPortrait", true, "New Medal End Portrait", "Toggle medal portrait");

        
        MoveEndScreen = PrefMoveEndScreen.Value;
        Slide = PrefSlide.Value;
        ImFinalPos = PrefFinalPos.Value;
        ImHidePos = PrefHidePos.Value;

        MoveFailure = PrefMoveFailure.Value;
        ButtonsPos = PrefButtonsPos.Value;
        MainPos = PrefMainPos.Value;
        FailurePos = PrefFailurePos.Value;
        LeaderboardPos = PrefLeaderboardPos.Value;
		
		ReplacePP = PrefReplacePP.Value;
		ImagePath[0] = PrefPPath.Value;
		ReplaceAP = PrefReplaceAP.Value;
		ImagePath[4] = PrefAPPath.Value;
		EndPortrait = PrefEndPortrait.Value;
        ReplaceEndBG = PrefReplaceEndBG.Value;
        ImagePath[1] = PrefEndBGPath.Value;
        ReplaceMedalBG = PrefReplaceMedalBG.Value;
		ImagePath[3] = PrefEPPath.Value;
        ImagePath[2] = PrefMedalBGPath.Value;
        EAPortrait = PrefEAPortrait.Value;
        MedalEAPortrait = PrefMedalEAPortrait.Value;
    }

    public override void OnSceneWasLoaded(int BuildIndex, string SceneName)
    {
		InitAll = true;

        MelonPreferences.Load();

        MoveEndScreen = PrefMoveEndScreen.Value;
        Slide = PrefSlide.Value;
        ImFinalPos = PrefFinalPos.Value;
        ImHidePos = PrefHidePos.Value;

        MoveFailure = PrefMoveFailure.Value;
        ButtonsPos = PrefButtonsPos.Value;
        MainPos = PrefMainPos.Value;
        FailurePos = PrefFailurePos.Value;
        LeaderboardPos = PrefLeaderboardPos.Value;
		
		ReplacePP = PrefReplacePP.Value;
		ImagePath[0] = PrefPPath.Value;
		ReplaceAP = PrefReplaceAP.Value;
		ImagePath[4] = PrefAPPath.Value;
		EndPortrait = PrefEndPortrait.Value;
        ReplaceEndBG = PrefReplaceEndBG.Value;
        ImagePath[1] = PrefEndBGPath.Value;
        ReplaceMedalBG = PrefReplaceMedalBG.Value;
		ImagePath[3] = PrefEPPath.Value;
        ImagePath[2] = PrefMedalBGPath.Value;
        EAPortrait = PrefEAPortrait.Value;
        MedalEAPortrait = PrefMedalEAPortrait.Value;

        if(MoveEndScreen)
        {
            try
            {
                GameObject A = GameObject.Find("Main Menu/Canvas/Ingame Menu/Menu Holder/Results Panel/Results Buttons");
                GameObject B = GameObject.Find("Main Menu/Canvas/Ingame Menu/Menu Holder/Results Panel/Leaderboards And LevelInfo");
                GameObject C = GameObject.Find("Main Menu/Canvas/Ingame Menu/Menu Holder/Results Panel/Leaderboards And LevelInfo/Leaderboards");
                if(A == null || B == null || C == null)
                {
                    return;
                }
                A.transform.localPosition = ButtonsPos;
                B.transform.localPosition = MainPos;
                C.transform.localPosition = LeaderboardPos;
            }
            catch{}
        }
    }

    public override void OnFixedUpdate()
    {
		//Assign
        if(ResultsPanel == null || AnimatedPortrait == null)
        {
            ResultsPanel = GameObject.Find("Main Menu/Canvas/Ingame Menu/Menu Holder/Results Panel");
        }
        if(AnimatedPortrait == null)
        {
            AnimatedPortrait = GameObject.Find("Main Menu/Canvas/Ingame Menu/Menu Holder/Results Panel/LevelCompleteScreen/LevelCompleteAnim/White");
        }
        if(MedalCheck == null)
        {
            MedalCheck = GameObject.Find("Main Menu/Canvas/Ingame Menu/Menu Holder/Results Panel/LevelCompleteScreen/Timer and Medals/Medal");
        }
        if(EndBG == null)
        {
            EndBG = GameObject.Find("Main Menu/Canvas/Ingame Menu/Menu Holder/Results Panel/LevelCompleteScreen/LevelCompleteAnim/BG");
        }
        if(EndHideFlames == null)
        {
            EndHideFlames = GameObject.Find("Main Menu/Canvas/Ingame Menu/Menu Holder/Results Panel/LevelCompleteScreen/LevelCompleteAnim/BGFlame");
        }
        if(PlayerAnchor == null)
        {
            PlayerAnchor = GameObject.Find("HUD/Player/PlayerAnchor");
        }
		if(ShakeHolder == null && PlayerAnchor != null)
		{
			ShakeHolder = PlayerAnchor.transform.Find("PlayerUIPortrait/PortraitHolder/Shake Holder").gameObject;
		}
        if(FailurePanel == null)
        {
            FailurePanel = GameObject.Find("HUD/PlayerOverlayCanvas/GameplayOverlays/Player/Restart Indicator Anchor/Restart Indicator Holder/Restart Indicator Panel");
            if(MoveFailure && FailurePanel != null)
            {
                FailurePanel.transform.position = FailurePos;
            }
        }
		if(PlayerPortrait == null && ShakeHolder != null)
		{
		    foreach (Transform child in ShakeHolder.transform)
			{
				if (child.gameObject.activeSelf && child.name.Contains("Portrait"))
				{
					PlayerPortrait = child.gameObject;
					break;
				}
			}
		}
		//Init PP, EP, BG, MBG, AP
		if(ReplacePP && File.Exists(ImagePath[0]) && ImagePath[0] != LastImagePath[0])
		{
			InitAll = true;
			(ImageLoads[0], ImageFPS[0]) = OutImage(ImagePath[0]);
			LastImagePath[0] = ImagePath[0];
		}
        if(EndPortraitObject == null && ResultsPanel != null && PlayerAnchor != null && EndPortrait && File.Exists(ImagePath[3]))
        {
            GameObject ImgObj = new GameObject("EndPortraitImage");
            ImgObj.transform.SetParent(ResultsPanel.transform, false);
            ImgObj.transform.SetSiblingIndex(0);
            EndPortraitObject = ImgObj;
            EndPortraitObject.transform.position = ImFinalPos;
            EndPortraitImage = ImgObj.AddComponent<Image>();
            EndPortraitObject.SetActive(true);

            RectTransform Rt = ImgObj.GetComponent<RectTransform>();
            Rt.anchorMin = new Vector2(0.25f, 0.25f);
            Rt.anchorMax = new Vector2(0.75f, 0.75f);
            Rt.offsetMin = Vector2.zero;
            Rt.offsetMax = Vector2.zero;

            (ImageLoads[3], ImageFPS[3]) = OutImage(ImagePath[3]);
            if(ImageLoads[3].Count > 0)
            {
                EndPortraitImage.sprite = ImageLoads[3][0];
            }
        }
		
        if(File.Exists(ImagePath[1]) && ImagePath[1] != LastImagePath[1])
        {
            (ImageLoads[1], ImageFPS[1]) = OutImage(ImagePath[1]);
            LastImagePath[1] = ImagePath[1];
        }

        if(File.Exists(ImagePath[2]) && ImagePath[2] != LastImagePath[2])
        {
            (ImageLoads[2], ImageFPS[2]) = OutImage(ImagePath[2]);
            LastImagePath[2] = ImagePath[2];
        }
        if(File.Exists(ImagePath[4]) && ImagePath[4] != LastImagePath[4])
        {
            (ImageLoads[4], ImageFPS[4]) = OutImage(ImagePath[4]);
            LastImagePath[4] = ImagePath[4];
        }
    }

    public override void OnLateUpdate()
    {
        if(EndPortraitObject == null || !EndPortrait)
        {
            return;
        }
        EndPortraitObject.transform.localScale = DefaultScale;
        if(EndPortraitObject.activeInHierarchy)
        {
			//Anim
            SlideTime += Time.unscaledDeltaTime;
            if((SlideTime > 0.5f && AnimatedPortrait.transform.position.x < -20) || ShowImage)
            {
                SlideTime = 1;
                ShowImage = true;
                float Ease = 1 - 1 / (1 + 1.3f * 5);
                EndPortraitObject.transform.position = Vector3.Lerp(EndPortraitObject.transform.position, ImFinalPos, Ease * Time.unscaledDeltaTime * 5 + (Slide ? 0 : 1));
            }

            PlayerAnchor.SetActive(false);
            if(ImageLoads[3].Count > 0)
            {
                EndPortraitImage.sprite = ImageLoads[3][ImageFrame[3]];
            }

            if(ImageLoads[3].Count > 1 && ImageFPS[3] > 0)
            {
                ImageTimer[3] += Time.unscaledDeltaTime;
                if(ImageTimer[3] >= 1f / ImageFPS[3])
                {
                    ImageTimer[3] -= 1f / ImageFPS[3];
                    ImageFrame[3] = (ImageFrame[3] + 1) % ImageLoads[3].Count;
                }
            }

            if(ImageLoads[1].Count > 0 && EndBG != null)
            {
                EndBG.GetComponent<Image>().sprite = ImageLoads[1][ImageFrame[1]];
            }

            if(ImageLoads[1].Count > 1 && ImageFPS[1] > 0 && EndBG != null)
            {
                ImageTimer[1] += Time.unscaledDeltaTime;

                if(ImageTimer[1] >= 1f / ImageFPS[1])
                {
                    ImageTimer[1] -= 1f / ImageFPS[1];
                    ImageFrame[1] = (ImageFrame[1] + 1) % ImageLoads[1].Count;
                }
            }

            if(ImageLoads[2].Count > 0 && EndBG != null && ReplaceMedalBG && MedalCheck.activeInHierarchy && SlideTime > 0.4f)
            {
                EndBG.GetComponent<Image>().sprite = ImageLoads[2][ImageFrame[2]];
            }

            if(ImageLoads[2].Count > 1 && ImageFPS[2] > 0 && EndBG != null && MedalCheck.activeInHierarchy)
            {
                ImageTimer[2] += Time.unscaledDeltaTime;
                if(ImageTimer[2] >= 1f / ImageFPS[2])
                {
                    ImageTimer[2] -= 1f / ImageFPS[2];
                    ImageFrame[2] = (ImageFrame[2] + 1) % ImageLoads[2].Count;
                }
            }
			if (AnimatedPortrait != null && ReplaceAP && ImageLoads[4].Count > 0)
			{
				if (ImageLoads[4].Count > 1 && ImageFPS[4] > 0)
				{
					ImageTimer[4] += Time.unscaledDeltaTime;
					if (ImageTimer[4] >= 1f / ImageFPS[4])
					{
						ImageTimer[4] -= 1f / ImageFPS[4];
						ImageFrame[4] = (ImageFrame[4] + 1) % ImageLoads[4].Count;
					}
				}
				AnimatedPortrait.GetComponent<Image>().sprite = ImageLoads[4][ImageFrame[4]];
			}
            if(EndBG != null && (ReplaceMedalBG || ReplaceEndBG))
            {
				EndHideFlames.SetActive(false);
                if(AnimatedPortrait != null && ((MedalEAPortrait && MedalCheck.activeInHierarchy && ReplaceMedalBG) || (EAPortrait && ReplaceEndBG && !MedalCheck.activeInHierarchy)))
                {
                    AnimatedPortrait.SetActive(true);
                }
                else
                {
                    AnimatedPortrait.SetActive(false);
                }
                EndBG.GetComponent<Image>().color = new Color(1,1,1,EndBG.GetComponent<Image>().color.a);
            }
        }
        else
        {
            SlideTime = 0;
            ShowImage = false;
			if (PlayerPortrait != null && ReplacePP && ImageLoads[0].Count > 0)
			{
				if (ImageLoads[0].Count > 1 && ImageFPS[0] > 0)
				{
					ImageTimer[0] += Time.unscaledDeltaTime;
					if (ImageTimer[0] >= 1f / ImageFPS[0])
					{
						ImageTimer[0] -= 1f / ImageFPS[0];
						ImageFrame[0] = (ImageFrame[0] + 1) % ImageLoads[0].Count;
					}
				}
				Texture2D tex = ImageLoads[0][ImageFrame[0]].texture;
				PlayerPortrait.GetComponent<MeshRenderer>().material.mainTexture = tex;					
			}
			if(ReplaceEndBG)
			{
				EndHideFlames.SetActive(false);
			}
            if(InitAll)
            {
				ImageTimer = ImageTimer.ConvertAll(_ => 0f);
				ImageFrame = ImageFrame.ConvertAll(_ => 0);
				
				if(PlayerPortrait != null && ReplacePP)
				{
					PlayerPortrait.GetComponent<MeshRenderer>().material.mainTexture = ImageLoads[0][ImageFrame[0]].texture;
				}
				if(AnimatedPortrait != null && ReplaceAP)
				{
					Image img = AnimatedPortrait.GetComponent<Image>();
					if(img != null && ImageLoads[4].Count > 4)
					{
						img.sprite = ImageLoads[4][0];
					}
				}
                if(AnimatedPortrait != null && EAPortrait)
                {
                    AnimatedPortrait.SetActive(true);
                }
                else
                {
                    AnimatedPortrait.SetActive(false);
                }

                Image Img = EndBG.GetComponent<Image>();

                if(Img != null && !ReplaceEndBG)
                {
                    InitAll = false;
                    Texture2D Tex = new Texture2D(1,1);
                    Tex.SetPixel(0,0,Color.white);
                    Tex.Apply();
                    Img.sprite = Sprite.Create(Tex,new Rect(0,0,1,1),new Vector2(0.5f,0.5f));
                    EndHideFlames.SetActive(true);
                }
                else if(Img != null && ImageLoads[1].Count > 0)
                {
                    InitAll = false;
                    Img.sprite = ImageLoads[1][0];
                    Img.color = new Color(1,1,1,Img.color.a);
                }
            }
            EndPortraitObject.transform.position = ImHidePos;
        }
    }

    public static (List<Sprite>, int) OutImage(string path)
    {
        List<Sprite> Frames = new List<Sprite>();
        int Framerate = 0;

        if(!File.Exists(path))
        {
            Texture2D Fallback = new Texture2D(1, 1);
            Fallback.SetPixel(0, 0, Color.white);
            Fallback.Apply();
            return (new List<Sprite>{Sprite.Create(Fallback, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f))}, 0);
        }

        if(Path.GetExtension(path).ToLower() != ".gif")
        {
            byte[] Bytes = File.ReadAllBytes(path);
            Texture2D Tex = new Texture2D(2, 2);
            Tex.LoadImage(Bytes);
            return (new List<Sprite>{Sprite.Create(Tex, new Rect(0, 0, Tex.width, Tex.height), new Vector2(0.5f, 0.5f))}, 0);
        }
        try
        {
            using(MagickImageCollection Gif = new MagickImageCollection(path))
            {
                Gif.Coalesce();

                if(Gif.Count > 0)
                {
                    int Delay = (int)Gif[0].AnimationDelay;
                    if(Delay > 0)
                    {
                        Framerate = Mathf.RoundToInt(100f / Delay);
                    }
                }

                foreach(MagickImage Frame in Gif)
                {
                    Frame.Flip();
                    byte[] Rgba = Frame.ToByteArray(MagickFormat.Rgba);
                    Texture2D Tex = new Texture2D((int)Frame.Width, (int)Frame.Height, TextureFormat.RGBA32, false);
                    Tex.LoadRawTextureData(Rgba);
                    Tex.Apply();
                    Frames.Add(Sprite.Create(Tex, new Rect(0, 0, Tex.width, Tex.height), new Vector2(0.5f, 0.5f)));
                }
            }

            if(Frames.Count > 0)
            {
                return (Frames, Framerate);
            }
        }
        catch{}

        Texture2D FallbackTex = new Texture2D(1, 1);
        FallbackTex.SetPixel(0, 0, Color.white);
        FallbackTex.Apply();
        return (new List<Sprite>{Sprite.Create(FallbackTex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f))}, 0);
    }
}
