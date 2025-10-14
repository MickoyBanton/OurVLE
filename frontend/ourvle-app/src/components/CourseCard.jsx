import React from "react";
import { useNavigate } from "react-router-dom";

function CourseCard({ title, code }) {

    const navigate = useNavigate();

  return (
    <div className="bg-white rounded-2xl shadow-md p-6 w-full md:w-80 hover:shadow-lg transition">
      {/* Course Code / Tag */}
      <div className="text-sm font-semibold text-blue-900 mb-2">
        {code}
      </div>

      {/* Course Title */}
      <h2 className="text-xl font-bold text-gray-900 mb-2">
        {title}
      </h2>


      {/* Action Button */}
      <button className="w-full bg-blue-900 text-white py-2 px-4 rounded-lg hover:bg-blue-700 transition"
        onClick={() => navigate(`/courses/${code}`)}
      >
        View Course
      </button>
    </div>
  );
}

export default CourseCard;
