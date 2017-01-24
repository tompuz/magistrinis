using System;
using System.Collections;

namespace Assets.Scripts.VuforiaApi
{
    public interface IUploadTarget
    {
        IEnumerator PostNewTarget(string targetName, byte[] image, Action successCallback);
    }
}
