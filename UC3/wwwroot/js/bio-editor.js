// Deze code converteert de bestaande bio-editor functionaliteit naar een webcomponent
// Dit is een minimale aanpassing die gebruik maakt van Custom Elements en Shadow DOM
// zoals beschreven in de slides

// Behoud dezelfde functionaliteit maar maak er een herbruikbaar component van
class BioEditor extends HTMLElement {
    constructor() {
        super();
        //shadow dom voor encapsu
        this.attachShadow({ mode: 'open' });

        // HTML template voo structuur
        this.shadowRoot.innerHTML = `
      <style>
        .bio {
          border: 1px solid #ddd;
          border-radius: 5px;
          padding: 15px;
          background-color: #f9f9f9;
        }
        .bio-header {
          display: flex;
          justify-content: space-between;
          align-items: center;
          margin-bottom: 10px;
        }
        .bio-header p {
          font-weight: bold;
          margin: 0;
        }
        .bio-btn {
          color: white;
          background-color: #007bff;
          border: none;
          padding: 5px 10px;
          border-radius: 4px;
          cursor: pointer;
        }
        .bio-content p {
          margin: 0;
          padding: 5px;
        }
        .bio-edit {
          display: none;
        }
        .bio-edit textarea {
          width: 100%;
          padding: 8px;
          border: 1px solid #ccc;
          border-radius: 4px;
          resize: vertical;
          margin-bottom: 10px;
        }
        .bio-edit-actions {
          display: flex;
          justify-content: flex-end;
        }
        .btn {
          padding: 5px 10px;
          margin-left: 5px;
          border-radius: 4px;
          cursor: pointer;
          border: none;
        }
        .btn-success {
          background-color: #28a745;
          color: white;
        }
        .btn-secondary {
          background-color: #6c757d;
          color: white;
        }
      </style>
      <div class="bio">
        <div class="bio-header">
          <p>Bio</p>
          <button type="button" id="changeBioBtn" class="bio-btn">Change Bio</button>
        </div>
        <div class="bio-content" id="bioViewContent">
          <p id="bioText">Bio: </p>
        </div>
        <div class="bio-edit" id="bioEditContent">
          <textarea id="bioTextarea" rows="5"></textarea>
          <div class="bio-edit-actions">
            <button type="button" id="saveBioBtn" class="btn btn-success">Save</button>
            <button type="button" id="cancelBioBtn" class="btn btn-secondary">Cancel</button>
          </div>
        </div>
      </div>
    `;

        //Bind event handlers
        this._changeBio = this._changeBio.bind(this);
        this._cancelBio = this._cancelBio.bind(this);
        this._saveBio = this._saveBio.bind(this);
    }

    // Wanneer het element aan de DOM wordt toegevoegd
    connectedCallback() {
        // Initiële bio-tekst instellen
        const bioText = this.getAttribute('bio-text') || '';
        this.shadowRoot.getElementById('bioText').textContent = 'Bio: ' + bioText;
        this.shadowRoot.getElementById('bioTextarea').value = bioText;

        // Event listeners toevoegen
        this.shadowRoot.getElementById('changeBioBtn').addEventListener('click', this._changeBio);
        this.shadowRoot.getElementById('cancelBioBtn').addEventListener('click', this._cancelBio);
        this.shadowRoot.getElementById('saveBioBtn').addEventListener('click', this._saveBio);
    }

    // Wanneer het element uit de DOM wordt verwijderd
    disconnectedCallback() {
        // Event listeners opruimen
        this.shadowRoot.getElementById('changeBioBtn').removeEventListener('click', this._changeBio);
        this.shadowRoot.getElementById('cancelBioBtn').removeEventListener('click', this._cancelBio);
        this.shadowRoot.getElementById('saveBioBtn').removeEventListener('click', this._saveBio);
    }

    // Welke attributen observeren we
    static get observedAttributes() {
        return ['bio-text'];
    }

    // Wat te doen als een attribuut wijzigt
    attributeChangedCallback(name, oldValue, newValue) {
        if (name === 'bio-text' && oldValue !== newValue) {
            this.shadowRoot.getElementById('bioText').textContent = 'Bio: ' + newValue;
            this.shadowRoot.getElementById('bioTextarea').value = newValue;
        }
    }

    // Event handler bio wijzigen
    _changeBio() {
        this.shadowRoot.getElementById('bioViewContent').style.display = 'none';
        this.shadowRoot.getElementById('bioEditContent').style.display = 'block';
    }

    //bio annuleren
    _cancelBio() {
        this.shadowRoot.getElementById('bioEditContent').style.display = 'none';
        this.shadowRoot.getElementById('bioViewContent').style.display = 'block';
    }

    //bio opslaan
    _saveBio() {
        const newBio = this.shadowRoot.getElementById('bioTextarea').value;

        // AJAX request naar de server
        fetch('/Home/UpdateBio', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: `bio=${encodeURIComponent(newBio)}`
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    this.shadowRoot.getElementById('bioText').textContent = 'Bio: ' + newBio;
                    this.shadowRoot.getElementById('bioEditContent').style.display = 'none';
                    this.shadowRoot.getElementById('bioViewContent').style.display = 'block';

                    // Update het attribuut
                    this.setAttribute('bio-text', newBio);

                    // Dispatch een event zodat de rest van de applicatie weet dat de bio is bijgewerkt
                    this.dispatchEvent(new CustomEvent('bio-updated', {
                        detail: { bioText: newBio },
                        bubbles: true,
                        composed: true
                    }));
                } else {
                    alert("Er is een fout opgetreden bij het opslaan van de bio.");
                }
            })
            .catch(error => {
                console.error('Error updating bio:', error);
                alert("Er is een fout opgetreden bij het opslaan van de bio.");
            });
    }
}

customElements.define('bio-editor', BioEditor);