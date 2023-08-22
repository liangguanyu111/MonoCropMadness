using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem 
{
    #region WeatherStructure
    public struct Weather
    {
       public WeatherType type;
       public WeatherLevel level;
        public Weather(WeatherType type)
        {
            this.type = type;
            level = WeatherLevel.Normal;
        }
    }
    public enum WeatherType
    {
        Rain,
        Snow,
        Wind,
        Normal,
        Storm,
    }

    public enum WeatherLevel
    {
        Normal,
        Heavey
    }

    #endregion

    Transform weatherTrans;

    public void GenerateRandomWeather()
    {
        if(weatherTrans ==null)
        {
            weatherTrans = GameObject.Find("Weather").transform;
        }
        else
        {
            for (int i = 0; i < weatherTrans.childCount; i++)
            {
                GameObject.Destroy(weatherTrans.GetChild(i).gameObject);
            }
        }


        Weather newWeather = new Weather();
        float index = Random.Range(0, 1.0f);
        if(index<=0.25f)
        {
            newWeather = new Weather(WeatherType.Rain);
            if(index<=0.1f)
            {
                newWeather.level = WeatherLevel.Heavey;
            }
        }
        else if(index<=0.4f)
        {
            newWeather = new Weather(WeatherType.Snow);
            if (index <= 0.325f)
            {
                newWeather.level = WeatherLevel.Heavey;
            }
        }
        else if(index<=0.65f)
        {
            newWeather = new Weather(WeatherType.Wind);
            if (index <= 0.55f)
            {
                newWeather.level = WeatherLevel.Heavey;
            }
        }
        else
        {
            newWeather = new Weather(WeatherType.Normal);
        }

        GenerateWeather(newWeather);
    }

    public void GenerateWeather(Weather weather)
    {
        string levelName = "average";
        if(weather.level==WeatherLevel.Heavey)
        {
            levelName = "heavy";
        }
 

        switch(weather.type)
        {
            case WeatherType.Rain:  
                GameObject rainEffect = Resources.Load<GameObject>($"Weather/Rain_{levelName}");
                GameObject.Instantiate(rainEffect,new Vector3(31,0,0),Quaternion.Euler(0,Random.Range(0,360),0) ,weatherTrans);
                GameObject.Instantiate(rainEffect, new Vector3(31, 0, 37.2f), Quaternion.Euler(0, Random.Range(0, 360), 0), weatherTrans);
                AudioManager._instance.PlayBKMusic("rain");
                break;

            case WeatherType.Snow:
                GameObject snowEffect = Resources.Load<GameObject>($"Weather/Snow_{levelName}");
                GameObject.Instantiate(snowEffect, new Vector3(31, 0, 0), Quaternion.Euler(0, Random.Range(0, 360), 0), weatherTrans);
                GameObject.Instantiate(snowEffect, new Vector3(31, 0, 37.2f), Quaternion.Euler(0, Random.Range(0, 360), 0), weatherTrans);
                AudioManager._instance.PlayBKMusic("snow");
                break;
            case WeatherType.Wind:
                if(Random.Range(0,1.0f)<0.2f)
                {
                    GameObject winEffect_sp = Resources.Load<GameObject>($"Weather/Wind_sp");
                    GameObject.Instantiate(winEffect_sp, new Vector3(30, 13.8f, -27.3f), Quaternion.Euler(0, -75f, 0), weatherTrans);
                    AudioManager._instance.PlayBKMusic("windy");
                }
                else
                {
                    GameObject winEffect = Resources.Load<GameObject>($"Weather/Wind");
                    GameObject.Instantiate(winEffect, new Vector3(50, 7.5f, -29), Quaternion.Euler(0, -90, 0), weatherTrans);
                    GameObject.Instantiate(winEffect, new Vector3(23, 7.5f, -29), Quaternion.Euler(0, -90, 0), weatherTrans);
                    if (weather.level == WeatherLevel.Heavey)
                    {
                        GameObject winEffect2 = Resources.Load<GameObject>($"Weather/Wind_heavy");
                        GameObject.Instantiate(winEffect2, new Vector3(26, 13, -58), Quaternion.Euler(10, 0, 0), weatherTrans);
                        AudioManager._instance.PlayBKMusic("windy");
                    }
                    else
                    {
                        AudioManager._instance.PlayBKMusic("normal");
                    }
                }
                break;
            case WeatherType.Normal:
                if(Random.Range(0,1.0f)<0.5f)
                {
                    GameObject normalEffect1 = Resources.Load<GameObject>($"Weather/Normal_1");
                    GameObject.Instantiate(normalEffect1, new Vector3(42, 20, 75), Quaternion.identity, weatherTrans);
                    AudioManager._instance.PlayBKMusic("normal_2");
                }
                if (Random.Range(0, 1.0f) < 0.5f)
                {
                    GameObject normalEffect2 = Resources.Load<GameObject>($"Weather/Normal_2");
                    GameObject.Instantiate(normalEffect2, new Vector3(20.6f, 5.8f, -48.3f), Quaternion.Euler(0, -90.0f, 0), weatherTrans);
                    AudioManager._instance.PlayBKMusic("normal_2");
                }

                break;


        }

    }
}
