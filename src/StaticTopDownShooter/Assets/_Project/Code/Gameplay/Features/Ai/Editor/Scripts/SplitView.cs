using UnityEngine.UIElements;

namespace NoOpArmy.WiseFeline
{
    public class SplitView : TwoPaneSplitView
    {
#pragma warning disable CS0618 // Type or member is obsolete
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }
#pragma warning restore CS0618 // Type or member is obsolete

        public SplitView()
        {

        }
    }
}
