using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Save
{

    [ExecuteInEditMode]
    public class Guid : MonoBehaviour
    {
        public string guid;
        private void Awake()
        {
            if (guid == string.Empty)
            {
                guid = System.Guid.NewGuid().ToString();
            }
        }
    }
}