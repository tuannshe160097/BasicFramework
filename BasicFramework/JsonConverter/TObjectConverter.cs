using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using BasicFramework.DataType;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.IO;

namespace BasicFramework.JsonConverter
{
    internal class TObjectConverter : CustomCreationConverter<TObject>
    {
        public override bool CanWrite => false;

        public override TObject Create(Type objectType)
        {
            if (objectType == typeof(TList))
            {
                return new TObject(true);
            }

            return new TObject();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) 
            {
                return null;
            }

            bool first = true;

            TObject result = Create(objectType);

            dynamic current = result;

            string? propName = null;

            do
            {
                switch (reader.TokenType)
                {
                    case JsonToken.None:
                        break;
                    case JsonToken.StartArray:
                        if (first)
                        {
                            first = false;
                            result = Create(typeof(TList));
                            current = result;
                        } 
                        else
                        {
                            if (current is TList)
                            {
                                current.Add(new TList());
                                current = current[current.Count - 1];
                            }
                            else
                            {
                                current.Add(propName, new TList());
                                current = current[propName];
                            }
                        }

                        break;
                    case JsonToken.EndArray:
                        current = current.Parent;
                        break;
                    case JsonToken.StartObject:
                        if (first)
                        {
                            first = false;
                            result = Create(typeof(TDictionary));
                            current = result;
                        }
                        else
                        {
                            if (current is TList)
                            {
                                current.Add(new TDictionary());
                                current = current[current.Count - 1];
                            }
                            else
                            {
                                current.Add(propName, new TDictionary());
                                current = current[propName];
                            }
                        }

                        break;
                    case JsonToken.EndObject:
                        current = current.Parent;
                        break;
                    case JsonToken.StartConstructor:
                        break;
                    case JsonToken.EndConstructor:
                        break;
                    case JsonToken.String:
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.Date:
                    case JsonToken.Boolean:
                    case JsonToken.Bytes:
                        if (current is TList)
                        {
                            current.Add(reader.Value);
                        }
                        else
                        {
                            current.Add(propName, reader.Value);
                        }
                        break;
                    case JsonToken.Comment:
                        break;
                    case JsonToken.Null:
                        current[propName] = null;
                        break;
                    case JsonToken.Undefined:
                        break;
                    case JsonToken.PropertyName:
                        propName = reader.Value?.ToString();
                        break;
                    default:
                        break;
                }
            } while (reader.Read());

            return result;

            /*object result = null;
            if (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.Null)
            {
                result = base.ReadJson(reader, objectType, existingValue, serializer);
                return result;
            }

            result = serializer.Deserialize(reader);
            return result;*/
        }
    }
}
