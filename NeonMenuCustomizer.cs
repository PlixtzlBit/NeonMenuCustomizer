#nullable disable
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using ImageMagick;

[assembly: MelonInfo(typeof(NeonMenuCustomizer), "NeonMenuCustomizer", "1.0.0", "PlixtzlBit")]

public class NeonMenuCustomizer : MelonMod
{
    private GameObject EndPortraitObject;
    private Image EndPortraitImage;
    private GameObject PlayerAnchor;
    private GameObject ResultsPanel;
	private GameObject AnimatedPortrait;
	private GameObject FailurePanel;
	private GameObject MedalCheck;
	private GameObject EndBG;
	private GameObject EndHideFlames;
	
	private List<Sprite> EndPortraitLoad = new List<Sprite>();
	private List<Sprite> EndBGLoad = new List<Sprite>();
	private List<Sprite> MedalBGLoad = new List<Sprite>();
	private int EndPortraitFPS;
	private int EndBGFPS;
	private int MedalBGFPS;
	private int EndPortraitFrame;
	private int EndBGFrame;
	private int MedalBGFrame;
	private float EndPortraitTimer;
	private float EndBGTimer;
	private float MedalBGTimer;

    private Vector3 ImFinalPos = new Vector3(-12.13f, 21.58f, 19.23f);
    private Vector3 ImHidePos = new Vector3(-12.74f, 17.83f, 19.23f);
    private Vector3 DefaultScale = new Vector3(0.4f, 0.75f, 1);
	private Vector3 FailurePos = new Vector3(1520, 850, 0);
	
	private bool MoveEndScreen;
	private bool MoveFailure;
    private bool EndPortrait = false;
    private bool Slide = true;
    private bool ShowImage;
	private float SlideTime;
	private bool ReplaceEndBG;
	private bool NewMedalBG;
	private bool MedalBGSet;
	private bool EAPortrait;
	private bool MedalEAPortrait;
	
    private Vector3 ButtonsPos = new Vector3(638f, -317.5959f, 0f);
    private Vector3 MainPos = new Vector3(461f, 52.1022f, 0f);
    private Vector3 LeaderboardPos = new Vector3(64.7999f, -36.9998f, 0f);

    private string ImagePath = "C:/EndPortrait.png";
	private string EndBGPath = "C:/EndBG.png";
	private string LastEndBGPath;
	private string NewMedalBGPath = "C:/NewMedalBG.png";
	private string LastNewMedalBGPath;
	
	private bool GetPrefs = true;
	
    private MelonPreferences_Category EndPortraitCategory;
    private MelonPreferences_Category MenuLayoutCategory;
	private MelonPreferences_Category EndAnimationCategory;

    private MelonPreferences_Entry<bool> PrefEndPortrait;
    private MelonPreferences_Entry<bool> PrefSlide;
    private MelonPreferences_Entry<bool> PrefMoveEndScreen;
    private MelonPreferences_Entry<bool> PrefMoveFailure;
	private MelonPreferences_Entry<bool> PrefEndBG;
	private MelonPreferences_Entry<bool> PrefNewMedalBG;
	private MelonPreferences_Entry<bool> PrefEAPortrait;
	private MelonPreferences_Entry<bool> PrefMedalEAPortrait;
    private MelonPreferences_Entry<Vector3> PrefFinalPos;
    private MelonPreferences_Entry<Vector3> PrefHidePos;
    private MelonPreferences_Entry<Vector3> PrefFailurePos;
    private MelonPreferences_Entry<string> PrefImagePath;
	private MelonPreferences_Entry<string> PrefEndBGPath;
	private MelonPreferences_Entry<string> PrefNewMedalBGPath;

    private MelonPreferences_Entry<Vector3> PrefButtonsPos;
    private MelonPreferences_Entry<Vector3> PrefMainPos;
    private MelonPreferences_Entry<Vector3> PrefLeaderboardPos;

    public override void OnInitializeMelon()
    {
		EndPortraitCategory = MelonPreferences.CreateCategory("NeonMenuCustomizer/EndPortrait");
        MenuLayoutCategory = MelonPreferences.CreateCategory("NeonMenuCustomizer/MenuLayout");
        EndAnimationCategory = MelonPreferences.CreateCategory("NeonMenuCustomizer/LevelEndAnimation");
		
		PrefEndPortrait = EndPortraitCategory.CreateEntry("EndPortrait", false, "End Portrait", "If true the player portrait will be replaced on the end screen by the image at \"ImagePath\"");
		PrefMoveEndScreen = EndPortraitCategory.CreateEntry("MoveEndScreen", false, "Move End Screen", "(Requires restart to disable!) If true the end screen will use the positions of ButtonsPosition, MainPosition, and Leaderboard Position");
		PrefSlide = EndPortraitCategory.CreateEntry("SlideAnimation", true, "Slide Animation", "if true replacement image will slide from HiddenPosition to FinalPosition. If false, the image will just show without any animation");
		PrefImagePath = EndPortraitCategory.CreateEntry("ImagePath", "C:/EndPortrait.png", "Image Path", "if End Portrait is true then the image at this path will replace the player portrait only on the end screen (Cannot include quotes!)");
		PrefFinalPos = EndPortraitCategory.CreateEntry("FinalPosition", new Vector3(-12.13f, 21.58f, 19.23f), "Final Position", "Final image position");
		PrefHidePos = EndPortraitCategory.CreateEntry("HiddenPosition", new Vector3(-12.74f, 17.83f, 19.23f), "Hidden Position", "Image will be at this position when beating a level. You may want to keep it offscreen, but can change it to set the starting position of the slide animation");
		
		PrefMoveFailure = MenuLayoutCategory.CreateEntry("MoveFailure", true, "Move Failure", "If true the failure text will be moved (Can prevent overlap with a custom EndScreen layout)");
		PrefButtonsPos = MenuLayoutCategory.CreateEntry("ButtonsPosition", new Vector3(638f, -317.5959f, 0f), "Buttons Position", "Position of Buttons on the end screen");
		PrefMainPos = MenuLayoutCategory.CreateEntry("MainPosition", new Vector3(461f, 52.1022f, 0f), "End Panels Position", "Position of Main holder (Includes Leaderboard and Level Info)");
		PrefFailurePos = MenuLayoutCategory.CreateEntry("FailurePosition", new Vector3(1520, 850, 0), "Failure Position", "The position of the failure text");
		PrefLeaderboardPos = MenuLayoutCategory.CreateEntry("LeaderboardPosition", new Vector3(64.7999f, -36.9998f, 0f), "Leaderboard Position", "Position of Leaderboard (Does not affect LevelInfo)");
		
		PrefEndBG = EndAnimationCategory.CreateEntry("EndBG", false, "Replace End Background", "If true the end animation's background will be replaced with the image at \"EndBGPath\"");
		PrefEndBGPath = EndAnimationCategory.CreateEntry("EndBGPath", "C:/EndBG.png", "End Background path", "If Replace End Background is true then the image at this path will replace the end animation background");
		PrefNewMedalBG = EndAnimationCategory.CreateEntry("NewMedalBG", false, "NewMedalBG", "If true the end animation's background will change to \"NewMedalBGPath\" when you get a new medal on the current level (Will override Replace End Background)");
		PrefNewMedalBGPath = EndAnimationCategory.CreateEntry("NewMedalBGPath", "C:/NewMedalBG.png", "NewMedalBG Path", "If New Medal Background is true then the image at this path will replace the background of the end animation, only if you unlock a new medal for that level");
		PrefEAPortrait = EndAnimationCategory.CreateEntry("EAPortrait", true, "End Animation Portrait", "If true the portrait for the end animation will show. Disable this if you want to hide it");
		PrefMedalEAPortrait = EndAnimationCategory.CreateEntry("MedalEAPortrait", true, "New Medal End Portrait", "If true the portrait for the end animation will show when you have New Medal Background set to true and you get the new medal. Disable this if you want to hide it (Will show even if End Animation Portrait is set to false)");
	
		EndPortrait = PrefEndPortrait.Value;
		MoveEndScreen = PrefMoveEndScreen.Value;
		Slide = PrefSlide.Value;
		ImagePath = PrefImagePath.Value;
		ImFinalPos = PrefFinalPos.Value;
		ImHidePos = PrefHidePos.Value;

		MoveFailure = PrefMoveFailure.Value;
		ButtonsPos = PrefButtonsPos.Value;
		MainPos = PrefMainPos.Value;
		FailurePos = PrefFailurePos.Value;
		LeaderboardPos = PrefLeaderboardPos.Value;

		ReplaceEndBG = PrefEndBG.Value;
		EndBGPath = PrefEndBGPath.Value;
		NewMedalBG = PrefNewMedalBG.Value;
		NewMedalBGPath = PrefNewMedalBGPath.Value;
		EAPortrait = PrefEAPortrait.Value;
		MedalEAPortrait = PrefMedalEAPortrait.Value;
		GetPrefs = true;
    }
	public override void OnPreferencesSaved()
	{
		GetPrefs = true;
	}
    public override void OnSceneWasLoaded(int BuildIndex, string SceneName)
    {
		if(!GetPrefs)
		{
			return;
		}
		MelonPreferences.Load();
		EndPortrait = PrefEndPortrait.Value;
		MoveEndScreen = PrefMoveEndScreen.Value;
		Slide = PrefSlide.Value;
		ImagePath = PrefImagePath.Value;
		ImFinalPos = PrefFinalPos.Value;
		ImHidePos = PrefHidePos.Value;

		MoveFailure = PrefMoveFailure.Value;
		ButtonsPos = PrefButtonsPos.Value;
		MainPos = PrefMainPos.Value;
		FailurePos = PrefFailurePos.Value;
		LeaderboardPos = PrefLeaderboardPos.Value;

		ReplaceEndBG = PrefEndBG.Value;
		EndBGPath = PrefEndBGPath.Value;
		NewMedalBG = PrefNewMedalBG.Value;
		NewMedalBGPath = PrefNewMedalBGPath.Value;
		EAPortrait = PrefEAPortrait.Value;
		MedalEAPortrait = PrefMedalEAPortrait.Value;
		if(MoveEndScreen)
		{
			try
			{
				GameObject A = GameObject.Find("Main Menu/Canvas/Ingame Menu/Menu Holder/Results Panel/Results Buttons");
				GameObject B = GameObject.Find("Main Menu/Canvas/Ingame Menu/Menu Holder/Results Panel/Leaderboards And LevelInfo");
				GameObject C = GameObject.Find("Main Menu/Canvas/Ingame Menu/Menu Holder/Results Panel/Leaderboards And LevelInfo/Leaderboards");
				if(A == null || B == null || C == null) return;
				A.transform.localPosition = ButtonsPos;
				B.transform.localPosition = MainPos;
				C.transform.localPosition = LeaderboardPos;
			}
			catch{}
		}		
		GetPrefs = false;
    }

    public override void OnFixedUpdate()
    {
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
		if(FailurePanel == null)
		{
			FailurePanel = GameObject.Find("HUD/PlayerOverlayCanvas/GameplayOverlays/Player/Restart Indicator Anchor/Restart Indicator Holder/Restart Indicator Panel");
			if(MoveFailure && FailurePanel != null)
			{
				FailurePanel.transform.position = FailurePos;
			}
		}
		if(EndPortraitObject == null && ResultsPanel != null && PlayerAnchor != null && EndPortrait && File.Exists(ImagePath))
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
			(EndPortraitLoad, EndPortraitFPS) = OutImage(ImagePath);
			if(EndPortraitLoad.Count > 0)
			{
				EndPortraitImage.sprite = EndPortraitLoad[0];
			}
		}
		if(File.Exists(EndBGPath) && EndBGPath != LastEndBGPath)
		{
			MedalBGSet = true;
			(EndBGLoad, EndBGFPS) = OutImage(EndBGPath);
			LastEndBGPath = EndBGPath;
		}
		if(File.Exists(NewMedalBGPath) && NewMedalBGPath != LastNewMedalBGPath)
		{
			MedalBGSet = true;
			(MedalBGLoad, MedalBGFPS) = OutImage(NewMedalBGPath);
			LastNewMedalBGPath = NewMedalBGPath;
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
			SlideTime += Time.unscaledDeltaTime;
			if((SlideTime > 0.5f && AnimatedPortrait.transform.position.x < -20) || ShowImage)
			{
				SlideTime = 1;
				ShowImage = true;
				float Ease = 1 - 1 / (1 + 1.3f * 5);
				EndPortraitObject.transform.position = Vector3.Lerp(EndPortraitObject.transform.position, ImFinalPos, Ease * Time.unscaledDeltaTime * 5 + (Slide ? 0 : 1));
			}
			PlayerAnchor.SetActive(false);
			if(EndPortraitLoad.Count > 0) EndPortraitImage.sprite = EndPortraitLoad[EndPortraitFrame];
			if(EndPortraitLoad.Count > 1 && EndPortraitFPS > 0)
			{
				EndPortraitTimer += Time.unscaledDeltaTime;
				float interval = 1f / EndPortraitFPS;
				if(EndPortraitTimer >= interval) { EndPortraitTimer -= interval; EndPortraitFrame = (EndPortraitFrame + 1) % EndPortraitLoad.Count; EndPortraitImage.sprite = EndPortraitLoad[EndPortraitFrame]; }
			}
			if(EndBGLoad.Count > 0 && EndBG != null) EndBG.GetComponent<Image>().sprite = EndBGLoad[EndBGFrame];
			if(EndBGLoad.Count > 1 && EndBGFPS > 0 && EndBG != null)
			{
				EndBGTimer += Time.unscaledDeltaTime;
				float interval = 1f / EndBGFPS;
				if(EndBGTimer >= interval) { EndBGTimer -= interval; EndBGFrame = (EndBGFrame + 1) % EndBGLoad.Count; EndBG.GetComponent<Image>().sprite = EndBGLoad[EndBGFrame]; }
			}
			if(MedalBGLoad.Count > 0 && EndBG != null && MedalBGSet) EndBG.GetComponent<Image>().sprite = MedalBGLoad[MedalBGFrame];
			if(MedalBGLoad.Count > 1 && MedalBGFPS > 0 && EndBG != null && MedalBGSet)
			{
				MedalBGTimer += Time.unscaledDeltaTime;
				float interval = 1f / MedalBGFPS;
				if(MedalBGTimer >= interval) { MedalBGTimer -= interval; MedalBGFrame = (MedalBGFrame + 1) % MedalBGLoad.Count; EndBG.GetComponent<Image>().sprite = MedalBGLoad[MedalBGFrame]; }
			}

			if(EndBG != null && MedalCheck != null && NewMedalBG && MedalCheck.activeInHierarchy)
			{
				if(AnimatedPortrait != null && MedalEAPortrait) AnimatedPortrait.SetActive(true); else AnimatedPortrait.SetActive(false);
				EndBG.GetComponent<Image>().color = new Color(1,1,1,EndBG.GetComponent<Image>().color.a);
				EndHideFlames.SetActive(false);
			}
		}
		else
		{
			SlideTime = 0;
			ShowImage = false;
			if(MedalBGSet)
			{
				if(AnimatedPortrait != null && EAPortrait) AnimatedPortrait.SetActive(true); else AnimatedPortrait.SetActive(false);
				Image Img = EndBG.GetComponent<Image>();
				if(Img != null && !ReplaceEndBG)
				{
					MedalBGSet = false;
					Texture2D Tex = new Texture2D(1,1);
					Tex.SetPixel(0,0,Color.white);
					Tex.Apply();
					Img.sprite = Sprite.Create(Tex,new Rect(0,0,1,1),new Vector2(0.5f,0.5f));
					EndHideFlames.SetActive(true);
				}
				else if(Img != null && EndBGLoad.Count > 0)
				{
					MedalBGSet = false;
					Img.sprite = EndBGLoad[0];
					EndHideFlames.SetActive(false);
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
		if (!File.Exists(path))
		{
			Texture2D Fallback = new Texture2D(1, 1);
			Fallback.SetPixel(0, 0, Color.white);
			Fallback.Apply();
			return (new List<Sprite> { Sprite.Create(Fallback, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f)) }, 0);
		}
		if (Path.GetExtension(path).ToLower() != ".gif")
		{
			byte[] Bytes = File.ReadAllBytes(path);
			Texture2D Tex = new Texture2D(2, 2);
			Tex.LoadImage(Bytes);
			return (new List<Sprite> { Sprite.Create(Tex, new Rect(0, 0, Tex.width, Tex.height), new Vector2(0.5f, 0.5f)) }, 0);
		}
		try
		{
			using (MagickImageCollection Gif = new MagickImageCollection(path))
			{
				Gif.Coalesce();
				if (Gif.Count > 0)
				{
					int Delay = (int)Gif[0].AnimationDelay;
					if (Delay > 0)
					{
						Framerate = Mathf.RoundToInt(100f / Delay);
					}
				}
				foreach (MagickImage Frame in Gif)
				{
					Frame.Flip();
					byte[] Rgba = Frame.ToByteArray(MagickFormat.Rgba);
					Texture2D Tex = new Texture2D((int)Frame.Width, (int)Frame.Height, TextureFormat.RGBA32, false);
					Tex.LoadRawTextureData(Rgba);
					Tex.Apply();
					Frames.Add(Sprite.Create(Tex, new Rect(0, 0, Tex.width, Tex.height), new Vector2(0.5f, 0.5f)));
				}
			}
			if (Frames.Count > 0)
			{
				return (Frames, Framerate);
			}
		}
		catch{}
		Texture2D FallbackTex = new Texture2D(1, 1);
		FallbackTex.SetPixel(0, 0, Color.white);
		FallbackTex.Apply();
		return (new List<Sprite> { Sprite.Create(FallbackTex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f)) }, 0);
	}
}