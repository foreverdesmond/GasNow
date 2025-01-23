using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GasNow.Helper
{
    /// <summary>
    /// A JSON converter for converting strings and numbers to <see cref="decimal"/> values.
    /// </summary>
    public class StringToDecimalConverter : JsonConverter<decimal>
    {
        /// <summary>
        /// Reads and converts the JSON token to a <see cref="decimal"/> value.
        /// </summary>
        /// <param name="reader">The reader used to read the JSON data.</param>
        /// <param name="typeToConvert">The type to convert to, which is <see cref="decimal"/>.</param>
        /// <param name="options">The options used during deserialization.</param>
        /// <returns>A <see cref="decimal"/> value converted from the JSON token.</returns>
        /// <exception cref="JsonException">Thrown when the JSON token cannot be converted to a <see cref="decimal"/>.</exception>
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && decimal.TryParse(reader.GetString(), out var result))
            {
                return result;
            }

            if (reader.TokenType == JsonTokenType.Number && reader.TryGetDecimal(out var number))
            {
                return number;
            }

            throw new JsonException($"Unable to convert {reader.GetString()} to decimal.");
        }

        /// <summary>
        /// Writes the <see cref="decimal"/> value as a JSON string.
        /// </summary>
        /// <param name="writer">The writer used to write the JSON data.</param>
        /// <param name="value">The <see cref="decimal"/> value to write.</param>
        /// <param name="options">The options used during serialization.</param>
        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}

