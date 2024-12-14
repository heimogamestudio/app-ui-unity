using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.UIWidgets;
using Unity.UIWidgets.foundation;
using Column = Unity.UIWidgets.Column;

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
    
    public class CounterNotifier : ValueNotifier<int> {

        public void increment()
        {
            value++;
            notifyListeners();
        }

        public CounterNotifier(int value) : base(value)
        {
        }
    }
    
    class CounterPageState : State
    {
        public CounterNotifier Counter = new CounterNotifier(0);
        
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
            return new Row(
                children: new List<Widget>()
                {
                    new UIText("Hello Good!" + Counter.value),
                    new UIButton("Button!", () =>
                    {
                        Counter.increment();
                    }),
                    
                    new Row(
                        children: new List<Widget>()
                        {
                            new UIText("Hello Good!" + Counter.value),
                            new UIButton("Button!", () =>
                            {
                                Counter.increment();
                            }),
                   
                        }
                    ),
                    
                    new Row(
                        children: new List<Widget>()
                        {
                            new UIText("Hello Good!" + Counter.value),
                            new UIButton("Button!", () =>
                            {
                                Counter.increment();
                            }),
                   
                        }
                    ),
                    
                    new Column(
                        children: new List<Widget>()
                        {
                            new Row(
                                children: new List<Widget>()
                                {
                                    new UIText("Hello Good!" + Counter.value),
                                    new UIButton("Button!", () =>
                                    {
                                        Counter.increment();
                                    }),
                   
                                }
                            ),
                    
                            new Row(
                                children: new List<Widget>()
                                {
                                    new UIText("Hello Good!" + Counter.value),
                                    new UIButton("Button!", () =>
                                    {
                                        Counter.increment();
                                    }),
                   
                                }
                            ),
                            
                            new Row(
                                children: new List<Widget>()
                                {
                                    new UIText("Hello Good!" + Counter.value),
                                    new UIButton("Button!", () =>
                                    {
                                        Counter.increment();
                                    }),
                   
                                }
                            ),
                    
                            new Row(
                                children: new List<Widget>()
                                {
                                    new UIText("Hello Good!" + Counter.value),
                                    new UIButton("Button!", () =>
                                    {
                                        Counter.increment();
                                    }),
                   
                                }
                            ),
                            
                            new Row(
                                children: new List<Widget>()
                                {
                                    new UIText("Hello Good!" + Counter.value),
                                    new UIButton("Button!", () =>
                                    {
                                        Counter.increment();
                                    }),
                   
                                }
                            ),
                            
                            new Row(
                                children: new List<Widget>()
                                {
                                    new UIText("Hello Good!" + Counter.value),
                                    new UIButton("Button!", () =>
                                    {
                                        Counter.increment();
                                    }),
                   
                                }
                            ),
                            
                            new Row(
                                children: new List<Widget>()
                                {
                                    new UIText("Hello Good!" + Counter.value),
                                    new UIButton("Button!", () =>
                                    {
                                        Counter.increment();
                                    }),
                   
                                }
                            ),
                            new Column(
                                children: new List<Widget>()
                                {
                                    new UIText("Hello Good!" + Counter.value),
                                    new UIButton("Button!", () =>
                                    {
                                        Counter.increment();
                                    }),
                                }
                            ),
                        }
                    ),
                }
            );
        }
    }
}
