import { useEffect, useState } from "react";
import Navbar from "../components/NavBar";
import CourseCard from "../components/CourseCard";
import { useNavigate } from "react-router-dom";

function StudentDashboard() {
  const [courses, setCourses] = useState([]);
  const [loading, setLoading] = useState(true);

  const navigate = useNavigate();

  useEffect(() => {
    document.title = "Dashboard";

    const fetchCourses = async () => {
      try {
        const token = localStorage.getItem("token");

        const res = await fetch("https://localhost:7214/student/course", {
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

    fetchCourses();
  }, []);

  return (
    <>
      <Navbar />

      <div className="p-6">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold">My Courses</h1>

          {/* Register button */}
          <button
            onClick={() => navigate("/course-registration")}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg shadow hover:bg-blue-700"
          >
            Register for a Course
          </button>
        </div>

        {loading ? (
          <p>Loading courses...</p>
        ) : courses.length === 0 ? (
          <p>You are not enrolled in any courses yet.</p>
        ) : (
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {courses.map((course) => (
              <CourseCard
                key={course.id}
                title={course.courseName}
                code={course.id}
              />
            ))}
          </div>
        )}
      </div>
    </>
  );
}

export default StudentDashboard;
