import { useState } from "react";
import type { ChangeEvent, FormEvent } from "react";
import { useChangePassword } from "../../hooks/useChangePassword";
import { type PasswordValidation, validatePasswords } from "../../validation/validatePasswords";
import "./ChangePasswordForm.css";

type ChangePasswordFormProps = {
  onSuccess: () => void;
  onCancel?: () => void;
};

type PasswordFields = {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
};

export function ChangePasswordForm({ onSuccess, onCancel }: ChangePasswordFormProps) {
  const [passwordFields, setPasswordFields] = useState<PasswordFields>({
    currentPassword: "",
    newPassword: "",
    confirmPassword: "",
  });
  const [hasSubmitted, setHasSubmitted] = useState(false);
  const [showSubmitError, setShowSubmitError] = useState(false);

  const { submit, loading, error } = useChangePassword();

  const validation: PasswordValidation | null = hasSubmitted
    ? validatePasswords(
        passwordFields.newPassword,
        passwordFields.confirmPassword
      )
    : null;

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    setHasSubmitted(true);
    setShowSubmitError(false);

    const submitValidation = validatePasswords(
      passwordFields.newPassword,
      passwordFields.confirmPassword
    );

    if (submitValidation.error) {
      return;
    }

    const success = await submit(
      passwordFields.currentPassword,
      passwordFields.newPassword
    );

    if (success) {
      onSuccess();
    } else {
      setShowSubmitError(true);
    }
  };

  const handleFieldChange = (
    field: keyof PasswordFields
  ) => (event: ChangeEvent<HTMLInputElement>) => {
    setPasswordFields((currentFields) => ({
      ...currentFields,
      [field]: event.target.value,
    }));
    setShowSubmitError(false);
  };

  const formError = validation?.error ?? (
    showSubmitError ? error : null
  );

  return (
    <form
      className="change-password-form"
      onSubmit={handleSubmit}
    >
      <div className="form-field">
        <label htmlFor="currentPassword">
          Current password
        </label>

        <input
          id="currentPassword"
          type="password"
          value={passwordFields.currentPassword}
          onChange={handleFieldChange("currentPassword")}
          disabled={loading}
          required
        />
      </div>

      <div className="form-field">
        <label htmlFor="newPassword">
          New password
        </label>

        <input
          id="newPassword"
          type="password"
          value={passwordFields.newPassword}
          onChange={handleFieldChange("newPassword")}
          className={
            validation?.passwordTooShort || validation?.passwordsDoNotMatch
              ? "error"
              : ""
          }
          disabled={loading}
          required
        />
      </div>

      <div className="form-field">
        <label htmlFor="confirmPassword">
          Confirm password
        </label>

        <input
          id="confirmPassword"
          type="password"
          value={passwordFields.confirmPassword}
          onChange={handleFieldChange("confirmPassword")}
          className={
            validation?.passwordsDoNotMatch
              ? "error"
              : ""
          }
          disabled={loading}
          required
        />
      </div>

      {formError && (
        <p className="error-text">
          {formError}
        </p>
      )}

      <div className="actions">
        <button
          type="submit"
          className="primary"
          disabled={loading}
        >
          {loading ? "Saving..." : "Save"}
        </button>

        {onCancel && (
          <button
            type="button"
            className="secondary"
            onClick={onCancel}
            disabled={loading}
          >
            Cancel
          </button>
        )}
      </div>
    </form>
  );
}
