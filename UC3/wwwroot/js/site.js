// Bestaande functie uit site.js
function HidePassword() {
    var x = document.getElementById("hiddenpw");
    if (x.type == null || x.type === "password") {
        x.type = "text";
    } else {
        x.type = "password";
    }
}

// Calendar functionality
$(document).ready(function () {
    // Controleer of we op de profielpagina zijn
    const isProfilePage = window.location.pathname.includes('/Profile');

    if (isProfilePage) {
        // Haal de gebruikers-ID op voor wie we de kalender weergeven
        const profileUserId = parseInt($('#weekCalendar').data('user-id') || 0);

        // Controleer of de huidige gebruiker dezelfde is als de profielgebruiker
        const isOwnProfile = $('#weekCalendar').data('is-own-profile') === "true";

        loadWorkoutPlannings(profileUserId, isOwnProfile);
    } else {
        // Home pagina functionality
        const userId = parseInt($('#weekCalendar').data('user-id') || 0);
        loadWorkoutPlannings(userId, true);

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
    }
});

// Functie om workout planningen van de server te laden
function loadWorkoutPlannings(userId, isEditable) {
    // Haal workout planningen op van server
    $.ajax({
        url: '/Home/GetWorkoutPlannings',
        type: 'GET',
        data: { userId: userId },
        success: function (data) {
            // Render de kalender met de ontvangen data
            renderSimpleWeekView(userId, isEditable, data.plannings);
        },
        error: function () {
            console.error('Fout bij ophalen workout planningen');
            // Fallback naar een lege lijst
            renderSimpleWeekView(userId, isEditable, []);
        }
    });
}

function renderSimpleWeekView(userId, isEditable, workoutDays) {
    // Get current date info
    const now = new Date();
    const currentDay = now.getDay(); // 0 = Sunday, 1 = Monday, ...

    // Calculate week number (ISO week)
    const date = new Date(now.getTime());
    date.setHours(0, 0, 0, 0);
    date.setDate(date.getDate() + 3 - (date.getDay() + 6) % 7);
    const week1 = new Date(date.getFullYear(), 0, 4);
    const weekNumber = 1 + Math.round(((date.getTime() - week1.getTime()) / 86400000 - 3 + (week1.getDay() + 6) % 7) / 7);

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

    // Add workout button functionality - alleen als het bewerkbaar is
    if (isEditable) {
        $(document).on('click', '.day-circle.cursor-pointer', function () {
            const dayIndex = $(this).data('day-index');
            const hasWorkout = !$(this).hasClass('has-workout');

            // Toggle class
            $(this).toggleClass('has-workout');

            // Update in database
            $.ajax({
                url: '/Home/UpdateWorkoutPlanning',
                type: 'POST',
                data: { dayIndex: dayIndex, hasWorkout: hasWorkout },
                success: function (response) {
                    if (!response.success) {
                        console.error('Fout bij opslaan van workout planning');
                    }
                },
                error: function () {
                    console.error('Fout bij opslaan van workout planning');
                }
            });
        });
    }
}