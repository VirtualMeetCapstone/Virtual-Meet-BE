namespace GOCAP.Api.Validation;

public static class ValidationMessage
{
	public static readonly string Required = "{0} is required.";
	public static readonly string InvalidEmail = "Invalid email format.";
	public static readonly string InvalidPhoneNumber = "{0} is not a valid phone number.";
	public static readonly string InvalidDate = "{0} is not a valid date.";

	public static readonly string MinLength = "{0} must be at least {1}.";
	public static readonly string MaxLength = "{0} must not exceed {1}.";
	public static readonly string ExactLength = "{0} must be exactly {1} characters.";

	public static readonly string GreaterThan = "{0} must be greater than {1}.";
	public static readonly string GreaterThanOrEqual = "{0} must be greater than or equal to {1}.";
	public static readonly string LessThan = "{0} must be less than {1}.";
	public static readonly string LessThanOrEqual = "{0} must be less than or equal to {1}.";
	public static readonly string Range = "{0} must be between {1} and {2}.";

	public static readonly string OnlyLetters = "{0} can only contain letters.";
	public static readonly string OnlyNumbers = "{0} can only contain numbers.";
	public static readonly string OnlyAlphanumeric = "{0} can only contain letters and numbers.";
	public static readonly string InvalidFormat = "{0} has an invalid format.";

	public static readonly string ListNotEmpty = "{0} must not be empty.";
	public static readonly string ListMinItems = "{0} must contain at least {1} items.";
	public static readonly string ListMaxItems = "{0} must contain no more than {1} items.";

	public static readonly string MustBe = "{0} must be {1}.";

	public static readonly string MustMatch = "{0} must match {1}.";
	public static readonly string MustNotMatch = "{0} must not be the same as {1}.";

	public static readonly string NotFound = "{0} with ID {1} was not found.";
	public static string Format(string message, params object[] values)
		=> string.Format(message, values);

}
