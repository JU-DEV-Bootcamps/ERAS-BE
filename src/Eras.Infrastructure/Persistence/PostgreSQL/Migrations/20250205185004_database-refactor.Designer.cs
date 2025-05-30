﻿// <auto-generated />
using System;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250205185004_database-refactor")]
    partial class DatabaseRefactor
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Eras.Domain.Entities.Answer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AnswerText")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("answer_text");

                    b.Property<int>("PollInstanceId")
                        .HasColumnType("integer");

                    b.Property<int>("RiskLevel")
                        .HasColumnType("integer")
                        .HasColumnName("risk_level");

                    b.HasKey("Id");

                    b.HasIndex("PollInstanceId");

                    b.ToTable("answers", (string)null);
                });

            modelBuilder.Entity("Eras.Domain.Entities.Cohort", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CourseCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("course_name");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int?>("VariableId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("VariableId");

                    b.ToTable("cohorts", (string)null);
                });

            modelBuilder.Entity("Eras.Domain.Entities.Component", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("components", (string)null);
                });

            modelBuilder.Entity("Eras.Domain.Entities.Poll", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Uuid")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("uuid");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("version");

                    b.HasKey("Id");

                    b.ToTable("polls", (string)null);
                });

            modelBuilder.Entity("Eras.Domain.Entities.PollInstance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("StudentId")
                        .HasColumnType("integer");

                    b.Property<string>("Uuid")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("uuid");

                    b.HasKey("Id");

                    b.HasIndex("StudentId");

                    b.ToTable("poll_instances", (string)null);
                });

            modelBuilder.Entity("Eras.Domain.Entities.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Uuid")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("uuid");

                    b.HasKey("Id");

                    b.ToTable("students", (string)null);
                });

            modelBuilder.Entity("Eras.Domain.Entities.StudentDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("AvgScore")
                        .HasColumnType("numeric")
                        .HasColumnName("avg_score");

                    b.Property<decimal>("CoursesUnderAvg")
                        .HasColumnType("numeric")
                        .HasColumnName("courses_under_avg");

                    b.Property<int>("EnrolledCourses")
                        .HasColumnType("integer")
                        .HasColumnName("enrolled_courses");

                    b.Property<int>("GradedCourses")
                        .HasColumnType("integer")
                        .HasColumnName("graded_courses");

                    b.Property<int>("LastAccessDays")
                        .HasColumnType("integer")
                        .HasColumnName("last_access_days");

                    b.Property<decimal>("PureScoreDiff")
                        .HasColumnType("numeric")
                        .HasColumnName("pure_score_diff");

                    b.Property<decimal>("StandardScoreDiff")
                        .HasColumnType("numeric")
                        .HasColumnName("standard_score_diff");

                    b.Property<int>("StudentId")
                        .HasColumnType("integer");

                    b.Property<int>("TimeDeliveryRate")
                        .HasColumnType("integer")
                        .HasColumnName("time_delivery_rate");

                    b.HasKey("Id");

                    b.HasIndex("StudentId")
                        .IsUnique();

                    b.ToTable("student_details", (string)null);
                });

            modelBuilder.Entity("Eras.Domain.Entities.Variable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ComponentId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.HasIndex("ComponentId");

                    b.ToTable("variables", (string)null);
                });

            modelBuilder.Entity("poll_variable", b =>
                {
                    b.Property<int>("poll_id")
                        .HasColumnType("integer");

                    b.Property<int>("variable_id")
                        .HasColumnType("integer");

                    b.HasKey("poll_id", "variable_id");

                    b.HasIndex("variable_id");

                    b.ToTable("poll_variable");
                });

            modelBuilder.Entity("student_cohort", b =>
                {
                    b.Property<int>("student_id")
                        .HasColumnType("integer");

                    b.Property<int>("cohort_id")
                        .HasColumnType("integer");

                    b.HasKey("student_id", "cohort_id");

                    b.HasIndex("cohort_id");

                    b.ToTable("student_cohort");
                });

            modelBuilder.Entity("Eras.Domain.Entities.Answer", b =>
                {
                    b.HasOne("Eras.Domain.Entities.PollInstance", "PollInstance")
                        .WithMany("Answers")
                        .HasForeignKey("PollInstanceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("AnswerId")
                                .HasColumnType("integer");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<string>("CreatedBy")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("created_by");

                            b1.Property<DateTime?>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.Property<string>("ModifiedBy")
                                .HasColumnType("text")
                                .HasColumnName("modified_by");

                            b1.HasKey("AnswerId");

                            b1.ToTable("answers");

                            b1.WithOwner()
                                .HasForeignKey("AnswerId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("PollInstance");
                });

            modelBuilder.Entity("Eras.Domain.Entities.Cohort", b =>
                {
                    b.HasOne("Eras.Domain.Entities.Variable", null)
                        .WithMany("Cohorts")
                        .HasForeignKey("VariableId");

                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("CohortId")
                                .HasColumnType("integer");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<string>("CreatedBy")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("created_by");

                            b1.Property<DateTime?>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.Property<string>("ModifiedBy")
                                .HasColumnType("text")
                                .HasColumnName("modified_by");

                            b1.HasKey("CohortId");

                            b1.ToTable("cohorts");

                            b1.WithOwner()
                                .HasForeignKey("CohortId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();
                });

            modelBuilder.Entity("Eras.Domain.Entities.Component", b =>
                {
                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("ComponentId")
                                .HasColumnType("integer");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<string>("CreatedBy")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("created_by");

                            b1.Property<DateTime?>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.Property<string>("ModifiedBy")
                                .HasColumnType("text")
                                .HasColumnName("modified_by");

                            b1.HasKey("ComponentId");

                            b1.ToTable("components");

                            b1.WithOwner()
                                .HasForeignKey("ComponentId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();
                });

            modelBuilder.Entity("Eras.Domain.Entities.Poll", b =>
                {
                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("PollId")
                                .HasColumnType("integer");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<string>("CreatedBy")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("created_by");

                            b1.Property<DateTime?>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.Property<string>("ModifiedBy")
                                .HasColumnType("text")
                                .HasColumnName("modified_by");

                            b1.HasKey("PollId");

                            b1.ToTable("polls");

                            b1.WithOwner()
                                .HasForeignKey("PollId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();
                });

            modelBuilder.Entity("Eras.Domain.Entities.PollInstance", b =>
                {
                    b.HasOne("Eras.Domain.Entities.Student", "Student")
                        .WithMany("PollInstances")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("PollInstanceId")
                                .HasColumnType("integer");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<string>("CreatedBy")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("created_by");

                            b1.Property<DateTime?>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.Property<string>("ModifiedBy")
                                .HasColumnType("text")
                                .HasColumnName("modified_by");

                            b1.HasKey("PollInstanceId");

                            b1.ToTable("poll_instances");

                            b1.WithOwner()
                                .HasForeignKey("PollInstanceId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("Student");
                });

            modelBuilder.Entity("Eras.Domain.Entities.Student", b =>
                {
                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("StudentId")
                                .HasColumnType("integer");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<string>("CreatedBy")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("created_by");

                            b1.Property<DateTime?>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.Property<string>("ModifiedBy")
                                .HasColumnType("text")
                                .HasColumnName("modified_by");

                            b1.HasKey("StudentId");

                            b1.ToTable("students");

                            b1.WithOwner()
                                .HasForeignKey("StudentId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();
                });

            modelBuilder.Entity("Eras.Domain.Entities.StudentDetail", b =>
                {
                    b.HasOne("Eras.Domain.Entities.Student", "Student")
                        .WithOne("StudentDetail")
                        .HasForeignKey("Eras.Domain.Entities.StudentDetail", "StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("StudentDetailId")
                                .HasColumnType("integer");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<string>("CreatedBy")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("created_by");

                            b1.Property<DateTime?>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.Property<string>("ModifiedBy")
                                .HasColumnType("text")
                                .HasColumnName("modified_by");

                            b1.HasKey("StudentDetailId");

                            b1.ToTable("student_details");

                            b1.WithOwner()
                                .HasForeignKey("StudentDetailId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("Student");
                });

            modelBuilder.Entity("Eras.Domain.Entities.Variable", b =>
                {
                    b.HasOne("Eras.Domain.Entities.Component", "Component")
                        .WithMany("Variables")
                        .HasForeignKey("ComponentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("VariableId")
                                .HasColumnType("integer");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<string>("CreatedBy")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("created_by");

                            b1.Property<DateTime?>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.Property<string>("ModifiedBy")
                                .HasColumnType("text")
                                .HasColumnName("modified_by");

                            b1.HasKey("VariableId");

                            b1.ToTable("variables");

                            b1.WithOwner()
                                .HasForeignKey("VariableId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("Component");
                });

            modelBuilder.Entity("poll_variable", b =>
                {
                    b.HasOne("Eras.Domain.Entities.Poll", null)
                        .WithMany()
                        .HasForeignKey("poll_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Eras.Domain.Entities.Variable", null)
                        .WithMany()
                        .HasForeignKey("variable_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("student_cohort", b =>
                {
                    b.HasOne("Eras.Domain.Entities.Cohort", null)
                        .WithMany()
                        .HasForeignKey("cohort_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Eras.Domain.Entities.Student", null)
                        .WithMany()
                        .HasForeignKey("student_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Eras.Domain.Entities.Component", b =>
                {
                    b.Navigation("Variables");
                });

            modelBuilder.Entity("Eras.Domain.Entities.PollInstance", b =>
                {
                    b.Navigation("Answers");
                });

            modelBuilder.Entity("Eras.Domain.Entities.Student", b =>
                {
                    b.Navigation("PollInstances");

                    b.Navigation("StudentDetail");
                });

            modelBuilder.Entity("Eras.Domain.Entities.Variable", b =>
                {
                    b.Navigation("Cohorts");
                });
#pragma warning restore 612, 618
        }
    }
}
