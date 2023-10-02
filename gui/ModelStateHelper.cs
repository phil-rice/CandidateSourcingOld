using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;

namespace common
{
    public static class ModelStateHelper
    {
        public static void DumpModelState(ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                Console.WriteLine("Model state is valid");
            }
            else
            {
                Console.WriteLine("Model state is invalid!");
                foreach (var state in modelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"  Key: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }
            }
        }
    }
}
