using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public Renderer mat;

    public Renderer floatMat;
    private float amount            = -1;

    public float desapearSpeed      = 2.0f;
    public float appearSpeed        = 2.5f;
    public float restartTime        = 1.0f;

    private bool disabled           = false;
    public bool ready              = true;
    private float lastTime;

    public ParticleSystem creatingParticle;
    public ParticleSystem destroyingParticles;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(disabled && Time.time > lastTime + restartTime)
        {
            TurnOn();
        }
    }

    public void OnChildTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            PickUpController pickController = other.GetComponentInParent<PickUpController>();
            if (!pickController.currentPickup)
            {
                TurnOff();
                int p = Random.Range(0, 10);
                Debug.Log(p);
                if(p > 5)
                    pickController.currentPickup = pickController.gameObject.AddComponent<Laser>();
                else
                    pickController.currentPickup = pickController.gameObject.AddComponent<TurboBoost>();
            }
        }
    }

    public void TurnOff()
    {
        if(ready)
            StartCoroutine(DisableAnimation());
    }

    public void TurnOn()
    {
        if(ready)
            StartCoroutine(EnableAnimation());
    }

    IEnumerator DisableAnimation()
    {
        ready = false;
        disabled = false;

        if (destroyingParticles)
            if (!destroyingParticles.isPlaying)
                destroyingParticles.Play();

        while (amount < 2.0f)
        {
            amount = Mathf.Min(2, amount + (desapearSpeed * Time.deltaTime));
            mat.material.SetFloat("_SliceAmount", amount);
            floatMat.material.SetColor("_TintColor", new Color(0.07843137f, 0.3754966f,1.0f,Mathf.Max(0, -amount)));
            yield return new WaitForEndOfFrame();
        }
        disabled = true;
        mat.gameObject.SetActive(false);
        lastTime = Time.time;
        ready = true;
    }

    IEnumerator EnableAnimation()
    {
        ready = false;
        if(creatingParticle)
            creatingParticle.Play();
        yield return new WaitForSeconds(0.3f);
        mat.gameObject.SetActive(true);
        while (amount > -2.0f)
        {
            if (disabled) yield return null;

            amount = Mathf.Max(-2, amount - (appearSpeed * Time.deltaTime));
            mat.material.SetFloat("_SliceAmount", amount);
            floatMat.material.SetColor("_TintColor", new Color(0.07843137f, 0.3754966f, 1.0f, Mathf.Min(0.47f, -amount)));
            yield return new WaitForEndOfFrame();
        }
        disabled = false;
        ready = true;
    }
}
