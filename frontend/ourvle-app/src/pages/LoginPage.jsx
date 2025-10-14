import React, { useState, useContext, useEffect } from "react";
import { AuthContext } from "../context/AuthContext";

import { useNavigate } from "react-router-dom";


function LoginPage() {

  const [userId, setUserId] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isDisable, setIsDisable] = useState(false);

  const { login } = useContext(AuthContext);
  const navigate = useNavigate();

  useEffect(() => {
      document.title = "Login";
    });

  const handleSubmit = async (e) => {

    e.preventDefault();
    setIsDisable(true);
    
    try 
    {

      const response = await fetch("https://localhost:7214/auth/login", {

        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(
          {
            "UserID": userId, 
            "Password":password
          }),

      });

      if (response.ok) 
      {

        const { token } = await response.json();
        const user = login(token);
        

        if (user?.role === "student") {
          navigate("/student");
        } 

        else if (user?.role === "lecturer") {
          navigate("/lecturer");
        }

        else if (user?.role === "admin") {
          navigate("/admin");
        } 

      } 

      else {
        setError("Invalid ID or password");
      }


    } 
    catch (error) {
      setError("Something went wrong. Please try again.");
    }

    setIsDisable(false);

  };


  const handleIdChange = (e) => {
    setUserId(e.target.value);
  };


  const handlePasswordChange = (e) => {
    setPassword(e.target.value);
  };

   return (

    <div className="flex items-center justify-center min-h-screen bg-gray-100">

      <div className="bg-white shadow-lg rounded-2xl p-8 w-full max-w-md">

        <h2 className="text-2xl font-bold text-center text-gray-800 mb-6">
          Welcome Back 👋
        </h2>

        {error && (
          <p className="bg-red-100 text-red-600 p-2 rounded mb-4 text-sm text-center">
            {error}
          </p>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">

          <div>

            <label className="block text-gray-600 text-sm mb-1">ID</label>

            <input
              type="text"
              value={userId}
              onChange={handleIdChange}
              placeholder="Enter your ID"
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring focus:ring-blue-300"
              required
            />

          </div>
          
          <div>
          
            <label className="block text-gray-600 text-sm mb-1">Password</label>

            <input
              type="password"
              value={password}
              onChange={handlePasswordChange}
              placeholder="Enter your password"
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring focus:ring-blue-300"
              required
            />

          </div>

          <button
            type="submit"
            disabled={isDisable}
            className="w-full bg-blue-600 text-white py-2 rounded-lg font-semibold hover:bg-blue-700 transition duration-200"
          >
            Log In
          </button>

        </form>

        <p className="text-center text-sm text-gray-500 mt-6">

          Don’t have an account?{" "}
          <a href="/register" className="text-blue-600 hover:underline">
            Register
          </a>

        </p>

      </div>

    </div>
  );
}

export default LoginPage;
