using Microsoft.EntityFrameworkCore;

namespace OURVLEWebAPI.Entities;

public partial class OurvleContext : DbContext
{
    internal object Account;

    public OurvleContext()
    {
    }

    public OurvleContext(DbContextOptions<OurvleContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Calendarevent> Calendarevents { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CoursesWith50OrMoreStudent> CoursesWith50OrMoreStudents { get; set; }

    public virtual DbSet<Discussionforum> Discussionforums { get; set; }

    public virtual DbSet<Discussionthread> Discussionthreads { get; set; }

    public virtual DbSet<Grading> Gradings { get; set; }

    public virtual DbSet<Lecturer> Lecturers { get; set; }

    public virtual DbSet<LecturersTeaching3OrMoreCourse> LecturersTeaching3OrMoreCourses { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<Sectionitem> Sectionitems { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentsEnrolledIn5OrMoreCourse> StudentsEnrolledIn5OrMoreCourses { get; set; }

    public virtual DbSet<Submitassignment> Submitassignments { get; set; }

    public virtual DbSet<Top10HighestAverage> Top10HighestAverages { get; set; }

    public virtual DbSet<Top10MostEnrolledCourse> Top10MostEnrolledCourses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("account");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.AccountType).HasMaxLength(9);
            entity.Property(e => e.Password).HasMaxLength(100);
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("admin");

            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PRIMARY");

            entity.ToTable("assignment");

            entity.HasIndex(e => e.CourseId, "CourseID");

            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.AssignmentTitle).HasMaxLength(200);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Date).HasColumnType("date");

            entity.HasOne(d => d.Course).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("assignment_ibfk_1");
        });

        modelBuilder.Entity<Calendarevent>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PRIMARY");

            entity.ToTable("calendarevents");

            entity.HasIndex(e => e.CourseId, "CourseID");

            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.DueDate).HasColumnType("date");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Course).WithMany(p => p.Calendarevents)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("calendarevents_ibfk_1");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PRIMARY");

            entity.ToTable("course");

            entity.HasIndex(e => e.CourseId, "CourseID").IsUnique();

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName).HasMaxLength(100);

            entity.HasMany(d => d.Users).WithMany(p => p.Courses)
                .UsingEntity<Dictionary<string, object>>(
                    "Assigned",
                    r => r.HasOne<Student>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("assigned_ibfk_2"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("assigned_ibfk_1"),
                    j =>
                    {
                        j.HasKey("CourseId", "UserId").HasName("PRIMARY");
                        j.ToTable("assigned");
                        j.HasIndex(new[] { "UserId" }, "UserID");
                        j.IndexerProperty<ulong>("CourseId").HasColumnName("CourseID");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                    });

            entity.HasMany(d => d.UsersNavigation).WithMany(p => p.Courses)
                .UsingEntity<Dictionary<string, object>>(
                    "Teach",
                    r => r.HasOne<Lecturer>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("teach_ibfk_2"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("teach_ibfk_1"),
                    j =>
                    {
                        j.HasKey("CourseId", "UserId").HasName("PRIMARY");
                        j.ToTable("teach");
                        j.HasIndex(new[] { "UserId" }, "UserID");
                        j.IndexerProperty<ulong>("CourseId").HasColumnName("CourseID");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                    });
        });

        modelBuilder.Entity<CoursesWith50OrMoreStudent>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("courses_with_50_or_more_students");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName).HasMaxLength(100);
        });

        modelBuilder.Entity<Discussionforum>(entity =>
        {
            entity.HasKey(e => e.ForumId).HasName("PRIMARY");

            entity.ToTable("discussionforum");

            entity.HasIndex(e => e.CourseId, "CourseID");

            entity.Property(e => e.ForumId).HasColumnName("ForumID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Question).HasColumnType("text");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Course).WithMany(p => p.Discussionforums)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("discussionforum_ibfk_1");
        });

        modelBuilder.Entity<Discussionthread>(entity =>
        {
            entity.HasKey(e => e.ThreadId).HasName("PRIMARY");

            entity.ToTable("discussionthread");

            entity.HasIndex(e => e.ForumId, "ForumID");

            entity.HasIndex(e => e.ParentThreadId, "ParentThreadId");

            entity.HasIndex(e => e.UserId, "UserID");

            entity.Property(e => e.ThreadId).HasColumnName("ThreadID");
            entity.Property(e => e.ForumId).HasColumnName("ForumID");
            entity.Property(e => e.Message).HasColumnType("text");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Forum).WithMany(p => p.Discussionthreads)
                .HasForeignKey(d => d.ForumId)
                .HasConstraintName("discussionthread_ibfk_1");

            entity.HasOne(d => d.ParentThread).WithMany(p => p.InverseParentThread)
                .HasForeignKey(d => d.ParentThreadId)
                .HasConstraintName("discussionthread_ibfk_3");

            entity.HasOne(d => d.User).WithMany(p => p.Discussionthreads)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("discussionthread_ibfk_2");
        });

        modelBuilder.Entity<Grading>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PRIMARY");

            entity.ToTable("grading");

            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.Grade).HasPrecision(5);

            entity.HasOne(d => d.Submission).WithOne(p => p.Grading)
                .HasForeignKey<Grading>(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("grading_ibfk_1");
        });

        modelBuilder.Entity<Lecturer>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("lecturer");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
        });

        modelBuilder.Entity<LecturersTeaching3OrMoreCourse>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("lecturers_teaching_3_or_more_courses");

            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.SectionId).HasName("PRIMARY");

            entity.ToTable("section");

            entity.Property(e => e.SectionId).HasColumnName("SectionID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.SectionName).HasMaxLength(100);
        });

        modelBuilder.Entity<Sectionitem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PRIMARY");

            entity.ToTable("sectionitems");

            entity.HasIndex(e => e.ItemId, "ItemID").IsUnique();

            entity.HasIndex(e => e.SectionId, "SectionID");

            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.FileType).HasMaxLength(9);
            entity.Property(e => e.SectionId).HasColumnName("SectionID");
            entity.Property(e => e.SectionItem).HasColumnType("text");

            entity.HasOne(d => d.Section).WithMany(p => p.Sectionitems)
                .HasForeignKey(d => d.SectionId)
                .HasConstraintName("sectionitems_ibfk_1");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("student");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
        });

        modelBuilder.Entity<StudentsEnrolledIn5OrMoreCourse>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("students_enrolled_in_5_or_more_courses");

            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<Submitassignment>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PRIMARY");

            entity.ToTable("submitassignment");

            entity.HasIndex(e => e.AssignmentId, "AssignmentID");

            entity.HasIndex(e => e.UserId, "UserID");

            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.SubmissionDate).HasColumnType("date");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Assignment).WithMany(p => p.Submitassignments)
                .HasForeignKey(d => d.AssignmentId)
                .HasConstraintName("submitassignment_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Submitassignments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("submitassignment_ibfk_1");
        });

        modelBuilder.Entity<Top10HighestAverage>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("top_10_highest_average");

            entity.Property(e => e.AverageGrade).HasPrecision(6);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<Top10MostEnrolledCourse>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("top_10_most_enrolled_courses");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
