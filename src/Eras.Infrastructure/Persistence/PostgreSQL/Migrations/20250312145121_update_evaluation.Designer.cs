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
    [Migration("20250312145121_update_evaluation")]
    partial class update_evaluation
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CohortEntityStudentEntity", b =>
                {
                    b.Property<int>("CohortsId")
                        .HasColumnType("integer");

                    b.Property<int>("StudentsId")
                        .HasColumnType("integer");

                    b.HasKey("CohortsId", "StudentsId");

                    b.HasIndex("StudentsId");

                    b.ToTable("CohortEntityStudentEntity");
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.AnswerEntity", b =>
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
                        .HasColumnType("integer")
                        .HasColumnName("poll_instance_id");

                    b.Property<int>("PollVariableId")
                        .HasColumnType("integer")
                        .HasColumnName("poll_variable_id");

                    b.Property<int>("RiskLevel")
                        .HasColumnType("integer")
                        .HasColumnName("risk_level");

                    b.HasKey("Id");

                    b.HasAlternateKey("PollInstanceId", "PollVariableId", "AnswerText")
                        .HasName("Unique_PollInstanceId_PollVariableId_AnswerText");

                    b.HasIndex("PollVariableId");

                    b.ToTable("answers", (string)null);
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.CohortEntity", b =>
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

                    b.HasKey("Id");

                    b.ToTable("cohorts", (string)null);
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.ComponentEntity", b =>
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

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.EvaluationEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("end_date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("PollName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("poll_name");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start_date");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.ToTable("evaluation", (string)null);
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.PollEntity", b =>
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

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.PollInstanceEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("FinishedAt")
                        .HasColumnType("timestamp with time zone");

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

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.StudentDetailEntity", b =>
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

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.StudentEntity", b =>
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

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.VariableEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ComponentId")
                        .HasColumnType("integer")
                        .HasColumnName("component_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.HasIndex("ComponentId");

                    b.ToTable("variables", (string)null);
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Joins.EvaluationPollJoin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("EvaluationId")
                        .HasColumnType("integer")
                        .HasColumnName("evaluation_id");

                    b.Property<int>("PollId")
                        .HasColumnType("integer")
                        .HasColumnName("poll_id");

                    b.HasKey("Id");

                    b.HasIndex("EvaluationId");

                    b.HasIndex("PollId");

                    b.ToTable("evaluation_poll", (string)null);
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Joins.PollVariableJoin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("PollId")
                        .HasColumnType("integer")
                        .HasColumnName("poll_id");

                    b.Property<int>("VariableId")
                        .HasColumnType("integer")
                        .HasColumnName("variable_id");

                    b.HasKey("Id");

                    b.HasIndex("PollId");

                    b.HasIndex("VariableId");

                    b.ToTable("poll_variable", (string)null);
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Joins.StudentCohortJoin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CohortId")
                        .HasColumnType("integer")
                        .HasColumnName("cohort_id");

                    b.Property<int>("StudentId")
                        .HasColumnType("integer")
                        .HasColumnName("student_id");

                    b.HasKey("Id");

                    b.HasIndex("CohortId");

                    b.HasIndex("StudentId");

                    b.ToTable("student_cohort", (string)null);
                });

            modelBuilder.Entity("CohortEntityStudentEntity", b =>
                {
                    b.HasOne("Eras.Infrastructure.Persistence.PostgreSQL.Entities.CohortEntity", null)
                        .WithMany()
                        .HasForeignKey("CohortsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Eras.Infrastructure.Persistence.PostgreSQL.Entities.StudentEntity", null)
                        .WithMany()
                        .HasForeignKey("StudentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.AnswerEntity", b =>
                {
                    b.HasOne("Eras.Infrastructure.Persistence.PostgreSQL.Entities.PollInstanceEntity", "PollInstance")
                        .WithMany("Answers")
                        .HasForeignKey("PollInstanceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Eras.Infrastructure.Persistence.PostgreSQL.Joins.PollVariableJoin", "PollVariable")
                        .WithMany("Answers")
                        .HasForeignKey("PollVariableId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("AnswerEntityId")
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

                            b1.HasKey("AnswerEntityId");

                            b1.ToTable("answers");

                            b1.WithOwner()
                                .HasForeignKey("AnswerEntityId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("PollInstance");

                    b.Navigation("PollVariable");
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.CohortEntity", b =>
                {
                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("CohortEntityId")
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

                            b1.HasKey("CohortEntityId");

                            b1.ToTable("cohorts");

                            b1.WithOwner()
                                .HasForeignKey("CohortEntityId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.ComponentEntity", b =>
                {
                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("ComponentEntityId")
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

                            b1.HasKey("ComponentEntityId");

                            b1.ToTable("components");

                            b1.WithOwner()
                                .HasForeignKey("ComponentEntityId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.EvaluationEntity", b =>
                {
                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("EvaluationEntityId")
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

                            b1.HasKey("EvaluationEntityId");

                            b1.ToTable("evaluation");

                            b1.WithOwner()
                                .HasForeignKey("EvaluationEntityId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.PollEntity", b =>
                {
                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("PollEntityId")
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

                            b1.HasKey("PollEntityId");

                            b1.ToTable("polls");

                            b1.WithOwner()
                                .HasForeignKey("PollEntityId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.PollInstanceEntity", b =>
                {
                    b.HasOne("Eras.Infrastructure.Persistence.PostgreSQL.Entities.StudentEntity", "Student")
                        .WithMany("PollInstances")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("PollInstanceEntityId")
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

                            b1.HasKey("PollInstanceEntityId");

                            b1.ToTable("poll_instances");

                            b1.WithOwner()
                                .HasForeignKey("PollInstanceEntityId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("Student");
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.StudentDetailEntity", b =>
                {
                    b.HasOne("Eras.Infrastructure.Persistence.PostgreSQL.Entities.StudentEntity", "Student")
                        .WithOne("StudentDetail")
                        .HasForeignKey("Eras.Infrastructure.Persistence.PostgreSQL.Entities.StudentDetailEntity", "StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("StudentDetailEntityId")
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

                            b1.HasKey("StudentDetailEntityId");

                            b1.ToTable("student_details");

                            b1.WithOwner()
                                .HasForeignKey("StudentDetailEntityId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("Student");
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.StudentEntity", b =>
                {
                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("StudentEntityId")
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

                            b1.HasKey("StudentEntityId");

                            b1.ToTable("students");

                            b1.WithOwner()
                                .HasForeignKey("StudentEntityId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.VariableEntity", b =>
                {
                    b.HasOne("Eras.Infrastructure.Persistence.PostgreSQL.Entities.ComponentEntity", "Component")
                        .WithMany("Variables")
                        .HasForeignKey("ComponentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Eras.Domain.Common.AuditInfo", "Audit", b1 =>
                        {
                            b1.Property<int>("VariableEntityId")
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

                            b1.HasKey("VariableEntityId");

                            b1.ToTable("variables");

                            b1.WithOwner()
                                .HasForeignKey("VariableEntityId");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("Component");
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Joins.EvaluationPollJoin", b =>
                {
                    b.HasOne("Eras.Infrastructure.Persistence.PostgreSQL.Entities.EvaluationEntity", "Evaluation")
                        .WithMany("EvaluationPolls")
                        .HasForeignKey("EvaluationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Eras.Infrastructure.Persistence.PostgreSQL.Entities.PollEntity", "Poll")
                        .WithMany("EvaluationPolls")
                        .HasForeignKey("PollId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Evaluation");

                    b.Navigation("Poll");
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Joins.PollVariableJoin", b =>
                {
                    b.HasOne("Eras.Infrastructure.Persistence.PostgreSQL.Entities.PollEntity", "Poll")
                        .WithMany("PollVariables")
                        .HasForeignKey("PollId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Eras.Infrastructure.Persistence.PostgreSQL.Entities.VariableEntity", "Variable")
                        .WithMany("PollVariables")
                        .HasForeignKey("VariableId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Poll");

                    b.Navigation("Variable");
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Joins.StudentCohortJoin", b =>
                {
                    b.HasOne("Eras.Infrastructure.Persistence.PostgreSQL.Entities.CohortEntity", "Cohort")
                        .WithMany("StudentCohorts")
                        .HasForeignKey("CohortId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Eras.Infrastructure.Persistence.PostgreSQL.Entities.StudentEntity", "Student")
                        .WithMany("StudentCohorts")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cohort");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.CohortEntity", b =>
                {
                    b.Navigation("StudentCohorts");
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.ComponentEntity", b =>
                {
                    b.Navigation("Variables");
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.EvaluationEntity", b =>
                {
                    b.Navigation("EvaluationPolls");
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.PollEntity", b =>
                {
                    b.Navigation("EvaluationPolls");

                    b.Navigation("PollVariables");
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.PollInstanceEntity", b =>
                {
                    b.Navigation("Answers");
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.StudentEntity", b =>
                {
                    b.Navigation("PollInstances");

                    b.Navigation("StudentCohorts");

                    b.Navigation("StudentDetail");
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Entities.VariableEntity", b =>
                {
                    b.Navigation("PollVariables");
                });

            modelBuilder.Entity("Eras.Infrastructure.Persistence.PostgreSQL.Joins.PollVariableJoin", b =>
                {
                    b.Navigation("Answers");
                });
#pragma warning restore 612, 618
        }
    }
}
