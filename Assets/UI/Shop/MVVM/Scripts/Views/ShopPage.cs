#nullable enable

using UnityEngine.UIElements;

using Unity.UIWidgets;

/*
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
*/


public class UIText : SingleChildRenderObjectWidget
{
    private string Text;

    public UIText(string text)
    {
        this.Text = text;
    }


    public override RenderObject createRenderObject(BuildContext context) {
      
        return new TextRenderObject(
            Text
        );
    }


    public override void updateRenderObject(BuildContext context, RenderObject renderObject) {
        renderObject = renderObject as TextRenderObject;

        if (renderObject is TextRenderObject textRenderObject)
        {
            textRenderObject.Text = Text;
        }
    }
}


namespace Unity.UI.Shop
{

    [UxmlElement]
    public sealed partial class ShopView : VisualElement
    {
        private Scope Scope;   
        
        public ShopView()
        {
            Scope = new Scope(this);
            Scope.Run(new ShopPage());
        }
    }
    
    public class Scope : RootVisualRenderObjectWidget
    {
        private BuildOwner BuildOwner = new BuildOwner();
        
        
        public void Run(Widget widget)
        {
           var element = widget.createElement();
           var root = createElement() as RootRenderObjectElement;
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
            return new UIText("Hello");
        }
    }
}
