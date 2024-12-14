using Unity.AppUI.MVVM;
using Unity.UIWidgets;
using UnityEngine;

namespace Unity.UI.Shop
{
    public class MyAppBuilder : UIToolkitAppBuilder<MyApp>
    {
        public static BuildOwner BuildOwner = new BuildOwner();

        [SerializeField]
        private Assets _Assets;
        public static Assets Assets;
        
        protected override void OnAppInitialized(MyApp app)
        {
            
            base.OnAppInitialized(app);
            Debug.Log("MyAppBuilder.OnAppInitialized");
        }

        private void Update()
        {
            BuildOwner.buildScope(Scope.Element, (() => {})); 
        }

        protected override void OnConfiguringApp(AppBuilder builder)
        {
            base.OnConfiguringApp(builder);
            Debug.Log("MyAppBuilder.OnConfiguringApp");

            Assets = _Assets;
            
            // Add services here
            // Add ViewModels and Views as services
            builder.services.AddSingleton<ShopView>();
            builder.services.AddSingleton<ShopViewModel>();
        }

        protected override void OnAppShuttingDown(MyApp app)
        {
            base.OnAppShuttingDown(app);
            Debug.Log("MyAppBuilder.OnAppShuttingDown");
        }
    }
}
