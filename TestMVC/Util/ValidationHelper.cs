using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TestMVC.Util
{
	public static class ValidationHelper
	{
		public static object ModelStateToJsonable(ModelStateDictionary modelState)
		{
			var errors = modelState.SelectMany(kvp => kvp.Value!.Errors, (kvp, e) => new {kvp.Key, e.ErrorMessage});

			return new
			{
				isValid = modelState.IsValid,
				PropertyErrors = errors.Where(e => !string.IsNullOrEmpty(e.Key)),
				ModelErrors = errors.Where(e => string.IsNullOrEmpty(e.Key))
			};
		}
	}
}
