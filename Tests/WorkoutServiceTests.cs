using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using UC3.Business;
using UC3.Data;
using UC3.Models;
using Xunit;

public class WorkoutServiceTests
{
    private readonly WorkoutService _service;
    private readonly Mock<WorkoutContext> _mockContext;

    public WorkoutServiceTests()
    {
        // Mock the WorkoutContext
        _mockContext = new Mock<WorkoutContext>();

        // Create the service with mocked context
        _service = new WorkoutService(_mockContext.Object);
    }

    [Fact]
    public async Task GetWorkoutsForUser_ReturnsUserWorkouts()
    {
        // Arrange
        int userId = 1;
        var workouts = new List<Workout>
        {
            new Workout { workoutId = 1, userId = userId, typeWorkout = "Strength", workoutDate = DateOnly.FromDateTime(DateTime.Now) },
            new Workout { workoutId = 2, userId = userId, typeWorkout = "Cardio", workoutDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)) },
            new Workout { workoutId = 3, userId = 2, typeWorkout = "Other User Workout", workoutDate = DateOnly.FromDateTime(DateTime.Now) }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Workout>>();
        mockDbSet.As<IQueryable<Workout>>().Setup(m => m.Provider).Returns(workouts.Provider);
        mockDbSet.As<IQueryable<Workout>>().Setup(m => m.Expression).Returns(workouts.Expression);
        mockDbSet.As<IQueryable<Workout>>().Setup(m => m.ElementType).Returns(workouts.ElementType);
        mockDbSet.As<IQueryable<Workout>>().Setup(m => m.GetEnumerator()).Returns(() => workouts.GetEnumerator());

        _mockContext.Setup(c => c.WorkoutModels).Returns(mockDbSet.Object);

        // Setup async provider
        var asyncQuery = workouts.Where(w => w.userId == userId).AsQueryable();
        mockDbSet.As<IAsyncEnumerable<Workout>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<System.Threading.CancellationToken>()))
            .Returns(new TestAsyncEnumerator<Workout>(asyncQuery.GetEnumerator()));

        mockDbSet.As<IQueryable<Workout>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<Workout>(asyncQuery.Provider));

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

        // Setup workout
        var workout = new Workout
        {
            workoutId = workoutId,
            userId = userId,
            typeWorkout = "Strength",
            workoutDate = DateOnly.FromDateTime(DateTime.Now),
            comments = "Test workout"
        };

        var workouts = new List<Workout> { workout }.AsQueryable();

        var mockWorkoutDbSet = new Mock<DbSet<Workout>>();
        mockWorkoutDbSet.As<IQueryable<Workout>>().Setup(m => m.Provider).Returns(workouts.Provider);
        mockWorkoutDbSet.As<IQueryable<Workout>>().Setup(m => m.Expression).Returns(workouts.Expression);
        mockWorkoutDbSet.As<IQueryable<Workout>>().Setup(m => m.ElementType).Returns(workouts.ElementType);
        mockWorkoutDbSet.As<IQueryable<Workout>>().Setup(m => m.GetEnumerator()).Returns(() => workouts.GetEnumerator());

        // Setup async for workout
        var asyncWorkoutQuery = workouts.Where(w => w.workoutId == workoutId && w.userId == userId).AsQueryable();
        mockWorkoutDbSet.As<IAsyncEnumerable<Workout>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<System.Threading.CancellationToken>()))
            .Returns(new TestAsyncEnumerator<Workout>(asyncWorkoutQuery.GetEnumerator()));

        mockWorkoutDbSet.As<IQueryable<Workout>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<Workout>(asyncWorkoutQuery.Provider));

        _mockContext.Setup(c => c.WorkoutModels).Returns(mockWorkoutDbSet.Object);

        // Setup training data
        var exercise1 = new Exercise { exerciseId = 1, exerciseName = "Bench Press", muscleGroup = "Chest" };
        var exercise2 = new Exercise { exerciseId = 2, exerciseName = "Squat", muscleGroup = "Legs" };

        var exercises = new List<Exercise> { exercise1, exercise2 }.AsQueryable();

        var mockExerciseDbSet = new Mock<DbSet<Exercise>>();
        mockExerciseDbSet.As<IQueryable<Exercise>>().Setup(m => m.Provider).Returns(exercises.Provider);
        mockExerciseDbSet.As<IQueryable<Exercise>>().Setup(m => m.Expression).Returns(exercises.Expression);
        mockExerciseDbSet.As<IQueryable<Exercise>>().Setup(m => m.ElementType).Returns(exercises.ElementType);
        mockExerciseDbSet.As<IQueryable<Exercise>>().Setup(m => m.GetEnumerator()).Returns(() => exercises.GetEnumerator());

        // Setup async for exercises
        mockExerciseDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
            .ReturnsAsync((object[] ids) => exercises.FirstOrDefault(e => e.exerciseId == (int)ids[0]));

        _mockContext.Setup(c => c.ExerciseModels).Returns(mockExerciseDbSet.Object);

        // Setup training data
        var trainingData = new List<TrainingData>
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

        var mockTrainingDataDbSet = new Mock<DbSet<TrainingData>>();
        mockTrainingDataDbSet.As<IQueryable<TrainingData>>().Setup(m => m.Provider).Returns(trainingData.Provider);
        mockTrainingDataDbSet.As<IQueryable<TrainingData>>().Setup(m => m.Expression).Returns(trainingData.Expression);
        mockTrainingDataDbSet.As<IQueryable<TrainingData>>().Setup(m => m.ElementType).Returns(trainingData.ElementType);
        mockTrainingDataDbSet.As<IQueryable<TrainingData>>().Setup(m => m.GetEnumerator()).Returns(() => trainingData.GetEnumerator());

        // Setup async for training data
        var asyncTrainingDataQuery = trainingData.Where(td => td.workoutId == workoutId).AsQueryable();
        mockTrainingDataDbSet.As<IAsyncEnumerable<TrainingData>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<System.Threading.CancellationToken>()))
            .Returns(new TestAsyncEnumerator<TrainingData>(asyncTrainingDataQuery.GetEnumerator()));

        mockTrainingDataDbSet.As<IQueryable<TrainingData>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<TrainingData>(asyncTrainingDataQuery.Provider));

        _mockContext.Setup(c => c.TrainingDataModels).Returns(mockTrainingDataDbSet.Object);

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
    }

    [Fact]
    public async Task GetWorkoutDetails_WithInvalidId_ReturnsNull()
    {
        // Arrange
        int invalidWorkoutId = 999;
        int userId = 1;

        var workouts = new List<Workout>
        {
            new Workout { workoutId = 1, userId = userId, typeWorkout = "Strength", workoutDate = DateOnly.FromDateTime(DateTime.Now) }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Workout>>();
        mockDbSet.As<IQueryable<Workout>>().Setup(m => m.Provider).Returns(workouts.Provider);
        mockDbSet.As<IQueryable<Workout>>().Setup(m => m.Expression).Returns(workouts.Expression);
        mockDbSet.As<IQueryable<Workout>>().Setup(m => m.ElementType).Returns(workouts.ElementType);
        mockDbSet.As<IQueryable<Workout>>().Setup(m => m.GetEnumerator()).Returns(() => workouts.GetEnumerator());

        // Setup async
        var asyncQuery = workouts.Where(w => w.workoutId == invalidWorkoutId && w.userId == userId).AsQueryable();
        mockDbSet.As<IAsyncEnumerable<Workout>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<System.Threading.CancellationToken>()))
            .Returns(new TestAsyncEnumerator<Workout>(asyncQuery.GetEnumerator()));

        mockDbSet.As<IQueryable<Workout>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<Workout>(asyncQuery.Provider));

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

        var workoutsCollection = new List<Workout>();
        var exercisesCollection = new List<Exercise>();
        var trainingDataCollection = new List<TrainingData>();

        // Mock DbSets
        var mockWorkoutDbSet = new Mock<DbSet<Workout>>();
        mockWorkoutDbSet.Setup(d => d.Add(It.IsAny<Workout>())).Callback<Workout>(e => {
            e.workoutId = 1; // Simulate auto-increment
            workoutsCollection.Add(e);
        });

        var mockExerciseDbSet = new Mock<DbSet<Exercise>>();
        mockExerciseDbSet.Setup(d => d.Add(It.IsAny<Exercise>())).Callback<Exercise>(e => {
            e.exerciseId = exercisesCollection.Count + 1; // Simulate auto-increment
            exercisesCollection.Add(e);
        });

        var exercises = new List<Exercise>().AsQueryable();
        mockExerciseDbSet.As<IQueryable<Exercise>>().Setup(m => m.Provider).Returns(exercises.Provider);
        mockExerciseDbSet.As<IQueryable<Exercise>>().Setup(m => m.Expression).Returns(exercises.Expression);
        mockExerciseDbSet.As<IQueryable<Exercise>>().Setup(m => m.ElementType).Returns(exercises.ElementType);
        mockExerciseDbSet.As<IQueryable<Exercise>>().Setup(m => m.GetEnumerator()).Returns(() => exercises.GetEnumerator());

        // Setup async for exercises find
        mockExerciseDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Exercise, bool>>>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync((System.Linq.Expressions.Expression<Func<Exercise, bool>> predicate, System.Threading.CancellationToken token) => null);

        var mockTrainingDataDbSet = new Mock<DbSet<TrainingData>>();
        mockTrainingDataDbSet.Setup(d => d.Add(It.IsAny<TrainingData>())).Callback<TrainingData>(e => {
            trainingDataCollection.Add(e);
        });

        var trainingData = new List<TrainingData>().AsQueryable();
        mockTrainingDataDbSet.As<IQueryable<TrainingData>>().Setup(m => m.Provider).Returns(trainingData.Provider);
        mockTrainingDataDbSet.As<IQueryable<TrainingData>>().Setup(m => m.Expression).Returns(trainingData.Expression);
        mockTrainingDataDbSet.As<IQueryable<TrainingData>>().Setup(m => m.ElementType).Returns(trainingData.ElementType);
        mockTrainingDataDbSet.As<IQueryable<TrainingData>>().Setup(m => m.GetEnumerator()).Returns(() => trainingData.GetEnumerator());

        // Setup for context
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

// Helper classes for async testing
public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    public TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
    {
        return new TestAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
    }

    public object Execute(System.Linq.Expressions.Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public System.Threading.Tasks.Task<object> ExecuteAsync(System.Linq.Expressions.Expression expression, System.Threading.CancellationToken cancellationToken)
    {
        return System.Threading.Tasks.Task.FromResult(Execute(expression));
    }

    public System.Threading.Tasks.Task<TResult> ExecuteAsync<TResult>(System.Linq.Expressions.Expression expression, System.Threading.CancellationToken cancellationToken)
    {
        return System.Threading.Tasks.Task.FromResult(Execute<TResult>(expression));
    }

    TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    { }

    public TestAsyncEnumerable(System.Linq.Expressions.Expression expression)
        : base(expression)
    { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(System.Threading.CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }
}

public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public System.Threading.Tasks.ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return new System.Threading.Tasks.ValueTask();
    }

    public System.Threading.Tasks.ValueTask<bool> MoveNextAsync()
    {
        return new System.Threading.Tasks.ValueTask<bool>(_inner.MoveNext());
    }
}