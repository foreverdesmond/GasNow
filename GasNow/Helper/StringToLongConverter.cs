using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GasNow.Helper
{
    /// <summary>
    /// A JSON converter for converting strings and numbers to <see cref="long"/> values.
    /// </summary>
    public class StringToLongConverter : JsonConverter<long>
    {
        /// <summary>
        /// Reads and converts the JSON token to a <see cref="long"/> value.
        /// </summary>
        /// <param name="reader">The reader used to read the JSON data.</param>
        /// <param name="typeToConvert">The type to convert to, which is <see cref="long"/>.</param>
        /// <param name="options">The options used during deserialization.</param>
        /// <returns>A <see cref="long"/> value converted from the JSON token.</returns>
        /// <exception cref="JsonException">Thrown when the JSON token cannot be converted to a <see cref="long"/>.</exception>
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && long.TryParse(reader.GetString(), out var result))
            {
                return result;
            }

            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out var number))
            {
                return number;
            }

            throw new JsonException($"Unable to convert {reader.GetString()} to long.");
        }

        /// <summary>
        /// Writes the <see cref="long"/> value as a JSON string.
        /// </summary>
        /// <param name="writer">The writer used to write the JSON data.</param>
        /// <param name="value">The <see cref="long"/> value to write.</param>
        /// <param name="options">The options used during serialization.</param>
        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}