import { useState } from "react";
import { changePassword } from "../api/auth";

export function useChangePassword() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const submit = async (
    currentPassword: string,
    newPassword: string
  ): Promise<boolean> => {
    try {
      setLoading(true);
      setError(null);

      await changePassword({
        currentPassword,
        newPassword,
      });

      return true;
    } catch {
      setError("Failed to change password.");
      return false;
    } finally {
      setLoading(false);
    }
  };

  return {
    submit,
    loading,
    error,
  };
}
