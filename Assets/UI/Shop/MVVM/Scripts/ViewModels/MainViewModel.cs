using System;
using Unity.AppUI.MVVM;


namespace Unity.UI.Shop
{
    public delegate void VoidCallback();
    
    public class Observable : ObservableObject
    {
        // ReSharper disable once EntityNameCapturedOnly.Global
        public void AddPropertyListener(object property, VoidCallback callback)
        {
            PropertyChanged += ((o, args) =>
            {
                if (args.PropertyName == nameof(property))
                {
                    callback.Invoke();
                }
            });
        }
    }
    
    
    public class ShopViewModel : Observable
    {
        int m_Counter;
        
        public int counter
        {
            get => m_Counter;
            set => SetProperty(ref m_Counter, value);
        }

        public ShopViewModel()
        {
            counter = 0;
        }

        public void IncrementCounter()
        {
            counter++;
        }
    }
}
