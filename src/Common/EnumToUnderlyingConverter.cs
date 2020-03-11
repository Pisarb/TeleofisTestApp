using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Swsu.StreetLights.Common
{
    public abstract class EnumToUnderlyingConverter<T, TUnderlying> : IConverter<T, TUnderlying>
        where T : Enum
        where TUnderlying : struct
    {
        #region Fields
        private static EnumToUnderlyingConverter<T, TUnderlying> _instance = null!;
        #endregion

        #region Constructors
        protected EnumToUnderlyingConverter()
        {
        }
        #endregion

        #region Properties
        public static EnumToUnderlyingConverter<T, TUnderlying> Instance
        {
            get
            {
                return LazyInitializer.EnsureInitialized(ref _instance, CreateInstance);
            }
        }
        #endregion

        #region Methods
        public abstract T ConvertFrom(TUnderlying value);

        public abstract TUnderlying ConvertTo(T value);

        private static EnumToUnderlyingConverter<T, TUnderlying> CreateInstance()
        {
            var type = typeof(T);
            var underlyingType = typeof(TUnderlying);

            if (Enum.GetUnderlyingType(type) != underlyingType)
            {
                throw new InvalidOperationException(string.Format(
                    "Specified underlying type {0} does not match actual underlying type of {1} ({2}).",
                    underlyingType,
                    type,
                    Enum.GetUnderlyingType(type)));
            }

            return (EnumToUnderlyingConverter<T, TUnderlying>)CreateInstance(type);
        }

        private static object CreateInstance(Type type)
        {
            return Activator.CreateInstance(CreateConverterType(type));
        }

        private static Type CreateConverterType(Type type)
        {
            Debug.Assert(type != null);
            Debug.Assert(type.IsEnum);

            var underlyingType = Enum.GetUnderlyingType(type);
            var baseConverterType = typeof(EnumToUnderlyingConverter<,>).MakeGenericType(type, underlyingType);
            var converterTypeName = string.Format("_{0:N}", Guid.NewGuid());
            var converterType = SharedBuilders.Module.DefineType(converterTypeName, TypeAttributes.Class, baseConverterType);

            var convertFromMethod = converterType.DefineMethod(
                nameof(ConvertFrom),
                MethodAttributes.Public | MethodAttributes.Virtual,
                type,
                new[] { underlyingType });

            var convertFromMethodGenerator = convertFromMethod.GetILGenerator();

            convertFromMethodGenerator.Emit(OpCodes.Ldarg_1);
            convertFromMethodGenerator.Emit(OpCodes.Ret);

            var convertToMethod = converterType.DefineMethod(
                nameof(ConvertTo),
                MethodAttributes.Public | MethodAttributes.Virtual,
                underlyingType,
                new[] { type });

            var convertToMethodGenerator = convertToMethod.GetILGenerator();

            convertToMethodGenerator.Emit(OpCodes.Ldarg_1);
            convertToMethodGenerator.Emit(OpCodes.Ret);

            converterType.DefineMethodOverride(
                convertFromMethod,
                baseConverterType.GetMethod(nameof(ConvertFrom)));

            converterType.DefineMethodOverride(
                convertToMethod,
                baseConverterType.GetMethod(nameof(ConvertTo)));

            return converterType.CreateType();
        }
        #endregion
    }
}
