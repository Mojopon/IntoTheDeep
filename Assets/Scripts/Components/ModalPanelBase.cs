using UnityEngine;
using System.Collections;

public abstract class ModalPanelBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;
                if (!instance)
                    Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
            }

            return instance;
        }
    }

    protected bool panelClosed = false;
    protected virtual void ClosePanel()
    {
        panelClosed = true;
    }
}