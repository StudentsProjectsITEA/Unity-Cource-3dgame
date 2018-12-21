using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeThrow : MonoBehaviour {

    public GameObject nadePrefab;
    public Transform initialPosition;
    //public Text PowerText;
    public Image powerImage;
    public float MaxPower;
    public float ChargingSpeed;

    private float Power;
    private float timer;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        
		if (Input.GetKey(KeyCode.Q) || Input.GetMouseButton(3))
        {
            Power = Mathf.PingPong(timer * ChargingSpeed, MaxPower);
            timer += Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.Q) || Input.GetMouseButtonUp(3))
        {
            Throw(Power);
            Power = 0f;
            timer = 0f;
        }
        powerImage.fillAmount = Power / MaxPower;
        //PowerText.text = Power.ToString();
    }

    void Throw(float power)
    {
        GameObject nade = Instantiate(nadePrefab, initialPosition.position, Camera.main.transform.rotation);
        Rigidbody nadeRigidBody = nade.GetComponent<Rigidbody>();
        nadeRigidBody.AddRelativeForce(Vector3.forward * power);
        //Quaternion.Euler(initialPosition.rotation.eulerAngles + Vector3.left * 25f);
    }
}
