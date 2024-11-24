using System;
namespace GasNow.Helper
{
	public class GasCalculate
    {
        /// <summary>
        /// Calculates the congestion percentage based on the provided gas used ratio.
        /// </summary>
        /// <param name="gasUsedRatio">A comma-separated string representing the gas used ratio.</param>
        /// <returns>An integer representing the congestion percentage.</returns>
        /// <exception cref="ArgumentException">Thrown when the gas used ratio contains invalid numbers.</exception>
        public int GetCongestionPercentage(string gasUsedRatio)
        {
            var roundedRatios = gasUsedRatio
                .Split(',')
                .Select(s =>
                {
                    if (decimal.TryParse(s, out var number))
                    {
                        return Math.Round(number, 2);
                    }
                    throw new ArgumentException($"Invalid number in gasUsedRatio: {s}");
                })
                .ToArray();

            var average = roundedRatios.Average();

            return (int)(average * 100);
        }

        /// <summary>
        /// Calculates the rapid maximum fee based on the fast maximum fee and congestion level.
        /// </summary>
        /// <param name="fastMaxFee">The fast maximum fee.</param>
        /// <param name="congestion">The congestion level as a percentage.</param>
        /// <returns>The calculated rapid maximum fee.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the congestion level is not between 1 and 100.</exception>
        public decimal CalculateRapidMaxFee(decimal fastMaxFee, int congestion)
        {
            if (congestion < 1 || congestion > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(congestion), "Congestion level must be between 1 and 100.");
            }

            decimal rapidMaxFee = fastMaxFee * (1.1M + (congestion - 1) * 0.009M);

            return rapidMaxFee;
        }
    }
}

