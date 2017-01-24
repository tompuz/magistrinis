using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.VuforiaApi
{
    public interface IGetTarget
    {
        IEnumerator GetSingleTarget(string transactionId, Action<GetTarget.TargetRecord> successCallback);
        IEnumerator GetAllTargets(Action<object> successCallback);
    }
}
