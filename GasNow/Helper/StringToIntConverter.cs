using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GasNow.Helper
{
    /// <summary>
    /// A JSON converter for converting strings and numbers to <see cref="int"/> values.
    /// </summary>
    public class StringToIntConverter : JsonConverter<int>
    {
        /// <summary>
        /// Reads and converts the JSON token to an <see cref="int"/> value.
        /// </summary>
        /// <param name="reader">The reader used to read the JSON data.</param>
        /// <param name="typeToConvert">The type to convert to, which is <see cref="int"/>.</param>
        /// <param name="options">The options used during deserialization.</param>
        /// <returns>An <see cref="int"/> value converted from the JSON token.</returns>
        /// <exception cref="JsonException">Thrown when the JSON token cannot be converted to an <see cref="int"/>.</exception>
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && int.TryParse(reader.GetString(), out var result))
            {
                return result;
            }

            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var number))
            {
                return number;
            }

            throw new JsonException($"Unable to convert {reader.GetString()} to int.");
        }

        /// <summary>
        /// Writes the <see cref="int"/> value as a JSON string.
        /// </summary>
        /// <param name="writer">The writer used to write the JSON data.</param>
        /// <param name="value">The <see cref="int"/> value to write.</param>
        /// <param name="options">The options used during serialization.</param>
        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}

