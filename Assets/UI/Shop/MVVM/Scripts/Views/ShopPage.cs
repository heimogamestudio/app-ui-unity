#nullable enable

using System.Collections.Generic;
using Codice.Client.Common.Connection;
using Unity.AppUI.UI;
using UnityEngine.UIElements;
using Button = Unity.AppUI.UI.Button;

public class BuildContext
{
    
}

public abstract class Element
{
    public Widget Widget;
    
    public Element? _Child;
    
    public Element? _Parent;
    
    private bool _Dirty = false;
    
    //owner!.scheduleBuildFor(this);
    

    protected Element(Widget widget)
    {
        Widget = widget;
    }
    
    //protected S Render;
    
    //_element!.markNeedsBuild();

    public void MarkNeedsBuild()
    {
        _Dirty = true;
    }
    
    protected abstract Widget Build();
    
    protected abstract VisualElement Create();

    protected abstract void Update(Widget widget);
}

public abstract class ComponentElement : Element
{
    protected override VisualElement Create()
    {
        return new VisualElement();
    }

    protected override void Update(Widget widget)
    {
        
    }

    public ComponentElement(Widget widget) : base(widget)
    {
        
    }
}

public abstract class RenderObjectElement : Element
{
    protected RenderObjectElement(Widget widget) : base(widget) { }
}

public class LeafRenderObjectElement<T>  : RenderObjectElement where T : VisualElement
{
    public LeafRenderObjectElement(RenderObjectWidget<T> widget) : base(widget) { }
    protected override Widget Build()
    {
        throw new System.NotImplementedException();
    }

    protected override VisualElement Create()
    {
        throw new System.NotImplementedException();
    }

    protected override void Update(Widget widget)
    {
        throw new System.NotImplementedException();
    }
}

public abstract class RenderObjectWidget<T> : Widget where T : VisualElement {
    protected RenderObjectWidget(object key = null) : base(key) { }

    public override Element CreateElement()
    {
        return CreateRenderObjectElement();
    }

    public abstract RenderObjectElement CreateRenderObjectElement();

    public abstract T CreateRenderObject(BuildContext context);

    public virtual void UpdateRenderObject(BuildContext context, T renderObject) { }

    public virtual void DidUnmountRenderObject(T renderObject) { }
}

public abstract class LeafRenderObjectWidget<T> : RenderObjectWidget<T> where T : VisualElement
{
    protected LeafRenderObjectWidget(object key = null) : base(key) { }

    public override RenderObjectElement CreateRenderObjectElement()
    {
        return new LeafRenderObjectElement<T>(this);
    }
}

public class UIText : LeafRenderObjectWidget<Text>
{
    public string Text;

    public override Text CreateRenderObject(BuildContext context)
    {
        return new Text
        {
            text = Text
        };
    }

    public override void UpdateRenderObject(BuildContext context, Text renderObject)
    {
        renderObject.text = Text;
    }
}

public abstract class Widget : VisualElement
{
    public object Key { get; }
    
    protected Widget(object key)
    {
        Key = key;
    }

    protected Widget()
    {
        Key = "";
    }

    public abstract Element CreateElement();
}

public abstract class StatelessWidget : Widget
{
    public override Element CreateElement() => new StatelessElement(this);
  
    public abstract Widget Build(BuildContext context);
}

public abstract class StatefulWidget : Widget
{
    public override Element CreateElement() => new StatefulElement(this);

    public abstract State CreateState(BuildContext context);
}

public abstract class State
{
    public StatefulWidget Widget;
    
    StatefulElement? _element;

    public abstract Widget build(BuildContext context);
}

public class StatefulElement : ComponentElement
{
    private BuildContext Context = new BuildContext();
    
    public StatefulElement(StatefulWidget widget) : base(widget)
    {
    }

    protected override Widget Build()
    {
        return (Widget as StatelessWidget)!.Build(Context);
    }

    protected override void Update(Widget widget)
    {
        throw new System.NotImplementedException();
    }
}



public class StatelessElement : ComponentElement
{
    private BuildContext Context = new BuildContext();
    
    public StatelessElement(StatelessWidget widget) : base(widget)
    {
    }

    protected override Widget Build()
    {
        return (Widget as StatelessWidget)!.Build(Context);
    }

    protected override void Update(Widget widget)
    {
        throw new System.NotImplementedException();
    }
}




public class CounterWidget : StatefulWidget
{
    public override State CreateState(BuildContext context)
    {
        return new CounterState();
    }
}

public class CounterState : State
{
    public override Widget build(BuildContext context)
    {
        return new UIText
        {
            Text = "My Text Component"
        };
    }
}


namespace Unity.UI.Shop
{
    public class ShopPage : StatelessWidget
    {
        private Dictionary<object, Widget> Widgets;
        private Dictionary<Widget, Element> Elements;
        private Dictionary<Element, VisualElement> Renders;
        
        public void Init()
        {
            Widgets = new Dictionary<object, Widget>();
            Elements = new Dictionary<Widget, Element>();
            Renders = new Dictionary<Element, VisualElement>();

            
            var widget = Build(new BuildContext());
            RenderWidget(widget);
        }
        
        public void RenderWidget(Widget widget)
        {
            var context = new BuildContext();
            
            
            

            if (Elements.TryGetValue(widget, out Element elementRef))
            {
                if (elementRef is RenderObjectElement)
                {
                    if (widget is RenderObjectWidget<VisualElement> renderWidget)
                    {
                        VisualElement visualElement;
                        Renders.TryGetValue(elementRef, out visualElement);
                        renderWidget.UpdateRenderObject(context, visualElement); 
                    }
                }
            }
            else
            {
                Widgets.Add(widget.Key, widget);
                var element = widget.CreateElement();
                Elements.Add(widget, element);

                if (elementRef is RenderObjectElement)
                {
                    if (widget is RenderObjectWidget<VisualElement> renderWidget)
                    {
                        var visualElement = renderWidget.CreateRenderObject(context); 
                        Renders.Add(elementRef, visualElement);
                        
                        Add(visualElement);
                    }
                }
            }
            
            
            
            if (Elements.TryGetValue(widget, out Element elementRef))
            {
                if (elementRef is StatefulElement)
                {
                    if (widget is RenderObjectWidget<VisualElement> renderWidget)
                    {
                        VisualElement visualElement;
                        Renders.TryGetValue(elementRef, out visualElement);
                        renderWidget.UpdateRenderObject(context, visualElement); 
                    }
                }
            }
            else
            {
                Widgets.Add(widget.Key, widget);
                var element = widget.CreateElement();
                Elements.Add(widget, element);

                if (elementRef is RenderObjectElement)
                {
                    if (widget is RenderObjectWidget<VisualElement> renderWidget)
                    {
                        var visualElement = renderWidget.CreateRenderObject(context); 
                        Renders.Add(elementRef, visualElement);
                        
                        Add(visualElement);
                    }
                }
            }
            
            
            /*
            
            if (widget.CreateElement() render)
            {
                render.CreateRenderObject(context);
            }*/
            
            //Add(widget.CreateElement());
            //Add(m_Button);
        }
        
        public override Widget Build(BuildContext context)
        {
            return new UIText
            {
                Text = "My Component"
            };
        }
        
        readonly Text m_Text;
        readonly Button m_Button;
        readonly ShopViewModel ViewModel;
        
        public ShopPage(ShopViewModel viewModel)
        {
            Init();
        }

        void OnButtonClicked()
        {
            ViewModel.IncrementCounter();
        }
    }
}
