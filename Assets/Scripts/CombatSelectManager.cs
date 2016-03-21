using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CombatSelectManager : MonoBehaviour
{
    public void GoCombat()
    {
        SceneManager.LoadScene("Combat");
    }
}
