using Emilia.Flow.Attributes;
using Emilia.Kit.Editor;
using Emilia.Node.Editor;
using Sirenix.Utilities;

namespace Emilia.Flow.Editor
{
    public class FlowCreateNodeHandle : CreateNodeHandle<FlowNodeAsset>
    {
        protected object _nodeData;
        protected string _path;
        protected int _priority;

        public override object nodeData => this._nodeData;
        public override string path => _path;
        public override int priority => _priority;

        public override void Initialize(object weakSmartValue)
        {
            base.Initialize(weakSmartValue);

            FlowNodeMenuAttribute menuAttribute = this.value.nodeType.GetCustomAttribute<FlowNodeMenuAttribute>();
            if (menuAttribute != null)
            {
                this._path = menuAttribute.path;
                this._priority = menuAttribute.priority;
            }

            this._nodeData = ReflectUtility.CreateInstance(this.value.nodeType);
        }
    }
}