using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class DropDownList : MonoBehaviour {

    public Dropdown myDropdown;
    private IDbConnection dbcon;
    Assets.DatabaseManager manager;
    string connection;
    // Use this for initialization
    void Start ()
    {
        manager = new Assets.DatabaseManager();

        if (Application.platform == RuntimePlatform.Android)
            LoadDatabase("Database.db");

        myDropdown = GameObject.Find("ListRecomendations").GetComponent<Dropdown>();
        myDropdown.ClearOptions();

        try
        {
            IDbConnection dbconn = manager.OpenConnection();
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = @"SELECT Description FROM Recomendations";
            dbcmd.CommandText = sqlQuery;
        
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                string description = reader.GetString(0);
                myDropdown.options.Add(new Dropdown.OptionData(description));
                //string name = reader.GetString(1);
                //int rand = reader.GetInt32(2);

            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }catch(System.Exception exc)
        {
            var test = GameObject.Find("Tittle").GetComponent<Text>();
            test.text = exc.InnerException.Message;
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnValueChanged()
    {
        var check = GameObject.Find("CheckProcessed").GetComponent<Toggle>();
        Assets.DatabaseManager manager = new Assets.DatabaseManager();

        IDbConnection dbconn = manager.OpenConnection();

        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = @"SELECT IsProcessed FROM Recomendations WHERE Description = @Description";
        dbcmd.CommandText = sqlQuery;

        IDbDataParameter dp = dbcmd.CreateParameter();
        dp.ParameterName = "@Description";
        dp.Value = myDropdown.captionText.text;
        dbcmd.Parameters.Add(dp);

        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            //string description = reader.GetString(0);
            //myDropdown.options.Add(new Dropdown.OptionData(description));
            //string name = reader.GetString(1);
            int isProcessed = reader.GetInt32(0);
            check.isOn = isProcessed == 1 ? true : false;

        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    public string GetConnectionPath()
    {
        if (Application.platform == RuntimePlatform.Android)
            //return Path.Combine(Application.streamingAssetsPath, "Database.db");
            return "URI=file:" + Application.persistentDataPath + "/" + "Database.db";
        else
            return "URI=file:" + Application.dataPath + "/" + "Database.db";
    }

    public void LoadDatabase(string DatabaseName)
    {
        string dbPath = Path.Combine(Application.persistentDataPath, "Database.db");
        var dbTemplatePath = Path.Combine(Application.streamingAssetsPath, "Database.db");

        if(File.Exists(dbPath))
        {
            File.Delete(dbPath);
        }

        if (!File.Exists(dbPath))
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                WWW reader = new WWW(dbTemplatePath);
                while (!reader.isDone) { }
                File.WriteAllBytes(dbPath, reader.bytes);
            }
            else
            {
                File.Copy(dbTemplatePath, dbPath, true);
            }
        }


    }
    public void OpenDB(string DatabaseName)
    {

            // check if file exists in Application.persistentDataPath
            var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);
       
            if (!File.Exists(filepath))
            {
                Debug.Log("Database not in Persistent path");
                // if it doesn't ->
                // open StreamingAssets directory and load the db ->
           
                var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
                while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
                // then save to Application.persistentDataPath
                File.WriteAllBytes(filepath, loadDb.bytes);
           
                Debug.Log("Database written");
            }
       
            var dbPath = filepath;

        //_connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);

        //open db connection
        connection = "URI=file:" + dbPath;
        Debug.Log("Stablishing connection to: " + connection);
        dbcon = new SqliteConnection(connection);
        dbcon.Open();
    }
}
