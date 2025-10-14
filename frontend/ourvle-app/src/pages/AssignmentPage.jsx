import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";

function AssignmentPage() {

  const { assignmentId } = useParams();
  const [file, setFile] = useState(null);
  const [message, setMessage] = useState("");
  const [uploading, setUploading] = useState(false);
  const [isDisable, setIsDisable] = useState(false);

  useEffect(() => {
    document.title = "Submit Assignment";
  }, []);

  const handleFileChange = (e) => {
    setFile(e.target.files[0]);
  };


  const formData = new FormData();
    formData.append("File", file);
    formData.append("AssignmentId", assignmentId);
    formData.append("SubmissionDate",  new Date());

  const handleSubmit = async () => {
  if (!file) {
    setMessage("Please select a file before submitting.");
    return;
  }

  const formData = new FormData();
  formData.append("File", file);

  try {
    setUploading(true);
    setMessage("");

    const token = localStorage.getItem("token");
    const res = await fetch("https://localhost:7214/student/assignments/submit", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body: formData,
    });

    if (!res.ok) {
      // Try to parse backend error message
      const errorData = await res.json().catch(() => null);
      const errorMsg =
        errorData?.message ||
        errorData?.error ||
        `Failed with status ${res.status}`;
      throw new Error(errorMsg);
    }

    setMessage("✅ Assignment submitted successfully!");
  } catch (err) {
    setMessage(`❌ Error: ${err.message}`);
    console.error(err.message);
  } finally {
    setUploading(false);
  }
};


  return (
    <div className="max-w-lg mx-auto mt-10 p-6 border rounded-lg shadow-md bg-white">
      <h1 className="text-2xl font-bold mb-4">Submit Assignment</h1>

      <input
        type="file"
        onChange={handleFileChange}
        className="mb-4 block w-full text-gray-700"
      />

      <button
        onClick={handleSubmit}
        disabled={uploading}
        className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50"
      >
        {uploading ? "Submitting..." : "Submit Assignment"}
      </button>

      {message && <p className="mt-4 text-gray-700">{message}</p>}
    </div>
  );
}

export default AssignmentPage;
