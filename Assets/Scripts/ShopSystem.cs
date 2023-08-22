using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShopSystem : MonoBehaviour
{
    public GameObject SlotPrefab;

    //农作物价格在 90%-110%左右波动
    //农作物的价格应该收到季节影响
    [Header("Panel")]
    public Transform buyPanelTransform;
    public Transform sellPanelTranform;
    public Transform sellSlotTransform;

    [Header("Price")]
    public WMG_Axis_Graph graph;

    public CropSO currentSelectSo;

    [Header("Slot Component")]
    public GameObject priceSlotPrefab;


    public TMP_InputField sellInputField;
    public int sellInputAmount;
    private void Awake()
    {
        
    }

    private void Start()
    {
        foreach (var crop in GameManager.instance.cropList)
        {
            GenerateShopSlot(crop);
            InitSellPanel(crop);
        }

        GameManager.instance.onUpdatePrice.AddListener(UpdateSlotPrice);
        BagSystem.instance.onHarvest.AddListener(UpdateCropAmount);
        sellInputField.onValueChanged.AddListener(InputValueLimit);
    }
    public GameObject GenerateShopSlot(CropSO crop)
    {
        GameObject cropSlot = Instantiate(SlotPrefab, buyPanelTransform);

        cropSlot.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            currentSelectSo = crop;
        });

        cropSlot.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = crop.cropSprite;
        cropSlot.transform.GetChild(1).GetComponent<Text>().text = crop.cropName;
        cropSlot.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "$" + GameManager.instance.eco.GetLastestSeedPrice(crop).ToString();

        return cropSlot;
    }

    public void UpdateSlotPrice()
    {
        for (int i = 0; i < buyPanelTransform.childCount; i++)
        {
            buyPanelTransform.GetChild(i).GetChild(2).GetChild(0).GetComponent<Text>().text = "$" + GameManager.instance.eco.GetLastestSeedPrice(GameManager.instance.cropList[i]).ToString();
        }
    }

    public void InitSellPanel(CropSO crop)
    {
       GameObject priceSlot = Instantiate(priceSlotPrefab, sellSlotTransform);
       priceSlot.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = crop.cropSprite;
        priceSlot.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = BagSystem.instance.GetCrop(crop).cropNum.ToString();
        priceSlot.GetComponentInChildren<Button>().onClick.AddListener(
       () =>
       {
           currentSelectSo = crop;
           InitCropPrice();
       });
    }

    public void UpdateCropAmount()
    {
        for (int i = 0; i < GameManager.instance.cropList.Count; i++)
        {
            sellSlotTransform.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Text>().text = BagSystem.instance.GetCrop(GameManager.instance.cropList[i]).cropNum.ToString();
        }
    }

    public void InitCropPrice()
    {
        Debug.Log("Price!");

        //上下边界
        int upBound = Mathf.RoundToInt(currentSelectSo.cropPrice * 2);
        int downBound = Mathf.RoundToInt(currentSelectSo.cropPrice / 2);

        if((upBound - downBound)<2)
        {
            graph.yAxis.AxisNumTicks = 2;
        }
        else
        {
            graph.yAxis.AxisNumTicks = upBound - downBound +1 ;
        }

        graph.yAxis.AxisMinValue = downBound;
        graph.yAxis.AxisMaxValue = upBound;


        int num = Mathf.RoundToInt(GameManager.instance.playerData.gameDay.dayNum / 5) + 1;
        //设置Group
        if (GameManager.instance.playerData.gameDay.dayNum <=50)
        {
            graph.groups.Clear();
            for (int i = 0; i < num; i++)
            {
                graph.groups.Add("Day " + ((i*5)+1));
            }
        }
       

        graph.lineSeries[0].GetComponent<WMG_Series>().pointValues.Clear();
        graph.lineSeries[0].GetComponent<WMG_Series>().seriesName = currentSelectSo.cropName;
        graph.lineSeries[0].GetComponent<WMG_Series>().pointColor = Color.yellow;

        Queue<float> priceQueue = GameManager.instance.eco.GetCropPrices(currentSelectSo);

        for (int i = 0; i < priceQueue.Count; i++)
        {
            float price = priceQueue.Dequeue();
            graph.lineSeries[0].GetComponent<WMG_Series>().pointValues.Add(new Vector2(i+1,price));
            priceQueue.Enqueue(price);
        }
    }

    public void InitSeedPrice()
    {
        Debug.Log("Price!");

        //上下边界
        int upBound = Mathf.RoundToInt(currentSelectSo.seedPrice * 2);
        int downBound = Mathf.RoundToInt(currentSelectSo.seedPrice / 2);

        if ((upBound - downBound) < 2)
        {
            graph.yAxis.AxisNumTicks = 2;
        }
        else
        {
            graph.yAxis.AxisNumTicks = upBound - downBound + 1;
        }

        graph.yAxis.AxisMinValue = downBound;
        graph.yAxis.AxisMaxValue = upBound;


        int num = Mathf.RoundToInt(GameManager.instance.playerData.gameDay.dayNum / 5) + 1;
        //设置Group
        if (GameManager.instance.playerData.gameDay.dayNum <= 50)
        {
            graph.groups.Clear();
            for (int i = 0; i < num; i++)
            {
                graph.groups.Add("Day " + ((i * 5) + 1));
            }
        }

        graph.lineSeries[0].GetComponent<WMG_Series>().pointValues.Clear();
        graph.lineSeries[0].GetComponent<WMG_Series>().seriesName = currentSelectSo.cropName;
        graph.lineSeries[0].GetComponent<WMG_Series>().pointColor = Color.green;

        Queue<float> priceQueue = GameManager.instance.eco.GetSeedPrices(currentSelectSo);

        for (int i = 0; i < priceQueue.Count; i++)
        {
            float price = priceQueue.Dequeue();
            graph.lineSeries[0].GetComponent<WMG_Series>().pointValues.Add(new Vector2(i + 1, price));
            priceQueue.Enqueue(price);
        }
    }

    public void BuySeed()
    {
        if(currentSelectSo!=null)
        {
            //TODO CHECK MONEY
            if(GameManager.instance.Gold >= GameManager.instance.eco.GetLastestSeedPrice(currentSelectSo))
            {
                BagSystem.instance.GetSeeds(currentSelectSo);
                GameManager.instance.Gold -= GameManager.instance.eco.GetLastestSeedPrice(currentSelectSo);

                AudioManager._instance.PlaySfx("buttonBuy");
            }
            else
            {
                AudioManager._instance.PlaySfx("buttonBuyFailed");
            }

        }
    }

    public void SellCrop()
    {
        if(currentSelectSo!=null)
        {
            int num;
            int.TryParse(sellInputField.text, out num);
            BagSystem.instance.SellCrop(currentSelectSo, num);
        }
    }


    public void InputValueLimit(string text)
    {
        int num;
        int.TryParse(text, out num);
        if(currentSelectSo!=null&&num!=0)
        {
            num = num > 0 ? num : 0;
            num = num < BagSystem.instance.GetCrop(currentSelectSo).cropNum ? num : BagSystem.instance.GetCrop(currentSelectSo).cropNum;
        }

        sellInputField.text = num.ToString();
    }
}
