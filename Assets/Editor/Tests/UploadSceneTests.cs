using System;
using System.Linq;
using Assets.Scripts.Buttons.Upload;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Enums;
using Assets.Scripts.Validators;
using Assets.Scripts.VuforiaApi;
using Assets.Scripts.Wrappers;
using Castle.Core.Internal;
using NSubstitute;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Text = UnityEngine.UI.Text;

namespace Assets.Editor.Tests
{
    public class UploadSceneTests
    {
        private IMobileMessageWrapper _mobileMessage;
        private IObjectsManager _objManager;
        private IUploadTarget _targetManager;

        [SetUp]
        public void Initialize()
        {
            EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/" + CreatedScene.UploadRecomendations + ".unity", OpenSceneMode.Single);
            _objManager = Substitute.For<IObjectsManager>();
            //_scene = Object.FindObjectOfType<PreviewScene>();
            //_objManager = Substitute.For<IObjectsManager>();
            _mobileMessage = Substitute.For<IMobileMessageWrapper>();
            _targetManager = Substitute.For<IUploadTarget>();
            var submitScript = Object.FindObjectOfType<SubmitButton>();

            submitScript.Start();
            submitScript.MobileMessage = _mobileMessage;
            submitScript.ObjectsManager = _objManager;
            submitScript.TargetManager = _targetManager;
        }

        [Test]
        public void TestValidationInputsNotFilled()
        {
            var submitButton = Object.FindObjectsOfType<Button>().FirstOrDefault(b => b.name == "Submit");
            if (submitButton != null) submitButton.onClick.Invoke();

            AssertMessageCalled(false);
        }

        [Test]
        public void TestValidationInputsNotFilledDescription()
        {
            Object.FindObjectsOfType<InputField>()
                .Where(i => i.name == "InputDistance" || i.name == "InputAngle")
                .ForEach(i => i.text = "10");
            var submitButton = Object.FindObjectsOfType<Button>().FirstOrDefault(b => b.name == "Submit");
            if (submitButton != null) submitButton.onClick.Invoke();

            var distance =
                        decimal.Parse(
                            GameObject.Find("InputDistance")
                                .GetComponentsInChildren<Text>()
                                .First(t => t.name == "Text")
                                .text);
            var angle =
                int.Parse(
                    GameObject.Find("InputAngle")
                        .GetComponentsInChildren<Text>()
                        .First(t => t.name == "Text")
                        .text);
            Assert.AreEqual(10,angle);
            Assert.AreEqual(10, distance);
            AssertMessageCalled(false);
        }

        [Test]
        public void TestValidator()
        {
            //Initialize validators
            Object.FindObjectsOfType<InputFieldValidator>().ForEach(v => v.Start());

            Object.FindObjectsOfType<InputField>()
                .Where(i => i.name == "InputField")
                .ForEach(i => i.text = "testas");
            Object.FindObjectsOfType<InputField>()
                .Where(i => i.name == "InputDistance" || i.name == "InputAngle")
                .ForEach(i => i.text = "");
            var descriptionValid = Object.FindObjectsOfType<InputField>().First(i => i.name == "InputField").GetComponent<InputFieldValidator>().Valid;
            var distanceValid = Object.FindObjectsOfType<InputField>().First(i => i.name == "InputDistance").GetComponent<InputFieldValidator>().Valid;
            var angleValid = Object.FindObjectsOfType<InputField>().First(i => i.name == "InputAngle").GetComponent<InputFieldValidator>().Valid;
            
            Assert.AreEqual(true, descriptionValid);
            Assert.AreEqual(false, distanceValid);
            Assert.AreEqual(false, angleValid);
        }

        [Test]
        public void TestValidationInputsValid()
        {
            Scripts.GlobalItems.PictureBytes = System.Text.Encoding.ASCII.GetBytes("test");
            Object.FindObjectsOfType<InputFieldValidator>().ForEach(v => v.Start());
            Object.FindObjectsOfType<InputField>()
                .Where(i => i.name == "InputDistance" || i.name == "InputAngle")
                .ForEach(i => i.text = "10");
            Object.FindObjectsOfType<InputField>()
                .Where(i => i.name == "InputField")
                .ForEach(i => i.text = "Testas");
            var submitButton = Object.FindObjectsOfType<Button>().FirstOrDefault(b => b.name == "Submit");
            if (submitButton != null) submitButton.onClick.Invoke();
            AssertMessageCalled(true);
            _objManager.Received(1).InsertObject(10, 10, "Testas");
            //_targetManager.Received(1).PostNewTarget("Testas", Scripts.GlobalItems.PictureBytes);

        }

        [Test]
        public void TestValidationInputsValidNoImage()
        {
            Scripts.GlobalItems.PictureBytes = null;
            Object.FindObjectsOfType<InputFieldValidator>().ForEach(v => v.Start());
            Object.FindObjectsOfType<InputField>()
                .Where(i => i.name == "InputDistance" || i.name == "InputAngle")
                .ForEach(i => i.text = "10");
            Object.FindObjectsOfType<InputField>()
                .Where(i => i.name == "InputField")
                .ForEach(i => i.text = "Testas");
            var submitButton = Object.FindObjectsOfType<Button>().FirstOrDefault(b => b.name == "Submit");
            if (submitButton != null) submitButton.onClick.Invoke();
            _mobileMessage.Received(1).ShowMessage("Klaida", "Trūksta įkeltos nuotraukоs!");
            _mobileMessage.Received(0).ShowMessage("Klaida", "Neužpildyti visi privalomi laukai");
            _mobileMessage.Received(0).ShowMessage("AR", "Objektas sukurtas!");
            _objManager.Received(0).InsertObject(10, 10, "Testas");
            //_targetManager.Received(0).PostNewTarget("Testas", null);

        }

        [Test]
        public void TestValidationThrowExceptionAndShowMessage()
        {
            Scripts.GlobalItems.PictureBytes = System.Text.Encoding.ASCII.GetBytes("test");
            Object.FindObjectsOfType<InputFieldValidator>().ForEach(v => v.Start());
            _objManager.When(x => x.InsertObject(10, 10, "Testas")).Do(x => { throw new Exception("Test db exception"); });
            Object.FindObjectsOfType<InputField>()
                .Where(i => i.name == "InputDistance" || i.name == "InputAngle")
                .ForEach(i => i.text = "10");
            Object.FindObjectsOfType<InputField>()
                .Where(i => i.name == "InputField")
                .ForEach(i => i.text = "Testas");
            var submitButton = Object.FindObjectsOfType<Button>().FirstOrDefault(b => b.name == "Submit");
            if (submitButton != null) submitButton.onClick.Invoke();
            
            //_targetManager.Received(0).PostNewTarget("Testas", null);
            _mobileMessage.Received(0).ShowMessage("Klaida", "Neužpildyti visi privalomi laukai");
            _mobileMessage.Received(0).ShowMessage("AR", "Objektas sukurtas!");
            _mobileMessage.Received(1).ShowMessage("Klaida", "Test db exception");
        }

        private void AssertMessageCalled(bool success)
        {
            if (success)
            {
                _mobileMessage.Received(0).ShowMessage("Klaida", "Neužpildyti visi privalomi laukai");
                _mobileMessage.Received(1).ShowMessage("AR", "Objektas sukurtas!");
            }
            else
            {
                _mobileMessage.Received(1).ShowMessage("Klaida", "Neužpildyti visi privalomi laukai");
                _mobileMessage.Received(0).ShowMessage("AR", "Objektas sukurtas!");
            }
        }
    }
}
