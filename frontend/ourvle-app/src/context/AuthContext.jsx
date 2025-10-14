// src/context/AuthContext.jsx
import React, { createContext, useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";


export const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);

  useEffect(() => {

    const token = localStorage.getItem("token");
    
    if (token) {

      try {

        const decoded = jwtDecode(token);
        setUser({ token, role: decoded.role });

      } 
      catch (err) {
        console.error("Invalid token:", err);
        localStorage.removeItem("token");

      }

    }
  }, []);

  const login = (token) => {

    localStorage.setItem("token", token);
    const decoded = jwtDecode(token);
    
    const userRole =
      decoded.role ||
      decoded.Role ||
      decoded.roles ||
      decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

    console.log("Extracted role:", userRole);
    const newUser = { token, role: userRole };
    setUser(newUser);
    return newUser;

  };


  const logout = () => {

    localStorage.removeItem("token");
    setUser(null);

  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}
