using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Wrappers
{
    public interface IMobileMessageWrapper
    {
        void ShowMessage(string title, string message);
    }
}
