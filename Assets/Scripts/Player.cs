using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class Player : MonoBehaviour {

    public float Speed;
    public float Gravity;
    public float JumpForce;
    public float CameraSensitivity;
    public float ZoomSpeed;
    public GameObject particlePrefab;
    public Text HealthText, AmmoText;
    public GameObject GameOverWindow;
    public EnemySpawn Spawner;
    public Transform WeaponPosition;
    public GameObject[] Weapon;
    public SoundManager soundManager;

    public PostProcessingProfile fxProfile;

    private Vector3 WeaponAngle;
    private int CurrentWeapon;
    private int PistolAmmo;
    private int PistolMag;
    private int MachinegunAmmo;
    private int MachinegunMag;
    private int CurrentAmmo;
    private int CurrentMag;

    private Vector3 MoveDirection;
    private CharacterController Controller;
    private Transform cameraTransform;
    private Vector3 cameraRotation;
    private int Health = 100;
    VignetteModel.Settings VignetteSettings;
    BloomModel.Settings BloomSettings;
    ColorGradingModel.Settings ColorGradingSettings;
    private bool isShaking;
    private float ShootCooldown;
    private float ZoomedSens;
    private bool Reloading;
    
    void Start () {
        VignetteSettings = fxProfile.vignette.settings;
        BloomSettings = fxProfile.bloom.settings;
        ColorGradingSettings = fxProfile.colorGrading.settings;
        VignetteSettings.intensity = 0;
        BloomSettings.bloom.intensity = 0;
        ColorGradingSettings.channelMixer.red.Set(1f, 0f, 0f);
        fxProfile.vignette.settings = VignetteSettings;
        fxProfile.bloom.settings = BloomSettings;
        fxProfile.colorGrading.settings = ColorGradingSettings;
        cameraTransform = Camera.main.transform;
        Controller = this.GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        HealthText.text = "+" + Health;
        GameOverWindow.SetActive(false);
        ZoomedSens = 1;
        MachinegunAmmo = 120;
        MachinegunMag = 30;
        PistolAmmo = 60;
        PistolMag = 12;
        SetCurrentWeapon(3);
    }

    void Update () {
        if (CurrentWeapon == 2)
        {
            CurrentAmmo = PistolAmmo;
            CurrentMag = PistolMag;
            AmmoText.text = string.Format("{0}/{1}", CurrentMag, CurrentAmmo);
        } else if (CurrentWeapon == 3)
        {
            CurrentAmmo = MachinegunAmmo;
            CurrentMag = MachinegunMag;
            AmmoText.text = string.Format("{0}/{1}", CurrentMag, CurrentAmmo);
        } else
        {
            CurrentAmmo = 1;
            CurrentMag = 1;
            AmmoText.text = "";
        }
        
        if (Input.GetMouseButton(1))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 20, Time.deltaTime * ZoomSpeed);
            VignetteSettings.intensity = Mathf.Lerp(VignetteSettings.intensity, 1, Time.deltaTime * ZoomSpeed * 2f);
            VignetteSettings.rounded = true;
            VignetteSettings.smoothness = 0;
            fxProfile.vignette.settings = VignetteSettings;
            ZoomedSens = 0.5f;
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, Time.deltaTime * ZoomSpeed * 2);
            VignetteSettings.intensity = 0;
            VignetteSettings.rounded = false;
            VignetteSettings.smoothness = 0.2f;
            fxProfile.vignette.settings = VignetteSettings;
            ZoomedSens = 1;
        }
        Move();
        Rotate(false, 0, 0, 5);
        if (Input.GetMouseButtonDown(0) && !Reloading && !isShaking)
        {
            if (CurrentWeapon == 1)
            {
                Shoot(1.5f);
                Rotate(true, 0.2f, 14f, 4);
                AmmoText.text = "";
            }
            if (CurrentWeapon == 2)
            {
                if (PistolMag > 0)
                {
                    Shoot(1000);
                    Rotate(true, 0.06f, 1f, 1);
                    PistolMag--;
                    CurrentMag = PistolMag;
                    AmmoText.text = string.Format("{0}/{1}", CurrentMag, CurrentAmmo);
                }
                else
                {
                    soundManager.PlayClickSound();
                }
            }
            if (CurrentWeapon == 3)
            {
                if (MachinegunMag == 0)
                {
                    soundManager.PlayClickSound();
                    AmmoText.text = string.Format("{0}/{1}", CurrentMag, CurrentAmmo);
                }
            }
            ShootCooldown = 0;
        }

        if (Input.GetMouseButton(0) && !Reloading && !isShaking)
        {
            if (CurrentWeapon == 3)
            {
                if (ShootCooldown <= 0)
                {
                    if (MachinegunMag > 0)
                    {
                        Shoot(1000);
                        //StartCoroutine(WeaponAnim(0.08f, 1f));
                        Rotate(true, 0.04f, 1f, 1);
                        ShootCooldown = 0.2f;
                        MachinegunMag--;
                        CurrentMag = MachinegunMag;
                        AmmoText.text = string.Format("{0}/{1}", CurrentMag, CurrentAmmo);
                    }
                }

            }
            ShootCooldown -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetCurrentWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetCurrentWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetCurrentWeapon(3);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

    private void Move()
    {
        if (Controller.isGrounded)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            MoveDirection = new Vector3(h, 0, v);
            MoveDirection = transform.TransformDirection(MoveDirection); 
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MoveDirection.y = JumpForce;
        }
        MoveDirection.y -= Gravity * Time.deltaTime;
        Controller.Move(MoveDirection * Speed * Time.deltaTime);
    }

    private void Rotate(bool isShoot, float duration, float angle, int sfx)
    {
        float sens = CameraSensitivity * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * sens;
        transform.Rotate(Vector3.up * mouseX);
        float mouseY = Input.GetAxis("Mouse Y") * sens;

        if (!isShoot && isShaking)
        {
            cameraRotation.x = Mathf.Clamp(cameraRotation.x -= mouseY, -90f, 90f);
        }
        else
        {
            StartCoroutine(WeaponAnim(duration, angle, sfx));
            StartCoroutine(Shake(0.08f));
        }
        cameraTransform.localEulerAngles = cameraRotation;
        WeaponPosition.localEulerAngles = WeaponAngle;
    }

    private void Shoot(float distance)
    {
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 100, Color.red);

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance))
        {
            if (hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<Animator>().SetBool("isDead", true);
                Instantiate(particlePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                hit.collider.GetComponent<NavMeshAgent>().speed = 0;
                hit.collider.gameObject.GetComponent<Collider>().enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Enemy"))
        {
            Debug.Log("123");
            Health -= 10;
            HealthText.text = "+" + Health;
            BloomSettings.bloom.intensity = 1f - ((float)Health / 100);
            fxProfile.bloom.settings = BloomSettings;
            ColorGradingSettings.channelMixer.red.Set(2f - ((float)Health / 100), 2f - ((float)Health / 50), 2f - ((float)Health / 50));
            fxProfile.colorGrading.settings = ColorGradingSettings;
        }
        if (Health <= 0)
        {
            GameOver();
        }
    }
    

    void GameOver()
    {
        GameOverWindow.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        StopAllCoroutines();
    }

    public void StartGame()
    {
        Spawner.Clear();
        GameOverWindow.SetActive(false);
        Spawner.Spawn();
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        Health = 100;
        HealthText.text = "+" + Health;
        VignetteSettings.intensity = 0;
        fxProfile.vignette.settings = VignetteSettings;
        BloomSettings.bloom.intensity = 0;
        fxProfile.bloom.settings = BloomSettings;
        ColorGradingSettings.channelMixer.red.Set(1f, 0f, 0f);
        fxProfile.colorGrading.settings = ColorGradingSettings;

    }

    IEnumerator Shake(float duration)
    {
        float time = 0.0f;
        while (time < duration / 2)
        {
            cameraRotation.x -= 1f;
            time += Time.deltaTime;
            yield return null;
        }
        time = 0.0f;
        while (time < duration / 2)
        {
            cameraRotation.x += 1f;
            time += Time.deltaTime;
            yield return null;
        }
    }

    private void SetCurrentWeapon(int weapon)
    {
        switch (weapon)
        {
            case 1:
                CurrentWeapon = 1;
                CurrentMag = 1;
                CurrentAmmo = 1;
                Weapon[0].gameObject.SetActive(true);
                Weapon[1].gameObject.SetActive(false);
                Weapon[2].gameObject.SetActive(false);
                AmmoText.text = "";
                break;
            case 2:
                CurrentWeapon = 2;
                CurrentMag = PistolMag;
                CurrentAmmo = PistolAmmo;
                Weapon[0].gameObject.SetActive(false);
                Weapon[1].gameObject.SetActive(true);
                Weapon[2].gameObject.SetActive(false);
                AmmoText.text = string.Format("{0}/{1}", CurrentMag, CurrentAmmo);
                break;
            case 3:
                CurrentWeapon = 3;
                CurrentMag = MachinegunMag;
                CurrentAmmo = MachinegunAmmo;
                Weapon[0].gameObject.SetActive(false);
                Weapon[1].gameObject.SetActive(false);
                Weapon[2].gameObject.SetActive(true);
                AmmoText.text = string.Format("{0}/{1}", CurrentMag, CurrentAmmo);
                break;
        }
    }
    private IEnumerator Reload()
    {
        Reloading = true;
        yield return new WaitForSeconds(2f);
        switch (CurrentWeapon)
        {
            case 1:
                break;
            case 2:
                if (CurrentAmmo > 0)
                {
                    soundManager.PlayReloadSound();
                    if (CurrentAmmo < 12)
                    {
                        CurrentMag = CurrentAmmo;
                        CurrentAmmo = 0;
                    }
                    else
                    {
                        CurrentAmmo -= 12 - CurrentMag;
                        CurrentMag = 12;
                    }
                    PistolAmmo = CurrentAmmo;
                    PistolMag = CurrentMag;
                }
                break;
            case 3:
                if (CurrentAmmo > 0)
                {
                    soundManager.PlayReloadSound();
                    if (CurrentAmmo < 30)
                    {
                        CurrentMag = CurrentAmmo;
                        CurrentAmmo = 0;
                    }
                    else
                    {
                        CurrentAmmo -= 30 - CurrentMag;
                        CurrentMag = 30;
                    }
                    MachinegunAmmo = CurrentAmmo;
                    MachinegunMag = CurrentMag;
                }
                break;
        }
        Reloading = false;
        AmmoText.text = string.Format("{0}/{1}", CurrentMag, CurrentAmmo);
    }

    IEnumerator WeaponAnim(float duration, float Angle, int sfx)
    {
        if (CurrentWeapon > 1)
            soundManager.PLaySoundEffect(sfx);
        isShaking = true;
        float time = 0.0f;
        while (time < duration / 2)
        {
            WeaponAngle.x -= Angle;
            time += Time.deltaTime;
            yield return null;
        }
        time = 0.0f;
        while (time < duration / 2)
        {
            WeaponAngle.x += Angle;
            time += Time.deltaTime;
            yield return null;
        }
        isShaking = false;
        if (CurrentWeapon == 1)
        {
            soundManager.PLaySoundEffect(sfx);
        }
    }
}
