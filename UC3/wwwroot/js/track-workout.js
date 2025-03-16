function navigateToNewWorkout() {
    window.location.href = '/Track/NewWorkout';
}

// Laad workouts zodra de pagina geladen is
document.addEventListener("DOMContentLoaded", function () {
    loadWorkouts();
});

function loadWorkouts() {
    fetch('/Track/GetWorkouts')
        .then(response => response.json())
        .then(data => {
            const workoutList = document.getElementById('workout-list');
            const noWorkoutsMsg = document.getElementById('no-workouts');
            workoutList.innerHTML = ''; // Leeg de lijst eerst

            if (data && data.length > 0) {
                data.forEach(workout => {
                    const workoutItem = createWorkoutItem(workout);
                    workoutList.appendChild(workoutItem);
                });
                noWorkoutsMsg.style.display = 'none';
            } else {
                noWorkoutsMsg.style.display = 'block';
            }
        })
        .catch(error => {
            console.error('Error loading workouts:', error);
            alert('Er is een fout opgetreden bij het laden van de workouts.');
        });
}

function createWorkoutItem(workout) {
    // Formatteer de datum
    const date = new Date(workout.workoutDate);
    const formattedDate = date.toLocaleDateString('nl-NL');

    // Maak het workout item element
    const workoutItem = document.createElement('div');
    workoutItem.className = 'workout-item';

    // Bouw de basis workout info op
    let workoutHTML = `
        <div class="workout-title">
            <span class="workout-type">${workout.typeWorkout}</span>
            <span class="workout-date">${formattedDate}</span>
        </div>
    `;

    // Voeg oefeningen toe als ze bestaan
    if (workout.exercises && workout.exercises.length > 0) {
        workoutHTML += `<div class="exercise-list">`;
        workout.exercises.forEach(exercise => {
            workoutHTML += `
                <div class="exercise-item">
                    <div><strong>${exercise.exerciseName}</strong> (${exercise.muscleGroup})</div>
                    <div>${exercise.trainingData.amountOfSets} sets × ${exercise.trainingData.amountOfReps} reps × ${exercise.trainingData.liftedWeight} kg</div>
                </div>
            `;
        });
        workoutHTML += `</div>`;
    }

    // Voeg opmerkingen toe als die bestaan
    if (workout.comments && workout.comments.trim() !== '') {
        workoutHTML += `
            <div class="comments-section">
                <strong>Opmerkingen:</strong>
                <p>${workout.comments}</p>
            </div>
        `;
    }

    // Voeg actieknoppen toe
    workoutHTML += `
        <div class="workout-actions">
        </div>
    `;

    workoutItem.innerHTML = workoutHTML;
    return workoutItem;
}

function viewWorkoutDetails(workoutId) {
    // Implementeer navigatie naar de details pagina
    window.location.href = `/Track/WorkoutDetails?id=${workoutId}`;
}

function editWorkout(workoutId) {
    // Implementeer navigatie naar de bewerk pagina
    window.location.href = `/Track/EditWorkout?id=${workoutId}`;
}