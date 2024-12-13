#nullable enable

using UnityEngine.UIElements;
using Unity.UIWidgets;
using Unity.UIWidgets.foundation;

namespace Unity.UI.Shop
{
    public class ShopView : VisualElement
    {
        private Scope Scope;   
        
        public ShopView()
        {
            Scope = new Scope(this);
            Scope.Run(new CounterPage());
        }
    }
    
    public class Scope : RootVisualRenderObjectWidget
    {
        private BuildOwner BuildOwner = new BuildOwner();

        public void Run(Widget widget)
        {
           var element = widget.createElement();
           var root = createElement() as RootRenderObjectElement;
           
           root.mount();
           root?.AssignOwner(BuildOwner);
           element.mount(root, "Root");
        }
     

        public Scope(VisualElement visualElement) : base(visualElement)
        {
            
        }
    }
    
    public class ShopPage : StatelessWidget
    {
        public override Widget build(BuildContext context)
        {
            return new UIText("Hello Good?");
        }
    }
    
    public class CounterPage: StatefulWidget
    {
        public override State createState()
        {
            return new CounterPageState();
        }
    }

    class CounterPageState : State
    {
        public ValueNotifier<int> Counter = new ValueNotifier<int>(0);
        
        public override void initState()
        {
            Counter.addListener(() =>
            {
                setState();
            });
            
            base.initState();
        }

        public override Widget build(BuildContext context)
        {
            return new UIText("Hello Good!" + Counter.value);
        }
    }
}
