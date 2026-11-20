using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{

    public static void SavePlayer(GameObject playerGO, List<GameObject> attackButtonsList)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.poo";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(playerGO, attackButtonsList);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.poo";

        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

    public static void SaveResources(ResourceManager resourceManager, Blacksmith blacksmith, Settlement settlement)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.res";
        FileStream stream = new FileStream(path, FileMode.Create);

        ResourceData data = new ResourceData(resourceManager, blacksmith, settlement);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static ResourceData LoadResources()
    {
        string path = Application.persistentDataPath + "/player.res";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            ResourceData data = formatter.Deserialize(stream) as ResourceData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

    public static void SaveEnemies(EnemyManager enemyManager, MAINmanager mainManager, int[] enemyCooldowns)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.enemy";
        FileStream stream = new FileStream(path, FileMode.Create);

        EnemyData data = new EnemyData(enemyManager, mainManager, enemyCooldowns);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static EnemyData LoadEnemies()
    {
        string path = Application.persistentDataPath + "/player.enemy";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            EnemyData data = formatter.Deserialize(stream) as EnemyData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

    public static void SaveRandom(Teleport teleport, Camera camera, SoundSystem soundSystem)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.random";
        FileStream stream = new FileStream(path, FileMode.Create);

        RandomData data = new RandomData(teleport, camera, soundSystem);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static RandomData LoadRandom()
    {
        string path = Application.persistentDataPath + "/player.random";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            RandomData data = formatter.Deserialize(stream) as RandomData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

    public static void SaveInventory(Inventory inventory)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.inv";
        FileStream stream = new FileStream(path, FileMode.Create);

        InventoryData data = new InventoryData(inventory);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static InventoryData LoadInventory()
    {
        string path = Application.persistentDataPath + "/player.inv";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            InventoryData data = formatter.Deserialize(stream) as InventoryData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

    public static void SaveGrid(GridBuildingSystem grid)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.grid";
        FileStream stream = new FileStream(path, FileMode.Create);

        GridData data = new GridData(grid, grid.GetGrid());

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static GridData LoadGrid()
    {
        string path = Application.persistentDataPath + "/player.grid";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GridData data = formatter.Deserialize(stream) as GridData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

}
