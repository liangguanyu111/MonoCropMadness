using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;
using UnityEngine.Events;
public class SoilPlacableObject : FarmPlacableObject
{
    [SerializeField]
    public SoilInfo soilInfo;

    public CropSO cropInfo;
    public CropSO lastCropInfo;

    public Crop crop = new Crop();

    [JsonIgnore]
    public UnityEvent<SoilPlacableObject,int, int> onPlant = new UnityEvent<SoilPlacableObject,int, int>();

    [JsonIgnore]
    public UnityEvent<SoilInfo> OnSoilChange = new UnityEvent<SoilInfo>();

    [JsonIgnore]
    public GameObject cropObj;

    [Header("VFX")]
    public GameObject cockroachEffect;
    public GameObject upgradeEffect;
    public void LoadCropPrefab(CropSO cropInfo)
    {
        this.cropInfo = cropInfo;
        lastCropInfo = cropInfo;

        crop = Instantiate(Resources.Load<Crop>("Crop/"+ cropInfo.cropName));
 
        crop.CropInit(this.transform.position+new Vector3(0,0.3f,0),this);

        crop.gameObject.transform.DOShakePosition(1.0f, 1.0f);
        UpdateSoilInfo();
        updateInteractState();
        onPlant.Invoke(this,origin.x,origin.y);
    }

    // Ëæ»úÊÂ¼þ
    public void OnNextDay()
    {
        float healthFactor = (soilInfo.healthLevel / 100) *Random.Range(0.8f,1.2f);
        float sickFactor = Random.Range(0, 0.8f);

        if(sickFactor>healthFactor&&!soilInfo.isSick)
        {
            SuddenEvents.instance.SoilSickEvent(this);
            soilInfo.isSick = true;
            soilInfo.isUpgrade = false;
            Upgrade();
        }

        if (crop == null)
        {
            if (!soilInfo.isSick)
            {
                float randomValue = soilInfo.healthLevel * Random.Range(1.2f, 1.35f);
                randomValue = randomValue > 5 ? randomValue : 5;
                soilInfo.healthLevel = soilInfo.healthLevel + randomValue < 120 ? soilInfo.healthLevel + randomValue : 120;
                float randomValue2 = soilInfo.nutritionalLevel * Random.Range(1.2f, 1.35f);
                randomValue2 = randomValue2 > 5 ? randomValue2 : 5;
                soilInfo.nutritionalLevel = soilInfo.nutritionalLevel + randomValue2 < 120 ? soilInfo.nutritionalLevel + randomValue2 : 120;
            }
        }
        else
        {
            soilInfo.healthLevel = soilInfo.healthLevel * Random.Range(0.9f, 1.05f);
            soilInfo.nutritionalLevel = soilInfo.nutritionalLevel * Random.Range(0.9f, 1.0f);
        }
        if(soilInfo.isSick)
        {
             soilInfo.healthLevel = soilInfo.healthLevel * Random.Range(0.75f, 0.85f);
             soilInfo.nutritionalLevel = soilInfo.nutritionalLevel * Random.Range(0.75f, 0.85f);
        }

        UpdateSoilInfo();
    }

    public void LoadCropBySoilInfo(SoilInfo info)
    {
        LoadCropPrefab(GameManager.instance.cropList[info.cropNum]);
    }

    public void UpdateSoilInfo()
    {
        if(soilInfo!=null)
        {
            Vector3 color1 = new Vector3(0.65f, 0.45f, 0);
            Vector3 color2 = new Vector3(0.22f, 0.16f, 0);
            Vector3 color = Vector3.Lerp(color2, color1, soilInfo.nutritionalLevel / 100);
            this.GetComponent<MeshRenderer>().material.color = new Color(color.x,color.y,color.z,1);
            soilInfo.UpdateSoilInfo(cropInfo);
            OnSoilChange.Invoke(soilInfo);
        }
    }

    public void Harvest()
    {
        if(crop!=null &&crop.cropState ==CropState.Matured)
        {
            int expectProduce =Mathf.RoundToInt(cropInfo.expectProduce * Random.Range(0.9f,1.1f));
            if(GameManager.instance.GetSeason()==cropInfo.SuitSeason|| cropInfo.SuitSeason == Season.AllSeason)
            {
                expectProduce = Mathf.RoundToInt(expectProduce * Random.Range(1.0f, 1.2f));
            }
            //NuritionLevel Factor
            float factor1 = (soilInfo.nutritionalLevel / 100f) > 0 ? (soilInfo.nutritionalLevel/100f) : 0;
            factor1 = factor1<1.2f ? factor1:1.2f;
            //HealthLevel Factor

            expectProduce = Mathf.RoundToInt(expectProduce *  factor1);

            BagSystem.instance.GetSellCrops(cropInfo, expectProduce);

            soilInfo.cropCountDay = 0;
            Destroy(crop.cropModel);
            Destroy(crop.gameObject);

            soilInfo.isUpgrade = false;
            if(upgradeEffect!=null)
            {
                upgradeEffect.SetActive(soilInfo.isUpgrade);
            }

            crop = null;
            cropInfo = null;

            updateInteractState();
        }
    }

    public void Shovel()
    {
        if(crop!=null)
        {
            soilInfo.cropCountDay = 0;
            Destroy(crop.cropModel);
            Destroy(crop.gameObject);
            crop = null;
            cropInfo = null;
            updateInteractState();
        }
    }

    public void Fertilisation()
    {
        soilInfo.nutritionalLevel = soilInfo.nutritionalLevel + 25 <= 120 ? soilInfo.nutritionalLevel : 120;


        UpdateSoilInfo();
    }

    public void Pesticion()
    {
        soilInfo.healthLevel = soilInfo.healthLevel + 20 <120 ? soilInfo.healthLevel +20  : 120;
        soilInfo.nutritionalLevel = soilInfo.nutritionalLevel - 10 > 0 ? soilInfo.nutritionalLevel -10 : 0;
        if (soilInfo.isSick)
        {
            soilInfo.isSick = false;
            Destroy( cockroachEffect.gameObject);
        }
        UpdateSoilInfo();
    }



    public override void Interact(ToolState toolState)
    {

        character ch = GameObject.FindGameObjectWithTag("Player").GetComponent<character>();
        ch.agent.SetDestination(this.transform.position);
        switch (toolState)
        {
            case ToolState.Seed:
                BagSystem.instance.OpenSeedBag(this);
                break;
            case ToolState.Shovel:
                ch.aniTodo = null;
                ch.actionTodo = null;
                ch.aniTodo = () => { ch.SetAni("Dig"); };
                ch.actionTodo = () =>
                {
                    Shovel();
                };              
                break;
            case ToolState.Pesticides:
                ch.aniTodo = null;
                ch.actionTodo = null;
                ch.aniTodo = () => { ch.SetAni("Work"); };
                ch.actionTodo = () =>
                {
                    Pesticion();
                };
                break;
            case ToolState.fertilizer:
                ch.aniTodo = null;
                ch.actionTodo = null;
                ch.aniTodo = () => { ch.SetAni("Work"); };
                ch.actionTodo = () =>
                {
                    Fertilisation();
                };
                break;
        }

        //canInteract = false;
        //UImanager.instance.soil = this;
        //UImanager.instance.ActiveSoilMenu(true, this.transform.position);
    }

    void updateInteractState()
    {
        SetToolStateStatus(ToolState.Shovel, crop != null);
        SetToolStateStatus(ToolState.Seed, crop == null);
    }

    public void Upgrade()
    {
        if(upgradeEffect==null && soilInfo.isUpgrade)
        {
            upgradeEffect = Instantiate(Resources.Load<GameObject>("vfx/UpgradeEffect"),new Vector3(this.transform.position.x,0.25f,this.transform.position.z),Quaternion.identity);
            upgradeEffect.transform.SetParent(this.transform);
        }
        else
        {
            upgradeEffect.SetActive(soilInfo.isUpgrade);
        }
    }

    void UpdateSoilGameObject()
    {

    }
}
