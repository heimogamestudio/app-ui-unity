using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Data source for a health bar UI component. Supports runtime data binding.
/// </summary>
public class HealthData : INotifyBindablePropertyChanged
{

    // Backing fields
    int m_CurrentHealth;
    int m_MaximumHealth;

    /// <summary>
    /// Notifies when a bindable property changes.
    /// </summary>
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
    
    /// <summary>
    /// The entity's current health points.
    /// </summary>
    [CreateProperty]
    public int CurrentHealth
    {
        get => m_CurrentHealth;
        set
        {
            if (m_CurrentHealth != value)
            {
                m_CurrentHealth = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(HealthStatText));
                NotifyPropertyChanged(nameof(HealthPercentage));
                NotifyPropertyChanged(nameof(HealthProgressStyleLength));
            }
        }
    }

    /// <summary>
    /// The maximum possible health points.
    /// </summary>
    [CreateProperty]
    public int MaximumHealth
    {
        get => m_MaximumHealth;
        set
        {
            if (m_MaximumHealth != value)
            {
                m_MaximumHealth = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(HealthStatText));
                NotifyPropertyChanged(nameof(HealthPercentage));
                NotifyPropertyChanged(nameof(HealthProgressStyleLength));
            }
        }
    }

    /// <summary>
    /// Formatted string showing "CurrentHealth/MaximumHealth".
    /// </summary>
    [CreateProperty]
    public string HealthStatText => $"{m_CurrentHealth}/{m_MaximumHealth}";
    
    /// <summary>
    /// Current health as a percentage (0-100) of maximum health.
    /// </summary>
    [CreateProperty]
    public float HealthPercentage => m_MaximumHealth > 0 ? Mathf.Clamp((float)m_CurrentHealth / m_MaximumHealth * 100f, 0f, 100f) : 0f;
    
    /// <summary>
    /// StyleLength for health bar width based on current health percentage.
    /// </summary>
    [CreateProperty]
    public StyleLength HealthProgressStyleLength => new StyleLength(Length.Percent(HealthPercentage));
    
   
    /// <summary>
    /// Triggers notifications for data binding updates.
    /// </summary>
    /// <param name="propertyName">The name of the property that has changed.</param>
    void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(propertyName));
    }

}