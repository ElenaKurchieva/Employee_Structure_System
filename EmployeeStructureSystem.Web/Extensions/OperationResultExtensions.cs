using EmployeeStructureSystem.Application.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EmployeeStructureSystem.Web.Extensions;

public static class OperationResultExtensions
{
    public static void ApplyToModelState(this OperationResult result, ModelStateDictionary modelState)
    {
        foreach (var error in result.ValidationErrors)
        {
            foreach (var message in error.Value)
            {
                modelState.AddModelError(error.Key, message);
            }
        }

        if (!string.IsNullOrWhiteSpace(result.ErrorMessage) && result.ValidationErrors.Count == 0)
        {
            modelState.AddModelError(string.Empty, result.ErrorMessage);
        }
    }
}