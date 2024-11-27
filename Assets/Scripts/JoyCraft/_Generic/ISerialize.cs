using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoyCraft {
    public interface ISerialize
    {
        public object[] Serialize();
        public void Deserialize(object[] data);
    }   
}
