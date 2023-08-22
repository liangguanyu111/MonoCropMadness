using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;
using Newtonsoft.Json;

public class Day
{
    public int dayNum;

    private int dayProcess;


    [JsonIgnore]
    public UnityEvent onNextDay = new UnityEvent();

    [JsonIgnore]
    public UnityEvent<int> onNextProcess = new UnityEvent<int>();
    public Day()
    {
        dayNum = 0;
        dayProcess = 0;
    }


    public void NextDaySkip()
    {
        dayNum += 1;
        onNextDay.Invoke();
 
        dayProcess = 0;
        onNextProcess.Invoke(dayProcess);
    }


    public void NextProcess()
    {

        if (dayProcess >= 4)
        {
            dayNum += 1;
            onNextDay.Invoke();
        }

        dayProcess = dayProcess + 1 <= 4 ? dayProcess+1 : 0;

        onNextProcess.Invoke(dayProcess);


    }
}
