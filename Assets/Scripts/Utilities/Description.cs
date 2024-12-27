using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
    /// <summary>
    /// This is a simple component for holding notes or explanation.
    /// </summary>
    public class Description : MonoBehaviour
    {
        [TextArea(10, 30)]
        public string note;
    }
}
