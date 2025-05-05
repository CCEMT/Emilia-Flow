using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Emilia.Flow.Attributes;

namespace Emilia.Flow.Editor
{
    public static class FlowShowOrHideUtility
    {
        public static void FilterPort(IUniversalFlowNodeAsset flowNodeAsset, List<string> portIds)
        {
            Dictionary<string, FlowPortIf> flowPortIfs = new Dictionary<string, FlowPortIf>();

            Type nodeAssetType = flowNodeAsset.GetType();
            MemberInfo[] memberInfos = nodeAssetType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            int methodAmount = memberInfos.Length;
            for (int i = 0; i < methodAmount; i++)
            {
                MemberInfo memberInfo = memberInfos[i];

                List<FlowPortShowIfAttribute> showIfAttributes = memberInfo.GetCustomAttributes<FlowPortShowIfAttribute>().ToList();

                showIfAttributes.ForEach((attribute) => {
                    if (flowPortIfs.ContainsKey(attribute.portId)) return;
                    FlowPortIf flowPortIf = new FlowPortIf();
                    flowPortIf.memberInfo = memberInfo;
                    flowPortIf.showIfAttribute = attribute;
                    flowPortIfs.Add(attribute.portId, flowPortIf);
                });

                List<FlowPortHideIfAttribute> hideIfAttributes = memberInfo.GetCustomAttributes<FlowPortHideIfAttribute>().ToList();

                hideIfAttributes.ForEach((attribute) => {
                    if (flowPortIfs.ContainsKey(attribute.portId)) return;
                    FlowPortIf flowPortIf = new FlowPortIf();
                    flowPortIf.memberInfo = memberInfo;
                    flowPortIf.hideIfAttribute = attribute;
                    flowPortIfs.Add(attribute.portId, flowPortIf);
                });
            }

            for (int i = 0; i < portIds.Count; i++)
            {
                string portId = portIds[i];
                if (flowPortIfs.TryGetValue(portId, out FlowPortIf flowPortIf) == false) continue;
                if (flowPortIf.showIfAttribute != null)
                {
                    bool? isShow = If(flowNodeAsset, flowPortIf.memberInfo, flowPortIf.showIfAttribute.value);
                    if (isShow == true) continue;

                    portIds.RemoveAt(i);
                    i--;
                }
                else if (flowPortIf.hideIfAttribute != null)
                {
                    bool? isHide = If(flowNodeAsset, flowPortIf.memberInfo, flowPortIf.hideIfAttribute.value);
                    if (isHide == false) continue;

                    portIds.RemoveAt(i);
                    i--;
                }
            }
        }

        private static bool? If(object thisObject, MemberInfo memberInfo, object value)
        {
            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                {
                    object fieldValue = fieldInfo.GetValue(thisObject);
                    return fieldValue.Equals(value);
                }

                case PropertyInfo propertyInfo:
                {
                    if (propertyInfo.CanRead == false) return null;
                    object propertyValue = propertyInfo.GetValue(thisObject, null);

                    if (propertyInfo.PropertyType == typeof(bool)) { return (bool) propertyValue; }
                    return propertyValue.Equals(value);
                }

                case MethodInfo methodInfo:
                {
                    if (methodInfo.ReturnType != typeof(bool)) return null;
                    if (methodInfo.IsStatic) { return (bool) methodInfo.Invoke(null, new object[] { }); }
                    return (bool) methodInfo.Invoke(thisObject, new object[] { });
                }
            }

            return null;
        }
    }
}