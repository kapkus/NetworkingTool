﻿using NetworkingTool.Utils.Validators;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace NetworkingTool.Views.ValidationRules
{
    public class PortsValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string input = GetBoundValue(value) as string;

            if (string.IsNullOrWhiteSpace(input))
            {
                return new ValidationResult(false, "Input cannot be empty.");
            }

            PortValidator portValidator = new PortValidator();

            try
            {
                if (portValidator.ParsePorts(input).Count > 0)
                {
                    return ValidationResult.ValidResult;
                }
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, ex.Message);
            }

            return new ValidationResult(false, "Invalid port format.");
        }
        private object GetBoundValue(object value)
        {
            if (value is BindingExpression)
            {
                BindingExpression binding = (BindingExpression)value;

                object dataItem = binding.DataItem;
                string propertyName = binding.ParentBinding.Path.Path;

                object propertyValue = dataItem.GetType().GetProperty(propertyName).GetValue(dataItem, null);

                return propertyValue;
            }
            else
            {
                return value;
            }
        }

    }
}