using UnityEngine;
public class SubmarineManager : MonoBehaviour
{
    [SerializeField] float fuel = 100f;
    [SerializeField] float maxFuel = 100f;
    [SerializeField] float fuelUsageSpeed = 1f;
    [SerializeField] float mineFuelReduction = 5f;

    [SerializeField] Vector3 impulseForce = Vector3.up * 10;
    [SerializeField] Vector3 constantForce = Vector3.up * 20;
    [SerializeField] Vector3 forwardForce = Vector3.right * 20;

    [SerializeField] ForceMode forceMode = ForceMode.Force;

    bool _thrust_right;
    bool _thrust_left;
    Rigidbody rb;

    [SerializeField]
    float minRotation = 35;

    [SerializeField]
    float maxRotaion = -35;

    [SerializeField]
    float pitchSpeed = 1;

    [SerializeField]
    float speed = 1;

    [SerializeField]
    Transform ship;

    private bool resetted = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        fuel -= Time.deltaTime * fuelUsageSpeed;

        if (fuel <= 0)
        {
            enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            _thrust_right = true;
            resetted = false;
        }

        else if (Input.GetKey(KeyCode.D))
        {
            Vector3 dest = new Vector3(0, ship.transform.localRotation.eulerAngles.y,
            maxRotaion);
            ship.transform.localRotation = Quaternion.Lerp(ship.transform.localRotation,
            Quaternion.Euler(dest), Time.deltaTime * pitchSpeed);
        }

        else if (Input.GetKeyUp(KeyCode.D))
        {
            _thrust_right = false;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            _thrust_left = true;
            resetted = false;
        }

        else if (Input.GetKey(KeyCode.A))
        {
            Vector3 dest = new Vector3(0, ship.transform.localRotation.eulerAngles.y,
            minRotation);
            ship.transform.localRotation = Quaternion.Lerp(ship.transform.localRotation,
            Quaternion.Euler(dest), Time.deltaTime * pitchSpeed);
        }

        else if (Input.GetKeyUp(KeyCode.A))
        {
            _thrust_left = false;
        }

        else
        {
            Vector3 dest = new Vector3(0, ship.transform.localRotation.eulerAngles.y, 0);

            ship.transform.localRotation = Quaternion.Lerp(ship.transform.localRotation, Quaternion.Euler(dest), Time.deltaTime * pitchSpeed);
        }
    }
    private void FixedUpdate()
    {
        if (_thrust_right)
        {
            if (forceMode == ForceMode.Impulse)
            {
                _thrust_right = false;
                resetted = true;
                rb.AddForce(impulseForce, forceMode);
                ship.transform.localEulerAngles = new Vector3(0,
                ship.transform.localRotation.eulerAngles.y, maxRotaion);
            }

            else
            {
                rb.AddForce(constantForce, forceMode);
            }

            if (ship.position.z >= 36)
            {
                rb.AddForce(-constantForce, forceMode);
            }

            //if (ship.position.z <= 27)
            //{
            //    rb.AddForce((-forwardForce * 2), 0);
            //}
        }

        if (_thrust_left)
        {
            if (forceMode == ForceMode.Impulse)
            {
                _thrust_left = false;
                resetted = true;
                rb.AddForce(impulseForce, forceMode);
                ship.transform.localEulerAngles = new Vector3(0,
                ship.transform.localRotation.eulerAngles.y, minRotation);
            }

            else
            {
                rb.AddForce(-constantForce, forceMode);
            }

            if (ship.position.z <= 26)
            {
                rb.AddForce(constantForce, forceMode);
            }

            //if (ship.position.z <= 27)
            //{
            //    rb.AddForce((-forwardForce * 2), 0);
            //}
        }

        rb.AddForce(forwardForce, forceMode);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger by:{other.gameObject}", other.gameObject);
        if (other.gameObject.CompareTag("Box"))
        {
            Destroy(other.gameObject);
            fuel = Mathf.Clamp(fuel + fuelUsageSpeed, 0, maxFuel);
            Debug.Log($"Fuel gained: {fuel}");
        }
        else if (other.gameObject.CompareTag("Mine"))
        {
            Destroy(other.gameObject);
            fuel = Mathf.Clamp(fuel - mineFuelReduction, 0, maxFuel);
            Debug.Log($"Fuel lost: {fuel}");
            if (fuel <= 0)
            {
                enabled = false;
            }
        }

    }
    private void OnCollisionEnter(Collision other)
    {
        rb.isKinematic = true;
        enabled = false;
    }
}