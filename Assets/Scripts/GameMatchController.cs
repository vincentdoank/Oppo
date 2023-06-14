using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMatchController : MonoBehaviour
{
    public bool isStarted;
    public Weather weather;
    public Locator locator;
    public SkyController skyController;
    public ScoreController scoreController;

    public Ball ball;

    public Camera screenCamera;

    public Dictionary<ulong, int> clientsRoleDict = new Dictionary<ulong, int>();

    public static GameMatchController Instance { get; protected set; }

    protected virtual void Start()
    {
        Instance = this;
    }

    public void CheckWeather()
    {
        
    }

    protected IEnumerator GetWeatherData()
    {
        float lati = 0f;
        float longi = 0f;
        Weather.WeatherType weatherType = Weather.WeatherType.Clear;
        yield return locator.GetLatitudeLongitude((latitude, longitude) => { lati = latitude; longi = longitude; });
        yield return weather.RequestWeather(lati, longi, (weather) => { weatherType = weather; });
        Debug.LogWarning("Get Weather data : " + weatherType);
        skyController.CheckCurrentTimeWeather(weatherType);
    }

    public virtual void StartMatch()
    {
        isStarted = true;
        CheckWeather();
        if (GameManager.Instance.IsServer)
        {
            Debug.Log("Start Match");
            scoreController.time.SetTime(10, () =>
            {
                AutoShoot();
                EventManager.onShootTimerEnded?.Invoke(GameManager.Instance.GetClientId());
            });
            EventManager.onShootTimerStarted?.Invoke(GameManager.Instance.GetClientId());
        }
    }

    protected virtual void AutoShoot()
    {
        
    }

    public virtual void SendPlayerData()
    {
       
    }

    public virtual void OnDisconnected(ulong clientId)
    {
        
    }
}
