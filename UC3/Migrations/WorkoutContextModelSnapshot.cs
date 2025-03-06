﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UC3.Data;

#nullable disable

namespace UC3.Migrations
{
    [DbContext(typeof(WorkoutContext))]
    partial class WorkoutContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.2");

            modelBuilder.Entity("UC3.Models.Exercise", b =>
                {
                    b.Property<int>("exerciseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("exerciseName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("muscleGroup")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("exerciseId");

                    b.ToTable("ExerciseModels");
                });

            modelBuilder.Entity("UC3.Models.TrainingData", b =>
                {
                    b.Property<int>("TrainingDataId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("amountOfReps")
                        .HasColumnType("INTEGER");

                    b.Property<int>("amountOfSets")
                        .HasColumnType("INTEGER");

                    b.Property<float>("e1RM")
                        .HasColumnType("REAL");

                    b.Property<int>("exerciseId")
                        .HasColumnType("INTEGER");

                    b.Property<float>("liftedWeight")
                        .HasColumnType("REAL");

                    b.Property<bool>("pr")
                        .HasColumnType("INTEGER");

                    b.Property<int>("workoutId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TrainingDataId");

                    b.ToTable("TrainingDataModels");
                });

            modelBuilder.Entity("UC3.Models.User", b =>
                {
                    b.Property<int>("userId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("RegisteryDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("bio")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("profilepicture")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("randomNumber")
                        .HasColumnType("INTEGER");

                    b.HasKey("userId");

                    b.ToTable("UserModels");
                });

            modelBuilder.Entity("UC3.Models.Workout", b =>
                {
                    b.Property<int>("workoutId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("comments")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("typeWorkout")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("userId")
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("workoutDate")
                        .HasColumnType("TEXT");

                    b.HasKey("workoutId");

                    b.ToTable("WorkoutModels");
                });
#pragma warning restore 612, 618
        }
    }
}
