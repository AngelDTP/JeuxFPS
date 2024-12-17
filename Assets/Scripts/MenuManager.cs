using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class MenuManager : MonoBehaviour
{
    // Menu principal
    private GameObject MenuTitle;
    private Color TitleColor;
    private Button CommencerButton;
    private Color CommencerButtonTextColor;
    private Image CommencerButtonImage;
    private Color CommencerButtonBackgroundColor;

    private GameObject MenuPrincipalTitle;
    Vector3 TitleTranslation;

    // Main Menu
    private Button playButton;
    private Color playButtonTextColor;
    private Image playButtonImage;
    private Color playButtonBackgroundColor;

    Vector3 positionMilieuBoutons;
    Vector3 middleButtonTranslation;

    private Button OptionsButton;
    private Color OptionsButtonTextColor;
    private Image OptionsButtonImage;
    private Color OptionsButtonBackgroundColor;

    // Menu jouer
    private GameObject playMenuTitle;
    private Color playMenuTitleColor;

    private Button backButtonFromPlay;
    private Color backButtonFromPlayTextColor;
    private Image backButtonFromPlayImage;
    private Color backButtonFromPlayBackgroundColor;

    private GameObject boutonsDifficulte;

    // Menu Options
    private GameObject optionsMenuTitle;
    private Color optionsMenuTitleColor;

    private GameObject OptionsContainer;

    private Button backButtonFromOptions;
    private Color backButtonFromOptionsTextColor;
    private Image backButtonFromOptionsImage;
    private Color backButtonFromOptionsBackgroundColor;

    Vector3 OptionsAndBackButtonFinalPosition;
    Vector3 OptionsAndBackButtonFinalTranslation;

    private enum MenuState
    {
        OpeningGame,
        PagePrincipalState,
        PagePrincipaleToMainMenuFirstAnimation,
        PagePrincipaleToMainMenuSecondAnimation,
        MainMenuState,
        MainMenuToOptionsMenuFirstAnimation,
        MainMenuToOptionsMenuSecondAnimation,
        OptionsMenuState,
        OptionsMenuToMainMenuFirstAnimation,
        OptionsMenuToMainMenuSecondAnimation,
        MainMenuToPlayMenuFirstAnimation,
        MainMenuToPlayMenuSecondAnimation,
        PlayMenu,
        PlayMenuToMainMenuFirstAnimation,
        PlayMenuToMainMenuSecondAnimation
    }

    private MenuState state;

    private float AnimationSpeed = 1f;
    
    TextMeshProUGUI musicValueText;
    Slider musicValueSlider;

    TextMeshProUGUI soundValueText;
    Slider soundValueSlider;

    AudioSource musicSource;
    AudioSource soundSource;
    
    // Start is called before the first frame update
    void Start()
    {
        state = MenuState.OpeningGame;

        MenuTitreInitialized();

        MenuPrincipalInitialized();

        PlayMenuInitialized();

        TitleTranslation = MenuPrincipalTitle.transform.position - MenuTitle.transform.position;

        middleButtonTranslation = CommencerButton.transform.position - playButton.transform.position;

        positionMilieuBoutons = CommencerButton.transform.position;

        OptionsAndBackButtonFinalPosition = GameObject.Find("OptionsAndBackButtonFinalPosition").transform.position;

        OptionsAndBackButtonFinalTranslation = OptionsAndBackButtonFinalPosition - OptionsButton.transform.position;

        musicSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        soundSource = GameObject.Find("SoundSource").GetComponent<AudioSource>();

        musicValueText = GameObject.Find("MusiqueValeur").GetComponent<TextMeshProUGUI>();
        musicValueSlider = GameObject.Find("SliderMusique").GetComponent<Slider>();
        musicValueSlider.value = SettingsManager.instance.GetMusicVolume();
        musicValueText.text = Mathf.CeilToInt(musicValueSlider.value * 100).ToString();
        musicSource.volume = musicValueSlider.value;

        soundValueText = GameObject.Find("SonValeur").GetComponent<TextMeshProUGUI>();
        soundValueSlider = GameObject.Find("SliderSon").GetComponent<Slider>();
        soundValueSlider.value = SettingsManager.instance.GetSoundVolume();
        soundValueText.text = Mathf.CeilToInt(soundValueSlider.value * 100).ToString();
        soundSource.volume = soundValueSlider.value;

        OptionsMenuInitialized();
    }

    // Update is called once per frame
    void Update()
    {
        DemarreJeu();

        MenuTitreVersMenuPrincipal();

        MenuPrincipalVersMenuJouer();

        MenuJouerVersMenuPrincipal();

        MenuPrincipalVersOptions();

        MenuOptionsVersMainMenu();
    }


    private void MenuTitreInitialized()
    {
        MenuTitle = GameObject.Find("GameTitle");

        TitleColor = MenuTitle.GetComponentInChildren<TextMeshProUGUI>().color;
        TitleColor.a = 0;
        MenuTitle.GetComponentInChildren<TextMeshProUGUI>().color = TitleColor;


        CommencerButton = GameObject.Find("BoutonCommencer").GetComponent<Button>();
        CommencerButtonImage = CommencerButton.GetComponent<Image>();
        CommencerButtonBackgroundColor = CommencerButtonImage.color;
        CommencerButtonBackgroundColor.a = 0;
        CommencerButtonImage.color = CommencerButtonBackgroundColor;

        CommencerButton.enabled = false;

        CommencerButtonTextColor = CommencerButton.GetComponentInChildren<TextMeshProUGUI>().color;
        CommencerButtonTextColor.a = 0;
        CommencerButton.GetComponentInChildren<TextMeshProUGUI>().color = CommencerButtonTextColor;
    }

    private void MenuPrincipalInitialized()
    {
        MenuPrincipalTitle = GameObject.Find("GameTitlePrincipal");
        
        playButton = GameObject.Find("BoutonJouer").GetComponent<Button>();
        playButton.gameObject.SetActive(false);

        playButtonImage = playButton.GetComponent<Image>();
        playButtonBackgroundColor = playButtonImage.color;
        playButtonBackgroundColor.a = 0;
        playButtonImage.color = playButtonBackgroundColor;

        playButtonTextColor = playButton.GetComponentInChildren<TextMeshProUGUI>().color;
        playButtonTextColor.a = 0;
        playButton.GetComponentInChildren<TextMeshProUGUI>().color = CommencerButtonTextColor;

        OptionsButton = GameObject.Find("BoutonOptions").GetComponentInChildren<Button>();
        OptionsButton.gameObject.SetActive(false);

        OptionsButtonImage = OptionsButton.GetComponentInChildren<Image>();
        OptionsButtonBackgroundColor = playButtonImage.color;
        OptionsButtonBackgroundColor.a = 0;
        OptionsButtonImage.color = OptionsButtonBackgroundColor;

        OptionsButtonTextColor = playButton.GetComponentInChildren<TextMeshProUGUI>().color;
        OptionsButtonTextColor.a = 0;
        OptionsButton.GetComponentInChildren<TextMeshProUGUI>().color = OptionsButtonTextColor;
    }

    private void PlayMenuInitialized()
    {
        playMenuTitle = GameObject.Find("ChoixDifficultéTitre");

        playMenuTitleColor = playMenuTitle.GetComponentInChildren<TextMeshProUGUI>().color;
        playMenuTitleColor.a = 0;
        playMenuTitle.GetComponentInChildren<TextMeshProUGUI>().color = playMenuTitleColor;

        backButtonFromPlay = GameObject.Find("BoutonRetourJouer").GetComponent<Button>();
        backButtonFromPlay.gameObject.SetActive(false);

        backButtonFromPlayImage = backButtonFromPlay.GetComponentInChildren<Image>();
        backButtonFromPlayBackgroundColor = backButtonFromPlayImage.color;
        backButtonFromPlayBackgroundColor.a = 0;
        backButtonFromPlayImage.color = backButtonFromPlayBackgroundColor;

        backButtonFromPlayTextColor = backButtonFromPlay.GetComponentInChildren <TextMeshProUGUI>().color;
        backButtonFromPlayTextColor.a = 0;
        backButtonFromPlay.GetComponentInChildren<TextMeshProUGUI>().color = backButtonFromPlayTextColor;

        boutonsDifficulte = GameObject.Find("BoutonsDifficulté");
        boutonsDifficulte.gameObject.SetActive(false);
    }

    private void OptionsMenuInitialized()
    {
        optionsMenuTitle = GameObject.Find("OptionsMenuTitle");

        optionsMenuTitleColor = optionsMenuTitle.GetComponentInChildren<TextMeshProUGUI>().color;
        optionsMenuTitleColor.a = 0;
        optionsMenuTitle.GetComponentInChildren<TextMeshProUGUI>().color = optionsMenuTitleColor;

        backButtonFromOptions = GameObject.Find("BoutonRetourOptions").GetComponent<Button>();
        backButtonFromOptions.gameObject.SetActive(false);

        backButtonFromOptionsImage = backButtonFromOptions.GetComponentInChildren<Image>();
        backButtonFromOptionsBackgroundColor = backButtonFromOptionsImage.color;
        backButtonFromOptionsBackgroundColor.a = 0;
        backButtonFromOptionsImage.color = backButtonFromOptionsBackgroundColor;

        backButtonFromOptionsTextColor = backButtonFromOptions.GetComponentInChildren<TextMeshProUGUI>().color;
        backButtonFromOptionsTextColor.a = 0;
        backButtonFromOptions.GetComponentInChildren<TextMeshProUGUI>().color = backButtonFromOptionsTextColor;

        OptionsContainer = GameObject.Find("ConteneurOptions");
        OptionsContainer.SetActive(false);
    }

    private void DemarreJeu()
    {
        if (state == MenuState.OpeningGame)
        {
            if (TitleColor.a < 1f)
            {
                TitleColor.a += AnimationSpeed * Time.deltaTime / 2f;
                MenuTitle.GetComponentInChildren<TextMeshProUGUI>().color = TitleColor;
            }
            else if (CommencerButtonBackgroundColor.a < 1f || CommencerButtonTextColor.a < 1f)
            {
                CommencerButtonBackgroundColor.a += AnimationSpeed * Time.deltaTime;
                CommencerButtonImage.color = CommencerButtonBackgroundColor;

                CommencerButtonTextColor.a += AnimationSpeed * Time.deltaTime;
                CommencerButton.GetComponentInChildren<TextMeshProUGUI>().color = CommencerButtonTextColor;
            }
            else
            {
                CommencerButton.enabled = true;
                state = MenuState.PagePrincipalState;
            }
        }
    }

    private void MenuTitreVersMenuPrincipal()
    {
        if (state == MenuState.PagePrincipaleToMainMenuFirstAnimation)
        {
            if (MenuTitle.transform.position.y < MenuPrincipalTitle.transform.position.y)
            {
                MenuTitle.transform.Translate(TitleTranslation * Time.deltaTime);
            }

            if (CommencerButtonBackgroundColor.a > 0f || CommencerButtonTextColor.a > 0f)
            {
                CommencerButtonBackgroundColor.a -= AnimationSpeed * Time.deltaTime;
                CommencerButtonImage.color = CommencerButtonBackgroundColor;

                CommencerButtonTextColor.a -= AnimationSpeed * Time.deltaTime;
                CommencerButton.GetComponentInChildren<TextMeshProUGUI>().color = CommencerButtonTextColor;
            }

            if (CommencerButton.transform.position.x > playButton.transform.position.x)
            {
                CommencerButton.transform.Translate(-middleButtonTranslation * Time.deltaTime);
            }
            
            if (MenuTitle.transform.position.y >= MenuPrincipalTitle.transform.position.y  && 
                CommencerButtonBackgroundColor.a <= 0f || CommencerButtonTextColor.a <= 0f && 
                CommencerButton.transform.position.x <= playButton.transform.position.x)
            {
                CommencerButton.gameObject.SetActive(false);
                CommencerButton.enabled = false;

                playButton.gameObject.SetActive(true);
                OptionsButton.gameObject.SetActive(true);

                OptionsButton.enabled = false;
                    playButton.enabled = false;

                state = MenuState.PagePrincipaleToMainMenuSecondAnimation;
            }
        }

        if (state == MenuState.PagePrincipaleToMainMenuSecondAnimation)
        {
            if (playButtonBackgroundColor.a < 1f || playButtonTextColor.a < 1f)
            {
                playButtonBackgroundColor.a += AnimationSpeed * Time.deltaTime;
                playButtonImage.color = playButtonBackgroundColor;

                playButtonTextColor.a += AnimationSpeed * Time.deltaTime;
                playButton.GetComponentInChildren<TextMeshProUGUI>().color = playButtonTextColor;
            }

            if (playButton.transform.position.x < positionMilieuBoutons.x)
            {
                playButton.transform.Translate(middleButtonTranslation * Time.deltaTime);
            }

            if (OptionsButtonBackgroundColor.a < 1f ||  OptionsButtonTextColor.a < 1f)
            {
                OptionsButtonBackgroundColor.a += AnimationSpeed * Time.deltaTime;
                OptionsButtonImage.color = OptionsButtonBackgroundColor;

                OptionsButtonTextColor.a += AnimationSpeed * Time.deltaTime;
                OptionsButton.GetComponentInChildren<TextMeshProUGUI>().color = OptionsButtonTextColor;
            }

            if (OptionsButton.transform.position.x < OptionsAndBackButtonFinalPosition.x)
            {
                OptionsButton.transform.Translate(OptionsAndBackButtonFinalTranslation * Time.deltaTime);
            }

            if ((playButtonBackgroundColor.a >= 1f || playButtonTextColor.a >= 1f) &&
                 playButton.transform.position.x >= positionMilieuBoutons.x && 
                (OptionsButtonBackgroundColor.a >= 1f ||  OptionsButtonTextColor.a >= 1f) &&
                 OptionsButton.transform.position.x >= OptionsAndBackButtonFinalPosition.x)
                {
                    playButton.enabled = true;
                    OptionsButton.enabled = true;
                    state = MenuState.MainMenuState;
                }
        }
    }

    public void MenuPrincipalVersMenuJouer()
    {
        if (state == MenuState.MainMenuToPlayMenuFirstAnimation)
        {
            if (TitleColor.a > 0f)
            {
                TitleColor.a -= AnimationSpeed * Time.deltaTime;
                MenuTitle.GetComponentInChildren<TextMeshProUGUI>().color = TitleColor;
            }

            if (playButtonBackgroundColor.a >= 0f || playButtonTextColor.a >= 0f)
            {
                playButtonBackgroundColor.a -= AnimationSpeed * Time.deltaTime;
                playButtonImage.color = playButtonBackgroundColor;

                playButtonTextColor.a -= AnimationSpeed * Time.deltaTime;
                playButton.GetComponentInChildren<TextMeshProUGUI>().color = playButtonTextColor;
            }

            if (playButton.transform.position.x > CommencerButton.transform.position.x)
            {
                playButton.transform.Translate(-middleButtonTranslation * Time.deltaTime);
            }

            if (OptionsButtonBackgroundColor.a >= 0f || OptionsButtonTextColor.a >= 0f)
            {
                OptionsButtonBackgroundColor.a -= AnimationSpeed * Time.deltaTime;
                OptionsButtonImage.color = OptionsButtonBackgroundColor;

                OptionsButtonTextColor.a -= AnimationSpeed * Time.deltaTime;
                OptionsButton.GetComponentInChildren<TextMeshProUGUI>().color = OptionsButtonTextColor;
            }

            if (OptionsButton.transform.position.x > backButtonFromPlay.gameObject.transform.position.x)
            {
                OptionsButton.transform.Translate(-OptionsAndBackButtonFinalTranslation * Time.deltaTime);
            }

            if (TitleColor.a <= 0f &&
                (playButtonBackgroundColor.a < 0f || playButtonTextColor.a < 0f) &&
                 playButton.transform.position.x < positionMilieuBoutons.x &&
                (OptionsButtonBackgroundColor.a < 0f || OptionsButtonTextColor.a < 0f) &&
                 OptionsButton.transform.position.x < OptionsAndBackButtonFinalPosition.x)
            {
                playButton.gameObject.SetActive(false);
                OptionsButton.gameObject.SetActive(false);
                backButtonFromPlay.gameObject.SetActive(true);

                playButton.enabled = false;
                OptionsButton.enabled = false;
                backButtonFromPlay.enabled = false;

                state = MenuState.MainMenuToPlayMenuSecondAnimation;
            }
        }

        if (state == MenuState.MainMenuToPlayMenuSecondAnimation)
        {
            if (playMenuTitleColor.a < 1f)
            {
                playMenuTitleColor.a += AnimationSpeed * Time.deltaTime;
                playMenuTitle.GetComponentInChildren<TextMeshProUGUI>().color = playMenuTitleColor;
            }

            if (backButtonFromPlayBackgroundColor.a < 1f || backButtonFromPlayTextColor.a < 1f)
            {
                backButtonFromPlayBackgroundColor.a += AnimationSpeed * Time.deltaTime;
                backButtonFromPlayImage.color = backButtonFromPlayBackgroundColor;

                backButtonFromPlayTextColor.a += AnimationSpeed * Time.deltaTime;
                backButtonFromPlay.GetComponentInChildren<TextMeshProUGUI>().color = backButtonFromPlayTextColor;
            }

            if (backButtonFromPlay.transform.position.x < OptionsAndBackButtonFinalPosition.x)
            {
                backButtonFromPlay.transform.Translate(OptionsAndBackButtonFinalTranslation * Time.deltaTime);
            }

            if (playMenuTitleColor.a >= 1f)
            {
                playButton.gameObject.SetActive(false);
                OptionsButton.gameObject.SetActive(false);

                boutonsDifficulte.SetActive(true);

                backButtonFromPlay.enabled = true;

                state = MenuState.PlayMenu;
            }
        }
    }

    private void MenuJouerVersMenuPrincipal()
    {
        if (state == MenuState.PlayMenuToMainMenuFirstAnimation)
        {
            if (playMenuTitleColor.a > 0f)
            {
                playMenuTitleColor.a -= AnimationSpeed * Time.deltaTime;
                playMenuTitle.GetComponentInChildren<TextMeshProUGUI>().color = playMenuTitleColor;
            }

            if (backButtonFromPlayBackgroundColor.a > 0f || backButtonFromPlayTextColor.a > 0f)
            {
                backButtonFromPlayBackgroundColor.a -= AnimationSpeed * Time.deltaTime;
                backButtonFromPlayImage.color = backButtonFromPlayBackgroundColor;

                backButtonFromPlayTextColor.a -= AnimationSpeed * Time.deltaTime;
                backButtonFromPlay.GetComponentInChildren<TextMeshProUGUI>().color = backButtonFromPlayTextColor;
            }

            if (backButtonFromPlay.transform.position.x > CommencerButton.transform.position.x)
            {
                backButtonFromPlay.transform.Translate(-OptionsAndBackButtonFinalTranslation * Time.deltaTime);
            }

            if (playMenuTitleColor.a <= 0f &&
                (backButtonFromPlayBackgroundColor.a <= 0f || backButtonFromPlayTextColor.a <= 0f) &&
                backButtonFromPlay.transform.position.x > CommencerButton.transform.position.x)
            {
                playButton.gameObject.SetActive(true);
                OptionsButton.gameObject.SetActive(true);
                backButtonFromPlay.gameObject.SetActive(false);
                boutonsDifficulte.SetActive(false);

                backButtonFromPlay.enabled = false;

                state = MenuState.PlayMenuToMainMenuSecondAnimation;
            }
        }

        if (state == MenuState.PlayMenuToMainMenuSecondAnimation)
        {
            if (TitleColor.a < 1f)
            {
                TitleColor.a += AnimationSpeed * Time.deltaTime;
                MenuTitle.GetComponentInChildren<TextMeshProUGUI>().color = TitleColor;
            }

            if (playButtonBackgroundColor.a < 1f || playButtonTextColor.a < 1f)
            {
                playButtonBackgroundColor.a += AnimationSpeed * Time.deltaTime;
                playButtonImage.color = playButtonBackgroundColor;

                playButtonTextColor.a += AnimationSpeed * Time.deltaTime;
                playButton.GetComponentInChildren<TextMeshProUGUI>().color = playButtonTextColor;
            }

            if (playButton.transform.position.x < positionMilieuBoutons.x)
            {
                playButton.transform.Translate(middleButtonTranslation * Time.deltaTime);
            }

            if (OptionsButtonBackgroundColor.a < 1f || OptionsButtonTextColor.a < 1f)
            {
                OptionsButtonBackgroundColor.a += AnimationSpeed * Time.deltaTime;
                OptionsButtonImage.color = OptionsButtonBackgroundColor;

                OptionsButtonTextColor.a += AnimationSpeed * Time.deltaTime;
                OptionsButton.GetComponentInChildren<TextMeshProUGUI>().color = OptionsButtonTextColor;
            }

            if (OptionsButton.transform.position.x < OptionsAndBackButtonFinalPosition.x)
            {
                OptionsButton.transform.Translate(OptionsAndBackButtonFinalTranslation * Time.deltaTime);
            }

            if (TitleColor.a >= 1f &&
                (playButtonBackgroundColor.a >= 1f || playButtonTextColor.a >= 1f) &&
                 playButton.transform.position.x >= positionMilieuBoutons.x &&
                (OptionsButtonBackgroundColor.a >= 1f || OptionsButtonTextColor.a >= 1f) &&
                 OptionsButton.transform.position.x >= OptionsAndBackButtonFinalPosition.x)
            {
                backButtonFromPlay.gameObject.SetActive(false);

                playButton.enabled = true;
                OptionsButton.enabled = true;

                state = MenuState.MainMenuState;
            }
        }
    }

    private void MenuPrincipalVersOptions()
    {
        if (state == MenuState.MainMenuToOptionsMenuFirstAnimation)
        {
            if (TitleColor.a > 0f)
            {
                TitleColor.a -= AnimationSpeed * Time.deltaTime;
                MenuTitle.GetComponentInChildren<TextMeshProUGUI>().color = TitleColor;
            }

            if (playButtonBackgroundColor.a >= 0f || playButtonTextColor.a >= 0f)
            {
                playButtonBackgroundColor.a -= AnimationSpeed * Time.deltaTime;
                playButtonImage.color = playButtonBackgroundColor;

                playButtonTextColor.a -= AnimationSpeed * Time.deltaTime;
                playButton.GetComponentInChildren<TextMeshProUGUI>().color = playButtonTextColor;
            }

            if (playButton.transform.position.x > CommencerButton.transform.position.x)
            {
                playButton.transform.Translate(-middleButtonTranslation * Time.deltaTime);
            }

            if (OptionsButtonBackgroundColor.a >= 0f || OptionsButtonTextColor.a >= 0f)
            {
                OptionsButtonBackgroundColor.a -= AnimationSpeed * Time.deltaTime;
                OptionsButtonImage.color = OptionsButtonBackgroundColor;

                OptionsButtonTextColor.a -= AnimationSpeed * Time.deltaTime;
                OptionsButton.GetComponentInChildren<TextMeshProUGUI>().color = OptionsButtonTextColor;
            }

            if (OptionsButton.transform.position.x > backButtonFromPlay.gameObject.transform.position.x)
            {
                OptionsButton.transform.Translate(-OptionsAndBackButtonFinalTranslation * Time.deltaTime);
            }

            if (TitleColor.a <= 0f &&
                (playButtonBackgroundColor.a < 0f || playButtonTextColor.a < 0f) &&
                 playButton.transform.position.x < positionMilieuBoutons.x &&
                (OptionsButtonBackgroundColor.a < 0f || OptionsButtonTextColor.a < 0f) &&
                 OptionsButton.transform.position.x < OptionsAndBackButtonFinalPosition.x)
            {
                playButton.gameObject.SetActive(false);
                OptionsButton.gameObject.SetActive(false);
                backButtonFromOptions.gameObject.SetActive(true);
                OptionsContainer.SetActive(true);

                playButton.enabled = false;
                OptionsButton.enabled = false;

                backButtonFromOptions.enabled = true;

                state = MenuState.MainMenuToOptionsMenuSecondAnimation;
            }
        }

        if (state == MenuState.MainMenuToOptionsMenuSecondAnimation)
        {
            if (optionsMenuTitleColor.a < 1f)
            {
                optionsMenuTitleColor.a += AnimationSpeed * Time.deltaTime;
                optionsMenuTitle.GetComponentInChildren<TextMeshProUGUI>().color = optionsMenuTitleColor;
            }

            if (backButtonFromOptionsBackgroundColor.a < 1f || backButtonFromOptionsTextColor.a < 1f)
            {
                backButtonFromOptionsBackgroundColor.a += AnimationSpeed * Time.deltaTime;
                backButtonFromOptionsImage.color = backButtonFromOptionsBackgroundColor;

                backButtonFromOptionsTextColor.a += AnimationSpeed * Time.deltaTime;
                backButtonFromOptions.GetComponentInChildren<TextMeshProUGUI>().color = backButtonFromOptionsTextColor;
            }

            if (backButtonFromOptions.transform.position.x < OptionsAndBackButtonFinalPosition.x)
            {
                backButtonFromOptions.transform.Translate(OptionsAndBackButtonFinalTranslation * Time.deltaTime);
            }

            if (playMenuTitleColor.a >= 1f)
            {
                playButton.gameObject.SetActive(false);
                OptionsButton.gameObject.SetActive(false);

                backButtonFromOptions.enabled = true;

                state = MenuState.OptionsMenuState;
            }
        }
    }

    private void MenuOptionsVersMainMenu()
    {
        if (state == MenuState.OptionsMenuToMainMenuFirstAnimation)
        {
            if (optionsMenuTitleColor.a > 0f)
            {
                optionsMenuTitleColor.a -= AnimationSpeed * Time.deltaTime;
                optionsMenuTitle.GetComponentInChildren<TextMeshProUGUI>().color = optionsMenuTitleColor;
            }

            if (backButtonFromOptionsBackgroundColor.a > 0f || backButtonFromOptionsTextColor.a > 0f)
            {
                backButtonFromOptionsBackgroundColor.a -= AnimationSpeed * Time.deltaTime;
                backButtonFromOptionsImage.color = backButtonFromOptionsBackgroundColor;

                backButtonFromOptionsTextColor.a -= AnimationSpeed * Time.deltaTime;
                backButtonFromOptions.GetComponentInChildren<TextMeshProUGUI>().color = backButtonFromOptionsTextColor;
            }

            if (backButtonFromOptions.transform.position.x > CommencerButton.transform.position.x)
            {
                backButtonFromOptions.transform.Translate(-OptionsAndBackButtonFinalTranslation * Time.deltaTime);
            }

            if (optionsMenuTitleColor.a <= 0f &&
                (backButtonFromOptionsBackgroundColor.a <= 0f || backButtonFromOptionsTextColor.a <= 0f) &&
                backButtonFromOptions.transform.position.x > CommencerButton.transform.position.x)
            {
                playButton.gameObject.SetActive(true);
                OptionsButton.gameObject.SetActive(true);
                backButtonFromOptions.gameObject.SetActive(false);
                OptionsContainer.SetActive(false);

                backButtonFromOptions.enabled = false;

                state = MenuState.OptionsMenuToMainMenuSecondAnimation;
            }
        }

        if (state == MenuState.OptionsMenuToMainMenuSecondAnimation)
        {
            if (TitleColor.a < 1f)
            {
                TitleColor.a += AnimationSpeed * Time.deltaTime;
                MenuTitle.GetComponentInChildren<TextMeshProUGUI>().color = TitleColor;
            }

            if (playButtonBackgroundColor.a < 1f || playButtonTextColor.a < 1f)
            {
                playButtonBackgroundColor.a += AnimationSpeed * Time.deltaTime;
                playButtonImage.color = playButtonBackgroundColor;

                playButtonTextColor.a += AnimationSpeed * Time.deltaTime;
                playButton.GetComponentInChildren<TextMeshProUGUI>().color = playButtonTextColor;
            }

            if (playButton.transform.position.x < positionMilieuBoutons.x)
            {
                playButton.transform.Translate(middleButtonTranslation * Time.deltaTime);
            }

            if (OptionsButtonBackgroundColor.a < 1f || OptionsButtonTextColor.a < 1f)
            {
                OptionsButtonBackgroundColor.a += AnimationSpeed * Time.deltaTime;
                OptionsButtonImage.color = OptionsButtonBackgroundColor;

                OptionsButtonTextColor.a += AnimationSpeed * Time.deltaTime;
                OptionsButton.GetComponentInChildren<TextMeshProUGUI>().color = OptionsButtonTextColor;
            }

            if (OptionsButton.transform.position.x < OptionsAndBackButtonFinalPosition.x)
            {
                OptionsButton.transform.Translate(OptionsAndBackButtonFinalTranslation * Time.deltaTime);
            }

            if (TitleColor.a >= 1f &&
                (playButtonBackgroundColor.a >= 1f || playButtonTextColor.a >= 1f) &&
                 playButton.transform.position.x >= positionMilieuBoutons.x &&
                (OptionsButtonBackgroundColor.a >= 1f || OptionsButtonTextColor.a >= 1f) &&
                 OptionsButton.transform.position.x >= OptionsAndBackButtonFinalPosition.x)
            {
                backButtonFromPlay.gameObject.SetActive(false);

                playButton.enabled = true;
                OptionsButton.enabled = true;

                state = MenuState.MainMenuState;
            }
        }
    }

    public void OnCommencerButton() {
        state = MenuState.PagePrincipaleToMainMenuFirstAnimation;
    }

    public void OnJouerButton()
    {
        state = MenuState.MainMenuToPlayMenuFirstAnimation;
    }

    public void OnRetourFromJouer()
    {
        state = MenuState.PlayMenuToMainMenuFirstAnimation;
    }

    public void OnOptionsButton()
    {
        state = MenuState.MainMenuToOptionsMenuFirstAnimation;
    }

    public void OnRetourFromOptions()
    {
        state = MenuState.OptionsMenuToMainMenuFirstAnimation;
    }

    public void OnDifficulteFacileChoisit()
    {
        SettingsManager.instance.setHardDifficulty(false);
        SceneManager.LoadScene("Level_01");
    }
    
    public void OnDifficulteDifficileChoisit()
    {
        SettingsManager.instance.setHardDifficulty(true);
        SceneManager.LoadScene("Level_01");
    }

    public void MusicSliderChange()
    {
        musicValueText.text = Mathf.CeilToInt(musicValueSlider.value * 100).ToString();
        musicSource.volume = musicValueSlider.value;
        SettingsManager.instance.SetMusicVolume(musicValueSlider.value);
  
    }

    public void SoundSliderChange()
    {
        soundValueText.text = Mathf.CeilToInt(soundValueSlider.value * 100).ToString();
        soundSource.volume = soundValueSlider.value;
        SettingsManager.instance.SetSoundVolume(soundValueSlider.value);
    }

    public void demarreJeu()
    {
        state = MenuState.OpeningGame;
    }
}
