namespace GOCAP.Commom;

public static class ErrorMessageBase
{
	public const string Required = "{PropertyName} is required.";
	public const string InvalidEmail = "Invalid email format.";
	public const string InvalidPhoneNumber = "{PropertyName} is not a valid phone number.";
	public const string InvalidDate = "{PropertyName} is not a valid date.";

	public const string MinLength = "{PropertyName} must be at least {MinLength} characters.";
	public const string MaxLength = "{PropertyName} must not exceed {MaxLength} characters.";
	public const string ExactLength = "{PropertyName} must be exactly {ExactLength} characters.";

	public const string GreaterThan = "{PropertyName} must be greater than {ComparisonValue}.";
	public const string GreaterThanOrEqual = "{PropertyName} must be greater than or equal to {ComparisonValue}.";
	public const string LessThan = "{PropertyName} must be less than {ComparisonValue}.";
	public const string LessThanOrEqual = "{PropertyName} must be less than or equal to {ComparisonValue}.";
	public const string Range = "{PropertyName} must be between {From} and {To}.";

	public const string OnlyLetters = "{PropertyName} can only contain letters.";
	public const string OnlyNumbers = "{PropertyName} can only contain numbers.";
	public const string OnlyAlphanumeric = "{PropertyName} can only contain letters and numbers.";
	public const string InvalidFormat = "{PropertyName} has an invalid format.";

	public const string ListNotEmpty = "{PropertyName} must not be empty.";
	public const string ListMinItems = "{PropertyName} must contain at least {MinItems} items.";
	public const string ListMaxItems = "{PropertyName} must contain no more than {MaxItems} items.";

	public const string MustBeTrue = "{PropertyName} must be true.";
	public const string MustBeFalse = "{PropertyName} must be false.";

	public const string MustMatch = "{PropertyName} must match {ComparisonProperty}.";
	public const string MustNotMatch = "{PropertyName} must not be the same as {ComparisonProperty}.";

	public static string NotFound = "{0} with ID {1} was not found.";
	public static string Format(string message, params object[] values)
		=> string.Format(message, values);

}
