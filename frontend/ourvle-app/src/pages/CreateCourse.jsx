import { useState } from "react";
import { useNavigate } from "react-router-dom";

function CreateCourse() {
  const [courseName, setCourseName] = useState("");
  const [lecturerId, setLecturerId] = useState("");
  const [message, setMessage] = useState("");

  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const token = localStorage.getItem("token");

      const res = await fetch("https://localhost:7214/admin", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          CourseName: courseName,
          LecturerId: lecturerId, 
        }),
      });

      const result = await res.json().catch(() => null);

      if (!res.ok) {
        throw new Error(result?.message || "❌ Failed to create course");
      }

      setMessage("✅ Course created successfully!");

      // Redirect back after success
      setTimeout(() => {
        navigate("/admin-dashboard");
      }, 1500);
    } catch (err) {
      setMessage(`❌ ${err.message}`);
    }
  };

  return (
    <div className="max-w-md mx-auto mt-10 bg-white p-6 rounded-lg shadow">
      <h2 className="text-2xl font-bold mb-4 text-center">Create Course</h2>

      {message && <p className="text-center mb-4 text-blue-600">{message}</p>}

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block font-semibold mb-1">Course Name</label>
          <input
            type="text"
            value={courseName}
            onChange={(e) => setCourseName(e.target.value)}
            required
            className="w-full border px-3 py-2 rounded"
            placeholder="Enter course name"
          />
        </div>

        <div>
          <label className="block font-semibold mb-1">Lecturer ID</label>
          <input
            type="text"
            value={lecturerId}
            onChange={(e) => setLecturerId(e.target.value)}
            required
            className="w-full border px-3 py-2 rounded"
            placeholder="Enter Lecturer Id"
          />
        </div>

        <button
          type="submit"
          className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700"
        >
          Create Course
        </button>
      </form>
    </div>
  );
}

export default CreateCourse;
