using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.VuforiaApi
{
    public interface IDeleteTarget
    {
        IEnumerator DeleteSingleTarget(string transactionId, Action<GetTarget.ResultTarget> successCallback);
    }
}
