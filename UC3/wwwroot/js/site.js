// Bestaande functie uit site.js
function HidePassword() {
    var x = document.getElementById("hiddenpw");
    if (x.type == null || x.type === "password") {
        x.type = "text";
    } else {
        x.type = "password";
    }
}

// Calendar functionality uit Index.cshtml en Profile.cshtml
$(document).ready(function () {
    // Controleer of we op de profielpagina zijn
    const isProfilePage = window.location.pathname.includes('/Profile');

    if (isProfilePage) {
        // Haal de gebruikers-ID op voor wie we de kalender weergeven
        const profileUserId = parseInt($('#weekCalendar').data('user-id') || 0);

        // Controleer of de huidige gebruiker dezelfde is als de profielgebruiker
        const isOwnProfile = $('#weekCalendar').data('is-own-profile') === "true";

        renderSimpleWeekView(profileUserId, isOwnProfile);

        // Gebruikersspecifieke key voor workout-dagen
        const workoutStorageKey = `workoutDays_user_${profileUserId}`;

        // Initialiseer lokale opslag als deze nog niet bestaat voor deze gebruiker
        if (!localStorage.getItem(workoutStorageKey)) {
            localStorage.setItem(workoutStorageKey, JSON.stringify([]));
        }

        // Workout toevoegen functionaliteit - alleen beschikbaar als het je eigen profiel is
        if (isOwnProfile) {
            $(document).on('click', '.day-circle', function () {
                const dayIndex = $(this).data('day-index');
                $(this).toggleClass('has-workout');

                // Update lokale opslag voor deze specifieke gebruiker
                const workoutDays = JSON.parse(localStorage.getItem(workoutStorageKey));

                if ($(this).hasClass('has-workout')) {
                    // Toevoegen als het nog niet bestaat
                    if (!workoutDays.includes(dayIndex)) {
                        workoutDays.push(dayIndex);
                    }
                } else {
                    // Verwijderen als het bestaat
                    const index = workoutDays.indexOf(dayIndex);
                    if (index > -1) {
                        workoutDays.splice(index, 1);
                    }
                }

                localStorage.setItem(workoutStorageKey, JSON.stringify(workoutDays));
            });
        }
    } else {
        // Home pagina functionality
        renderSimpleWeekView();

        // Bio veranderen functionaliteit
        $("#changeBioBtn").click(function () {
            // Verberg de weergave en toon het bewerkingsformulier
            $("#bioViewContent").hide();
            $("#bioEditContent").show();
        });

        $("#cancelBioBtn").click(function () {
            // Verberg het bewerkingsformulier en toon de weergave
            $("#bioEditContent").hide();
            $("#bioViewContent").show();
        });

        $("#saveBioBtn").click(function () {
            const newBio = $("#bioTextarea").val();

            // Stuur AJAX-verzoek om de bio bij te werken
            $.ajax({
                url: '/Home/UpdateBio',
                type: 'POST',
                data: { bio: newBio },
                success: function (response) {
                    if (response.success) {
                        // Werk de weergave bij met de nieuwe bio
                        $("#bioViewContent p").text("Bio: " + newBio);
                        $("#bioEditContent").hide();
                        $("#bioViewContent").show();
                    } else {
                        alert("Er is een fout opgetreden bij het opslaan van de bio.");
                    }
                },
                error: function () {
                    alert("Er is een fout opgetreden bij het opslaan van de bio.");
                }
            });
        });

        // Lokale opslag voor workouts
        if (!localStorage.getItem('workoutDays')) {
            localStorage.setItem('workoutDays', JSON.stringify([]));
        }

        // Add workout button functionality
        $(document).on('click', '.day-circle', function () {
            const dayIndex = $(this).data('day-index');
            $(this).toggleClass('has-workout');

            // Update lokale opslag
            const workoutDays = JSON.parse(localStorage.getItem('workoutDays'));

            if ($(this).hasClass('has-workout')) {
                // Toevoegen als het nog niet bestaat
                if (!workoutDays.includes(dayIndex)) {
                    workoutDays.push(dayIndex);
                }
            } else {
                // Verwijderen als het bestaat
                const index = workoutDays.indexOf(dayIndex);
                if (index > -1) {
                    workoutDays.splice(index, 1);
                }
            }

            localStorage.setItem('workoutDays', JSON.stringify(workoutDays));
        });
    }
});

function renderSimpleWeekView(userId, isEditable) {
    // Get current date info
    const now = new Date();
    const currentDay = now.getDay(); // 0 = Sunday, 1 = Monday, ...

    // Calculate week number (ISO week)
    const date = new Date(now.getTime());
    date.setHours(0, 0, 0, 0);
    date.setDate(date.getDate() + 3 - (date.getDay() + 6) % 7);
    const week1 = new Date(date.getFullYear(), 0, 4);
    const weekNumber = 1 + Math.round(((date.getTime() - week1.getTime()) / 86400000 - 3 + (week1.getDay() + 6) % 7) / 7);

    // Gebruikersspecifieke key voor workout-dagen
    const workoutStorageKey = userId ? `workoutDays_user_${userId}` : 'workoutDays';

    // Workout dagen uit lokale opslag halen voor deze specifieke gebruiker
    const workoutDays = JSON.parse(localStorage.getItem(workoutStorageKey) || '[]');

    // Create week view HTML
    let weekView = `
        <div class="week-header">
            <h3>Week: ${weekNumber}</h3>
            <div class="week-navigation">
                <button class="btn btn-sm btn-outline-primary prev-week">←</button>
                <button class="btn btn-sm btn-outline-primary next-week">→</button>
            </div>
        </div>
        <div class="weekdays-container">`;

    // Create weekday circles
    const dayNames = ['Maandag', 'Dinsdag', 'Woensdag', 'Donderdag', 'Vrijdag', 'Zaterdag', 'Zondag'];

    for (let i = 0; i < 7; i++) {
        const hasWorkout = workoutDays.includes(i);
        const isToday = (i === (currentDay === 0 ? 6 : currentDay - 1)); // Adjust for Monday as first day

        // Voeg cursor-pointer en data-attribuut alleen toe als het bewerkbaar is
        const editableClass = isEditable ? 'cursor-pointer' : '';
        const dataAttr = isEditable ? `data-day-index="${i}"` : '';

        weekView += `
            <div class="day-container ${isToday ? 'today' : ''}">
                <div class="day-name">${dayNames[i]}</div>
                <div class="day-circle ${hasWorkout ? 'has-workout' : ''} ${editableClass}" ${dataAttr}></div>
            </div>`;
    }

    weekView += `
        </div>`;

    // Voeg alleen de footer met instructies toe als het bewerkbaar is
    if (isEditable) {
        weekView += `
        <div class="week-footer">
            <span class="workout-legend">
                <span class="legend-item">
                    <span class="day-circle has-workout"></span> Workout gepland
                </span>
                <span class="legend-item">
                    <span class="day-circle"></span> Geen workout
                </span>
            </span>
            <span class="workout-help">Klik op een cirkel om een workout toe te voegen of te verwijderen</span>
        </div>`;
    }

    // Add to DOM
    $('#weekCalendar').html(weekView);

    // Add event listeners for navigation
    $('.prev-week').click(function () {
        // Navigate to previous week (example implementation)
        alert('Naar vorige week');
    });

    $('.next-week').click(function () {
        // Navigate to next week (example implementation)
        alert('Naar volgende week');
    });
}