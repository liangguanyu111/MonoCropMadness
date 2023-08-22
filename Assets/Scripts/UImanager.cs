using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class UImanager : MonoBehaviour
{
    public static UImanager instance;

    [Header("Menu Panel")]
    public GameObject toolBar;
    public GameObject grids;
    private Vector2 lastPick;

    public Transform day;

    [Header("Shop/Bag Panel")]
    public GameObject soilPanel;

    public Transform cropCamera;
    public SoilPlacableObject soil;
    public Transform soilInfoPanel;

    [Header("Tool Bar")]
    public List<GameObject> toolSlots = new List<GameObject>();
    int toolIndex = 2;
    public Text toolName;

    [Header("SFXSlider")]

    public Slider sfxSlider;

    [Header("DayProcess")]

    public GameObject sunSprite;

    public Animator nextDay;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        BuildManager.instance.onBuildMode.AddListener(ToolBar);
        //BuildManager.instance.onBuildMode.AddListener(GridUI);

        GameManager.instance.playerData.gameDay.onNextDay.AddListener(SetDay);
        GameManager.instance.playerData.gameDay.onNextDay.AddListener(NextDayAni);
    }

    private void Update()
    {
        
    }

    public void ToolBar(bool isBuildMode)
    {
        if(isBuildMode)
        {
            toolBar.GetComponent<RectTransform>().DOMoveY(42, 1.0f);
        }
        else
        {
            toolBar.GetComponent<RectTransform>().DOMoveY(-37, 1.0f);
        }
    }

    public void GridUI(bool isBuildMode)
    {
        grids.SetActive(isBuildMode);
    }
    public void InitGrid()
    {
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 7; j++)
            {
              
            }
        }
    }


    public void GridPick(int x,int z)
    {   
        if(x<0||x>=7||z<0||z>=12)
        {
            return;
        }
        if (!BuildManager.instance.grid.GridArray[x, z].canBuild)
        {
           // grids.transform.GetChild(12 * x + z).GetComponent<Image>().color = Color.red;
        }
        else
        {
            //grids.transform.GetChild(12 * x + z).GetComponent<Image>().color = Color.green;
        }
        if (lastPick!=null&&(lastPick.x!=x||lastPick.y!=z))
        {
            //grids.transform.GetChild(12 * (int)lastPick.x + (int)lastPick.y).GetComponent<Image>().color = new Color(1,1,1,0.5725f);
            lastPick = new Vector2(x, z);
        }

    }

    public void SetDay()
    {
        day.GetComponent<Text>().text = "Day:" + GameManager.instance.playerData.gameDay.dayNum.ToString();
        day.GetChild(0).GetComponent<Text>().text = GameManager.instance.GetSeason().ToString();
    }

    public void ActiveSoilMenu(bool active,Vector3 position)
    {

        soilPanel.SetActive(active);
        if (soil!= null)
        {
            soilPanel.GetComponent<RectTransform>().transform.position = Camera.main.WorldToScreenPoint(position);
            soilPanel.GetComponent<RectTransform>().transform.position += new Vector3(20, 30, 0);
        }
   
    }
    public void SoilMenuPlant()
    {
        if(soil!=null&&soil.crop==null)
        {
            //BagSystem.instance.OpenSeedBag();
        }
    }

    public void SoilMenuHarvest()
    {
        if(soil!=null)
        {
            soil.Harvest();
        }
    }

    public void SoilInfoPanel()
    {
        cropCamera.transform.position = soil.transform.position +new Vector3(0,7f,-10f);
        soilInfoPanel.gameObject.SetActive(true);
    }

    public void ToolBarAniLeft2()
    {
        foreach (var item in toolSlots)
        {
            item.transform.DOComplete();
        }
        toolSlots[toolIndex].transform.DOScale(new Vector3(0.55f, 0.55f, 0.55f), 0.5f);
        toolSlots[toolIndex].transform.GetChild(0).GetComponent<Image>().enabled = false;
        toolIndex = toolIndex + 1 <= 5 ? toolIndex + 1 : 0;
        for (int i = 0; i < toolSlots.Count; i++)
        {
            if (i!=toolIndex)
            {
                int offset = toolIndex - i;
                if(offset<-2)
                {
                    offset = 6 - i + toolIndex;
                }
                else if(offset>=4)
                {
                    offset = toolIndex - i - 6;
                }
                float posx = -offset * 50;
                if(posx>-150)
                {
                    toolSlots[i].transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(posx, -10, 0), 0.5f);
                }
                else
                {
                    toolSlots[i].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(150, -10, 0);
                }
                         
            }
            else
            {
                toolSlots[toolIndex].transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0, -10, 0), 0.5f);
            }
        }
        toolSlots[toolIndex].transform.GetChild(0).GetComponent<Image>().enabled = true;
        toolSlots[toolIndex].transform.DOScale(new Vector3(0.85f, 0.85f, 0.85f), 0.5f);

        AudioManager._instance.PlaySfx("buttonSwap",0.25f);

        toolName.text = BuildManager.instance.BuildMode(toolIndex);
    }

  
    public void ToolBarAniRight()
    {
        foreach (var item in toolSlots)
        {
            item.transform.DOComplete();
        }
        toolSlots[toolIndex].transform.DOScale(new Vector3(0.55f, 0.55f, 0.55f), 0.5f);
        toolSlots[toolIndex].transform.GetChild(0).GetComponent<Image>().enabled = false;
        toolIndex = toolIndex - 1 >= 0 ? toolIndex - 1 : 5;
        for (int i = 0; i < toolSlots.Count; i++)
        {
            if (i != toolIndex)
            {
                int offset = i-toolIndex;
                if (offset < -2)
                {
                    offset = 6 - toolIndex +i;
                }
                else if (offset >= 5)
                {
                    offset = i - 6 - toolIndex;
                }
                float posx = offset * 50;
                if (posx < 150)
                {
                    toolSlots[i].transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(posx, -10, 0), 0.5f);
                }
                else
                {
                    toolSlots[i].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-100, -10, 0);
                }

            }
            else
            {
                toolSlots[toolIndex].transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0, -10, 0), 0.5f);
            }
        }
        toolSlots[toolIndex].transform.GetChild(0).GetComponent<Image>().enabled = true;
        toolSlots[toolIndex].transform.DOScale(new Vector3(0.85f, 0.85f, 0.85f), 0.5f);

        AudioManager._instance.PlaySfx("buttonSwap",0.25f);
        toolName.text = BuildManager.instance.BuildMode(toolIndex);
    }

    public void VolumeChange()
    {
        if(sfxSlider!=null)
        {
            GameManager.instance.SetVolume(sfxSlider.value);
        }
    }

    public void MoveSunByDayProcess(int val)
    {
        sunSprite.GetComponent<RectTransform>().DOAnchorPosX(-50f+20*val,0.3f);
    }

    public void NextDayAni()
    {
        nextDay.SetTrigger("NextDay");
    }
}
