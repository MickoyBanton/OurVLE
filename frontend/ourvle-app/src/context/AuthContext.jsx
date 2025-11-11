import React, { createContext, useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";

export const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem("token");

    if (token) {
      try {
        const decoded = jwtDecode(token);
        const userRole =
          decoded.role ||
          decoded.Role ||
          decoded.roles ||
          decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

        setUser({ token, role: userRole });
      } catch (err) {
        console.error("Invalid token:", err);
        localStorage.removeItem("token");
      }
    }

    setLoading(false);


    
  }, []);

  const login = (token) => {
    localStorage.setItem("token", token);
    const decoded = jwtDecode(token);
    const userRole =
      decoded.role ||
      decoded.Role ||
      decoded.roles ||
      decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

    const newUser = { token, role: userRole };
    setUser(newUser);
    return newUser;
  };

  const logout = () => {
    localStorage.removeItem("token");
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}
