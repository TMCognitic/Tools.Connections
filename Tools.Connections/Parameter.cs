using System;
using System.Collections.Generic;
using System.Text;

namespace Tools.Connections
{
    internal class Parameter
    {
        private object _parameterValue;
        private readonly Direction _direction;

        public Parameter(object value, Direction direction)
        {
            _parameterValue = value;
            _direction = direction;
        }

        public object ParameterValue 
        {
            get 
            {
                return _parameterValue; 
            }
            internal set
            {
                _parameterValue = value;
            }
        }

        public Direction Direction
        {
            get
            {
                return _direction;
            }
        }
    }
}
