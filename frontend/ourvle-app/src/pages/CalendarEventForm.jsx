import React, { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";

function CalendarEventForm() {
  const { code } = useParams(); // course code from URL
  const navigate = useNavigate();

  const [title, setTitle] = useState("");
  const [dueDate, setDueDate] = useState(null);
  const [message, setMessage] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!dueDate) {
      setMessage("❌ Please select a due date.");
      return;
    }

    try {
      const token = localStorage.getItem("token");
      console.log({
          courseId: code,
          dueDate: dueDate,
          title: title
        })

      const res = await fetch(`https://localhost:7214/lecturer/calendar`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          courseId: code,
          dueDate: dueDate,
          title: title
        }),
      });

      const result = await res.json().catch(() => null);

      if (!res.ok) {
        throw new Error(result?.message || "Failed to create event ❌");
      }

      setMessage(result?.message || "✅ Event created successfully!");

      // Redirect after a short delay
      setTimeout(() => {
        navigate(`/courses/${code}`);
      }, 1500);
    } catch (err) {
      setMessage(`❌ ${err.message}`);
    }
  };

  return (
    <div className="max-w-md mx-auto bg-white p-6 rounded shadow">
      <h2 className="text-2xl font-bold mb-4">Create Calendar Event</h2>

      {message && <p className="mb-4 text-blue-600">{message}</p>}

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block font-semibold mb-1">Event Title</label>
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
            className="w-full border px-3 py-2 rounded"
            placeholder="Enter event title"
          />
        </div>

        <div>
          <label className="block font-semibold mb-1">Due Date</label>
          <input
            type="date"
            value={dueDate}
            onChange={(e) => setDueDate(e.target.value)}
            required
            className="w-full border px-3 py-2 rounded"
          />
        </div>

        <button
          type="submit"
          className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700"
        >
          Create Event
        </button>
      </form>
    </div>
  );
}

export default CalendarEventForm;
