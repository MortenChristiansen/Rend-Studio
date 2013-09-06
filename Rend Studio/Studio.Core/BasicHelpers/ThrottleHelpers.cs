using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Studio.Core.BasicHelpers
{
    public static class ThrottleHelpers
    {
        private static DateTime _lastHit;

        /// <summary>
        /// Determines whether this method call has been within
        /// the given timespan. Useful for throttling logic.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static bool HasBeenCalledWithin(TimeSpan timeSpan)
        {
            var now = DateTime.UtcNow;
            if (now - _lastHit < timeSpan)
            {
                _lastHit = DateTime.MinValue;
                return true;
            }
            else
            {
                _lastHit = now;
                return false;
            }
        }
    }
}
