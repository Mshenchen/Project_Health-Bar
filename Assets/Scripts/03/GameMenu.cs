using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;

public class GameMenu : MonoBehaviour
{
    public Image gameMenuImage;
    public Toggle BGMToggle;
    private AudioSource BGMSource;
    private PlayerMovement player;
    private void Start()
    {
        gameMenuImage.gameObject.SetActive(false);
        BGMSource = GetComponent<AudioSource>();
        player = FindObjectOfType<PlayerMovement>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Gamemanager.instance.isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        BGMManager();
    }
    public void Resume()
    {
        gameMenuImage.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
        Gamemanager.instance.isPaused = false;
    }
    public void Pause()
    {
        gameMenuImage.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
        Gamemanager.instance.isPaused = true;
    }
    public void BGMToggleButton()
    {
        if (BGMToggle.isOn)
        {
            PlayerPrefs.SetInt("BGM", 1);
            Debug.Log(PlayerPrefs.GetInt("BGM"));
        }
        else
        {
            PlayerPrefs.SetInt("BGM", 0);
            Debug.Log(PlayerPrefs.GetInt("BGM"));
        }
    }
    private void BGMManager()
    {
        if (PlayerPrefs.GetInt("BGM") == 1)
        {
            BGMToggle.isOn = true;
            BGMSource.enabled = true;
        }else if (PlayerPrefs.GetInt("BGM") ==0)
        {
            BGMToggle.isOn = false;
            BGMSource.enabled = false;
        }
    }
    public void SaveButton()
    {
        //SaveByPlayerPrefs();
        //SaveBySerialization();
        SaveByJSON();
        //SaveByXML();
    }
    public void LoadButton()
    {
        //LoadByPlayerPrefs();
        //LoadBySerialization();
        LoadByJSON();
        //LoadByXML();
        Resume();
    }
    private void SaveByXML()
    {
        Save save = createSaveGameObject();
        XmlDocument xmlDocument = new XmlDocument();
        #region CreateXML elements
        XmlElement Root = xmlDocument.CreateElement("Save");
        Root.SetAttribute("FileName", "File_01");
        XmlElement coinNumElement = xmlDocument.CreateElement("CoinNum");
        coinNumElement.InnerText = save.coinsNum.ToString();
        Root.AppendChild(coinNumElement);
        XmlElement diamondNumElement = xmlDocument.CreateElement("DiamondNum");
        diamondNumElement.InnerText = save.diamondsNum.ToString();
        Root.AppendChild(diamondNumElement);
        xmlDocument.AppendChild(Root);
        #endregion
        xmlDocument.Save(Application.dataPath + "/DataXML.text");
        if(File.Exists(Application.dataPath + "/DataXML.text"))
        {
            Debug.Log("XML FILE SAVED");
        }
    }
    private void LoadByXML()
    {
        if (File.Exists(Application.dataPath + "/DataXML.text"))
        {
            Save save = new Save();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Application.dataPath + "/DataXML.text");
            XmlNodeList coinNum = xmlDocument.GetElementsByTagName("CoinNum");
            int coinNumCount = int.Parse(coinNum[0].InnerText);
            save.coinsNum = coinNumCount;
            XmlNodeList diamondNum = xmlDocument.GetElementsByTagName("DiamondNum");
            int diamondNumCount = int.Parse(diamondNum[0].InnerText);
            save.diamondsNum = diamondNumCount;
            Gamemanager.instance.coins = save.coinsNum;
            Gamemanager.instance.diamonds = save.diamondsNum;
            Debug.Log("XML FILE OK");
        }
    }
    private Save createSaveGameObject()
    {
        Save save = new Save();
        save.coinsNum = Gamemanager.instance.coins;
        save.diamondsNum = Gamemanager.instance.diamonds;
        save.playerPositionX = player.transform.position.x;
        save.playerPositionY = player.transform.position.y;
        return save;
    }
    private void SaveByJSON()
    {
        Save save = createSaveGameObject();
        string JsonString = JsonUtility.ToJson(save);
        StreamWriter sw = new StreamWriter(Application.dataPath + "/JSONData.text");
        sw.Write(JsonString);
        sw.Close();
    }
    private void LoadByJSON()
    {
        if(File.Exists(Application.dataPath+ "/JSONData.text"))
        {
            StreamReader sr = new StreamReader(Application.dataPath + "/JSONData.text");
            string JsonString = sr.ReadToEnd();
            sr.Close();
            Save save = JsonUtility.FromJson<Save>(JsonString);
            Gamemanager.instance.coins = save.coinsNum;
            Gamemanager.instance.diamonds = save.diamondsNum;
            player.transform.position = new Vector2(save.playerPositionX, save.playerPositionY);
        }
        else
        {
            Debug.Log("NOT FOUND FILE");
        }
    }
    private void SaveBySerialization()
    {
        Save save = createSaveGameObject();
        BinaryFormatter bf = new BinaryFormatter();
        //FileStream fileStream = File.Create(Application.persistentDataPath + "/Data.text");
        FileStream fileStream = File.Create(Application.dataPath + "/Data.text");
        bf.Serialize(fileStream, save);
        fileStream.Close();
    }
    private void LoadBySerialization()
    {
        if (File.Exists(Application.persistentDataPath + "/Data.text"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            //FileStream fileStream = File.Open(Application.persistentDataPath + "/Data.text", FileMode.Open);
            FileStream fileStream = File.Open(Application.dataPath + "/Data.text", FileMode.Open);
            Save save = bf.Deserialize(fileStream) as Save;
            fileStream.Close();
            Gamemanager.instance.coins = save.coinsNum;
            Gamemanager.instance.diamonds = save.diamondsNum;
            player.transform.position = new Vector2(save.playerPositionX, save.playerPositionY);

        }
        else
        {
            Debug.Log("NOT FOUND THIS FILE");
        }
    }
    private void SaveByPlayerPrefs()
    {
        PlayerPrefs.SetInt("Coins", Gamemanager.instance.coins);
        PlayerPrefs.SetInt("Diamonds", Gamemanager.instance.diamonds);
    }
    private void LoadByPlayerPrefs()
    {
        Gamemanager.instance.coins = PlayerPrefs.GetInt("Coins");
        Gamemanager.instance.diamonds = PlayerPrefs.GetInt("Diamonds");
    }
}
  