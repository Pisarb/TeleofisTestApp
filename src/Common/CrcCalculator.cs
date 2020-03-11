using System;
using System.Collections.Generic;

namespace Swsu.StreetLights.Common
{
    public abstract class CrcCalculator
    {
        #region Fields
        private readonly static Dictionary<Type, Delegate> _typeToImplFactory;
        #endregion

        #region Constructors
        private protected CrcCalculator()
        {
        }

        static CrcCalculator()
        {
            _typeToImplFactory = new Dictionary<Type, Delegate>
            {
                {
                    typeof(ushort),
                    new ImplFactory<ushort>(p => new Impl16(p))
                }
            };
        }
        #endregion

        #region Methods
        public abstract void Append(ReadOnlySpan<byte> buffer);

        public static CrcCalculator<T> Create<T>(CrcParameters<T> parameters)
        {
            var implFactory = (ImplFactory<T>)_typeToImplFactory[typeof(T)];
            return implFactory(parameters);
        }

        public abstract void Clear();
        #endregion

        #region Nested Types
        private delegate CrcCalculator<T> ImplFactory<T>(CrcParameters<T> parameters);

        private class Impl16 : CrcCalculator<ushort>
        {
            #region Fields
            private ushort _value;
            #endregion

            #region Constructors
            internal Impl16(CrcParameters<ushort> parameters) :
                base(parameters)
            {
                _value = parameters.InitialValue;
            }
            #endregion

            #region Properties
            public override ushort Crc
            {
                get
                {
                    var crc = (ushort)((uint)_value ^ Parameters.FinalXorValue);

                    if (Parameters.ReflectOutput)
                    {
                        crc = BitHelpers.ChangeBitOrder(crc);
                    }

                    return crc;
                }
            }
            #endregion

            #region Methods
            public override void Append(ReadOnlySpan<byte> buffer)
            {
                var polynomial = Parameters.Polynomial;
                var reflectInput = Parameters.ReflectInput;

                for (var i = 0; i < buffer.Length; ++i)
                {
                    var b = buffer[i];

                    if (reflectInput)
                    {
                        b = BitHelpers.ChangeBitOrder(b);
                    }

                    _value ^= (ushort)((uint)b << 8);

                    for (var j = 0; j < 8; ++j)
                    {
                        if ((_value & 0x8000) != 0)
                        {
                            _value = (ushort)((_value << 1) ^ polynomial);
                        }
                        else
                        {
                            _value <<= 1;
                        }
                    }
                }
            }

            public override void Clear()
            {
                _value = Parameters.InitialValue;
            }
            #endregion
        }
        #endregion
    }

    public abstract class CrcCalculator<T> : CrcCalculator
    {
        #region Construtors
        private protected CrcCalculator(CrcParameters<T> parameters)
        {
            Parameters = parameters;
        }
        #endregion

        #region Properties
        public abstract T Crc
        {
            get;
        }

        protected CrcParameters<T> Parameters
        {
            get;
        }
        #endregion        
    }
}
