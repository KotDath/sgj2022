using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    // Start is called before the first frame update
    [Label("Çהמנמגüו"), SerializeField] float _health;
    [Label("Áאנ חהמנמגüÿ"), SerializeField] Image _healthBar;

    float _maxHealth;
    void Start()
    {
        _maxHealth = _health;
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            Destroy(this.gameObject);
        } else
        {
            Debug.Log(_health);
            _healthBar.fillAmount = _health / _maxHealth;
        }
    }

    public void Heal(float heal)
    {
        _health += heal;
        _health = Mathf.Min(_maxHealth, _health);
        _healthBar.fillAmount = _health / _maxHealth;
    }

    private void Update()
    {
        TakeDamage(Time.deltaTime);
        Debug.Log(_health);
    }
}
