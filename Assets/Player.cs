using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float playerSpeed = 7f;
    [SerializeField] float smoothMoveTime = 0.1f;
    [SerializeField] float turnSpeed = 8;

    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;

    new Rigidbody rigidbody;

    Vector3 velocity;
    bool disabled;

    public event System.Action endOfLevel;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Guard.OnGuardSpottedPlayer += Disable;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 inputDirection = Vector3.zero;

        if (!disabled) inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, turnSpeed * Time.deltaTime * inputMagnitude);
        //transform.eulerAngles = Vector3.up * angle;

        //transform.Translate(transform.forward * playerSpeed * Time.deltaTime * smoothInputMagnitude, Space.World);
        velocity = transform.forward * playerSpeed * smoothInputMagnitude;
    }

    void Disable()
    {
        disabled = true;
    }

    private void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));

        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
    }

    private void OnDestroy()
    {
        Guard.OnGuardSpottedPlayer -= Disable;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Finish")
        {
            Disable();
            if (endOfLevel != null)
            {
                endOfLevel();
            }
        }
    }
}
