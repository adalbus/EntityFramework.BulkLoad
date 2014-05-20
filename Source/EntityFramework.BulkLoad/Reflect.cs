namespace EntityFramework.BulkLoad
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    public static class Reflect
    {
        #region Public Methods and Operators

        public static TSignature CompileMethod<TSignature>(string name, Type ownerType, Action<ILGenerator> ilGenerator)
            where TSignature : class
        {
            if (!typeof(TSignature).IsSubclassOf(typeof(Delegate)))
            {
                throw new InvalidOperationException("TSignature must be a delegate type.");
            }

            MethodInfo invoke = typeof(TSignature).GetMethod("Invoke");
            ParameterInfo[] parameters = invoke.GetParameters();

            var method = new DynamicMethod(
                name,
                invoke.ReturnType,
                parameters.Select(p => typeof(object)).ToArray(),
                ownerType,
                true);
            for (int i = 0; i < parameters.Length; i++)
            {
                method.DefineParameter(
                    i,
                    parameters[i].Attributes & (ParameterAttributes.In | ParameterAttributes.Out),
                    parameters[i].Name);
            }

            ilGenerator(method.GetILGenerator());

            return (TSignature)(object)method.CreateDelegate(typeof(TSignature));
        }

        #endregion
    }
}