using System;
using System.Collections.Generic;
using UnityEngine;

namespace Emilia.Flow
{
    [Serializable]
    public sealed class FlowDynamicOutputPortDescriptor
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private float _order;
        [SerializeField] private bool _canMultiConnect = true;
        [SerializeField] private bool _hasColor;
        [SerializeField] private Color _color = Color.white;

        public string id => _id;
        public string displayName => _displayName;
        public float order => _order;
        public bool canMultiConnect => _canMultiConnect;
        public bool hasColor => _hasColor;
        public Color color => _color;

        public FlowDynamicOutputPortDescriptor(
            string id,
            string displayName,
            float order,
            bool canMultiConnect = true,
            bool hasColor = false,
            Color color = default)
        {
            _id = id;
            _displayName = displayName;
            _order = order;
            _canMultiConnect = canMultiConnect;
            _hasColor = hasColor;
            _color = hasColor ? color : Color.white;
        }
    }

    public interface IFlowDynamicOutputPortProvider
    {
        IReadOnlyList<FlowDynamicOutputPortDescriptor> dynamicOutputPorts { get; }
    }
}
