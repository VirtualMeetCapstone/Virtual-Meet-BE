﻿using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GOCAP.Api.Common;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                            .Where(x => x.Value?.Errors?.Any() == true)
                            .SelectMany(x => x.Value?.Errors?.Where(e => e != null)
                            .Select(e => new ValidationFailure(x.Key, e.ErrorMessage)) ?? [])
                            .ToList();

            throw new ValidationException(errors);
        }
    }
}
