using UnityEngine;
using Unity.AppUI.MVVM;
using Unity.AppUI.UI;

namespace Unity.UI.Shop
{
    public class MyApp : App
    {
        public MyApp(ShopPage mainPage)
        {
            var panel = new Panel();
            //panel.Add(mainPage);
            this.mainPage = panel;
        }

        public override void Shutdown()
        {
            base.Shutdown();
            Debug.Log("MyApp.Shutdown");
        }
    }
}
