using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnnemyLogic : MonoBehaviour
{
    private NavMeshAgent agent;

    [SerializeField]
    GameObject allWayPoints;
    private List<Vector3> wayPointsPositions;
    private int currentWayPoint;
    private Vector3 positionInitial;

    private bool vivant = true;

    private float pointsDeVie = 100f;

    public bool playerDetected = false;
    private GameObject cible;

    private PistolLogic pistoletEnnemi;

    private float tempsAvantDeTirer = 2f;

    private float reloadingTime = 0f;
    private bool reloading = false;

    [SerializeField]
    GameObject zoneDetection;

    private float hardDifficultySpeed = 1.25f;
    private float hardDifficultyDetection = 1.3f;

    private float easyDifficultyPrecision = 0.33f;
    private float hardDifficultyPrecision = 0.5f;

    [SerializeField]
    GameObject pistolPrefab;

    [SerializeField]
    GameObject munitionPrefab;

    private bool lookAtPlayer = false;

    // Sounds
    private AudioSource soundSource;

    [SerializeField]
    AudioClip dyingScream;

    // Start is called before the first frame update
    void Start()
    {
        soundSource = GetComponentInChildren<AudioSource>();

        cible = GameObject.Find("Player");

        pistoletEnnemi = GetComponentInChildren<PistolLogic>();

        agent = GetComponent<NavMeshAgent>();

        // Ennemi bouge si il contient une trajectoire
        if (allWayPoints != null)
        {
            wayPointsPositions = new List<Vector3>();
            foreach (Transform waypoint in allWayPoints.GetComponentsInChildren<Transform>())
            {
                wayPointsPositions.Add(waypoint.position);
            }
            wayPointsPositions.Remove(allWayPoints.transform.position);
            currentWayPoint = 0;
            agent.SetDestination(wayPointsPositions[0]);

            if (SettingsManager.instance.GetHardDifficulty())
            {
                agent.speed = agent.speed * hardDifficultySpeed;
            }

            positionInitial = transform.position;
        }

        if (SettingsManager.instance.GetHardDifficulty())
        {
            if (zoneDetection != null)
            {
                zoneDetection.transform.localScale *= hardDifficultyDetection;
                Debug.Log(zoneDetection.transform.localScale);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (vivant)
        {
            tempsAvantDeTirer += Time.deltaTime;

            if (reloading)
            {
                reloadingTime += Time.deltaTime;

                if (reloadingTime >= 3f)
                {
                    pistoletEnnemi.recharge(pistoletEnnemi.getNbBallesMaximals());
                    pistoletEnnemi.makeReloadingSound();

                    reloadingTime = 0f;
                    reloading = false;
                    return;
                }
            }

            // Mouvement vers joueur si detecté, sinon continuer trajectoire de base
            if (playerDetected)
            {
                detectionEnnemi();
            }
            else
            {
                if (allWayPoints != null)
                {
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        currentWayPoint = ++currentWayPoint % wayPointsPositions.Count;

                        agent.SetDestination(wayPointsPositions[currentWayPoint]);
                    }
                }
            }
        }
    }

    void detectionEnnemi()
    {
        if (lookAtPlayer)
            transform.LookAt(cible.transform.position);

        // Verification si joueur se cache derrière un mur ou autre objet pour fuire
        RaycastHit hit;
        Vector3 rayDirection = cible.transform.position - transform.position;
        if (Physics.Raycast(transform.position, rayDirection, out hit))
        {
            if (hit.collider.gameObject == cible)
            {
                if (!lookAtPlayer)
                    lookAtPlayer = true;
                if (!playerDetected)
                    playerDetected = true;

                transform.LookAt(cible.transform.position);
                distanceEntreJoueur();
                gestionTir();
            }
            else
            {
                playerDetected = false;
                lookAtPlayer = false;
                if (allWayPoints == null)
                {
                    agent.isStopped = true;
                }
                else
                {
                    agent.SetDestination(wayPointsPositions[currentWayPoint]);
                }
            }
        }
    }

    void distanceEntreJoueur()
    {
        if (agent.isStopped)
            agent.isStopped = false;

        Vector3 direction = (cible.transform.position - transform.position).normalized;
        Vector3 targetPosition = cible.transform.position - direction * 5;
        agent.SetDestination(targetPosition);
    }

    private void gestionTir()
    {
        if (!reloading)
        {
            if (tempsAvantDeTirer >= 2f)
            {
                pistoletEnnemi.tirer();

                if (!SettingsManager.instance.GetHardDifficulty())
                {
                    if (Random.value <= easyDifficultyPrecision && tempsAvantDeTirer >= 2f)
                    {
                        cible.GetComponent<PlayerLogic>().touche(pistoletEnnemi.getDegatsParBalle());
                    }
                }
                else
                {
                    if (Random.value <= hardDifficultyPrecision && tempsAvantDeTirer >= 2f)
                    {
                        cible.GetComponent<PlayerLogic>().touche(pistoletEnnemi.getDegatsParBalle());
                    }
                }

                tempsAvantDeTirer = 0f;

                if (pistoletEnnemi.getNbMunitions() <= 0)
                {
                    reloading = true;
                }
            }
        }
    }

    public void PrendreDegats(float degats)
    {
        if (vivant)
        {
            pointsDeVie -= degats;

            if (pointsDeVie <= 0)
            {
                makeDyingScream();

                // Échapper objets lors de mort
                Instantiate(pistolPrefab, transform.position, Quaternion.identity);
                Instantiate(munitionPrefab, transform.position, Quaternion.identity);

                // Désactiver le NavMeshAgent et autres composants
                agent.enabled = false;
                if (zoneDetection != null)
                {
                    zoneDetection.SetActive(false);
                }

                // Changer la rotation pour simuler la mort
                Vector3 anglesMort = gameObject.transform.rotation.eulerAngles;
                anglesMort.x = 90;
                transform.rotation = Quaternion.Euler(anglesMort);

                vivant = false;
            }
        }
    }

    public void onPlayerDetected()
    {
        playerDetected = true;
    }

    // Cri lors de sa mort
    public void makeDyingScream()
    {
        soundSource.clip = dyingScream;
        soundSource.volume = SettingsManager.instance.GetSoundVolume();
        soundSource.Play();
    }
}
