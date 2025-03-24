// login.spec.js
describe('Login Pagina', () => {
  it('Moet de login pagina elementen bevatten', () => {
    cy.visit('https://localhost:7044/Home/Login');
    
    // Check basis elementen
    cy.get('h1').contains('Login');
    cy.get('form').should('exist');
    cy.get('input[name="email"]').should('exist');
    cy.get('input[name="password"]').should('exist');
    cy.get('input[value="Send verification"]').should('exist');
    cy.get('input[name="vericode"]').should('exist');
    cy.get('input[value="Log in"]').should('exist');
    cy.get('a').contains('Nog geen account?').should('exist');
  });
  
  it('Moet het login proces doorlopen', () => {
    cy.visit('https://localhost:7044/Home/Login');
    
    // Vul inloggegevens in
    cy.get('input[name="email"]').type('test@example.com');
    cy.get('input[name="password"]').type('Password123!');
    
    // Stuur verificatie
    cy.get('input[value="Send verification"]').click();
    
    // Vul verificatiecode in
    cy.get('input[name="vericode"]').type('123456');
    
    // Log in
    cy.get('input[value="Log in"]').click();
  });
});

// register.spec.js
describe('Register Pagina', () => {
  it('Moet de registratie pagina elementen bevatten', () => {
    cy.visit('https://localhost:7044/Home/Register');
    
    // Check basis elementen
    cy.get('h1').contains('Register');
    cy.get('form').should('exist');
    cy.get('input[name="name"]').should('exist');
    cy.get('input[name="email"]').should('exist');
    cy.get('input[name="password"]').should('exist');
    cy.get('input[value="Create"]').should('exist');
    cy.get('a').contains('Terug naar Inloggen').should('exist');
  });
  
  it('Moet wachtwoordvereisten tonen bij ongeldige wachtwoorden', () => {
    cy.visit('https://localhost:7044/Home/Register');
    
    cy.get('#password-field').type('weak');
    cy.get('#password-requirements').should('be.visible');
  });
  
  it('Moet registratie proces doorlopen', () => {
    cy.visit('https://localhost:7044/Home/Register');
    
    // Vul registratiegegevens in
    cy.get('input[name="name"]').type('Test User');
    cy.get('input[name="email"]').type('test.user@example.com');
    cy.get('#password-field').type('StrongP@ssw0rd');
    
    // Registreer
    cy.get('input[value="Create"]').click();
  });
});

// home.spec.js
describe('Home Pagina', () => {
  it('Moet home pagina elementen bevatten na login', () => {
    // Login eerst
    cy.visit('https://localhost:7044/Home/Login');
    cy.get('input[name="email"]').type('test@example.com');
    cy.get('input[name="password"]').type('Password123!');
    cy.get('input[value="Send verification"]').click();
    cy.get('input[name="vericode"]').type('123456');
    cy.get('input[value="Log in"]').click();
    
    // Check home pagina elementen
    cy.get('form[action="/Home/Search"]').should('exist');
    cy.get('input[name="searchTerm"]').should('exist');
    cy.get('.bio').should('exist');
    cy.get('#weekCalendar').should('exist');
  });
});

// new-workout.spec.js
describe('Nieuwe Workout Pagina', () => {
  it('Moet workout formulier elementen bevatten na login', () => {
    // Login eerst
    cy.visit('https://localhost:7044/Home/Login');
    cy.get('input[name="email"]').type('test@example.com');
    cy.get('input[name="password"]').type('Password123!');
    cy.get('input[value="Send verification"]').click();
    cy.get('input[name="vericode"]').type('123456');
    cy.get('input[value="Log in"]').click();
    
    // Ga naar nieuwe workout pagina
    cy.visit('https://localhost:7044/Home/NewWorkout');
    
    // Check workout formulier elementen
    cy.get('#workout-form').should('exist');
    cy.get('#typeWorkout').should('exist');
    cy.get('#workoutDate').should('exist');
    cy.get('.exercise-container').should('exist');
    cy.get('#add-exercise').should('exist');
  });
  
  it('Moet oefeningen kunnen toevoegen', () => {
    // Login eerst
    cy.visit('https://localhost:7044/Home/Login');
    cy.get('input[name="email"]').type('test@example.com');
    cy.get('input[name="password"]').type('Password123!');
    cy.get('input[value="Send verification"]').click();
    cy.get('input[name="vericode"]').type('123456');
    cy.get('input[value="Log in"]').click();
    
    // Ga naar nieuwe workout pagina
    cy.visit('https://localhost:7044/Home/NewWorkout');
    
    // Voeg oefening toe
    cy.get('#add-exercise').click();
    
    // Vul workout details in
    cy.get('#typeWorkout').type('Kracht Training');
    cy.get('#workoutDate').type('2025-03-23');
    
    // Vul oefening details in
    cy.get('#exerciseName-0').type('Bankdrukken');
    cy.get('#muscleGroup-0').select('Borst');
  });
});

// profile.spec.js
describe('Profiel Pagina', () => {
  it('Moet profiel pagina elementen bevatten na login', () => {
    // Login eerst
    cy.visit('https://localhost:7044/Home/Login');
    cy.get('input[name="email"]').type('test@example.com');
    cy.get('input[name="password"]').type('Password123!');
    cy.get('input[value="Send verification"]').click();
    cy.get('input[name="vericode"]').type('123456');
    cy.get('input[value="Log in"]').click();
    
    // Ga naar profiel pagina
    cy.visit('https://localhost:7044/Home/Profile');
    
    // Check profiel pagina elementen
    cy.get('.profile-header').should('exist');
    cy.get('.bio').should('exist');
    cy.get('#weekCalendar').should('exist');
  });
});

// track.spec.js
describe('Track Workouts Pagina', () => {
  it('Moet track pagina elementen bevatten na login', () => {
    // Login eerst
    cy.visit('https://localhost:7044/Home/Login');
    cy.get('input[name="email"]').type('test@example.com');
    cy.get('input[name="password"]').type('Password123!');
    cy.get('input[value="Send verification"]').click();
    cy.get('input[name="vericode"]').type('123456');
    cy.get('input[value="Log in"]').click();
    
    // Ga naar track pagina
    cy.visit('https://localhost:7044/Home/Track');
    
    // Check track pagina elementen
    cy.get('h3').contains('Workout Geschiedenis');
    cy.get('.log-workout-btn').should('exist');
    cy.get('#workouts-container').should('exist');
  });
  
  it('Moet navigeren naar de nieuwe workout pagina', () => {
    // Login eerst
    cy.visit('https://localhost:7044/Home/Login');
    cy.get('input[name="email"]').type('test@example.com');
    cy.get('input[name="password"]').type('Password123!');
    cy.get('input[value="Send verification"]').click();
    cy.get('input[name="vericode"]').type('123456');
    cy.get('input[value="Log in"]').click();
    
    // Ga naar track pagina
    cy.visit('https://localhost:7044/Home/Track');
    
    // Klik op nieuwe workout knop
    cy.get('.log-workout-btn').click();
    
    // Check of we op de nieuwe workout pagina zijn
    cy.url().should('include', '/Home/NewWorkout');
  });
});