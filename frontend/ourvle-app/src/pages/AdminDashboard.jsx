import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

function AdminDashboard() {
  const [courses, setCourses] = useState([]);
  const [loading, setLoading] = useState(true);

  const navigate = useNavigate();

  useEffect(() => {
    document.title = "Dashboard";

    const fetchAllCourses = async () => {
      try {
        const token = localStorage.getItem("token");

        const res = await fetch("https://localhost:7214/courses", {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        });

        if (!res.ok) {
          throw new Error("Failed to fetch courses");
        }

        const data = await res.json();
        setCourses(data);
        console.log(data);
      } catch (err) {
        console.error(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchAllCourses();
  }, []);

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">All Courses</h1>

        {/* Create course button */}
        <button
          onClick={() => navigate("/create-course")}
          className="px-4 py-2 bg-blue-600 text-white rounded-lg shadow hover:bg-blue-700"
        >
          Create Course
        </button>
      </div>

      {loading ? (
        <p>Loading courses...</p>
      ) : courses.length === 0 ? (
        <p>There are no courses.</p>
      ) : (
        <div className="space-y-4">
          {courses.map((course) => (
            <div
              key={course.courseId}
              className="flex justify-between items-center bg-white shadow-sm border rounded-lg p-4 hover:bg-gray-50 transition"
            >
              <div>
                <h2 className="text-lg font-semibold text-gray-800">
                  {course.courseName}
                </h2>
                <p className="text-gray-500 text-sm">
                  {course.courseDescription || "No description available."}
                </p>
                <p className="text-gray-400 text-xs mt-1">
                  Course ID: {course.courseId}
                </p>
              </div>

              <button
                onClick={() => navigate(`/course/${course.courseId}`)}
                className="px-3 py-2 bg-green-600 text-white rounded hover:bg-green-700"
              >
                View Details
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default AdminDashboard;
