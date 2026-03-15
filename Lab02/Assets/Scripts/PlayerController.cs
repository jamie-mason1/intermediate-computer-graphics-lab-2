using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using FMODUnity;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float force = 0.2f;
    [SerializeField] private float jumpForce = 1f;

    [Header("References")]
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private PauseMenu pause;
    [SerializeField] private CinemachineBrain brain; // Main camera's Cinemachine Brain
    [SerializeField] private CinemachineVirtualCamera controlCamera; // VC that allows player input

    private Rigidbody rb;
    private Camera cam;
    private Vector3 movementDir;
    private bool grounded;
    private bool justStarted;
    private bool wasBouncing;
    private bool wasRolling;
    private bool wasExploding;
    private bool lastPausedState;

    // Audio objects
    private Explosion explosionSound;
    private BackgroundMusic music;
    private BallSounds[] bounce = new BallSounds[3];
    private BallSounds roll;
    private float currentTransitionMusic;
    private float t;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;

        explosionSound = new Explosion();
        roll = new BallSounds();
        music = new BackgroundMusic();
        bounce = new BallSounds[3];

        justStarted = true;
        grounded = false;
        wasBouncing = false;
        wasRolling = false;
        wasExploding = false;
        currentTransitionMusic = 0;
        t = 0;

        // Start background music
        music.StrollMusic.StartEventSound();
        music.ManageMusic(0);
    }

    private void Update()
    {
        // Skip controls if the game is paused
        if (pause.paused)
        {
            HandlePausedSounds();
            lastPausedState = pause.paused;
            return;
        }

        // Skip controls if the current Cinemachine camera is not the one we allow
        if (!IsControlCameraActive())
            return;

        HandleMovement();
        HandleMusic();
        lastPausedState = pause.paused;

        if (justStarted)
            justStarted = false;
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        movementDir = (cam.transform.right * horizontalInput) + (cam.transform.forward * verticalInput);
        movementDir.y = 0f;
        movementDir.Normalize();

        // Jump
        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }

        // Roll sounds based on angular velocity
        roll.ManageRollSpeedSound(rb.angularVelocity.magnitude, transform.position);

        // Apply movement
        rb.AddForce(movementDir * speed * Time.deltaTime, ForceMode.Acceleration);
    }

    private void HandleMusic()
    {
        float distanceToTarget = Vector3.Distance(transform.position, GameObject.FindWithTag("Push").transform.position);

        if (distanceToTarget <= 4f)
        {
            if (!music.BattleMusic.IsEventPlaying())
                music.BattleMusic.StartEventSound();

            t += Time.deltaTime;
        }
        else
        {
            t -= Time.deltaTime;
            t = Mathf.Clamp01(t);

            if (t < 0.5f && music.BattleMusic.IsEventPlaying())
                music.BattleMusic.stopSound();
        }

        t = Mathf.Clamp01(t);
        music.ManageMusic(t);
    }

    private bool IsControlCameraActive()
    {
        if (brain == null || controlCamera == null)
            return true; // default to true if references missing

        var liveCam = brain.ActiveVirtualCamera;
        if (liveCam != null)
            return liveCam.VirtualCameraGameObject == controlCamera.gameObject;

        return false;
    }

    private void HandlePausedSounds()
    {
        if (explosionSound.explode.IsEventPlaying())
            explosionSound.explode.PauseEventSound();

        if (roll.rollSound.IsEventPlaying())
            roll.rollSound.PauseEventSound();

        for (int i = 0; i < bounce.Length; i++)
        {
            if (bounce[i] == null) bounce[i] = new BallSounds();
            if (bounce[i].bounceSound.IsEventPlaying())
                bounce[i].bounceSound.PauseEventSound();
        }

        if (music.BattleMusic.IsEventPlaying())
            music.BattleMusic.PauseEventSound();

        if (music.StrollMusic.IsEventPlaying())
            music.StrollMusic.PauseEventSound();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Push"))
        {
            if (collision.gameObject.GetComponent<Rigidbody>() != null)
            {
                collision.gameObject.GetComponent<Rigidbody>().AddForce(movementDir * force, ForceMode.Impulse);
                rb.AddForce(movementDir * force, ForceMode.Impulse);

                explosion.transform.position = collision.contacts[0].point;
                for (int i = 0; i < collision.contacts.Length; i++)
                {
                    if ((collision.contacts[i].point - transform.position).y < 0)
                        grounded = true;
                }
            }
        }

        if (collision.gameObject.CompareTag("Floor"))
        {
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if ((collision.contacts[i].point - transform.position).y < 0)
                    grounded = true;

                Vector3 impulse = collision.impulse;
                Vector3 forceVec = impulse / Time.fixedDeltaTime;

                if (!justStarted)
                {
                    for (int j = 0; j < bounce.Length; j++)
                    {
                        if (bounce[j] == null) bounce[j] = new BallSounds();
                        bounce[j].ManageBounceIntensity(forceVec.magnitude, collision.contacts[0].point);

                        if (!bounce[j].bounceSound.IsEventPlaying())
                        {
                            bounce[j].bounceSound.StartEventSound();
                            wasBouncing = true;
                            break;
                        }
                    }
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Push") && collision.gameObject.GetComponent<Rigidbody>() != null)
            collision.gameObject.GetComponent<Rigidbody>().AddForce(movementDir * force, ForceMode.Force);

        if (collision.gameObject.CompareTag("Floor"))
        {
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if ((collision.contacts[i].point - transform.position).y < 0)
                    grounded = true;

                if (!pause.paused && !roll.rollSound.IsEventPlaying())
                {
                    roll.rollSound.StartEventSound();
                    wasRolling = true;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Push") || collision.gameObject.CompareTag("Floor"))
        {
            grounded = false;
            roll.rollSound.stopSound();
        }
    }

    private void OnDestroy()
    {
        explosionSound.explode.EndSoundInstance();
        roll.rollSound.EndSoundInstance();
        foreach (var b in bounce)
        {
            if (b == null) continue;
            b.bounceSound.EndSoundInstance();
        }

        music.BattleMusic.EndSoundInstance();
        music.StrollMusic.EndSoundInstance();
    }

    private void OnDisable()
    {
        OnDestroy(); // reuse cleanup
    }
}