import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

function ViewSubmittedAssignment() {
  const { assignmentId } = useParams();
  const [submissions, setSubmissions] = useState([]);
  const [message, setMessage] = useState("");

  const token = localStorage.getItem("token");

  // Fetch submitted assignments for this assignment
  useEffect(() => {
    const fetchSubmissions = async () => {
      try {
        const res = await fetch(
          `https://localhost:7214/lecturer/submitted/assignments/${assignmentId}`,
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        if (!res.ok) throw new Error("Failed to fetch submissions");

        const data = await res.json();
        setSubmissions(data);
      } catch (err) {
        console.error(err.message);
        setMessage("❌ Could not load submissions");
      }
    };

    fetchSubmissions();
  }, [assignmentId, token]);

  // Handle download
  const handleDownload = async (fileName) => {
    try {
      const res = await fetch(
        `https://localhost:7214/lecturer/download/${fileName}`,
        {
          method: "GET",
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      if (!res.ok) throw new Error("Failed to download file");

      const blob = await res.blob();
      const url = window.URL.createObjectURL(blob);

      const a = document.createElement("a");
      a.href = url;
      a.download = fileName;
      a.click();

      window.URL.revokeObjectURL(url);
    } catch (err) {
      console.error(err.message);
      setMessage("❌ Error downloading file");
    }
  };

  // Handle grading
  const handleGrade = async (submissionId) => {
    const grade = prompt("Enter grade for this submission:");

    if (grade === null || grade.trim() === "") {
      alert("Grade cannot be empty.");
      return;
    }

    try {
      const res = await fetch(
        "https://localhost:7214/lecturer/assignment/grade",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            submissionId,
            grade,
          }),
        }
      );

      if (!res.ok) throw new Error("Failed to submit grade");

      setMessage("✅ Grade submitted successfully!");
    } catch (err) {
      console.error(err.message);
      setMessage("❌ Failed to submit grade");
    }
  };

  return (
    <div className="max-w-5xl mx-auto mt-8 bg-white p-6 rounded shadow">
      <h2 className="text-2xl font-bold mb-4">
        Submitted Assignments for #{assignmentId}
      </h2>

      {message && <p className="mb-4 text-blue-600">{message}</p>}

      {submissions.length === 0 ? (
        <p>No submissions found.</p>
      ) : (
        <table className="w-full border-collapse">
          <thead>
            <tr className="bg-gray-100 text-left">
              <th className="p-2 border">Submission ID</th>
              <th className="p-2 border">User ID</th>
              <th className="p-2 border">Submission Date</th>
              <th className="p-2 border">File</th>
              <th className="p-2 border">Action</th>
            </tr>
          </thead>
          <tbody>
            {submissions.map((s) => (
              <tr key={s.submissionId}>
                <td className="p-2 border">{s.submissionId}</td>
                <td className="p-2 border">{s.userId || "N/A"}</td>
                <td className="p-2 border">
                  {s.submissionDate
                    ? new Date(s.submissionDate).toLocaleString()
                    : "N/A"}
                </td>
                <td className="p-2 border">
                  {s.fileName ? (
                    <button
                      onClick={() => handleDownload(s.fileName)}
                      className="bg-blue-600 text-white px-3 py-1 rounded hover:bg-blue-700"
                    >
                      ⬇ Download
                    </button>
                  ) : (
                    "No file"
                  )}
                </td>
                <td className="p-2 border">
                  <button
                    onClick={() => handleGrade(s.submissionId)}
                    className="bg-green-600 text-white px-3 py-1 rounded hover:bg-green-700"
                  >
                    Grade
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default ViewSubmittedAssignment;
