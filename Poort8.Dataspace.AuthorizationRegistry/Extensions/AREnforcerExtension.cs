using Casbin;
using Casbin.Model;

namespace Poort8.Dataspace.AuthorizationRegistry.Extensions;
public static class AREnforcerExtension
{
    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
    EnforceEx<T1, T2, T3, T4, T5>(this IEnforcer enforcer, EnforceContext context,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
    EnforceExAsync<T1, T2, T3, T4, T5>(this IEnforcer enforcer, EnforceContext context,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
    EnforceEx<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEnforcer enforcer, EnforceContext context,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
    EnforceExAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEnforcer enforcer, EnforceContext context,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
    EnforceExAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEnforcer enforcer, EnforceContext context,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }
}
