using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using MarketR.Service.CsvParser.Models;
using CsvHelper.TypeConversion;
using MarketR.Service.CsvParser.Exceptions;
using MarketR.Service.CsvParser.Infrastructure;
using ValidationResult = MarketR.Service.CsvParser.Models.ValidationResult;
using System.Globalization;

namespace MarketR.Service.CsvParser
{
    public class CsvParseService<TDataRecord> : ICsvParseService<TDataRecord> where TDataRecord : class, new()
    {
        public CsvParseService() { }

        public CsvParseService(TypeConvertSettings convertSettings)
        {
            SetCovertSettings(convertSettings.ConvertSettings);
        }
        public ParseResult<TDataRecord> ParseData(Stream fileStream, string separator)
        {
            if (fileStream == null || fileStream.Length <= 0)
            {
                throw new CsvParseException("File is empty");
            }
            var result = new ParseResult<TDataRecord>();

            try
            {
                var file = new StreamReader(fileStream);
                //
                //Stream stream = file;
                //DataTable csvTable = new DataTable();
                //using (CsvReader csvReader =
                //    new CsvReader(new StreamReader(stream), true))
                //{
                //    csvTable.Load(csvReader);
                //}

                var reader = new CsvReader(file);

                reader.Configuration.Delimiter = separator;
                reader.Configuration.IgnoreReadingExceptions = true;

                reader.Configuration.ReadingExceptionCallback = (ex, row) =>
                {
                    if (ex.GetType() == typeof(CsvTypeConverterException))
                    {
                        var error = ex.Data["CsvHelper"];

                        result.AddError("", string.Format("Could not read value {0}", error));
                    }
                    else
                    {
                        result.AddError("", "Could not read Csv file.");
                    }
                };

                while (reader.Read())
                {
                    var rowResult = ParseRow(reader);

                    if (!rowResult.IsValid)
                    {
                        result.AddErrors(rowResult.Errors);
                        return result;
                    }
                    result.Records.Add(rowResult.Record);
                }
            }
            catch (CsvParseException ex)
            {
                result.AddError("", ex.Message);
            }
            catch (CsvMissingFieldException ex)
            {
                result.AddError("", ex.Message);
            }
            catch (Exception ex)
            {
                throw new CsvParseException("Coud not read csv file");
            }

            return result;
        }

        private ParseResultRow<TDataRecord> ParseRow(CsvReader reader)
        {
            var result = new ParseResultRow<TDataRecord>();

            var entityType = typeof(TDataRecord);

            var properties = entityType.GetProperties().ToArray();

            if (properties.Length <= 0)
            {
                throw new CsvParseException("Object properties not found");
            }
            foreach (var property in properties)
            {
                object value = null;

                var fieldName = GetFieldName(property);

                try
                {
                    //var validateResult = Validate(property, reader[fieldName], reader.Row);
                    var validateResult = Validate(property, reader.FieldHeaders[0], reader.Row);
                    if (!validateResult.IsValid)
                    {
                        result.AddErrors(validateResult.Errors);

                        return result;
                    }
                    if (property.PropertyType.Name.ToLower() == "double")
                    {
                        value = reader.GetField(fieldName);
                        if (!string.IsNullOrEmpty(value.ToString()))
                            value = Convert.ToDouble(value, new CultureInfo("en-US"));
                        else
                            value = Convert.ToDouble("0.00", new CultureInfo("en-US"));
                    }
                    else
                    {
                        value = reader.GetField(property.PropertyType, fieldName);
                    }
                    //value = reader.GetField(fieldName);
                    property.SetValue(result.Record, value);
                }
                catch (CsvTypeConverterException ex)
                {
                    result.AddError("TypeConverterException", string.Format("Could not convert {0} to {1}. Row: {2}. Field Name: {3} ", reader[fieldName], property.PropertyType, reader.Row, fieldName));

                    return result;
                }
                catch (FormatException ex)
                {
                    result.AddError("TypeConverterException", string.Format("Could not convert {0} to {1}. Row: {2}. Field Name: {3} ", reader[fieldName], property.PropertyType, reader.Row, fieldName));

                    return result;
                }
            }

            return result;
        }

        private ValidationResult Validate(PropertyInfo property, object value, int rowNumber)
        {
            var result = new ValidationResult();

            var validationAttr = property.GetCustomAttributes(typeof(ValidationAttribute), true);

            if (validationAttr.Length <= 0) return result;

            foreach (var attr in validationAttr)
            {
                var validAttr = attr as ValidationAttribute;

                if (validAttr == null) continue;

                if (validAttr.IsValid(value)) continue;

                var fieldName = GetFieldName(property);

                result.AddError("Validation",
                    validAttr.ErrorMessage != null
                        ? string.Format("{0}. Row {1}, Field: {2}", validAttr.ErrorMessage, rowNumber, fieldName)
                        : string.Format("{0} Row {1}", validAttr.FormatErrorMessage(fieldName), rowNumber));

                return result;
            }

            return result;
        }

        private string GetFieldName(PropertyInfo property)
        {
            var fieldName = property.Name;

            var attr =
                property.GetCustomAttributes(typeof(CsvFieldAttribute), true).FirstOrDefault() as CsvFieldAttribute;

            if (attr != null)
            {
                fieldName = attr.Name;
            }

            return fieldName;
        }

        private void SetCovertSettings(List<TypeConverter> converters)
        {
            foreach (var converter in converters)
            {
                TypeConverterOptionsFactory.AddOptions(converter.Type, converter.TypeConverterOptions);
            }
        }
    }
}
