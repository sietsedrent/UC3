describe('template spec', () => {
  it('passes', () => {
    cy.visit('https://localhost:7044');

    cy.get("h1").contains("Login");


    cy.get("input[type=email]")
 });
});