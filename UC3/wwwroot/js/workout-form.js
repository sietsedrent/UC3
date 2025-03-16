let exerciseCount = 1;

// Functie voor form validatie
function validateForm() {
    let isValid = true;
    const requiredFields = document.querySelectorAll('#workout-form [required]');

    // Reset alle validatie visuele feedback
    requiredFields.forEach(field => {
        field.classList.remove('is-invalid');
    });

    // Controleer elk verplicht veld
    requiredFields.forEach(field => {
        if (!field.value.trim()) {
            field.classList.add('is-invalid');
            isValid = false;
        }
    });

    // Controleer of gewichten geldig zijn (geen negatieve waarden)
    const weightFields = document.querySelectorAll('[id^="liftedWeight-"]');
    weightFields.forEach(field => {
        if (parseFloat(field.value) < 0) {
            field.classList.add('is-invalid');
            isValid = false;
        }
    });

    return isValid;
}

// Voegt een nieuwe oefening toe
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('add-exercise').addEventListener('click', function () {
        const exercisesContainer = document.getElementById('exercises-container');
        const newExerciseContainer = document.createElement('div');
        newExerciseContainer.className = 'exercise-container';

        // Update exercise count
        exerciseCount++;

        // Array van spiergroepen
        const muscleGroups = [
            "Borst", "Rug", "Schouders", "Biceps", "Triceps", "Benen", "Buik",
            "Kuiten", "Hamstrings", "Quadriceps", "Glutes", "Core", "Cardio", "Full Body"
        ];

        // Maak de opties voor de dropdown
        let muscleGroupOptions = '<option value="">Selecteer spiergroep</option>';
        muscleGroups.forEach(group => {
            muscleGroupOptions += `<option value="${group}">${group}</option>`;
        });

        newExerciseContainer.innerHTML = `
            <h4>Oefening ${exerciseCount}</h4>
            <div class="exercise-fields">
                <div>
                    <label for="exerciseName-${exerciseCount - 1}">Naam Oefening:</label>
                    <input type="text" id="exerciseName-${exerciseCount - 1}" name="exerciseName" required>
                </div>
                <div>
                    <label for="muscleGroup-${exerciseCount - 1}">Spiergroep:</label>
                    <select id="muscleGroup-${exerciseCount - 1}" name="muscleGroup" required>
                        ${muscleGroupOptions}
                    </select>
                </div>
                <div>
                    <label for="amountOfSets-${exerciseCount - 1}">Aantal Sets:</label>
                    <input type="number" id="amountOfSets-${exerciseCount - 1}" name="amountOfSets" min="1" required>
                </div>
                <div>
                    <label for="amountOfReps-${exerciseCount - 1}">Aantal Herhalingen:</label>
                    <input type="number" id="amountOfReps-${exerciseCount - 1}" name="amountOfReps" min="1" required>
                </div>
                <div>
                    <label for="liftedWeight-${exerciseCount - 1}">Gewicht (kg):</label>
                    <input type="number" id="liftedWeight-${exerciseCount - 1}" name="liftedWeight" step="0.5" min="0" required>
                </div>
            </div>
            <button type="button" class="btn btn-danger remove-exercise">Verwijder Oefening</button>
        `;

        exercisesContainer.appendChild(newExerciseContainer);

        // Toon de verwijderknop voor de eerste oefening als er meer dan één oefening is
        if (exerciseCount > 1) {
            document.querySelector('.exercise-container .remove-exercise').style.display = 'block';
        }

        // Voeg event listener toe aan de nieuwe verwijderknop
        newExerciseContainer.querySelector('.remove-exercise').addEventListener('click', function () {
            newExerciseContainer.remove();
            exerciseCount--;

            // Verberg de verwijderknop voor de eerste oefening als er maar één oefening over is
            if (exerciseCount === 1) {
                document.querySelector('.exercise-container .remove-exercise').style.display = 'none';
            }

            // Hernummer de oefeningen
            const exerciseContainers = document.querySelectorAll('.exercise-container');
            exerciseContainers.forEach((container, index) => {
                container.querySelector('h4').textContent = `Oefening ${index + 1}`;
            });
        });
    });

    // Referenties naar modal elementen
    const modal = document.getElementById('confirmation-modal');
    const closeButtons = document.querySelectorAll('[data-dismiss="modal"]');
    const confirmSaveButton = document.getElementById('confirm-save');
    const workoutSummary = document.getElementById('workout-summary');

    // Form submission
    document.getElementById('workout-form').addEventListener('submit', function (event) {
        event.preventDefault();

        if (!validateForm()) {
            alert('Vul alle verplichte velden correct in.');
            return;
        }

        // Voorbereiden van de workout data voor zowel de modal als de server
        const workoutData = {
            typeWorkout: document.getElementById('typeWorkout').value,
            workoutDate: document.getElementById('workoutDate').value,
            comments: document.getElementById('comments').value,
            exercises: []
        };

        // Verzamel data van alle oefeningen
        const exerciseContainers = document.querySelectorAll('.exercise-container');
        exerciseContainers.forEach((container, index) => {
            const exerciseData = {
                exerciseName: document.getElementById(`exerciseName-${index}`).value,
                muscleGroup: document.getElementById(`muscleGroup-${index}`).value,
                trainingData: {
                    amountOfSets: parseInt(document.getElementById(`amountOfSets-${index}`).value),
                    amountOfReps: parseInt(document.getElementById(`amountOfReps-${index}`).value),
                    liftedWeight: parseFloat(document.getElementById(`liftedWeight-${index}`).value)
                }
            };
            workoutData.exercises.push(exerciseData);
        });

        // Toon een samenvatting in de modal
        showWorkoutSummary(workoutData);

        // Toon de modal
        modal.style.display = 'block';

        // Sla de workout data op voor later gebruik
        modal.dataset.workoutData = JSON.stringify(workoutData);
    });

    // Functie om de workout samenvatting te tonen
    function showWorkoutSummary(workoutData) {
        // Formatteer de datum
        const date = new Date(workoutData.workoutDate);
        const formattedDate = date.toLocaleDateString('nl-NL');

        // Bouw de samenvatting op
        let summaryHTML = `
            <div class="summary-header">
                <p><strong>Datum:</strong> ${formattedDate}</p>
                <p><strong>Type Workout:</strong> ${workoutData.typeWorkout}</p>
            </div>
            <h6>Oefeningen:</h6>
        `;

        workoutData.exercises.forEach((exercise, index) => {
            summaryHTML += `
                <div class="summary-item">
                    <p><strong>${index + 1}. ${exercise.exerciseName}</strong> (${exercise.muscleGroup})</p>
                    <p>${exercise.trainingData.amountOfSets} sets × ${exercise.trainingData.amountOfReps} reps × ${exercise.trainingData.liftedWeight} kg</p>
                </div>
            `;
        });

        // Voeg opmerkingen toe indien aanwezig
        if (workoutData.comments && workoutData.comments.trim() !== '') {
            summaryHTML += `
                <div class="summary-footer">
                    <p><strong>Opmerkingen:</strong> ${workoutData.comments}</p>
                </div>
            `;
        }

        workoutSummary.innerHTML = summaryHTML;
    }

    // Event listeners voor de modal
    closeButtons.forEach(button => {
        button.addEventListener('click', function () {
            modal.style.display = 'none';
        });
    });

    // Wanneer gebruiker buiten de modal klikt, sluit de modal
    window.addEventListener('click', function (event) {
        if (event.target === modal) {
            modal.style.display = 'none';
        }
    });

    // Bevestig opslaan handler
    confirmSaveButton.addEventListener('click', function () {
        // Haal de workout data op
        const workoutData = JSON.parse(modal.dataset.workoutData);

        // Stuur data naar de server
        fetch('/Track/SaveWorkout', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(workoutData)
        }).then(response => {
            if (response.ok) {
                window.location.href = '/Home/Track';
            } else {
                modal.style.display = 'none';
                alert('Er is een fout opgetreden bij het opslaan van de workout.');
            }
        }).catch(error => {
            console.error('Error:', error);
            modal.style.display = 'none';
            alert('Er is een fout opgetreden bij het opslaan van de workout.');
        });
    });
});