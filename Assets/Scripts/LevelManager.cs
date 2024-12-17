using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private bool gameUnactive = false;

    private GameObject HUD;
    private GameObject pauseMenu;
    private GameObject deathMenu;
    private GameObject winMenu;

    private GameObject affichageGrenade;

    private TextMeshProUGUI victoireTexte;

    private bool isGamePaused;
    private bool gameWon = false;

    private TextMeshProUGUI timerText;
    private float timer = 0f;

    private PlayerLogic player;
    private PistolLogic pistoletJoueur;
    private TextMeshProUGUI NombreMunitionsDuJoueurText;

    private TextMeshProUGUI affichageKey;
    private TextMeshProUGUI affichageDocumentUn;
    private TextMeshProUGUI affichageDocumentDeux;
    private Color couleurDesChamps;

    private GameObject cochePremDocument;
    private GameObject cocheDeuxDocument;
    private GameObject cocheKey;

    private GameObject premPorte;
    private GameObject deuxPorte;

    private bool grenadeTireParJoueur = false;

    [SerializeField]
    Image healthBar;

    private GameObject conteneurPistolImage;
    private GameObject conteneurSmgImage;
    private GameObject conteneurArImage;
    
    private Image pistolImage;
    private Image smgImage;
    private Image arImage;

    private Color activeColor = Color.yellow;
    private Color inactiveColor = Color.white;

    // Sounds
    private AudioSource soundSource;

    [SerializeField]
    AudioClip backgroundMusic;

    [SerializeField]
    AudioClip missedGunshot;

    private AudioSource musicSource;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        gameUnactive = false;

        pauseMenu = GameObject.Find("PauseMenu");
        pauseMenu.SetActive(false);

        deathMenu = GameObject.Find("DeathMenu");
        deathMenu.SetActive(false);

        winMenu = GameObject.Find("WinMenu");
        victoireTexte = GameObject.Find("WinMenuTitle").GetComponent<TextMeshProUGUI>();
        winMenu.SetActive(false);
        
        HUD = GameObject.Find("HUD");

        gameUnactive = false;

        affichageGrenade = GameObject.Find("AffichageGrenade");
        affichageGrenade.SetActive(false);

        conteneurPistolImage = GameObject.Find("PistolImage");
        pistolImage = conteneurPistolImage.GetComponent<Image>();
        pistolImage.color = activeColor;

        conteneurSmgImage = GameObject.Find("SMGImage");
        smgImage = conteneurSmgImage.GetComponent<Image>();
        conteneurSmgImage.SetActive(false);

        conteneurArImage = GameObject.Find("ARImage");
        arImage = conteneurArImage.GetComponent<Image>();
        conteneurArImage.SetActive(false);

        timerText = GameObject.Find("TimePassed").GetComponent<TextMeshProUGUI>();

        player = GameObject.Find("Player").GetComponent<PlayerLogic>();
        pistoletJoueur = player.GetComponentInChildren<PistolLogic>();
        NombreMunitionsDuJoueurText = GameObject.Find("MunitionsJoueur").GetComponent<TextMeshProUGUI>();

        affichageDocumentUn = GameObject.Find("Document_01_text").GetComponent<TextMeshProUGUI>();
        affichageDocumentDeux = GameObject.Find("Document_02_text").GetComponent<TextMeshProUGUI>();
        affichageKey = GameObject.Find("KeyText").GetComponent<TextMeshProUGUI>();
        couleurDesChamps = affichageDocumentUn.color;

        cochePremDocument = GameObject.Find("Document_01_check");
        cochePremDocument.SetActive(false);
        
        cocheDeuxDocument = GameObject.Find("Document_02_check");
        cocheDeuxDocument.SetActive(false);
        
        cocheKey = GameObject.Find("Key_check");
        cocheKey.SetActive(false);

        premPorte = GameObject.Find("Door01");
        deuxPorte = GameObject.Find("Door02");

        // Musique
        musicSource = GameObject.Find("MusicSource").GetComponent<AudioSource>();
        soundSource = GameObject.Find("SoundSource").GetComponent<AudioSource>();

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = SettingsManager.instance.GetMusicVolume();
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // Arrêt du jeu à la mort
        if (!player.getIsPlayerAlive())
        {
            Time.timeScale = 0f;
            deathMenu.SetActive(true);
            HUD.SetActive(false);

            gameUnactive = true;

            return;
        }
        
        // Affichage victoire
        if (gameWon)
        {
            Time.timeScale = 0f;
            winMenu.SetActive(true);
            HUD.SetActive(false);

            gameUnactive = true;

            return;
        }

        TimePlayedUpdate();

        // Affichage du menu de jeu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AffichageMenuOptions();
        }

        ChangementHUDMunitionsJoueur();
        
        ChangementHUDVie();
    }

    private void TimePlayedUpdate()
    {
        timer += Time.deltaTime;

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        timerText.text = string.Format("Temps : {0:00}:{1:00}", minutes, seconds);
    }

    public bool getIsGamePaused()
    {
        return isGamePaused;
    }

    public void AffichageMenuOptions()
    {
        if (isGamePaused)
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            gameUnactive = false;

            HUD.SetActive(true);
        }
        else
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
            HUD.SetActive(false);
            gameUnactive = true;
        }

        isGamePaused = !isGamePaused;
    }

    public void OnBoutonContinuer()
    {
        pauseMenu.SetActive(false);
        HUD.SetActive(true);

        gameUnactive = false;

        Time.timeScale = 1f;
        isGamePaused = false;
    }

    public void OnBoutonQuitter()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Affichage munitions du joueur
    public void ChangementHUDMunitionsJoueur()
    {
        NombreMunitionsDuJoueurText.text = pistoletJoueur.getNbMunitions() + " x " + player.getNbMunitions();
    }

    public void ChangementHUDVie()
    {
        healthBar.fillAmount = player.getNombreDePointsVie() / 100;
    }

    public bool getGameUnactive()
    {
        return gameUnactive;
    }

    // Affichage prise des documents par joueur
    public void cocherElement(GameObject elementRecup)
    {
        if (elementRecup.name == "Document_01")
        {
            cochePremDocument.SetActive(true);
        }
        else if (elementRecup.name == "Document_02")
        {
            cocheDeuxDocument.SetActive(true);
        }
        else if (elementRecup.name == "Key")
        {
            cocheKey.SetActive(true);
        }

        Destroy(elementRecup);
    }

    // Zone victoire avec les objets
    public void joueurAtteintZoneVictoire(string[] objetsCollectioneJoueur)
    {
        if (objetsCollectioneJoueur.Contains("Document_01") && objetsCollectioneJoueur.Contains("Document_02") && objetsCollectioneJoueur.Contains("Key"))
        {
            Vector3 anglesPremPorte = premPorte.transform.rotation.eulerAngles;
            anglesPremPorte.z = 45;
            premPorte.transform.rotation = Quaternion.Euler(anglesPremPorte);

            Vector3 anglesDeuxPorte = deuxPorte.transform.rotation.eulerAngles;
            anglesDeuxPorte.z = -45;
            deuxPorte.transform.rotation = Quaternion.Euler(anglesDeuxPorte);

            GameWon();
            return;
        }
        
        if (!objetsCollectioneJoueur.Contains("Document_01"))
        {
            affichageDocumentUn.color = Color.yellow;
        }

        if (!objetsCollectioneJoueur.Contains("Document_02"))
        {
            affichageDocumentDeux.color = Color.yellow;
        }

        if (!objetsCollectioneJoueur.Contains("Key"))
        {
            affichageKey.color = Color.yellow;
        }
    }

    public void GameWon()
    {
        gameWon = true;

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        timerText.text = string.Format("Temps : {0:00}:{1:00}", minutes, seconds);

        victoireTexte.text = string.Format("Vous avez sauvé la planète en {0:00}:{1:00}", minutes, seconds);
        
        winMenu.SetActive(true);
    }

    public void OnReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void retourMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        //SettingsManager.instance.redemarreAnimationsJeu();
    }

    // Affichage de grenade
    public void JoueurContientGrenade(GameObject grenade)
    {
        affichageGrenade.SetActive(true);
        Destroy(grenade);
    }

    public void GrenadeTirer()
    {
        affichageGrenade.SetActive(false);
        grenadeTireParJoueur = true;
    }

    public bool getGrenadeTire()
    {
        return grenadeTireParJoueur;
    }

    // Son lorsqu'on rate les ennemis
    public void makeMissedGunshotSound()
    {
        AudioClip clip = missedGunshot;
        soundSource.clip = clip;
        soundSource.volume = SettingsManager.instance.GetSoundVolume();
        soundSource.Play();
    }

    // Affichage des armes prises
    public void SMGPickesUpAppear()
    {
        conteneurSmgImage.SetActive(true);
    }
    
    public void ARPickesUpAppear()
    {
        conteneurArImage.SetActive(true);
    }

    // Affichage de l'arm actuelle
    public void changementArmeJoueur(PistolLogic nouvArme)
    {
        GameObject arme = nouvArme.gameObject;
        pistoletJoueur = nouvArme;

        pistolImage.color = inactiveColor;
        smgImage.color = inactiveColor;
        arImage.color = inactiveColor;

        if (arme.CompareTag("Pistol"))
            pistolImage.color = activeColor;
        else if (arme.CompareTag("SMG"))
            smgImage.color = activeColor;
        else if (arme.CompareTag("AR"))
            arImage.color = activeColor;
    }

    public void sortieZoneVictoire()
    {
        affichageKey.color = couleurDesChamps;
        affichageDocumentUn.color = couleurDesChamps;
        affichageDocumentDeux.color = couleurDesChamps;
    }
}
