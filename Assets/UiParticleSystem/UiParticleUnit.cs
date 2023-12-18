using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiParticleUnit : MonoBehaviour
{
    [Header("ParticleProperties")]
    public float lifetime;
    public float startSpeed;
    public Vector3 startSize;
    public Vector3 startAngularSpeed;
    public Vector2 moveDirection;
    public float gravityIntensity;
    public Gradient colorOverLifetimeGradient;
    public AnimationCurve acTransparency;
    public AnimationCurve acSizeOverLifetime;
    public UiParticleSystem.SimulationSpaceEnum simulationSpace;
    Vector3 startLocalPos;
    Quaternion startLocalRot;
    public float noiseInputPixelIndex;
    public float noiseScale = 1;
    public float noiseIntensity = 1;
    public Vector2 noiseRandomDirectionMultiplier;
    public GameObject convergeTarget;

    [Header("OtherAssignments")]
    
    float lifetimeTimer;
    Vector3 velocityAdditionDueToGravity;
    UiParticleSystemPool uiParticleSystemPool;
    UiParticleSystem uiParticleSystem;
    Material myMat;
    float lifetimeProgression;
    Vector3 emitterLocalPosAtSpawn;
    Vector3 posProgressDueTovelocity;
    [SerializeField]
    float noiseProgressionAsFrames;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LifetimeRun();
    }

    public void ResetParticleUnit()
    {
        lifetimeTimer = 0;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        velocityAdditionDueToGravity = Vector3.zero;
        posProgressDueTovelocity = Vector3.zero;
        noiseProgressionAsFrames = 0;
        gameObject.SetActive(false);
        
    }
    public void InitializeParticleUnit()
    {
        gameObject.SetActive(true);
    }

    public void LifetimeRun()
    {
        lifetimeTimer += Time.deltaTime;
        
        if (lifetimeTimer >= lifetime)
        {
            
            lifetimeTimer = lifetime;
        }
        lifetimeProgression = lifetimeTimer / lifetime;

        MoveParticle();
        ConvergeParticle();
        RotateParticle();
        ColorMaintain();
        SizeMaintain();

        if (lifetimeTimer == lifetime)
        {
            uiParticleSystemPool.SleepAParticle(this);
            ResetParticleUnit();
        }
    }
    void MoveParticle()
    {
        Vector3 moveDirection3D = moveDirection;
        Vector3 velocity = moveDirection3D.normalized * startSpeed;

        //velocityAdditionDueToGravity +=  ((Vector3.down) * gravityIntensity);
        velocityAdditionDueToGravity += uiParticleSystem.transform.InverseTransformDirection((Vector3.down)) * gravityIntensity;
        velocity += (velocityAdditionDueToGravity);
        
        Vector3 localPosOffset = Vector3.zero;
        switch (simulationSpace)
        {
            case UiParticleSystem.SimulationSpaceEnum.Local:
                {
                    break;
                }
            case UiParticleSystem.SimulationSpaceEnum.Global:
                {
                    localPosOffset = -(uiParticleSystem.GetEmitterCurrentLocalPos() - emitterLocalPosAtSpawn);
                    break;
                }
        }
        
        posProgressDueTovelocity += ((velocity * Time.deltaTime * 100f));

        float noiseX = noiseProgressionAsFrames * noiseScale / 100f;
        float noiseY = noiseInputPixelIndex * noiseScale / 100f;
        Vector3 noiseDistortionOffset = (new Vector3(Mathf.PerlinNoise(noiseX, noiseY ) * noiseRandomDirectionMultiplier.x,
            Mathf.PerlinNoise(noiseX, noiseY + 0.5f)*noiseRandomDirectionMultiplier.y,0)- new Vector3(0.0f, 0.0f, 0)) * 200f *noiseIntensity*(lifetimeProgression);
        //Debug.Log(noiseX+"f"+noiseDistortionOffset);
       noiseProgressionAsFrames += 1f;



        transform.localPosition =startLocalPos + posProgressDueTovelocity + localPosOffset + noiseDistortionOffset;

    }
    void ConvergeParticle()
    {
        if(convergeTarget != null)
        {
            transform.position = Vector3.Lerp(transform.position, convergeTarget.transform.position, lifetimeProgression);
        }
    }
    void RotateParticle()
    {
        transform.Rotate(startAngularSpeed*Time.deltaTime*100f);
    }
    void ColorMaintain()
    {
        
        Color c = colorOverLifetimeGradient.Evaluate(lifetimeProgression);
        c.a = acTransparency.Evaluate(lifetimeProgression);
        if (GetComponent<Image>() != null)
        {
            GetComponent<Image>().color = c;
        }
    }
    void SizeMaintain()
    {
        transform.localScale = startSize* acSizeOverLifetime.Evaluate(lifetimeProgression);
    }



    public void SetUiParticleSystemPool(UiParticleSystemPool uiP)
    {
        uiParticleSystemPool = uiP;
        uiParticleSystem = uiP.GetComponent<UiParticleSystem>();
    }
    public void SetMyMat(Material mat)
    {
        myMat = mat;
        if(GetComponent<Image>() != null)
        {
            GetComponent<Image>().material = mat;
        }
    }
    public void SetStartLocalPosition(Vector3 localPosition)
    {
        startLocalPos = localPosition;
        
    }
    public void SetEmitterLocalPositionAtSpawn(Vector3 localPosition)
    {
        emitterLocalPosAtSpawn = localPosition;
    }
    public void SetStartLocalRot(Quaternion r)
    {
        startLocalRot = r;
        transform.localRotation = startLocalRot;
    }
}
