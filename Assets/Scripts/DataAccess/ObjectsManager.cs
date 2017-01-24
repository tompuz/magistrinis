using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Assets.Scripts.VuforiaApi;

namespace Assets.Scripts.DataAccess
{
    public class ObjectsManager : IObjectsManager
    {
        public virtual List<ObjectImageEntity> GetActiveObjects()
        {
            var objList = new List<ObjectImageEntity>();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = GlobalItems.ConnectionString;
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = @"select Photo,Distance,Angle,Name,TransactionId,Description,Rating from ImageObjects where TransactionId is not null";

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    objList.Add(new ObjectImageEntity
                    {
                        Name = reader.GetString(3),
                        TransactionId = reader.GetString(4),
                        Angle = reader.GetInt32(2),
                        Description = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Distance = reader.GetDecimal(1),
                        Photo = (byte[])reader.GetValue(0),
                        Rating = reader.GetInt32(6)
                    });
                }
                conn.Close();
            }
            return objList;
        }

        public virtual void InsertObject(int angle, decimal distance, string decription)
        {
           using (SqlConnection conn = new SqlConnection())
           {
               conn.ConnectionString = GlobalItems.ConnectionString;
               conn.Open();

               SqlCommand cmd = new SqlCommand();
               cmd.Connection = conn;
               cmd.CommandText = @"insert into dbo.ImageObjects (Photo,Angle,Distance,Name) 
                           values (@photo, @angle, @distance, @description)";

               cmd.Parameters.Add("@photo", SqlDbType.Image).Value = GlobalItems.PictureBytes;
               cmd.Parameters.Add("@angle", SqlDbType.Int).Value = angle;
               cmd.Parameters.Add("@distance", SqlDbType.Decimal).Value = distance;
               cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = decription;

               cmd.ExecuteNonQuery();

               conn.Close();
           }
        }

        public string GetObjectDesription(string transactionId)
        {
            string description = null;
            using (var conn = new SqlConnection(GlobalItems.ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand
                {
                    Connection = conn,
                    CommandText = @"select Description from dbo.ImageObjects where TransactionId = @transactionID"
                };

                cmd.Parameters.Add("@transactionID", SqlDbType.NVarChar).Value = transactionId;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    description = reader.IsDBNull(0) ? null : reader.GetString(0);
                }
                conn.Close();
            }
            return description;
        }

        public void DeleteObject(string transactionId)
        {
            using (var conn = new SqlConnection(GlobalItems.ConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand
                {
                    Connection = conn,
                    CommandText = @"delete from ImageObjects where TransactionId = @transactionId"
                };

                cmd.Parameters.Add("@transactionId", SqlDbType.NVarChar).Value = transactionId;
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public virtual List<GetTarget.DatabaseObject> GetDatabaseObjects()
        {
            var objList = new List<GetTarget.DatabaseObject>();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = GlobalItems.ConnectionString;
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = @"select Name, TransactionId from ImageObjects";

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string name = reader.GetString(0);
                    string transactionId = reader.IsDBNull(1) ? null : reader.GetString(1);
                    objList.Add(new GetTarget.DatabaseObject { Name = name, TransactionId = transactionId });
                }
                conn.Close();
            }
            return objList;
        }

        public static ObjectImageEntity GetObjectByName(string name)
        {
            var objList = new List<ObjectImageEntity>();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = GlobalItems.ConnectionString;
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = @"select Photo,Distance,Angle,Name,TransactionId,Description,Rating from ImageObjects where Name = @Name";
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    objList.Add(new ObjectImageEntity
                    {
                        Name = reader.GetString(3),
                        TransactionId = reader.GetString(4),
                        Angle = reader.GetInt32(2),
                        Description = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Distance = reader.GetDecimal(1),
                        Photo = (byte[])reader.GetValue(0),
                        Rating = reader.GetInt32(6)
                    });
                }
                conn.Close();
            }
            return objList.FirstOrDefault();
        }

        public void UpdateObject(string name, string transactionId, int rating)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = GlobalItems.ConnectionString;
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = @"update ImageObjects set TransactionId = @TransactionId, Rating = @Rating where Name = @Name";

                cmd.Parameters.Add("@TransactionId", SqlDbType.NVarChar).Value = transactionId;
                cmd.Parameters.Add("@Rating", SqlDbType.Int).Value = rating;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;

                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }

        public static void UpdateObjectDescription(string name, string description)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = GlobalItems.ConnectionString;
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = @"update ImageObjects set Description = @Description where Name = @Name";

                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;

                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }
    }
}
