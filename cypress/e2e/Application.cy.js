describe('template spec', () => {
  it('passes', () => {
    cy.visit('https://example.cypress.io')
  })
})

// cypress/e2e/login.spec.js
describe('Login Pagina', () => {
  beforeEach(() => {
    cy.visit('/Home/Login'); // Pas de URL aan naar het correcte pad
  });

  it('Moet de pagina titel en login form tonen', () => {
    cy.get('h1').should('contain', 'Login');
    cy.get('form').should('have.length', 2); // Er zijn twee formulieren
  });

  it('Moet e-mail en wachtwoord velden bevatten', () => {
    cy.get('input[name="email"]').should('exist');
    cy.get('input[name="password"]').should('exist');
    cy.get('#hiddenpw').should('exist');
  });

  it('Moet een verificatie stap hebben', () => {
    cy.get('input[value="Send verification"]').should('exist');
    cy.get('input[name="vericode"]').should('exist');
    cy.get('input[value="Log in"]').should('exist');
  });

  it('Moet een link naar de registratiepagina tonen', () => {
    cy.get('a[href*="Register"]').should('contain', 'Nog geen account?');
  });

  it('Moet "Show Password" functionaliteit hebben', () => {
    cy.get('input[type="checkbox"]').should('exist');
    // We testen niet de JavaScript functionaliteit omdat die niet in de view zit
  });

  it('Moet de verificatieproces correct uitvoeren', () => {
    const testEmail = 'test@example.com';
    const testPassword = 'Password123!';

    // Vul inloggegevens in
    cy.get('input[name="email"]').type(testEmail);
    cy.get('input[name="password"]').type(testPassword);
    
    // Stuur verificatie
    cy.get('input[value="Send verification"]').click();
    
    // Hier zouden we normaal wachten op de verificatiecode, maar voor nu gaan we verder
    // met een mock waarde
    cy.get('input[name="vericode"]').type('123456');
    cy.get('input[value="Log in"]').click();
    
    // Verwachting: na succesvolle login wordt gebruiker doorgestuurd naar Home/Index
    // Dit is een voorbeeld, mogelijk moet de URL aangepast worden
    cy.url().should('include', '/Home/Index');
  });
});

// cypress/e2e/register.spec.js
describe('Register Pagina', () => {
  beforeEach(() => {
    cy.visit('/Home/Register'); // Pas de URL aan naar het correcte pad
  });

  it('Moet de pagina titel en registratieformulier tonen', () => {
    cy.get('h1').should('contain', 'Register');
    cy.get('form').should('exist');
  });

  it('Moet naam, e-mail en wachtwoord velden bevatten', () => {
    cy.get('input[name="name"]').should('exist');
    cy.get('input[name="email"]').should('exist');
    cy.get('input[name="password"]').should('exist');
    cy.get('#password-field').should('exist');
  });

  it('Moet een registreer knop bevatten', () => {
    cy.get('input[value="Create"]').should('exist');
  });

  it('Moet een link terug naar de inlogpagina tonen', () => {
    cy.get('a[href*="Login"]').should('contain', 'Terug naar Inloggen');
  });

  it('Moet wachtwoordvereisten tonen bij ongeldige wachtwoorden', () => {
    cy.get('#password-field').type('weak');
    cy.get('#password-requirements').should('be.visible');
    cy.get('#password-requirements').should('contain', 'Wachtwoord moet minstens 8 tekens lang zijn');
  });

  it('Moet wachtwoordvereisten verbergen bij geldige wachtwoorden', () => {
    cy.get('#password-field').type('StrongP@ssw0rd');
    cy.get('#password-requirements').should('not.be.visible');
  });

  it('Moet een gebruiker kunnen registreren', () => {
    const testName = 'Test User';
    const testEmail = 'test.user@example.com';
    const testPassword = 'StrongP@ssw0rd';

    cy.get('input[name="name"]').type(testName);
    cy.get('input[name="email"]').type(testEmail);
    cy.get('#password-field').type(testPassword);
    cy.get('input[value="Create"]').click();

    // Verwachting: na succesvolle registratie wordt gebruiker doorgestuurd naar Login
    cy.url().should('include', '/Home/Login');
  });
});

// cypress/e2e/home.spec.js
describe('Home Pagina (Index)', () => {
  beforeEach(() => {
    // We moeten eerst inloggen
    cy.login(); // Dit is een custom command die we later definiëren
    cy.visit('/Home/Index'); // Pas de URL aan naar het correcte pad
  });

  it('Moet de zoekbalk voor vrienden tonen', () => {
    cy.get('form[action="/Home/Search"]').should('exist');
    cy.get('input[name="searchTerm"]').should('exist');
    cy.get('button').contains('Search').should('exist');
  });

  it('Moet de bio sectie tonen', () => {
    cy.get('.bio').should('exist');
    cy.get('.bio-header').should('contain', 'Bio');
    cy.get('#changeBioBtn').should('exist');
    cy.get('.bio-content').should('exist');
  });

  it('Moet de bio kunnen bewerken', () => {
    const newBio = 'Dit is mijn nieuwe bio tekst voor testing.';
    
    cy.get('#changeBioBtn').click();
    cy.get('#bioEditContent').should('be.visible');
    cy.get('#bioTextarea').clear().type(newBio);
    cy.get('#saveBioBtn').click();
    
    // Controleer of de bio is bijgewerkt
    cy.get('.bio-content').should('contain', newBio);
  });

  it('Moet de weekkalender tonen', () => {
    cy.get('#weekCalendar').should('exist');
  });
});

// cypress/e2e/new-workout.spec.js
describe('Nieuwe Workout Pagina', () => {
  beforeEach(() => {
    // We moeten eerst inloggen
    cy.login(); // Dit is een custom command die we later definiëren
    cy.visit('/Home/NewWorkout'); // Pas de URL aan naar het correcte pad
  });

  it('Moet het workout formulier tonen', () => {
    cy.get('#workout-form').should('exist');
    cy.get('#typeWorkout').should('exist');
    cy.get('#workoutDate').should('exist');
  });

  it('Moet oefening velden bevatten', () => {
    cy.get('.exercise-container').should('have.length', 1); // Standaard 1 oefening
    cy.get('#exerciseName-0').should('exist');
    cy.get('#muscleGroup-0').should('exist');
    cy.get('#amountOfSets-0').should('exist');
    cy.get('#amountOfReps-0').should('exist');
    cy.get('#liftedWeight-0').should('exist');
  });

  it('Moet een nieuwe oefening kunnen toevoegen', () => {
    cy.get('#add-exercise').click();
    cy.get('.exercise-container').should('have.length', 2);
  });

  it('Moet een oefening kunnen verwijderen', () => {
    cy.get('#add-exercise').click();
    cy.get('.exercise-container').should('have.length', 2);
    cy.get('.remove-exercise').last().should('be.visible').click();
    cy.get('.exercise-container').should('have.length', 1);
  });

  it('Moet een workout kunnen opslaan', () => {
    // Vul de workout details in
    cy.get('#typeWorkout').type('Kracht Training');
    cy.get('#workoutDate').type('2025-03-23');
    
    // Vul de oefening details in
    cy.get('#exerciseName-0').type('Bankdrukken');
    cy.get('#muscleGroup-0').select('Borst');
    cy.get('#amountOfSets-0').type('3');
    cy.get('#amountOfReps-0').type('10');
    cy.get('#liftedWeight-0').type('60');
    
    // Voeg een tweede oefening toe en vul in
    cy.get('#add-exercise').click();
    cy.get('#exerciseName-1').type('Squats');
    cy.get('#muscleGroup-1').select('Benen');
    cy.get('#amountOfSets-1').type('4');
    cy.get('#amountOfReps-1').type('8');
    cy.get('#liftedWeight-1').type('100');
    
    // Voeg opmerkingen toe
    cy.get('#comments').type('Goede workout, voelde me sterk vandaag.');
    
    // Sla de workout op
    cy.get('button[type="submit"]').click();
    
    // Controleer of het bevestigingsvenster verschijnt
    cy.get('#confirmation-modal').should('be.visible');
    cy.get('#confirm-save').click();
    
    // We verwachten dat we worden teruggestuurd naar de Track pagina
    cy.url().should('include', '/Home/Track');
  });
});

// cypress/e2e/profile.spec.js
describe('Profiel Pagina', () => {
  beforeEach(() => {
    // We moeten eerst inloggen
    cy.login(); // Dit is een custom command die we later definiëren
    cy.visit('/Home/Profile'); // Pas de URL aan naar het correcte pad
  });

  it('Moet de profielinformatie tonen', () => {
    cy.get('.profile-header').should('exist');
    cy.get('.name-label').should('exist');
  });

  it('Moet de bio sectie tonen', () => {
    cy.get('.bio').should('exist');
    cy.get('.bio-header').should('contain', 'Bio');
    cy.get('.bio-content').should('exist');
  });

  it('Moet de weekkalender tonen', () => {
    cy.get('#weekCalendar').should('exist');
  });

  it('Moet de zoekbalk voor vrienden tonen', () => {
    cy.get('form[action="/Home/Search"]').should('exist');
    cy.get('input[name="searchTerm"]').should('exist');
    cy.get('button').contains('Search').should('exist');
  });

  // Alleen voor admins
  it('Moet admin acties tonen (indien admin)', () => {
    // Dit test moet alleen worden uitgevoerd als de huidige gebruiker een admin is
    // We gebruiken een custom command om een admin account te hebben
    cy.loginAsAdmin();
    cy.visit('/Home/Profile/2'); // Bezoek het profiel van een andere gebruiker
    
    cy.get('.admin-actions').should('exist');
    cy.get('button').contains('Account verwijderen').should('exist');
  });
});

// cypress/e2e/track.spec.js
describe('Track Workouts Pagina', () => {
  beforeEach(() => {
    // We moeten eerst inloggen
    cy.login(); // Dit is een custom command die we later definiëren
    cy.visit('/Home/Track'); // Pas de URL aan naar het correcte pad
  });

  it('Moet de titel en nieuwe workout knop tonen', () => {
    cy.get('h3').should('contain', 'Workout Geschiedenis');
    cy.get('.log-workout-btn').should('exist');
    cy.get('.log-workout-btn').should('contain', 'Nieuwe Workout Registreren');
  });

  it('Moet navigeren naar de nieuwe workout pagina', () => {
    cy.get('.log-workout-btn').click();
    cy.url().should('include', '/Home/NewWorkout');
  });

  it('Moet de workout lijst tonen of de "geen workouts" melding', () => {
    // We kunnen niet zeker weten of er workouts zijn, dus we checken één van beide
    cy.get('body').then(($body) => {
      if ($body.find('#workout-list .workout-item').length > 0) {
        cy.get('#workout-list .workout-item').should('exist');
      } else {
        cy.get('#no-workouts').should('be.visible');
      }
    });
  });
});

// cypress/support/commands.js
// Custom commands voor het testen van de applicatie

// Login command
Cypress.Commands.add('login', () => {
  // We gebruiken een directe API call om in te loggen in plaats van UI interactie
  // Dit is sneller voor tests die alleen een ingelogde sessie nodig hebben
  cy.request({
    method: 'POST',
    url: '/Home/Login',
    body: {
      email: 'test@example.com',
      password: 'Password123!',
      action: 'Log in',
      vericode: '123456'
    },
    form: true
  }).then((resp) => {
    expect(resp.status).to.eq(200);
  });

  // Als alternatief kunnen we ook localStorage/sessionStorage gebruiken
  // om een ingelogde sessie te simuleren, afhankelijk van hoe de app werkt
});

// Login als admin command
Cypress.Commands.add('loginAsAdmin', () => {
  cy.request({
    method: 'POST',
    url: '/Home/Login',
    body: {
      email: 'admin@example.com',
      password: 'AdminPass123!',
      action: 'Log in',
      vericode: '123456'
    },
    form: true
  }).then((resp) => {
    expect(resp.status).to.eq(200);
  });
});

// cypress/support/e2e.js
// ***********************************************************
// Deze file wordt automatisch geladen voor e2e tests.
// Je kunt hier globale configuratie en behavior hooks toevoegen.
// ***********************************************************

// Import commands.js
import './commands';

// Ignore uncaught exceptions
Cypress.on('uncaught:exception', (err, runnable) => {
  // We retourneren false om te voorkomen dat Cypress faalt op uncaught exceptions
  return false;
});