namespace CandyCustomizer.Runtime
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal sealed class ReferenceComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceComparer Instance = new ReferenceComparer();

        private ReferenceComparer()
        {
        }

        public new bool Equals(object x, object y) => ReferenceEquals(x, y);

        public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
}
