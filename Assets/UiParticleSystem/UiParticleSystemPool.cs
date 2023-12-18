using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiParticleSystemPool : MonoBehaviour
{
    public GameObject uiParticleUnitPrefab;
    public int maxPartclesCount;

    public List<UiParticleUnit> uiParticlesList;
    public List<UiParticleUnit> uiParticlesAlive;
    UiParticleSystem uiParticleSystem;

    bool initialized;

    // Start is called before the first frame update
    void Start()
    {
        //CreateUiParticles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateUiParticles()
    {
        if (initialized)
        {
            return;
        }
        uiParticlesList.Clear();
        for (int i = 0; i < maxPartclesCount; i++)
        {
            GameObject go = Instantiate(uiParticleUnitPrefab.gameObject, transform.position, transform.rotation);
            go.transform.SetParent(transform);
            UiParticleUnit uP = go.GetComponent<UiParticleUnit>();
            uP.ResetParticleUnit();
            uP.SetUiParticleSystemPool(this);
            uiParticlesList.Add(uP);

            if(uiParticleSystem == null)
            {
                uiParticleSystem = GetComponent<UiParticleSystem>();
            }
            
            //Material mat = new Material(uiParticleSystem.matParticleSystem);
            Material mat = (uiParticleSystem.matParticleSystem);
            uP.SetMyMat(mat);
            
            
        }
        initialized = true;
    }
    public UiParticleUnit AliveAParticle()
    {
        if(uiParticlesList.Count > 0)
        {
            UiParticleUnit uP = uiParticlesList[0];
            uP.InitializeParticleUnit();
            uiParticlesAlive.Add(uP);
            uiParticlesList.RemoveAt(0);
            uP.SetEmitterLocalPositionAtSpawn(uiParticleSystem.GetEmitterCurrentLocalPos());
            return uP;
        }
        else
        {
            return null;
        }
    }
    public void SleepAParticle(UiParticleUnit uP)
    {
        uiParticlesAlive.Remove(uP);
        uiParticlesList.Add(uP);

    }
}
