using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Panteon.Sdk
{
    public static class Requires
    {
        [DebuggerStepThrough]
        public static void NotNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        [DebuggerStepThrough]
        public static void NotNullOrEmpty(string value, string parameterName)
        {
            NotNull(value, parameterName);
            if (value.Length == 0)
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, string.Empty, parameterName), parameterName);
        }

        [DebuggerStepThrough]
        public static void NotNullOrNullElements<T>(IEnumerable<T> values, string parameterName) where T : class
        {
            NotNull(values, parameterName);
            NotNullElements(values, parameterName);
        }

        [DebuggerStepThrough]
        public static void NullOrNotNullElements<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> values, string parameterName)
            where TKey : class
            where TValue : class
        {
            NotNullElements(values, parameterName);
        }

        [DebuggerStepThrough]
        public static void NullOrNotNullElements<T>(IEnumerable<T> values, string parameterName) where T : class
        {
            NotNullElements(values, parameterName);
        }

        [DebuggerStepThrough]
        private static void NotNullElements<T>(string parameterName) where T : class
        {
            NotNullElements<T>(null, parameterName);
        }

        [DebuggerStepThrough]
        private static void NotNullElements<T>(IEnumerable<T> values, string parameterName) where T : class
        {
            if (values != null && !Contract.ForAll<T>(values, value => (object)value != null))
                throw new ArgumentException("Contains Null Element", parameterName);
        }

        [DebuggerStepThrough]
        public static void NullOrNotNullElements<T>(T[] values, string parameterName) where T : class
        {
            NotNullElements(values, parameterName);
        }

        [DebuggerStepThrough]
        private static void NotNullElements<T>(T[] values, string parameterName) where T : class
        {
            if (values == null)
                return;
            if (values.Any(obj => obj == null))
            {
                throw new ArgumentException("Contains Null Element", parameterName);
            }
        }

        [DebuggerStepThrough]
        private static void NotNullElements<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> values, string parameterName)
            where TKey : class
            where TValue : class
        {
            if (values != null && !Contract.ForAll(values, keyValue =>
            {
                if (keyValue.Key != null)
                    return (object)keyValue.Value != null;
                return false;
            }))
                throw new ArgumentException("Contains Null Element", parameterName);
        }

        [DebuggerStepThrough]
        public static void IsInMembertypeSet(MemberTypes value, string parameterName, MemberTypes enumFlagSet)
        {
            if ((value & enumFlagSet) != value || (value & value - 1) != 0)
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "ArgumentOutOfRange Invalid Enum In Set"), parameterName);
        }
    }
}