using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : Miscellaneous
{
	private Button _btnSon, _btnMusique;
	private AudioSource _soundCtrl, _musicCtrl;
	private Slider _soundSlider, _musicSlider;
	private Text _pourcentSon, _pourcentMusique;
	private float _previousSoundVol = 0.0f;
	private float _previousMusicVol = 0.0f;
	float lastSoundValue = 0;
	float lastMusicValue = 0;
	public Toggle toggle_french, toggle_english, toggle_german;
	private GameObject containerButtons;
	private Transform optionsMenu, OCB, OCS, OCT; // Options Container Buttons
	// Start is called before the first frame update
	void Start()
	{
		// INITIALISATION
		optionsMenu = GameObject.Find("SubMenus").transform.Find("OptionsMenu").transform;
		OCB = optionsMenu.Find("Buttons").transform;
		OCS = optionsMenu.Find("Sliders").transform;
		OCT = optionsMenu.Find("Text").transform;
		_soundSlider = OCS.Find("Slider Son").GetComponent<Slider>();
		_musicSlider = OCS.Find("Slider Musique").GetComponent<Slider>();
		_pourcentSon = OCT.Find("Pourcent Son").GetComponent<Text>();
		_pourcentMusique = OCT.Find("Pourcent Musique").GetComponent<Text>();
		_btnMusique = OCB.Find("SwitchSound").GetComponent<Button>();
		_btnSon = OCB.Find("SwitchSound").GetComponent<Button>();
		_soundCtrl = GameObject.Find("SoundController").GetComponent<AudioSource>();
		_musicCtrl = GameObject.Find("MusicController").GetComponent<AudioSource>();
		DefaultMusicSound();
		//Subscribe to the Slider event
		_soundSlider.onValueChanged.AddListener(SoundSliderCallBack);
		lastSoundValue = _soundSlider.value;
		_musicSlider.onValueChanged.AddListener(MusicSliderCallBack);
		lastMusicValue = _musicSlider.value;
	}

	public void ToggleValueChangedOM(Toggle curT)
	{
		if (curT.isOn)
			Debug.Log(curT.name);
	}

	//---------------------------- Music/Sound Begin ----------------------------//
	public void Volume(AudioSource ads, Text txt, Slider sb)
	{
		ads.volume = sb.value;
		txt.text = Mathf.RoundToInt(sb.value * 100) + "%";
	}

	public void DisplayVolume(int b, float value)
	{
		GameObject g = _btnMusique.gameObject;
		GameObject gs = _btnSon.gameObject;
		bool tmpS = true; 
		bool tmpM = true;
		if (value > 0)
			if (b == 0)
				tmpS = true;
			else 
				tmpM = true;
		else if (b == 0)
				tmpS = false;
			else 
				tmpM = false;
		gs.transform.Find("SonON").gameObject.SetActive(tmpS);
		gs.transform.Find("SonOFF").gameObject.SetActive(!tmpS);
		g.transform.Find("MusicON").gameObject.SetActive(tmpM);
		g.transform.Find("MusicOFF").gameObject.SetActive(!tmpM);
		if (b == 0)
			Volume(_soundCtrl, _pourcentSon, _soundSlider);
		else
			Volume(_musicCtrl, _pourcentMusique, _musicSlider);
	}

	public void DefaultMusicSound()
	{
		_soundSlider.maxValue = _musicSlider.maxValue = 1; 
		_soundCtrl.volume = _musicCtrl.volume = _soundSlider.value = _musicSlider.value = 0.4f;
		Volume(_soundCtrl, _pourcentSon, _soundSlider);
		Volume(_musicCtrl, _pourcentMusique, _musicSlider);
	}

	//Will be called when Scrollbar changes
	public void SoundSliderCallBack(float value)
	{
		DisplayVolume(0, value);
	}

	//Will be called when Scrollbar changes
	public void MusicSliderCallBack(float value)
	{
		DisplayVolume(1, value);
	}

	public void SwitchSound()
	{
		if (_soundSlider.value != 0)
		{
			_previousSoundVol = _soundCtrl.volume;
			_soundSlider.value = 0.0f;
			DisplayVolume(0, _soundSlider.value);
		}
		else
		{
			_soundSlider.value = _previousSoundVol;
			DisplayVolume(0, _previousSoundVol);
		}
	}

	public void SwitchMusic()
	{
		if (_musicSlider.value != 0)
		{
			_previousMusicVol = _musicCtrl.volume;
			_musicSlider.value = 0.0f;
			DisplayVolume(1, _musicSlider.value);
		}
		else
		{
			_musicSlider.value = _previousMusicVol;
			DisplayVolume(1, _previousMusicVol);
		}
	}

	// -------------- Music/Sound End -----------------------//
	public void HideOptions()
	{
		ChangeMenu("OptionsMenu", "HomeMenu");
	}

	public void FullScreen()
	{
		Screen.fullScreen = !Screen.fullScreen;
	}

	public void Help()
	{
		Application.OpenURL("https://tinyurl.com/SlapDance");
	}

	public void ShowCredits()
	{
		ChangeMenu("OptionsMenu", "CreditsMenu");
	}
}