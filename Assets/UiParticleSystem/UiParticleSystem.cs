using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiParticleSystem : MonoBehaviour
{
    [Header("ParticleSystemParameters")]
    public float duration = 1;
    float durationTimer;
    bool running;
    public bool loopingOn;
    public bool burst;
    public bool startOnAwake;

    [Header("EmitterParameters")]
    public UIParticleEmitterType emitterType;
    public float width;
    public float height;
    public UIParticleEmitterConeType emitterConeType;
    [Range(0,360)]
    public float emitterConeDirectionAsAngle;
    Vector2 emitterConeDirection;
    [Range(0,360)]
    public float emitterConeAngle;

    public float particleGenerationRate = 5;
    
    [Header("ParticleParameters")]

    public Vector2 lifetimeMinMax;
    float lifetime;
    public Vector2 startSpeedMinMax;
    float startSpeed;
    public StartRotationEnum startRotationMode;
    public Vector2 startSizeMinMax;
    public Vector3 startSizeProportion = Vector3.one;
    Vector3 startSize;
    public StartRotationEnum angularSpeedMode;
    public Vector2 angularSpeedFromTo;
    //public Vector3 angularSpeedFrom;
    //public Vector3 angularSpeedTo;
    Vector3 startAngularSpeed;
    Vector2 moveDirection;

   

    public Gradient colorOverLifetimeGradient;
    public AnimationCurve acTransparency;
    public AnimationCurve acSizeOverLifetime;

    Vector3 startLocalPosition;
    Quaternion startLocalRot;
    [Header("SpaceParameters")]
    public Vector2 gravityIntensityMinMax;
    float gravityIntensity;
    public SimulationSpaceEnum simulationSpace;
    [Header("NoiseRelated")]
    public float noiseScale = 1;
    public float noiseIntensity = 1;
    float noiseInputPixelIndex;
    Vector2 noiseRandomDirectionMultiplier;

    [Header("ConvergeToATarget")]
    public ConvergeToATargetEnum convergeMode;
    public GameObject convergeTargetToAssign;
    GameObject convergeTarget;

    [Header("RendererParameters")]
    public Material matParticleSystem;
    [Header("OtherAssignments")]
    //[SerializeField]
    float generationTimer;
    UiParticleSystemPool uiParticleSystemPool;

    public enum UIParticleEmitterType
    {
        Circle, Rectangle
    }
    public enum UIParticleEmitterConeType
    {
        Omni,Conical, directional,
    }
    public enum SimulationSpaceEnum
    {
        Local, Global
    }
    public enum StartRotationEnum
    {
        Same, FlatRamdomZ, ThreeDimensionalRandom
    }
    public enum ConvergeToATargetEnum
    {
       None, ConvergeToATarget 
    }
    private void Awake()
    {
        if (startOnAwake)
        {
            StartParticleSystem();
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RunParticleSystem();
    }
    public void StartParticleSystem()
    {
        if (uiParticleSystemPool == null)
        {
            uiParticleSystemPool = GetComponent<UiParticleSystemPool>();
        }
        uiParticleSystemPool.CreateUiParticles();
        running = true;
    }
    public void StopParticleSystem()
    {
        running = false;
    }
    void RunParticleSystem()
    {
        if (running)
        {
            durationTimer += Time.deltaTime;
            if(durationTimer > duration)
            {
                durationTimer -= duration;
                if (!loopingOn)
                {
                    durationTimer = 0;
                    running = false;
                }
            }
            RunParticleGeneration();
        }
    }
    public void RunParticleGeneration()
    {
        if(particleGenerationRate <= 0)
        {
            return;
        }
        float interval = 1 / particleGenerationRate;

        generationTimer += Time.deltaTime;
        if(generationTimer >= interval)
        {
            if(uiParticleSystemPool == null)
            {
                uiParticleSystemPool = GetComponent<UiParticleSystemPool>();
            }


            UiParticleUnit uP = uiParticleSystemPool.AliveAParticle();

            if(uP != null)
            {
                CalcuateParametersAccordingToRanges();
                uP.lifetime = lifetime;
                uP.startSpeed = startSpeed;
                uP.startSize = startSize;
                uP.startAngularSpeed = startAngularSpeed;
                uP.moveDirection = moveDirection;
                uP.SetStartLocalPosition(startLocalPosition);
                uP.SetStartLocalRot(startLocalRot);
                uP.gravityIntensity = gravityIntensity;
                uP.colorOverLifetimeGradient = colorOverLifetimeGradient;
                uP.acTransparency =  acTransparency;
                uP.acSizeOverLifetime = acSizeOverLifetime;
                uP.simulationSpace = simulationSpace;

                uP.noiseInputPixelIndex = noiseInputPixelIndex;
                uP.noiseScale = noiseScale;
                uP.noiseIntensity = noiseIntensity;
                uP.noiseRandomDirectionMultiplier = noiseRandomDirectionMultiplier;
                uP.convergeTarget = convergeTarget;
}
            generationTimer = 0;
        }

    }

    void CalcuateParametersAccordingToRanges()
    {
        lifetime = UnityEngine.Random.Range(lifetimeMinMax.x, lifetimeMinMax.y);
        startSpeed = UnityEngine.Random.Range(startSpeedMinMax.x, startSpeedMinMax.y);
        startSize = startSizeProportion * UnityEngine.Random.Range(startSizeMinMax.x, startSizeMinMax.y);

        switch (angularSpeedMode)
        {
            case StartRotationEnum.Same:
                {
                    startAngularSpeed = new Vector3(0,0,0);
                    break;
                }
            case StartRotationEnum.FlatRamdomZ:
                {

                    startAngularSpeed = new Vector3(0,0,
            UnityEngine.Random.Range(angularSpeedFromTo.x, angularSpeedFromTo.y));
                    break;
                }
            case StartRotationEnum.ThreeDimensionalRandom:
                {
                    startAngularSpeed = new Vector3(UnityEngine.Random.Range(angularSpeedFromTo.x, angularSpeedFromTo.y),
            UnityEngine.Random.Range(angularSpeedFromTo.x, angularSpeedFromTo.y),
            UnityEngine.Random.Range(angularSpeedFromTo.x, angularSpeedFromTo.y));
                    break;
                }
        }
        

        float directionAngle = UnityEngine.Random.Range(-1f, 1f) * emitterConeAngle * 0.5f + emitterConeDirectionAsAngle;
        emitterConeDirection = new Vector2(Mathf.Sin(directionAngle * Mathf.Deg2Rad), Mathf.Cos(directionAngle * Mathf.Deg2Rad));
        moveDirection = emitterConeDirection;

        startLocalPosition = new Vector3(UnityEngine.Random.Range(-1f, 1f)*0.5f*width, UnityEngine.Random.Range(-1f, 1f)*0.5f * height,0);
        gravityIntensity = UnityEngine.Random.Range(gravityIntensityMinMax.x, gravityIntensityMinMax.y);

        Vector3 localRotToAssign = Vector3.zero;
        switch (startRotationMode)
        {
            case StartRotationEnum.Same:
                {
                    break;
                }
            case StartRotationEnum.FlatRamdomZ:
                {
                    localRotToAssign = new Vector3(0, 0, UnityEngine.Random.Range(-1f, 1f) * 180f);
                    break;
                }
            case StartRotationEnum.ThreeDimensionalRandom:
                {
                    localRotToAssign = new Vector3(UnityEngine.Random.Range(-1f, 1f) * 180f, UnityEngine.Random.Range(-1f, 1f) * 180f, UnityEngine.Random.Range(-1f, 1f) * 180f);
                    break;
                }
        }
        startLocalRot = Quaternion.Euler(localRotToAssign);

        noiseInputPixelIndex = UnityEngine.Random.Range(-100f, 100f);
        noiseRandomDirectionMultiplier = new Vector2(UnityEngine.Random.Range(-1f, 1), UnityEngine.Random.Range(-1f, 1));

        switch (convergeMode)
        {
            case ConvergeToATargetEnum.None:
                {
                    convergeTarget = null;
                    break;
                }
            case ConvergeToATargetEnum.ConvergeToATarget:
                {
                    convergeTarget = convergeTargetToAssign;
                    break;
                }
        }
}


    
    public Vector3 GetEmitterCurrentLocalPos()
    {
        return transform.localPosition;
    }
    
    
    /// <summary>
    /// //////////////////GIZMOS/////////////////////////////
    /// </summary>
    void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(transform.position,5);
#if UNITY_EDITOR
        Gizmos.color = Color.red;
        
        switch (emitterType)
        {
            case UIParticleEmitterType.Circle:
                {
                    Gizmos.DrawWireSphere(transform.position, width/2f);

                    
                    break;
                }
            case UIParticleEmitterType.Rectangle:
                {
                    Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));
                    break;
                }

        }
        float angleCurrent = 0;
        Vector3 upPos = Vector3.zero;
        Vector3 rightSidePos = Vector3.zero;
        Vector3 leftSidePos = Vector3.zero;
        Gizmos.color = Color.green;
        switch (emitterConeType)
        {
            case UIParticleEmitterConeType.Omni:
                {
                    angleCurrent = 0;
                    upPos = transform.position + 200 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));
                    angleCurrent += 10;
                    rightSidePos = transform.position + 150 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));
                    angleCurrent -= 10 * 2;
                    leftSidePos = transform.position + 150 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));

                    Gizmos.DrawLine(transform.position, upPos);
                    Gizmos.DrawLine(rightSidePos, upPos);
                    Gizmos.DrawLine(leftSidePos, upPos);

                    angleCurrent = 90;
                    upPos = transform.position + 200 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));
                    angleCurrent += 10;
                    rightSidePos = transform.position + 150 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));
                    angleCurrent -= 10 * 2;
                    leftSidePos = transform.position + 150 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));

                    Gizmos.DrawLine(transform.position, upPos);
                    Gizmos.DrawLine(rightSidePos, upPos);
                    Gizmos.DrawLine(leftSidePos, upPos);

                    angleCurrent = 180;
                    upPos = transform.position + 200 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));
                    angleCurrent += 10;
                    rightSidePos = transform.position + 150 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));
                    angleCurrent -= 10 * 2;
                    leftSidePos = transform.position + 150 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));

                    Gizmos.DrawLine(transform.position, upPos);
                    Gizmos.DrawLine(rightSidePos, upPos);
                    Gizmos.DrawLine(leftSidePos, upPos);

                    angleCurrent = -90;
                    upPos = transform.position + 200 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));
                    angleCurrent += 10;
                    rightSidePos = transform.position + 150 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));
                    angleCurrent -= 10 * 2;
                    leftSidePos = transform.position + 150 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));

                    Gizmos.DrawLine(transform.position, upPos);
                    Gizmos.DrawLine(rightSidePos, upPos);
                    Gizmos.DrawLine(leftSidePos, upPos);
                    break;
                }
            case UIParticleEmitterConeType.Conical:
                {
                    angleCurrent = emitterConeDirectionAsAngle;
                    upPos = transform.position + 300 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));
                    angleCurrent += 10;
                    rightSidePos = transform.position + 250 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));
                    angleCurrent -= 10 * 2;
                    leftSidePos = transform.position + 250 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));

                    Gizmos.DrawLine(transform.position, upPos);
                    Gizmos.DrawLine(rightSidePos, upPos);
                    Gizmos.DrawLine(leftSidePos, upPos);
                    break;
                }
            case UIParticleEmitterConeType.directional:
                {
                    angleCurrent = emitterConeDirectionAsAngle;
                    upPos = transform.position + 300 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));
                    angleCurrent += 10;
                    rightSidePos = transform.position + 250 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));
                    angleCurrent -= 10 * 2;
                    leftSidePos = transform.position + 250 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));

                    Gizmos.DrawLine(transform.position, upPos);
                    Gizmos.DrawLine(rightSidePos, upPos);
                    Gizmos.DrawLine(leftSidePos, upPos);
                    break;
                }
        }
        Gizmos.color = Color.yellow;
        switch (emitterConeType)
        {
            case UIParticleEmitterConeType.Omni:
                {
                    
                    break;
                }
            case UIParticleEmitterConeType.Conical:
                {
                    angleCurrent = emitterConeDirectionAsAngle;
                    float halfCone = emitterConeAngle * 0.5f;
                    angleCurrent += halfCone;
                    rightSidePos = transform.position + 400 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));
                    angleCurrent -= halfCone * 2;
                    leftSidePos = transform.position + 400 *
                        (new Vector3(Mathf.Sin(angleCurrent * Mathf.Deg2Rad), Mathf.Cos(angleCurrent * Mathf.Deg2Rad), 0));

                    Gizmos.DrawLine(rightSidePos, transform.position);
                    Gizmos.DrawLine(leftSidePos, transform.position);
                    break;
                }
            case UIParticleEmitterConeType.directional:
                {
                    
                    break;
                }
        }

#endif
    }
}
