using UnityEngine;
using System.Collections;
using UniRx;

public class HealthBar : MonoBehaviour
{
    public Transform healthBar;

    public void ObserveOnCharacter(Character character)
    {
        character.Health
                 .Subscribe(x => UpdateHealthBar(x, character.maxHealth))
                 .AddTo(gameObject);
    }

    void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        var percentage = (float)currentHealth / maxHealth;
        if (percentage < 0) percentage = 0;

        healthBar.localScale = new Vector3(percentage, healthBar.localScale.y, healthBar.localScale.z);
    }
	
    public void Toggle()
    {

    }
}
