using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using System.IO;

namespace Assets
{
    public class DatabaseManager
    {
        public IDbConnection OpenConnection()
        {
            if (Application.platform == RuntimePlatform.Android)
                return OpenDB("Database.db");

                string conn = GetConnectionPath();
            IDbConnection dbconn = new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            return dbconn;
        }

        private string GetConnectionPath()
        {
            if (Application.platform == RuntimePlatform.Android)
                //return Path.Combine(Application.streamingAssetsPath, "Database.db");
                return "URI=file:" + Application.persistentDataPath + "/" + "Database.db";
            else
                return "URI=file:" + Application.dataPath + "/" + "Database.db";
        }

        public IDbConnection OpenDB(string DatabaseName)
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
            var connection = "URI=file:" + dbPath;
            Debug.Log("Stablishing connection to: " + connection);
            var dbcon = new SqliteConnection(connection);
            dbcon.Open();
            return dbcon;
        }
    }
}
