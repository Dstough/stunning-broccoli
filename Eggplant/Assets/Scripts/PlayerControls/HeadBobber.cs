using UnityEngine;

public class HeadBobber : MonoBehaviour
{
    private Vector3 restPosition;
    private float timer = Mathf.PI / 2;

    public float transitionSpeed = 20f;
    public float bobSpeed = 7f;
    public float bobAmount = 0.1f;

    void Start()
    {
        restPosition = transform.localPosition;
    }

    void Update()
    {
        if (transform.parent.GetComponent<PlayerMovement>().grounded && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        {
            timer += bobSpeed * Time.deltaTime;
            transform.localPosition = new Vector3()
            {
                x = Mathf.Cos(timer) * bobAmount,
                y = restPosition.y + Mathf.Abs(Mathf.Sin(timer * 2) * bobAmount),
                z = restPosition.z
            };
        }
        else
        {
            timer = Mathf.PI / 2;
            transform.localPosition = new Vector3()
            {
                x = Mathf.Lerp(transform.localPosition.x, restPosition.x, transitionSpeed * Time.deltaTime),
                y = Mathf.Lerp(transform.localPosition.y, restPosition.y, transitionSpeed * Time.deltaTime),
                z = Mathf.Lerp(transform.localPosition.z, restPosition.z, transitionSpeed * Time.deltaTime)
            };
        }

        if (timer > Mathf.PI * 2)
            timer = 0;
    }
}