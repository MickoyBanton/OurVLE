import React, { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

function CreateSection() {
  const { code } = useParams();
  const navigate = useNavigate();
  const [sectionName, setSectionName] = useState("");
  const [message, setMessage] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!sectionName.trim()) {
      setMessage("❌ Section name is required.");
      return;
    }

    try {
      const token = localStorage.getItem("token");
      const res = await fetch("https://localhost:7214/lecturer/section", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          CourseId: code,
          SectionName: sectionName,
        }),
      });

      const result = await res.json().catch(() => null);

      if (!res.ok) throw new Error(result?.message || "Failed to create section");

      setMessage("✅ Section created successfully!");

      setTimeout(() => navigate(`/courses/${code}`), 1500);
    } catch (err) {
      setMessage(`❌ ${err.message}`);
    }
  };

  
  return (
    <div className="max-w-md mx-auto bg-white p-6 rounded shadow mt-10">
      <h2 className="text-2xl font-bold mb-4 text-center">Create New Section</h2>

      {message && <p className="mb-4 text-blue-600 text-center">{message}</p>}

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block font-semibold mb-1">Section Name</label>
          <input
            type="text"
            value={sectionName}
            onChange={(e) => setSectionName(e.target.value)}
            className="w-full border px-3 py-2 rounded"
            placeholder="Enter section name (e.g., Topic 8)"
          />
        </div>

        <button
          type="submit"
          className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700"
        >
          Create Section
        </button>
      </form>
    </div>
  );
}

export default CreateSection;
