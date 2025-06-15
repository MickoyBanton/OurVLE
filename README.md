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

## 🚀 How to Run the Program

### 📦 Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- MySQL
- Postman (for API testing)
- Git

### 📁 Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/MickoyBanton/OurVLE
   cd OurVLE
   ```

2. **Configure the Database**
      
      Create a database using schema.sql
      
      Update the connection string in appsettings.json:
      ```bash
      "ConnectionStrings": {
      "DefaultConnection": "server=localhost;database=ourvle_db;user=root;password=your_password;"
      }
      ```


3. **Execute the SQL files**
    
      Insert data into the database using the insert.sql

4. **Build and Run the Application**
      ```bash
      dotnet build
      dotnet run
      ```
5. Test API with Postman
