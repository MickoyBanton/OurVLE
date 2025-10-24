import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

function CreateSectionItem() {
  const { code } = useParams();
  const [sections, setSections] = useState([]);
  const [formData, setFormData] = useState({
    sectionId: "",
    sectionItem: "",
    fileType: "Files",
    file: null,
  });
  const [message, setMessage] = useState("");
  const navigate = useNavigate();


  useEffect(() => {
    const fetchSections = async () => {
      try {
        const token = localStorage.getItem("token");
        const res = await fetch(`https://localhost:7214/lecturer/sections/${code}`, {
          method: "GET",
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        if (!res.ok) {
          const errText = await res.text();
          throw new Error(errText || "Failed to fetch sections");
        }

        const data = await res.json();
        setSections(data);
      } catch (err) {
        setMessage(err.message);
      }
    };

    fetchSections();
  }, [code]);


  const handleChange = (e) => {
    const { name, value, files } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: files ? files[0] : value,
    }));
  };


  const handleSubmit = async (e) => {
    e.preventDefault();

    const token = localStorage.getItem("token");
    const form = new FormData();
    form.append("File", formData.file);
    form.append("FileType", formData.fileType);
    form.append("SectionItem", formData.sectionItem);
    form.append("SectionId", formData.sectionId);

    try {
      const res = await fetch("https://localhost:7214/lecturer/section/item", {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
        },
        body: form,
      });

      if (!res.ok) {
        const errText = await res.text();
        throw new Error(errText || "Failed to upload section item");
      }

      const data = await res.text();
      setMessage("Section item uploaded successfully!");
      setTimeout(() => navigate(`/courses/${code}`), 1500);
    } catch (err) {
      setMessage(err.message);
    }
  };

  return (
    <div className="p-6 max-w-lg mx-auto bg-white shadow rounded-lg">
      <h2 className="text-2xl font-bold mb-4">Create Section Item</h2>

      {message && (
        <div className="mb-4 text-center text-blue-600 font-semibold">{message}</div>
      )}

      <form onSubmit={handleSubmit} className="space-y-4">
        {/* Section selection */}
        <div>
          <label className="block font-semibold mb-1">Select Section</label>
          <select
            name="sectionId"
            value={formData.sectionId}
            onChange={handleChange}
            className="w-full border rounded p-2"
            required
          >
            <option value="">-- Choose a section --</option>
            {sections.map((s) => (
              <option key={s.sectionId} value={s.sectionId}>
                {s.sectionName}
              </option>
            ))}
          </select>
        </div>

        {/* Section item name */}
        <div>
          <label className="block font-semibold mb-1">Section Item Name</label>
          <input
            type="text"
            name="sectionItem"
            value={formData.sectionItem}
            onChange={handleChange}
            className="w-full border rounded p-2"
            placeholder="Enter section item name"
            required
          />
        </div>

        {/* File type */}
        <div>
          <label className="block font-semibold mb-1">File Type</label>
          <select
            name="fileType"
            value={formData.fileType}
            onChange={handleChange}
            className="w-full border rounded p-2"
          >
            <option value="Files">Files</option>
            <option value="Videos">Videos</option>
            <option value="Links">Links</option>
          </select>
        </div>

        {/* File upload */}
        <div>
          <label className="block font-semibold mb-1">Upload File</label>
          <input
            type="file"
            name="file"
            accept=".pdf,.docx,.pptx"
            onChange={handleChange}
            className="w-full"
            required
          />
        </div>

        {/* Submit */}
        <button
          type="submit"
          className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700"
        >
          Upload Section Item
        </button>
      </form>
    </div>
  );
}

export default CreateSectionItem;
