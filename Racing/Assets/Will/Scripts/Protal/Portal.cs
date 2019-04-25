using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class Portal : MonoBehaviour {


    public Transform      pointB;
	public Camera         scoutCamera;
	public Vector3        faceNormal = Vector3.forward;

	public int            portalTextureSize = 256;
	public float          clipPlaneOffset = 0.07f;

	private RenderTexture _portalTexture = null;
	private int           _oldPortalTextureSize = 0;

	private static bool   _isInsideRendering = false;

	// Use this for initialization
	void Start () {
	
	}

	public void OnWillRenderObject()
	{

		if(!enabled || !scoutCamera || !pointB)
			return;

		Camera cam = Camera.current /*Camera.main*/;
		if( !cam )
			return;

		var rend = GetComponent<Renderer>();
		if (!rend || !rend.sharedMaterial || !rend.enabled)
			return;

		CreateNeededObjects();

		if(!_portalTexture.IsCreated())
			return;
    
		if( _isInsideRendering)
			return;
		_isInsideRendering = true;



		Matrix4x4 self_toWorld = transform.localToWorldMatrix;
		Matrix4x4 self_toLocal = transform.worldToLocalMatrix;
		Matrix4x4 pointB_toWorld = pointB.localToWorldMatrix;


           Vector3 pos = -self_toLocal.MultiplyPoint(cam.transform.position);
           pos.y = -pos.y;

           scoutCamera.transform.position = pointB_toWorld.MultiplyPoint(pos);

           Vector3 rot = -self_toLocal.MultiplyVector(cam.transform.forward);  

           rot.y = -rot.y;

           Vector3 pitch = self_toLocal.MultiplyVector(cam.transform.up);

           //pitch.x = -pitch.x;

           scoutCamera.transform.rotation = Quaternion.LookRotation(
			pointB_toWorld.MultiplyVector(rot),
			pointB_toWorld.MultiplyVector(pitch));

		Vector4   clipPlane = CameraSpacePlane( cam, transform.position, self_toWorld.MultiplyVector(faceNormal), -1.0f );
		Matrix4x4 projection = cam.CalculateObliqueMatrix(clipPlane);
		scoutCamera.projectionMatrix = projection;

		if(scoutCamera.enabled)
			scoutCamera.enabled = false;

		scoutCamera.Render();


		_isInsideRendering = false;

	}
		
	void OnDisable()
	{
		if( _portalTexture ) {
			_portalTexture.Release();
			CustomDestroy(_portalTexture);
		}
	}


	private void CreateNeededObjects()
	{
		if(portalTextureSize > 1)
		if( !_portalTexture || _oldPortalTextureSize != portalTextureSize)
		{

			if( _portalTexture ) {
				_portalTexture.Release();
				CustomDestroy(_portalTexture);
			}

			_portalTexture = new RenderTexture( portalTextureSize, portalTextureSize, 16, RenderTextureFormat.ARGB32);
			_portalTexture.name = "__PortalRenderTexture" + GetInstanceID();
			_portalTexture.hideFlags = HideFlags.DontSave;
			_portalTexture.Create();

			scoutCamera.targetTexture = _portalTexture;


			_oldPortalTextureSize = portalTextureSize;

		}

		Material[] materials = GetComponent<Renderer>().sharedMaterials;
		foreach( Material mat in materials ) {
			if( mat.HasProperty("_PortalTex") )
				mat.SetTexture( "_PortalTex", _portalTexture);
		}
	      
	}

	private Vector4 CameraSpacePlane (Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 offsetPos = pos + normal * -clipPlaneOffset;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint( offsetPos );
		Vector3 cnormal = m.MultiplyVector( normal ).normalized * sideSign;
		return new Vector4( cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos,cnormal) );
	}
		
	private void CustomDestroy(Object target)
    {

		#if UNITY_EDITOR
		if(!Application.isPlaying)
			DestroyImmediate( target );
		else
			Destroy( target );
		#else
		Destroy( target );
		#endif

	}
}
