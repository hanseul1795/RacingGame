using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
	public float duration   = 0.5f;
	public float speed      = 1.0f;
	public float magnitude  = 0.1f;

    private bool shaking = false;


	void Start()
	{
    }

    void Update() 
	{

        if(!shaking)
        {
            if ((transform.localPosition - Vector3.zero).sqrMagnitude > 0.05f)
            {
                //Debug.Log("not in place: " + (transform.localPosition - Vector3.zero).sqrMagnitude);
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime);
            }
        }
	}
	
	//This function is used outside (or inside) the script
	public void PlayShake(float p_duration, float p_speed, float p_magnitude)
    {
        //StopAllCoroutines();
        //StopCoroutine("Shake");
        if(!shaking)
            shaking = true;
        StartCoroutine(Shake(p_duration, p_speed, p_magnitude));
	}
	
	private IEnumerator Shake(float p_duration, float p_speed, float p_magnitude)
    {
		float elapsed = 0.0f;
        //Vector3 originalCamPos = transform.localPosition;
        Vector3 originalCamPos = Vector3.zero;
        float randomStart = Random.Range(-1000.0f, 1000.0f);
		
		while (shaking)
        {
            shaking = true;

            elapsed += Time.deltaTime;			
			
			float percentComplete = elapsed / p_duration;			
			
			float damper    = 1.0f - Mathf.Clamp(1.5f * percentComplete - 1.0f, 0.0f, 1.0f);
			float alpha     = randomStart + p_speed * percentComplete;
			
			float x     = Mathf.PerlinNoise(alpha, 0.0f) * 2.0f - 1.0f;
			float y     = Mathf.PerlinNoise(0.0f, alpha) * 2.0f - 1.0f;
			
			x *= p_magnitude * damper;
			y *= p_magnitude * damper;
			
			transform.localPosition = new Vector3(x + originalCamPos.x, y + originalCamPos.y, transform.localPosition.z );

            if(percentComplete > 1.0f)
            {
                shaking = false;
               // StopCoroutine("Shake");
            }

			yield return new WaitForEndOfFrame();
		}
	}
}