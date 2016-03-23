using UnityEngine;
using System.Collections;

[System.Serializable]
public class SerializableCharacterData : ICharacterData
{
    public string name { get; set; }
    public int level { get; set; }
    public int expToNextLevel { get; set; }
    public int jobID { get; set; }

    public SerializableCharacterData() { }
}
