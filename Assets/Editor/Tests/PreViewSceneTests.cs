using System;
using System.Collections.Generic;
using Assets.Scripts.Buttons.Preview;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Enums;
using Assets.Scripts.Scenes;
using Assets.Scripts.VuforiaApi;
using Assets.Scripts.VuforiaApi.VuforiaData;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Editor.Tests
{
    public class PreviewSceneTests
    {
        private PreviewScene _scene;
        private IObjectsManager _objManager;
        private IGetTarget _getTarget;

        [SetUp]
        public void Initialize()
        {
            EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/" + CreatedScene.Preview + ".unity");
            _scene = Object.FindObjectOfType<PreviewScene>();
            _objManager = Substitute.For<IObjectsManager>();
            _getTarget = Substitute.For<IGetTarget>();
        }

        private List<ObjectImageEntity> TestObjectsList1()
        {
            var list = new List<ObjectImageEntity>();
            list.Add
            (
                new ObjectImageEntity
                {
                    Rating = 3,
                    TransactionId = "222222222",
                    Description = "test1",
                    Angle = 15,
                    Distance = 1,
                    Name = "test1Name"
                }
            );
            list.Add
            (
                new ObjectImageEntity
                {
                    Rating = 1,
                    TransactionId = "111111111",
                    Description = "test2",
                    Angle = 15,
                    Distance = 1,
                    Name = "test2Name"
                }
            );
            return list;
        }

        private List<GetTarget.DatabaseObject> DatabaseObjectsList()
        {
            var list = new List<GetTarget.DatabaseObject>();
            list.Add(new GetTarget.DatabaseObject
            {
                Name = "test2",
                TransactionId = "222222222"
            });
            list.Add(new GetTarget.DatabaseObject
            {
                Name = "test1",
                TransactionId = "111111111"
            });
            return list;
        }

        private List<GetTarget.DatabaseObject> DatabaseObjectsList1()
        {
            var list = new List<GetTarget.DatabaseObject>();
            list.Add(new GetTarget.DatabaseObject
            {
                Name = "test2",
                TransactionId = "222222222"
            });
            return list;
        }

        private void ResetObjects()
        {
            _scene.dropDownList.options.Clear();
            _scene.DescriptionField.text = string.Empty;
            _scene.ImageField.sprite = _scene.Sprites[0];
        }

        [Test]
        public void TestLoadFirstObjectToGui()
        {
            ResetObjects();
            //Prepare
            _objManager.GetActiveObjects().Returns(TestObjectsList1());
            _scene._objectsManager = _objManager;

            _scene.RefreshObjectsList();
            Assert.AreEqual(2, _scene.dropDownList.options.Count, "Unexpected returned options count");
            Assert.AreEqual("test1", _scene.DescriptionField.text, "Setted incorrect description value");
            Assert.AreEqual(_scene.Sprites[3], _scene.ImageField.sprite, "Invalid rating for sprite");
        }

        [Test]
        public void TestLoadedEmptyDatabase()
        {
            ResetObjects();
            //Prepare
            _objManager.GetActiveObjects().Returns(new List<ObjectImageEntity>());
            _scene._objectsManager = _objManager;

            _scene.RefreshObjectsList();
            Assert.AreEqual(0, _scene.dropDownList.options.Count, "Unexpected returned options count");
            Assert.AreEqual(string.Empty, _scene.DescriptionField.text, "Setted incorrect description value");
            Assert.AreEqual(_scene.Sprites[0],_scene.ImageField.sprite, "Invalid rating for sprite");
        }

        [Test]
        public void TestUpdateTargetsFromVuforiaNothingToUpdate()
        {
            ResetObjects();
            //Prepare
            _objManager.GetDatabaseObjects().Returns(DatabaseObjectsList());
            _objManager.GetActiveObjects().Returns(TestObjectsList1());
            _getTarget.GetSingleTarget("222222222", _scene.UpdateSingleTarget);
            _getTarget.GetSingleTarget("111111111", _scene.UpdateSingleTarget);
            _scene._objectsManager = _objManager;
            _scene.target = _getTarget;
            var targetsList = new List<string> {"222222222", "111111111"};
            _scene.UpdateTargets(targetsList);

            Assert.AreEqual(_scene.dropDownList.options.Count, 2, "Unexpected returned options count");
            Assert.AreEqual(_scene.DescriptionField.text, "test1", "Setted incorrect description value");
            Assert.AreEqual(_scene.ImageField.sprite, _scene.Sprites[3], "Invalid rating for sprite");
        }

        [Test]
        public void TestUpdateTargetsFromVuforiaUpdateSingleRecord()
        {
            ResetObjects();
            //Prepare
            _objManager.GetDatabaseObjects().Returns(DatabaseObjectsList1());
            _objManager.GetActiveObjects().Returns(TestObjectsList1());
            
            _scene._objectsManager = _objManager;
            _scene.target = _getTarget;
            var targetsList = new List<string> { "222222222", "111111111" };
            _scene.UpdateTargets(targetsList);

            _getTarget.Received(1).GetSingleTarget("111111111", _scene.UpdateSingleTarget);
            Assert.AreEqual(_scene.dropDownList.options.Count, 2, "Unexpected returned options count");
            Assert.AreEqual(_scene.DescriptionField.text, "test1", "Setted incorrect description value");
            Assert.AreEqual(_scene.ImageField.sprite, _scene.Sprites[3], "Invalid rating for sprite");
        }

        [Test]
        public void TestRefreshButton()
        {
            var refreshButtonScript = Object.FindObjectOfType<RefreshButton>();
            refreshButtonScript.Start();
            refreshButtonScript._target = _getTarget;
            refreshButtonScript._objectsManager = _objManager;

            //Prepare mock objects
            UnprocessedTransactions.TransactionIdList = new List<string>
            { "test1", "test2", "test3"};

            refreshButtonScript._btn.onClick.Invoke();
            _getTarget.Received(1).GetSingleTarget("test1", refreshButtonScript.UpdateSingleTarget);
            _getTarget.Received(1).GetSingleTarget("test2", refreshButtonScript.UpdateSingleTarget);
            _getTarget.Received(1).GetSingleTarget("test3", refreshButtonScript.UpdateSingleTarget);

        }

        [Test]
        public void TestUpdateSingleTargetActive()
        {
            _objManager.GetActiveObjects().Returns(TestObjectsList1());
            UnprocessedTransactions.TransactionIdList = new List<string>
            { "testId", "test2", "test3"};

            var refreshButtonScript = Object.FindObjectOfType<RefreshButton>();
            refreshButtonScript.Start();
            refreshButtonScript._target = _getTarget;
            refreshButtonScript._objectsManager = _objManager;
            refreshButtonScript.UpdateSingleTarget(new GetTarget.TargetRecord
            {
                target_id = "testId",
                name = "targetName",
                tracking_rating = 3,
                active_flag = true
            });
            _objManager.Received(1).UpdateObject("targetName", "testId", 3);
            Assert.AreEqual(2, UnprocessedTransactions.TransactionIdList.Count);
        }

        [Test]
        public void TestUpdateSingleTargetInactive()
        {
            _objManager.GetActiveObjects().Returns(TestObjectsList1());
            UnprocessedTransactions.TransactionIdList = new List<string>
            { "test1", "test2", "test3"};

            var refreshButtonScript = Object.FindObjectOfType<RefreshButton>();
            refreshButtonScript.Start();
            refreshButtonScript._target = _getTarget;
            refreshButtonScript._objectsManager = _objManager;
            refreshButtonScript.UpdateSingleTarget(new GetTarget.TargetRecord
            {
                target_id = "testId",
                name = "targetName",
                tracking_rating = 3,
                active_flag = false
            });
            _objManager.Received(0).UpdateObject("targetName", "testId", 3);
            _objManager.Received(0).GetActiveObjects();
            Assert.AreEqual(4, UnprocessedTransactions.TransactionIdList.Count);
        }
    }
}
