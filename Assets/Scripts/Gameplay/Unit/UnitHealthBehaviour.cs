using UnityEngine;
using UnityEngine.Events;
using Utilities.Inspector;
using System;

public class UnitHealthBehaviour : MonoBehaviour
{

    [Header("Events")]
    
    [Tooltip("Triggered when health changes, passing the difference value")]
    public UnityEvent<int> healthDifferenceEvent;
    
    [Tooltip("Triggered when health reaches zero")]
    public UnityEvent healthIsZeroEvent;
    
    /// <summary>
    /// Notifies listeners of current health value changes.
    /// </summary>
    public Action<int> HealthChanged;

    HealthData m_HealthData;
    
    /// <summary>
    /// Gets the HealthData instance managing this unit's health.
    /// </summary>
    public HealthData HealthData => m_HealthData;
    
    /// <summary>
    /// Initializes the health system with provided HealthData.
    /// </summary>
    public void SetupHealth(HealthData healthData)
    {
        if (healthData == null)
        {
            Debug.LogError($"Null HealthData provided to {gameObject.name}");
            return;
        }

        m_HealthData = healthData;
    }
    /// <summary>
    /// Modifies the unit's health by the specified amount.
    /// </summary>
    /// <param name="healthDifference">Amount to change health by (positive or negative)</param>
    public void ChangeHealth(int healthDifference)
    {
        if (m_HealthData == null)
        {
            Debug.LogError($"HealthData not set in {gameObject.name}");
            return;
        }

        int newHealth = Mathf.Clamp(
            m_HealthData.CurrentHealth + healthDifference, 
            0, 
            m_HealthData.MaximumHealth
        );

        m_HealthData.CurrentHealth = newHealth;
        healthDifferenceEvent.Invoke(healthDifference);

        if (newHealth <= 0)
        {
            healthIsZeroEvent.Invoke();
        }

        HealthChanged?.Invoke(newHealth);
    }

    /// <summary>
    /// Gets the current health value.
    /// </summary>
    public int GetCurrentHealth() => m_HealthData?.CurrentHealth ?? 0;



}
