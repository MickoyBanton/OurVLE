import { useEffect, useState } from "react";

function CourseRegistration() {
  
  const [courses, setCourses] = useState([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedCourse, setSelectedCourse] = useState(null);
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    document.title = "Course Registration";

    const fetchCourses = async () => {
      try {
        const token = localStorage.getItem("token");
        const res = await fetch("https://localhost:7214/courses", {
          headers: { Authorization: `Bearer ${token}` },
        });

        if (!res.ok) {
          throw new Error("Failed to fetch courses");
        }

        const data = await res.json();
        setCourses(data);
      } catch (err) {
        console.error(err.message);
        setMessage("❌ Error loading courses.");
      }
    };

    fetchCourses();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!selectedCourse) {
      setMessage("⚠️ Please select a course.");
      return;
    }

    try {
      setLoading(true);
      setMessage("");

      const token = localStorage.getItem("token");
      const userId = localStorage.getItem("userId");

      const res = await fetch(
        `https://localhost:7214/student/course/${selectedCourse.courseId}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            UserID: userId,
          }),
        }
      );

      const result = await res.json().catch(() => null);

      if (!res.ok) {
        throw new Error(result?.message || "Registration failed");
      }

      setMessage(`✅ ${result?.message || "Course registered successfully!"}`);
      setSearchTerm("");
      setSelectedCourse(null);
    } catch (err) {
      setMessage(`❌ ${err.message}`);
    } finally {
      setLoading(false);
    }
  };

  // Filtered course suggestions
  const filteredCourses = courses.filter((course) =>
    course.courseName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="max-w-lg mx-auto mt-10 p-6 border rounded shadow bg-white">
      <h1 className="text-2xl font-bold mb-4">Course Registration</h1>

      <form onSubmit={handleSubmit}>
        <label className="block mb-2 font-semibold">Search Course</label>
        <input
          type="text"
          value={searchTerm}
          onChange={(e) => {
            setSearchTerm(e.target.value);
            setSelectedCourse(null); // reset selection if typing again
          }}
          placeholder="Type course name..."
          className="w-full p-2 border rounded mb-2"
        />

        {/* Suggestions dropdown */}
        {searchTerm && filteredCourses.length > 0 && (
          <ul className="border rounded bg-white max-h-40 overflow-y-auto mb-4">
            {filteredCourses.map((course) => (
              <li
                key={course.courseId}
                className={`p-2 cursor-pointer hover:bg-blue-100 ${
                  selectedCourse?.courseId === course.courseId
                    ? "bg-blue-200"
                    : ""
                }`}
                onClick={() => {
                  setSelectedCourse(course);
                  setSearchTerm(course.courseName);
                }}
              >
                {course.courseName}
              </li>
            ))}
          </ul>
        )}

        <button
          type="submit"
          disabled={loading}
          className="w-full bg-blue-600 text-white py-2 px-4 rounded hover:bg-blue-700"
        >
          {loading ? "Registering..." : "Register"}
        </button>
      </form>

      {message && (
        <p
          className={`mt-4 font-semibold ${
            message.startsWith("✅") ? "text-green-600" : "text-red-600"
          }`}
        >
          {message}
        </p>
      )}
    </div>
  );
}

export default CourseRegistration;
