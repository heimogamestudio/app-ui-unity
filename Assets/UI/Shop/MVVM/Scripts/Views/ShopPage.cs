using UnityEngine.UIElements;
using Unity.UIWidgets;

namespace Unity.UI.Shop
{
    public class ShopView : VisualElement
    {
        private Scope Scope;   
        
        public ShopView()
        {
            Scope = new Scope(this, MyAppBuilder.BuildOwner);
            Scope.Run(new CounterPage());
        }
    }
    
    public class Scope : RootVisualRenderObjectWidget
    {
        private BuildOwner Owner;

        public static Element Element;
        
        public void Run(Widget widget)
        {
           var element = widget.createElement();
           var root = createElement() as RootRenderObjectElement;
           root.mount();
           Element = root;
           root?.AssignOwner(Owner);
           element.mount(root, "Root");
        }
     
        public Scope(VisualElement visualElement, BuildOwner owner) : base(visualElement)
        {
            Owner = owner;
        }
    }
}
