export type PasswordValidation = {
  error: string | null;
  passwordsDoNotMatch: boolean;
  passwordTooShort: boolean;
};

export function validatePasswords(
  newPassword: string,
  confirmPassword: string
): PasswordValidation {
  const passwordsDoNotMatch = newPassword !== confirmPassword;
  const passwordTooShort = newPassword.length < 10;

  return {
    error: passwordsDoNotMatch
      ? "Passwords do not match."
      : passwordTooShort
        ? "Password must be at least 10 characters long."
        : null,
    passwordsDoNotMatch,
    passwordTooShort,
  };
}
