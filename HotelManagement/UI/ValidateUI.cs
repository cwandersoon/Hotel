using FluentValidation;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.UI
{
    public static class ValidateUI
    {
        public static bool IsInvalid<T>(this IValidator<T> validator, T dto)
        {
            var result = validator.Validate(dto);

            if (!result.IsValid)
            {
                ShowValidationErrors(result);
                return true;
            }
            return false;
        }

        public static void ShowValidationErrors(FluentValidation.Results.ValidationResult result)
        {
            AnsiConsole.MarkupLine("\n[red]Validation failed:[/]");
            foreach (var error in result.Errors)
            {
                AnsiConsole.MarkupLine($"[red]{error.ErrorMessage}[/]");
            }
            AnsiConsole.MarkupLine("[grey]Press any key to try again.[/]");
            Console.ReadKey(true);
        }

    }
}
