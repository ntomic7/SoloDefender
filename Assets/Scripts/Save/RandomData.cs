using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomData 
{
    public bool[] activeTeleportAttacks = new bool[4];
    public float[] cameraPosition = new float[3];

    public float sfxVolume;
    public float musicVolume;

    public RandomData(Teleport teleport, Camera camera, SoundSystem soundSystem)
    {
        int i = 0;
        foreach (bool b in teleport.GetIsAttacked().Values)
        { 


            activeTeleportAttacks[i] = b; i++;
        }

        cameraPosition[0] = camera.transform.position.x;
        cameraPosition[1] = camera.transform.position.y;
        cameraPosition[2] = camera.transform.position.z;

        sfxVolume = soundSystem.GetSFXVolume();
        musicVolume = soundSystem.GetMusicVolume();

    }



}
