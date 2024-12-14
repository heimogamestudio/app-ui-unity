using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.UIWidgets;
using Unity.UIWidgets.foundation;
using UnityEngine;

namespace Unity.UI.Shop
{
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
    
    public class CounterPage: StatefulWidget
    {
        public override State createState()
        {
            return new CounterPageState();
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
                    new Row(
                        children: new List<Widget>()
                        {
                            new UIText("Hello Good!" + Counter.value),
                            new UIButton("Button!", () =>
                            {
                                Counter.increment();
                            }),
                            
                            new VisualWidget(
                                new UIStyle
                                {
                                    BackgroundColor = new StyleColor(Color.cyan),
                                    BorderBottomColor = new StyleColor(Color.green),
                                    BorderBottomWidth = new StyleFloat(3f)
                                },
                               new Row(
                                   children: new List<Widget>()
                                   {
                                       new UIText("Hello Good!" + Counter.value),
                                       new UIText("Hello Good!" + Counter.value),
                                       new UIText("Hello Good!" + Counter.value)
                                   }
                                )
                            ),
                            
                            new VisualWidget(
                                new UIStyle
                                {
                                    BackgroundColor = new StyleColor(Color.cyan),
                                    BorderBottomColor = new StyleColor(Color.green),
                                    BorderBottomWidth = new StyleFloat(3f),
                                    BackgroundImage = new StyleBackground()
                                },
                                new Row(
                                    children: new List<Widget>()
                                    {
                                        new UIText("Hello Good!" + Counter.value),
                                        new UIText("Hello Good!" + Counter.value),
                                        new UIText("Hello Good!" + Counter.value)
                                    }
                                )
                            )
                        }
                    ),
                }
            );
        }
    }
}
