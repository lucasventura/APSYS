namespace APSYS.Infrastructure.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Utils for Enum 
    /// </summary>
    public static class EnumUtil
    {
        /// <summary>
        /// Get the Enum Values
        /// </summary>
        /// <typeparam name="T">Enum Type Generic</typeparam>
        /// <returns>List with Enum Types</returns>
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}