using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Swsu.StreetLights.Common
{
    internal static class SharedBuilders
    {
        #region Fields
        private static AssemblyBuilder _assembly = null!;

        private static ModuleBuilder _module = null!;
        #endregion

        #region Properties
        internal static AssemblyBuilder Assembly
        {
            get
            {
                return LazyInitializer.EnsureInitialized(ref _assembly, () =>
                {
                    var name = new AssemblyName(typeof(SharedBuilders).Assembly.GetName().Name + "_dynamic");
                    return AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
                });
            }
        }

        internal static ModuleBuilder Module
        {
            get
            {
                return LazyInitializer.EnsureInitialized(ref _module, () =>
                {
                    return Assembly.DefineDynamicModule("main");
                });
            }
        }
        #endregion
    }
}
