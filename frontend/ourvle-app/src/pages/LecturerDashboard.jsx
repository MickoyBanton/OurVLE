import { useEffect, useState } from "react";
import Navbar from "../components/NavBar";
import CourseCard from "../components/CourseCard";


function StudentDashboard() {

  const [courses, setCourses] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    document.title = "Dashboard";

    const fetchCourses = async () => {
      try {
        const token = localStorage.getItem("token");

        const res = await fetch("https://localhost:7214/lecturer/course", {
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

        </div>

        {loading ? (
          <p>Loading courses...</p>
        ) : courses.length === 0 ? (
          <p>You are not lecturering in any courses yet.</p>
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
