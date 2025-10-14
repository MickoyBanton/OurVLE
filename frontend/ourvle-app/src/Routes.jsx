import React from "react";
import { Routes, Route } from "react-router-dom";
import LoginPage from "./pages/LoginPage";
import StudentDashboard from "./pages/StudentDashboard";
import LecturerDashboard from "./pages/LecturerDashboard";
import AdminDashboard  from "./pages/AdminDashboard";
import Unauthorized from "./pages/Unauthorized";
import RegisterStudentPage from "./pages/RegisterStudent";
import CoursePage from "./pages/CoursePage";
import AssignmentPage from "./pages/AssignmentPage";
import CourseRegistration from "./pages/CourseRegistration";
import CalendarEventForm from "./pages/CalendarEventForm";
import AssignmentForm from "./pages/AssignmentForm";
import ViewSubmittedAssignment from "./pages/ViewSubmittedAssignment";
import CreateCourse from "./pages/CreateCourse";

import ProtectedRoute from "./components/ProtectedRoute";


function AppRoutes() {
  return (
    <Routes>

      <Route path="/login" element={<LoginPage />} />

      <Route path="/unauthorized" element={<Unauthorized />} />
      <Route path="/register" element={<RegisterStudentPage />} />

      {/* Student Routes */}
      <Route
        path="/student"
        element={
          <ProtectedRoute
            element={StudentDashboard}
            requiredRoles={["student"]}
          />
        }
      />

      {/* Lecturer Routes */}
      <Route
        path="/lecturer"
        element={
          <ProtectedRoute
            element={LecturerDashboard}
            requiredRoles={["lecturer"]}
          />
        }
      />

      {/* Admin Routes */}
      <Route
        path="/admin"
        element={
          <ProtectedRoute
            element={AdminDashboard}
            requiredRoles={["admin"]}
          />
        }
      />


      {/* CoursePage */}
      <Route
        path="/courses/:code"
        element={
          <ProtectedRoute
            element={CoursePage}
            requiredRoles={["student","lecturer"]}
          />
        }
      />

      {/* AssignmentPage */}
      <Route
        path="/assignment/:assignmentId"
        element={
          <ProtectedRoute
            element={AssignmentPage}
            requiredRoles={["student"]}
          />
        }
      />

      {/* CourseRegistration Page */}
      <Route
        path="/course-registration"
        element={
          <ProtectedRoute
            element={CourseRegistration}
            requiredRoles={["student"]}
          />
        }
      />

      {/* CalendarEventForm Page */}
      <Route
        path="/courses/:code/calendar/create"
        element={
          <ProtectedRoute
            element={CalendarEventForm}
            requiredRoles={["lecturer"]}
          />
        }
      />

      {/* AssignmentForm Page */}
      <Route
        path="/courses/:code/assignment/create"
        element={
          <ProtectedRoute
            element={AssignmentForm}
            requiredRoles={["lecturer"]}
          />
        }
      />

      {/* ViewSubmittedAssignment Page */}
      <Route
        path="/lecturer/view-submitted/:assignmentId"
        element={
          <ProtectedRoute
            element={ViewSubmittedAssignment}
            requiredRoles={["lecturer"]}
          />
        }
      />

      
      {/* CreateCourse Page */}
      <Route
        path="/create-course"
        element={
          <ProtectedRoute
            element={CreateCourse}
            requiredRoles={["admin"]}
          />
        }
      />

    </Routes>
 
  );
}

export default AppRoutes;
