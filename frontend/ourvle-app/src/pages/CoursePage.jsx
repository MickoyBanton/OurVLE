import { useParams } from "react-router-dom";
import { useEffect, useState, useContext } from "react";
import Navbar from "../components/NavBar";
import { useNavigate } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";

function CoursePage() {

  const { user } = useContext(AuthContext);
  const { code } = useParams();
  const [activeTab, setActiveTab] = useState("members");
  const [members, setMembers] = useState(null);
  const [events, setEvents] = useState([]);
  const [assignments, setAssignments] = useState([]);
  const [sectionItems, setSectionItems] = useState([]);

  const navigate = useNavigate();

  useEffect(() => {
    document.title = `CourseId: ${code}`;

    const token = localStorage.getItem("token");

    const fetchMembers = async () => {
      try {
        const res = await fetch(`https://localhost:7214/courses/${code}/member`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        if (!res.ok) throw new Error("Failed to fetch course members");
        setMembers(await res.json());
      } catch (err) {
        console.error(err.message);
      }
    };

    const fetchCalendarEvents = async () => {
      try {
        const res = await fetch(`https://localhost:7214/calendar/${code}`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        if (!res.ok) throw new Error("Failed to fetch calendar events");
        setEvents(await res.json());
      } catch (err) {
        console.error(err.message);
      }
    };

    const fetchAssignments = async () => {
      try {
        const res = await fetch(`https://localhost:7214/courses/${code}/assignment`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        if (!res.ok) throw new Error("Failed to fetch assignments");
        setAssignments(await res.json());
      } catch (err) {
        console.error(err.message);
      }
    };

    const fetchSectionItems = async () => {
      try {
        const res = await fetch(`https://localhost:7214/courses/${code}/sectionitems`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        if (!res.ok) throw new Error("Failed to fetch section items");
        setSectionItems(await res.json());
        
      } catch (err) {
        console.error(err.message);
      }
    };

    console.log(sectionItems);

    fetchMembers();
    fetchCalendarEvents();
    fetchAssignments();
    fetchSectionItems();
  }, [code]);

  if (!members) return <p>Loading course...</p>;

  return (
    <>
      <Navbar />

      <div className="max-w-5xl mx-auto mt-8 px-4">
        {/* Tabs */}
        <div className="flex border-b border-gray-300 mb-4 flex-wrap">
          {["members", "assignments", "calendar", "sectionItems"].map((tab) => (
            <button
              key={tab}
              className={`px-4 py-2 font-semibold ${
                activeTab === tab ? "border-b-2 border-blue-500 text-blue-600" : "text-gray-600"
              }`}
              onClick={() => setActiveTab(tab)}
            >
              {tab === "members"
                ? "Members"
                : tab === "assignments"
                ? "Assignments"
                : tab === "calendar"
                ? "Calendar Events"
                : "Section Items"}
            </button>
          ))}
        </div>

        {/* Tab Content */}
        {activeTab === "members" && (
          <div>
            <h2 className="text-xl font-bold mb-2">Lecturer</h2>
            <ul className="mb-4">
              {members.lecturer.map((lec, idx) => (
                <li key={idx} className="p-2 bg-gray-100 rounded mb-2">
                  {lec.firstName} {lec.lastName}
                </li>
              ))}
            </ul>

            <h2 className="text-xl font-bold mb-2">Students</h2>
            <ul>
              {members.student.map((stu, idx) => (
                <li key={idx} className="p-2 bg-gray-50 rounded mb-2">
                  {stu.firstName} {stu.lastName}
                </li>
              ))}
            </ul>
          </div>
        )}

        {activeTab === "assignments" && (

          <div>
            <h2 className="text-xl font-bold mb-4">Assignments</h2>

            {/* Button only visible to lecturers */}

            {user.role === "lecturer" && (
              
            <button
              onClick={() => navigate(`/courses/${code}/assignment/create`)}
              className="mb-4 bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
            >
              ➕ Create Assignment
              
            </button>
           
            )}


            {assignments.length === 0 ? (
              <p>No assignments available.</p>
            ) : (
              <ul>
                {assignments.map((a) => (
                  <li
                    key={a.assignmentId}
                    className="p-3 border rounded mb-2 shadow-sm bg-white"

                    onClick={() => {
                              if (user.role === "lecturer") {
                                // Lecturer views submissions
                                navigate(`/lecturer/view-submitted/${a.assignmentId}`);
                              } else if (user.role === "student") {
                                // Student submits assignment
                                navigate(`/assignment/${a.assignmentId}/submit`);
                              }
                            }}

                    style={{ cursor: 'pointer' }}
                  >
                    <p className="font-semibold">{a.assignmentTitle}</p>
                    <p className="text-gray-500">
                      Date: {new Date(a.date).toLocaleDateString()}
                    </p>
                  </li>
                ))}
              </ul>
            )}
          </div>
        )}

        {activeTab === "calendar" && (

         <div>

            <h2 className="text-xl font-bold mb-4">Calendar Events</h2>

            {/* Button only visible to lecturers */}

            {user.role === "lecturer" && (
              
            <button
              onClick={() => navigate(`/courses/${code}/calendar/create`)}
              className="mb-4 bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
            >
              ➕ Create Event
            </button>
           
            )}

          {events.length === 0 ? (
            <p>No calendar events available.</p>
            ) : (
                <ul>
                {events.map((event) => (
                <li
                   key={event.eventId}
                   className="p-3 border rounded mb-2 shadow-sm bg-white"
                >
                    <p className="font-semibold">{event.title}</p>
                    <p className="text-gray-500">
                    Due: {new Date(event.dueDate).toLocaleDateString()}
                </p>
                </li>
                ))}
                </ul>
              )}
        </div>
      )}

      {
        /**[
    {
        "itemId": 1,
        "sectionId": 1,
        "sectionItem": "Chapter Notes PDF",
        "fileType": "Files",
        "section": null
    }
      ]**/
      }

        {activeTab === "sectionItems" && (
            <div>
              <div className="flex justify-between items-center mb-4">
                <h2 className="text-xl font-bold">Section Items</h2>

                {/* Button only visible to lecturers */}
                {user.role === "lecturer" && (
                  <button
                    onClick={() => navigate(`/create-section/${code}`)}
                    className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 shadow"
                  >
                    ➕ Create Section
                  </button>
                )}
              </div>

              {sectionItems.length === 0 ? (
                <p>No section items available.</p>
              ) : (
                <>
                  {/** Group items by sectionName **/}
                  {Object.entries(
                    sectionItems.reduce((groups, item) => {
                      const section = item.sectionName || "Uncategorized";
                      if (!groups[section]) groups[section] = [];
                      groups[section].push(item);
                      return groups;
                    }, {})
                  ).map(([sectionName, items]) => (
                    <div key={sectionName} className="mb-6">
                      <h3 className="text-lg font-semibold mb-2 text-blue-600">
                        {sectionName}
                      </h3>
                      <ul>
                        {items.map((item) => (
                          <li
                            key={item.itemId}
                            className="p-3 border rounded mb-2 shadow-sm bg-white"
                          >
                            <p className="font-medium">{item.sectionItem}</p>
                            <p className="text-sm text-gray-500">Type: {item.fileType}</p>
                          </li>
                        ))}
                      </ul>
                    </div>
                  ))}
                </>
              )}
            </div>
          )}



      </div>
    </>
  );
}

export default CoursePage;
