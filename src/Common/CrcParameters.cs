namespace Swsu.StreetLights.Common
{
    public struct CrcParameters<T>
    {
        #region Constructors
        public CrcParameters(T polynomial, T initialValue = default, T finalXorValue = default, bool reflectInput = false, bool reflectOutput = false)
        {
            Polynomial = polynomial;
            InitialValue = initialValue;
            FinalXorValue = finalXorValue;
            ReflectInput = reflectInput;
            ReflectOutput = reflectOutput;
        }
        #endregion

        #region Properties
        public T FinalXorValue
        {
            get;
        }

        public T InitialValue
        {
            get;
        }

        public T Polynomial
        {
            get;
        }

        public bool ReflectInput
        {
            get;
        }

        public bool ReflectOutput
        {
            get;
        }
        #endregion

        #region Methods
        public CrcCalculator<T> CreateCalculator()
        {
            return CrcCalculator.Create<T>(this);
        }

        public void Deconstruct(out T polynomial, out T initialValue, out T finalXorValue, out bool reflectInput, out bool reflectOutput)
        {
            polynomial = Polynomial;
            initialValue = InitialValue;
            finalXorValue = FinalXorValue;
            reflectInput = ReflectInput;
            reflectOutput = ReflectOutput;
        }
        #endregion
    }
}
