using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GOCAP.Api.Common;

public static class ModelStateExtensions
{
    public static void AddValidationErrors(this ModelStateDictionary modelState, ValidationResult validationResult)
    {
        foreach (var failure in validationResult.Errors)
        {
            modelState.AddModelError(failure.PropertyName, $"{failure.ErrorMessage} (Code: {failure.ErrorCode})");
        }
    }
}
