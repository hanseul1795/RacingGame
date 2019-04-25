using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMove : MonoBehaviour
{
    public float speed      = 1.0f;
    public int maxBounces   = 3;
    public float maxTime    = 10.0f;

    public float groundCheckDistance = 2.5f;

    public LayerMask groundCheckMask;

    public float hitRadius;
    public LayerMask hitMask;

    private Rigidbody rb;

    private float   timer;
    private int     bounce = 0;

    private GameObject owner;

    public void SetOwner(GameObject p_owner)
    {
        if (p_owner) owner = p_owner;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {

        //StartCoroutine(CheckPosition());

        timer += Time.deltaTime;
        if (bounce >= maxBounces || timer >= maxTime)
        {
            //Debug.Log("Destroying Laser");
            Destroy(gameObject);
        }

        Collider[] col = Physics.OverlapSphere(transform.position, hitRadius, hitMask);

        if (col.Length > 0 && owner)
        {
            foreach(Collider c in col)
            {
                if(c.transform.root.gameObject != owner)
                {
                    Debug.Log(c.transform.root.name + " was hit");
                }
                else Debug.Log("Don't worry dad you're safe");
            }
        }
    }

    IEnumerator CheckPosition()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.DrawLine(transform.position, transform.position + rb.velocity, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, groundCheckDistance, groundCheckMask))
        {
            transform.position = hit.point + hit.normal * 2.0f;
            Debug.DrawLine(transform.position, hit.point);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        bounce++;
        bounce = Mathf.Min(bounce, maxBounces);
        //transform.rotation  = Quaternion.FromToRotation(transform.forward, transform.forward + collision.contacts[0].normal);
        rb.velocity = (transform.forward + collision.contacts[0].normal) * speed;
        transform.LookAt(transform.position + rb.velocity);
    }

    private void OnEnable()
    {
        timer = 0;
        bounce = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}
