using UnityEngine;
using System.Collections;

public interface IWorldUtilitiesProvider
{
    void ProvideWorldUtilities(IWorldUtilitiesUser user);
}
