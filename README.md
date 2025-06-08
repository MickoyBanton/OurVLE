# OurVLE

## Features Implemented

### 👤 User Management

#### Register User
- Register as a student, lecturer, or admin

#### Login User
- Authenticate users with credentials

---

### 📚 Course Management

#### Create Course
- Admins can create new courses

#### Retrieve Courses
- Retrieve all courses  
- Retrieve courses registered by a student  
- Retrieve courses taught by a lecturer

#### Register for Course
- Students can register for courses  
- Only one lecturer per course

#### Retrieve Members
- Get all members (students + lecturer) in a course

---

### 📆 Calendar Events

#### Retrieve Events
- All events for a course  
- Events by date for a specific student

#### Create Event
- Add events to a course's calendar

---

### 💬 Forums & Discussions

#### Forums
- List all forums for a course  
- Create new forums

#### Discussion Threads
- Retrieve threads in a forum  
- Add new threads with title and initial post  
- Reply to threads (supports nested replies like Reddit)

---

### 📄 Course Content

#### Add Content
- Lecturers can upload links, files, and slides

#### Organize by Section
- Course content organized into sections

#### Retrieve Content
- Retrieve all content by course

---

### 📝 Assignments

#### Submit Assignment
- Students can upload assignments

#### Grade Assignment
- Lecturers can assign grades  
- Grades affect final averages

---

### 📊 Reports (SQL Views)

- Courses with ≥50 students  
- Students enrolled in ≥5 courses  
- Lecturers teaching ≥3 courses  
- Top 10 most enrolled courses  
- Top 10 students by overall average
