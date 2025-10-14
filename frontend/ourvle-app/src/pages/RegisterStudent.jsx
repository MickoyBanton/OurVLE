import React, { useEffect, useState } from "react";
import Navbar from "../components/NavBar";

import { useNavigate } from "react-router-dom";


function RegisterStudentPage() {

  const [userId, setUserId] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [error, setError] = useState("");
  const [isDisable, setIsDisable] = useState(false);

  const navigate = useNavigate();

  useEffect(() => {
    document.title = "Register Student";
  });


  const handleSubmit = async (e) => {

    e.preventDefault();
    setIsDisable(true);
    
    try 
    {

      const response = await fetch("https://localhost:7214/student", {

        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(
          {
            "userID": userId, 
            "firstName": firstName,
            "lastName": lastName
          }),

      });

      if (response.ok) 
      {
        navigate("/login");
      } 

      else {
        setError("User already exist");
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

  const handleFirstNameChange = (e) => {
    setFirstName(e.target.value);
  };


  const handleLastNameChange = (e) => {
    setLastName(e.target.value);
  };

  return (

    <div className="flex items-center justify-center min-h-screen bg-gray-100">

      <div className="bg-white shadow-lg rounded-2xl p-8 w-full max-w-md">

        <h2 className="text-2xl font-bold text-center text-gray-800 mb-6">
          Student Sign Up
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
          
            <label className="block text-gray-600 text-sm mb-1">Firstname</label>

            <input
              type="text"
              value={firstName}
              onChange={handleFirstNameChange}
              placeholder="Enter your Firstname"
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring focus:ring-blue-300"
              required
            />

          </div>


          <div>
          
            <label className="block text-gray-600 text-sm mb-1">Lastname</label>

            <input
              type="text"
              value={lastName}
              onChange={handleLastNameChange}
              placeholder="Enter your Lastname"
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring focus:ring-blue-300"
              required
            />

          </div>

          <button
            type="submit"
            disabled={isDisable}
            className="w-full bg-blue-900 text-white py-2 rounded-lg font-semibold hover:bg-blue-700 transition duration-200"
          >
            Register
          </button>

        </form>

        <p className="text-center text-sm text-gray-500 mt-6">

        </p>

      </div>

    </div>

  );
}

export default RegisterStudentPage;
