using System.Collections.Generic;
using Assets.Scripts.VuforiaApi;

namespace Assets.Scripts.DataAccess
{
    public interface IObjectsManager
    {
        List<ObjectImageEntity> GetActiveObjects();
        List<GetTarget.DatabaseObject> GetDatabaseObjects();
        void UpdateObject(string name, string transactionId, int rating);
        void InsertObject(int angle, decimal distance, string decription);

        string GetObjectDesription(string transactionId);

        void DeleteObject(string transactionId);
    } 
}
