namespace Assets.Scripts.Wrappers
{
    public class MobileMessageWrapper : IMobileMessageWrapper
    {
        public void ShowMessage(string title, string message)
        {
            AndroidMessage.Create(title, message);
            //new MobileNativeMessage(title, message);
        }
    }
}
