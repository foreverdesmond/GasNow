using System;
using System.Globalization;

namespace GasNow.Helper
{
	public class DateTimeHelper
	{
        /// <summary>
        /// Gets the start and end of the UTC day range for a specified number of days ago.
        /// </summary>
        /// <param name="day">The number of days ago to get the range for.</param>
        /// <param name="dateTimeNow">The current date and time as a DateTimeOffset.</param>
        /// <returns>A tuple containing the start and end of the day in UTC.</returns>
        public (DateTime StartOfDay, DateTime EndOfDay) GetUtcDayRange(int day, DateTimeOffset dateTimeNow)
        {
            var nowUtc = dateTimeNow.UtcDateTime;

            var targetDate = nowUtc.AddDays(-day).Date;

            var startOfDay = targetDate;

            var endOfDay = targetDate.AddDays(1).AddTicks(-1);

            return (startOfDay, endOfDay);
        }
    }
}

