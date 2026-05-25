namespace CandyCustomizer.Runtime
{
    using System.Reflection;

    internal static class CandyEventReflection
    {
        public static object GetCandy(object eventArgs)
        {
            if (eventArgs is null)
                return null;

            PropertyInfo property = eventArgs.GetType().GetProperty("Candy", BindingFlags.Instance | BindingFlags.Public);
            return property?.GetValue(eventArgs);
        }
    }
}
