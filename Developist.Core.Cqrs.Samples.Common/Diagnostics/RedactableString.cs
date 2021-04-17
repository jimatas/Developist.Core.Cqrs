// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Developist.Core.Cqrs.Samples.Common.Diagnostics
{
    /// <summary>
    /// Helps keeping sensitive data, such as passwords, out of log files.
    /// </summary>
    /// <remarks>
    /// This is by no means a replacement for something like <see cref="System.Security.SecureString"/>, which will also keep that data out of memory.
    /// </remarks>
    [JsonConverter(typeof(RedactableStringConverter))]
    public record RedactableString(string ClearText)
    {
        /// <summary>
        /// A redacted version of the clear text string. 
        /// </summary>
        /// <value>
        /// A string in which all the characters have been replaced with asterisks.
        /// </value>
        public string RedactedText => string.IsNullOrEmpty(ClearText) ? ClearText : new('*', ClearText.Length);

        /// <summary>
        /// Overridden to return the redacted version of the clear text string.
        /// </summary>
        /// <returns>A string representing this object.</returns>
        public override string ToString() => RedactedText ?? string.Empty; // ToString should always return a string, even if just an empty one, never null.

        public static implicit operator RedactableString(string s) => new(s);
        public static implicit operator string(RedactableString s) => new(s?.ClearText);

        private class RedactableStringConverter : JsonConverter<RedactableString>
        {
            public override RedactableString Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) => reader.GetString();
            public override void Write(Utf8JsonWriter writer, RedactableString value, JsonSerializerOptions options) => writer.WriteStringValue(value?.ToString());
        }
    }
}
