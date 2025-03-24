using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using UC3.Business;
using UC3.Data;
using UC3.Models;
using Xunit;

public class WorkoutServiceTests
{
    private readonly WorkoutService _service;
    private readonly Mock<IWorkoutContext> _mockContext;

    public WorkoutServiceTests()
    {
        // Mock de IWorkoutContext interface
        _mockContext = new Mock<IWorkoutContext>();

        // Maak de service met de gemockte context
        _service = new WorkoutService(_mockContext.Object);
    }

    [Fact]
    public async Task GetWorkoutsForUser_ReturnsUserWorkouts()
    {
        // Arrange
        int userId = 1;

        // Maak testdata
        var workouts = new List<Workout>
        {
            new Workout { workoutId = 1, userId = userId, typeWorkout = "Strength", workoutDate = DateOnly.FromDateTime(DateTime.Now) },
            new Workout { workoutId = 2, userId = userId, typeWorkout = "Cardio", workoutDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)) },
            new Workout { workoutId = 3, userId = 2, typeWorkout = "Other User Workout", workoutDate = DateOnly.FromDateTime(DateTime.Now) }
        }.AsQueryable();

        // Mock de DbSet
        var mockDbSet = new Mock<DbSet<Workout>>();
        mockDbSet.As<IQueryable<Workout>>().Setup(m => m.Provider).Returns(workouts.Provider);
        mockDbSet.As<IQueryable<Workout>>().Setup(m => m.Expression).Returns(workouts.Expression);
        mockDbSet.As<IQueryable<Workout>>().Setup(m => m.ElementType).Returns(workouts.ElementType);
        mockDbSet.As<IQueryable<Workout>>().Setup(m => m.GetEnumerator()).Returns(() => workouts.GetEnumerator());

        // Setup de context om de gemockte DbSet terug te geven
        _mockContext.Setup(c => c.WorkoutModels).Returns(mockDbSet.Object);

        // Act
        var result = await _service.GetWorkoutsForUser(userId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, workout => Assert.Equal(userId, workout.userId));
    }

    [Fact]
    public async Task GetWorkoutDetails_WithValidId_ReturnsWorkoutDetails()
    {
        // Arrange
        int workoutId = 1;
        int userId = 1;

        // Setup workout data
        var workout = new Workout
        {
            workoutId = workoutId,
            userId = userId,
            typeWorkout = "Strength",
            workoutDate = DateOnly.FromDateTime(DateTime.Now),
            comments = "Test workout"
        };

        var workouts = new List<Workout> { workout }.AsQueryable();

        // Setup training data
        var trainingDataList = new List<TrainingData>
        {
            new TrainingData
            {
                workoutId = workoutId,
                exerciseId = 1,
                amountOfSets = 3,
                amountOfReps = 10,
                liftedWeight = 100,
                e1RM = 133,
                pr = true
            },
            new TrainingData
            {
                workoutId = workoutId,
                exerciseId = 2,
                amountOfSets = 4,
                amountOfReps = 8,
                liftedWeight = 150,
                e1RM = 180,
                pr = false
            }
        }.AsQueryable();

        // Setup exercises
        var exercises = new List<Exercise>
        {
            new Exercise { exerciseId = 1, exerciseName = "Bench Press", muscleGroup = "Chest" },
            new Exercise { exerciseId = 2, exerciseName = "Squat", muscleGroup = "Legs" }
        }.AsQueryable();

        // Mock de DbSets
        var mockWorkoutDbSet = new Mock<DbSet<Workout>>();
        mockWorkoutDbSet.As<IQueryable<Workout>>().Setup(m => m.Provider).Returns(workouts.Provider);
        mockWorkoutDbSet.As<IQueryable<Workout>>().Setup(m => m.Expression).Returns(workouts.Expression);
        mockWorkoutDbSet.As<IQueryable<Workout>>().Setup(m => m.ElementType).Returns(workouts.ElementType);
        mockWorkoutDbSet.As<IQueryable<Workout>>().Setup(m => m.GetEnumerator()).Returns(() => workouts.GetEnumerator());

        var mockTrainingDataDbSet = new Mock<DbSet<TrainingData>>();
        mockTrainingDataDbSet.As<IQueryable<TrainingData>>().Setup(m => m.Provider).Returns(trainingDataList.Provider);
        mockTrainingDataDbSet.As<IQueryable<TrainingData>>().Setup(m => m.Expression).Returns(trainingDataList.Expression);
        mockTrainingDataDbSet.As<IQueryable<TrainingData>>().Setup(m => m.ElementType).Returns(trainingDataList.ElementType);
        mockTrainingDataDbSet.As<IQueryable<TrainingData>>().Setup(m => m.GetEnumerator()).Returns(() => trainingDataList.GetEnumerator());

        var mockExerciseDbSet = new Mock<DbSet<Exercise>>();
        mockExerciseDbSet.As<IQueryable<Exercise>>().Setup(m => m.Provider).Returns(exercises.Provider);
        mockExerciseDbSet.As<IQueryable<Exercise>>().Setup(m => m.Expression).Returns(exercises.Expression);
        mockExerciseDbSet.As<IQueryable<Exercise>>().Setup(m => m.ElementType).Returns(exercises.ElementType);
        mockExerciseDbSet.As<IQueryable<Exercise>>().Setup(m => m.GetEnumerator()).Returns(() => exercises.GetEnumerator());

        // Setup de context om de gemockte DbSets terug te geven
        _mockContext.Setup(c => c.WorkoutModels).Returns(mockWorkoutDbSet.Object);
        _mockContext.Setup(c => c.TrainingDataModels).Returns(mockTrainingDataDbSet.Object);
        _mockContext.Setup(c => c.ExerciseModels).Returns(mockExerciseDbSet.Object);

        // Act
        var result = await _service.GetWorkoutDetails(workoutId, userId);

        // Assert
        Assert.NotNull(result);
        dynamic dynamicResult = result;
        Assert.Equal(workoutId, dynamicResult.workoutId);
        Assert.Equal("Strength", dynamicResult.typeWorkout);
        Assert.Equal(workout.workoutDate.ToString("yyyy-MM-dd"), dynamicResult.workoutDate);
        Assert.Equal("Test workout", dynamicResult.comments);
        Assert.NotNull(dynamicResult.exercises);
        Assert.Equal(2, dynamicResult.exercises.Count);
    }

    [Fact]
    public async Task GetWorkoutDetails_WithInvalidId_ReturnsNull()
    {
        // Arrange
        int invalidWorkoutId = 999;
        int userId = 1;

        // Maak een lege collectie voor workouts
        var workouts = new List<Workout>().AsQueryable();

        // Mock de DbSet
        var mockDbSet = new Mock<DbSet<Workout>>();
        mockDbSet.As<IQueryable<Workout>>().Setup(m => m.Provider).Returns(workouts.Provider);
        mockDbSet.As<IQueryable<Workout>>().Setup(m => m.Expression).Returns(workouts.Expression);
        mockDbSet.As<IQueryable<Workout>>().Setup(m => m.ElementType).Returns(workouts.ElementType);
        mockDbSet.As<IQueryable<Workout>>().Setup(m => m.GetEnumerator()).Returns(() => workouts.GetEnumerator());

        // Setup de context om de gemockte DbSet terug te geven
        _mockContext.Setup(c => c.WorkoutModels).Returns(mockDbSet.Object);

        // Act
        var result = await _service.GetWorkoutDetails(invalidWorkoutId, userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SaveWorkout_CreatesWorkoutAndRelatedData()
    {
        // Arrange
        int userId = 1;

        // Maak een DTO met testdata
        var workoutDTO = new WorkoutDTO
        {
            typeWorkout = "Strength",
            workoutDate = DateTime.Now.ToString("yyyy-MM-dd"),
            comments = "Test workout",
            exercises = new List<ExerciseDTO>
            {
                new ExerciseDTO
                {
                    exerciseName = "Bench Press",
                    muscleGroup = "Chest",
                    trainingData = new TrainingDataDTO
                    {
                        amountOfSets = 3,
                        amountOfReps = 10,
                        liftedWeight = 100
                    }
                }
            }
        };

        // Collecties om de toegevoegde entiteiten op te slaan
        var workoutsCollection = new List<Workout>();
        var exercisesCollection = new List<Exercise>();
        var trainingDataCollection = new List<TrainingData>();

        // Mock de DbSets
        var mockWorkoutDbSet = new Mock<DbSet<Workout>>();
        mockWorkoutDbSet.Setup(d => d.Add(It.IsAny<Workout>())).Callback<Workout>(e => {
            e.workoutId = 1; // Simuleer auto-increment
            workoutsCollection.Add(e);
        });

        var mockExerciseDbSet = new Mock<DbSet<Exercise>>();
        mockExerciseDbSet.Setup(d => d.Add(It.IsAny<Exercise>())).Callback<Exercise>(e => {
            e.exerciseId = exercisesCollection.Count + 1; // Simuleer auto-increment
            exercisesCollection.Add(e);
        });

        // Voor de FirstOrDefaultAsync op Exercise
        var exercises = new List<Exercise>().AsQueryable();
        mockExerciseDbSet.As<IQueryable<Exercise>>().Setup(m => m.Provider).Returns(exercises.Provider);
        mockExerciseDbSet.As<IQueryable<Exercise>>().Setup(m => m.Expression).Returns(exercises.Expression);
        mockExerciseDbSet.As<IQueryable<Exercise>>().Setup(m => m.ElementType).Returns(exercises.ElementType);
        mockExerciseDbSet.As<IQueryable<Exercise>>().Setup(m => m.GetEnumerator()).Returns(() => exercises.GetEnumerator());

        var mockTrainingDataDbSet = new Mock<DbSet<TrainingData>>();
        mockTrainingDataDbSet.Setup(d => d.Add(It.IsAny<TrainingData>())).Callback<TrainingData>(e => {
            trainingDataCollection.Add(e);
        });

        // Voor de Where en ToListAsync op TrainingData
        var trainingData = new List<TrainingData>().AsQueryable();
        mockTrainingDataDbSet.As<IQueryable<TrainingData>>().Setup(m => m.Provider).Returns(trainingData.Provider);
        mockTrainingDataDbSet.As<IQueryable<TrainingData>>().Setup(m => m.Expression).Returns(trainingData.Expression);
        mockTrainingDataDbSet.As<IQueryable<TrainingData>>().Setup(m => m.ElementType).Returns(trainingData.ElementType);
        mockTrainingDataDbSet.As<IQueryable<TrainingData>>().Setup(m => m.GetEnumerator()).Returns(() => trainingData.GetEnumerator());

        // Setup de context om de gemockte DbSets terug te geven
        _mockContext.Setup(c => c.WorkoutModels).Returns(mockWorkoutDbSet.Object);
        _mockContext.Setup(c => c.ExerciseModels).Returns(mockExerciseDbSet.Object);
        _mockContext.Setup(c => c.TrainingDataModels).Returns(mockTrainingDataDbSet.Object);
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _service.SaveWorkout(workoutDTO, userId);

        // Assert
        Assert.Equal(1, result); // Should return the new workout ID
        Assert.Single(workoutsCollection);
        Assert.Equal(userId, workoutsCollection[0].userId);
        Assert.Equal(workoutDTO.typeWorkout, workoutsCollection[0].typeWorkout);
        Assert.Equal(workoutDTO.comments, workoutsCollection[0].comments);

        Assert.Single(exercisesCollection);
        Assert.Equal(workoutDTO.exercises[0].exerciseName, exercisesCollection[0].exerciseName);
        Assert.Equal(workoutDTO.exercises[0].muscleGroup, exercisesCollection[0].muscleGroup);

        Assert.Single(trainingDataCollection);
        Assert.Equal(1, trainingDataCollection[0].workoutId);
        Assert.Equal(1, trainingDataCollection[0].exerciseId);
        Assert.Equal(workoutDTO.exercises[0].trainingData.amountOfSets, trainingDataCollection[0].amountOfSets);
        Assert.Equal(workoutDTO.exercises[0].trainingData.amountOfReps, trainingDataCollection[0].amountOfReps);
        Assert.Equal(workoutDTO.exercises[0].trainingData.liftedWeight, trainingDataCollection[0].liftedWeight);
    }
}