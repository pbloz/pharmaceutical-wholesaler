using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hurtownia.ViewModels
{

    public class DateRangeAttribute : RangeAttribute
    {
        public DateRangeAttribute(string minimumValue)
            : base(typeof(DateTime), DateTime.Now.ToShortDateString(), minimumValue)
        {

        }
    }

    public class CurrentDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dateTime = Convert.ToDateTime(value);

            return dateTime >= DateTime.Now;
        }
    }


    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName);

            ModelState modelState = new ModelState { Value = valueResult };

            object actualValue = null;

            if (valueResult.AttemptedValue != string.Empty)
            {
                try
                {
                    actualValue = Convert.ToDecimal(valueResult.AttemptedValue, CultureInfo.CurrentCulture);
                }
                catch (FormatException e)
                {
                    modelState.Errors.Add(e);
                }
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);

            return actualValue;
        }
    }
    //[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    //public sealed class ValidBirthDate : ValidationAttribute
    //{
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        if (value != null)
    //        {
    //            DateTime _birthJoin = Convert.ToDateTime(value);
    //            if (_birthJoin > DateTime.Now)
    //            {
    //                return new ValidationResult("Birth date can not be greater than current date.");
    //            }
    //        }
    //        return ValidationResult.Success;
    //    }
    //}



    //[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    //public sealed class DateMustBeEqualOrGreaterThanCurrentDateValidation : ValidationAttribute, IClientValidatable
    //{

    //    private const string DefaultErrorMessage = "Date selected {0} must be on or after today";

    //    public DateMustBeEqualOrGreaterThanCurrentDateValidation()
    //        : base(DefaultErrorMessage)
    //    {
    //    }

    //    public override string FormatErrorMessage(string name)
    //    {
    //        return string.Format(DefaultErrorMessage, name);
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        var dateEntered = (DateTime)value;
    //        if (dateEntered < DateTime.Today)
    //        {
    //            var message = FormatErrorMessage(dateEntered.ToShortDateString());
    //            return new ValidationResult(message);
    //        }
    //        return null;
    //    }

    //    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    //    {
    //        var rule = new ModelClientCustomDateValidationRule(FormatErrorMessage(metadata.DisplayName));
    //        yield return rule;
    //    }
    //}

    //public sealed class ModelClientCustomDateValidationRule : ModelClientValidationRule
    //{

    //    public ModelClientCustomDateValidationRule(string errorMessage)
    //    {
    //        ErrorMessage = errorMessage;
    //        ValidationType = "datemustbeequalorgreaterthancurrentdate";
    //    }
    //}
}