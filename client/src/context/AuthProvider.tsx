import { useState, useEffect } from "react";
import type { ReactNode } from "react";
import agent from "../lib/api/agent";
import { AuthContext } from "./AuthContext";
import type { User } from "./AuthContext";

export default function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const checkAuth = async () => {
    try {
      const response = await agent.get("/auth/me");
      setUser(response.data);
    } catch {
      setUser(null);
    } finally {
      setIsLoading(false);
    }
  };

  const logout = async () => {
    try {
      await agent.post("/auth/logout");
    } catch (error) {
      console.error(error);
    } finally {
      setUser(null);
    }
  };

  useEffect(() => {
    checkAuth();
  }, []);

  return (
    <AuthContext.Provider
      value={{ user, isLoggedIn: !!user, isLoading, checkAuth, logout }}
    >
      {children}
    </AuthContext.Provider>
  );
}
