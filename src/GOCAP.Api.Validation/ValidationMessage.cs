namespace GOCAP.Api.Validation;

public static class ValidationMessage
{
	public static readonly string Required = "{PropertyName} is required.";
	public static readonly string InvalidEmail = "Invalid email format.";
	public static readonly string InvalidPhoneNumber = "{PropertyName} is not a valid phone number.";
	public static readonly string InvalidDate = "{PropertyName} is not a valid date.";

	public static readonly string MinLength = "{PropertyName} must be at least {MinLength} characters.";
	public static readonly string MaxLength = "{PropertyName} must not exceed {MaxLength} characters.";
	public static readonly string ExactLength = "{PropertyName} must be exactly {ExactLength} characters.";

	public static readonly string GreaterThan = "{PropertyName} must be greater than {ComparisonValue}.";
	public static readonly string GreaterThanOrEqual = "{PropertyName} must be greater than or equal to {ComparisonValue}.";
	public static readonly string LessThan = "{PropertyName} must be less than {ComparisonValue}.";
	public static readonly string LessThanOrEqual = "{PropertyName} must be less than or equal to {ComparisonValue}.";
	public static readonly string Range = "{PropertyName} must be between {From} and {To}.";

	public static readonly string OnlyLetters = "{PropertyName} can only contain letters.";
	public static readonly string OnlyNumbers = "{PropertyName} can only contain numbers.";
	public static readonly string OnlyAlphanumeric = "{PropertyName} can only contain letters and numbers.";
	public static readonly string InvalidFormat = "{PropertyName} has an invalid format.";

	public static readonly string ListNotEmpty = "{PropertyName} must not be empty.";
	public static readonly string ListMinItems = "{PropertyName} must contain at least {MinItems} items.";
	public static readonly string ListMaxItems = "{PropertyName} must contain no more than {MaxItems} items.";

	public static readonly string MustBeTrue = "{PropertyName} must be true.";
	public static readonly string MustBeFalse = "{PropertyName} must be false.";

	public static readonly string MustMatch = "{PropertyName} must match {ComparisonProperty}.";
	public static readonly string MustNotMatch = "{PropertyName} must not be the same as {ComparisonProperty}.";

	public static readonly string NotFound = "{0} with ID {1} was not found.";
	public static string Format(string message, params object[] values)
		=> string.Format(message, values);

}
