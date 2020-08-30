using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshRenderer))]
public class ForceField : MonoBehaviour
{
    
    //Public Script Parameters
    public Shader forceFieldShader;
    public AudioClip impactSound;
    public bool ImpactOnCollision;

    //Public shader parameters
    public Vector3 _Scale = Vector3.zero;

    public Color _FieldColor =new Color(0,0.5f,1,1);

    public float _FieldSplashLifetime = 2f;

    public float _FieldSplashRadius = 1;

    public float _FieldSplashGrowthMultiplier = 4;

    public float _FieldSplashPow = 3;

    public float _FieldSplashDecayStartAfterLifeTimePercent = 20;

    public float _ImpactSplashLifetime = 1.25f;

    public float _ImpactSplashRadius = 0.5f;

    [Range(0,1.5f)]
    public float _ImpactSplashVisibility = 1;

    public float _ImpactSplashPow = 2.6f;

    public float _ImpactSplashGrowthMultiplier = 3;

    [Range(0,0.5f)]
    public float _PassiveFieldVisibility = 0.05f;

    [Range(0,1f)]
    public float _FieldRimVisibility = 0.15f;

    [Range(0,1f)]
    public float _FieldRimColorVisibility = 1;

    [Range(0,1f)]
    public float _FieldDischargeVisibility = 0.5f;

    [Range(0,1f)]
	public float _FieldHighlightVisibility=0.6f;

	public float _FieldHighlightPow=1.8f;

	public float _FieldOutlineThickness=0.0f;

	public float _Octaves = 3.0f;

	public float _Frequency = 2;

	public float _Amplitude = 0.9f;

	public float _Lacunarity = 2.5f;

	public float _Persistence = 0.5f;

	public Vector4 _Offset = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);

	public float _RidgeOffset= 0.15f;

	public float _AnimSpeed=10;

	public float _PowInner=1.5f;

	public float _PowOuter=5;

	public float _PowInner2=2;

	public float _PowOuter2=5;



	AudioSource sound;
    Material mat;
    public Vector3[] times = new Vector3[10];

    public Vector3[] Times
    {
        get
        {
            return times;
        }
    }

	void Start () {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        List<Material> mats = new List<Material>();
        mats.AddRange(renderer.materials);
        mats.Add( new Material(forceFieldShader));

        renderer.materials = mats.ToArray();
        mat = renderer.materials[0];


		sound = GetComponent<AudioSource>();

        if (sound != null)
        {
            sound.clip = impactSound;
        }

		for(int i=0;i<10;i++){
			times[i]=Vector3.zero;
            mat.SetVector("coords" + i.ToString(), Vector4.zero);
            mat.SetVector("times" + i.ToString(), new Vector4(0,0,0,0));
		}

        SetShaderParameters();
	}
	

	void Update () {
		for(int i=0;i<10;i++){
			if(times[i].y>0){

                if (times[i].x > 0)
                    times[i].x -= Time.deltaTime / _ImpactSplashLifetime;
				
				if (times[i].x < 0)
					times[i].x=0;

                if (times[i].y > 0)
                    times[i].y -= Time.deltaTime / _FieldSplashLifetime;
				
				if (times[i].y < 0)
                    times[i].y = 0;

				mat.SetVector("times"+i.ToString(),times[i]);
			}
		}
	}

    void OnCollisionEnter(Collision col)
    {
        if (ImpactOnCollision)
        {
            AddImpact(col.contacts[0].point, col.contacts[0].normal);
        }
    }

	int GetFirstFreeImpactPoint(){
		for(int i=0;i<times.Length;i++){
			if(times[i].y<=0)
				return i;
		}
		return -1;
	}

    public void AddImpact(Vector3 point, Vector3 normal)
    {
       
        int freeIndex = GetFirstFreeImpactPoint();
        if (freeIndex != -1)
        {
            times[freeIndex] = Vector3.one;

            mat.SetVector("coords" + freeIndex.ToString(), transform.InverseTransformPoint(point));
            mat.SetVector("times" + freeIndex.ToString(), times[freeIndex]);
            mat.SetVector("normals" + freeIndex.ToString(), transform.InverseTransformDirection(normal));
            
            if (sound != null)
            {
                sound.PlayOneShot(sound.clip);
            }
        }
    }

    void SetShaderParameters()
    {


        
        
        mat.SetColor("_FieldColor",_FieldColor);

        mat.SetFloat("_FieldSplashRadius", _FieldSplashRadius);
        mat.SetFloat("_FieldSplashGrowthMultiplier", _FieldSplashGrowthMultiplier);
        mat.SetFloat("_FieldSplashPow", _FieldSplashPow);
        mat.SetFloat("_FieldSplashDecayStartAfterLifeTimePercent", _FieldSplashDecayStartAfterLifeTimePercent);
        mat.SetFloat("_ImpactSplashRadius", _ImpactSplashRadius);
        mat.SetFloat("_ImpactSplashVisibility", _ImpactSplashVisibility);
        mat.SetFloat("_ImpactSplashPow", _ImpactSplashPow);
        mat.SetFloat("_ImpactSplashGrowthMultiplier", _ImpactSplashGrowthMultiplier);
        mat.SetFloat("_PassiveFieldVisibility", _PassiveFieldVisibility);
        mat.SetFloat("_FieldRimVisibility", _FieldRimVisibility);
        mat.SetFloat("_FieldRimColorVisibility", _FieldRimColorVisibility);
        mat.SetFloat("_FieldDischargeVisibility", _FieldDischargeVisibility);
        mat.SetFloat("_FieldHighlightVisibility", _FieldHighlightVisibility);
        mat.SetFloat("_FieldHighlightPow", _FieldHighlightPow);
        mat.SetFloat("_FieldOutlineThickness", _FieldOutlineThickness);
        mat.SetFloat("_Octaves", _Octaves);
        mat.SetFloat("_Frequency", _Frequency);
        mat.SetFloat("_Amplitude", _Amplitude);
        mat.SetFloat("_Lacunarity", _Lacunarity);
        mat.SetFloat("_Persistence", _Persistence);
        mat.SetVector("_Offset", _Offset);
        mat.SetFloat("_RidgeOffset", _RidgeOffset);
        mat.SetFloat("_AnimSpeed", _AnimSpeed);
        mat.SetFloat("_PowInner", _PowInner);
        mat.SetFloat("_PowOuter", _PowOuter);
        mat.SetFloat("_PowInner2", _PowInner2);
        mat.SetFloat("_PowOuter2", _PowOuter2);

        mat.SetFloat("_ScaleX", _Scale.x);
        mat.SetFloat("_ScaleY", _Scale.y);
        mat.SetFloat("_ScaleZ", _Scale.z);
    }

}
