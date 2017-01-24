using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class DontDestroy : MonoBehaviour
    {
        protected void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
